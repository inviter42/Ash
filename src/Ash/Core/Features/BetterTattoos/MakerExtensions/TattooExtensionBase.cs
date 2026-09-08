using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.BetterTattoos.MakerExtensions.ExtendedControls;
using Ash.Core.Features.BetterTattoos.MakerExtensions.ExtendedLayouts;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.BetterTattoos.MakerExtensions
{
    internal abstract class TattooExtensionBase : MonoBehaviour
    {
        internal static readonly AshLogger Logger = new AshLogger(LoggingSettings.LoggingModules.ExtDataTattoos);

        internal TattooDataManager.SerializableTattooData[] UnsavedChanges { get; private set; }

        internal int ActiveTattooLayer { get; set; }


        protected abstract MakerCategory MakerCategory { get; }
        protected abstract float OffsetMinValue { get; }
        protected abstract float OffsetMaxValue { get; }
        protected abstract TattooDataManager.Part Part { get; }
        protected abstract string TattooLayerSelectorsGroupId { get; }

        protected abstract Action RenderSkinTexture { get; }

        protected abstract TattooDataManager.SerializableTattooData CreateTattooData(CustomSelectSet set);
        protected abstract void UpdateNativeGuiControlsState();

        protected abstract void SubscribeToTattooEvents();
        protected abstract void UnsubscribeFromTattooEvents();

        protected abstract void OnTattooLayerButtonPressed(int index);


        private ExtGridLayout TattooLayerSelectorsGridLayout;
        private MakerSlider OffsetXSlider;
        private MakerSlider OffsetYSlider;
        private MakerSlider ScaleXSlider;
        private MakerSlider ScaleYSlider;

        private static readonly Dictionary<TattooDataManager.Part, RenderTexture> TattooTextureCache =
            new Dictionary<TattooDataManager.Part, RenderTexture>();

        private readonly CompositeDisposable MakerDisposables = new CompositeDisposable();


        protected void OnTattooChanged(CustomSelectSet set) {
            UnsavedChanges[ActiveTattooLayer] = CreateTattooData(set);
            UpdateGuiControlsState();
        }

        protected void OnTattooColorChanged(Color color) {
            if (UnsavedChanges[ActiveTattooLayer] == null)
                return;

            UnsavedChanges[ActiveTattooLayer].TattooColor = color;
        }


        internal static RenderTexture GetCachedRenderTexture(TattooDataManager.Part part) {
            return TattooTextureCache.GetValueOrDefaultValue(part, null);
        }

        internal void UpdateGuiControlsState() {
            var activeLayerHasTattoo = UnsavedChanges[ActiveTattooLayer] != null;
            OffsetXSlider.ControlObject.SetActive(activeLayerHasTattoo);
            OffsetYSlider.ControlObject.SetActive(activeLayerHasTattoo);
            ScaleXSlider.ControlObject.SetActive(activeLayerHasTattoo);
            ScaleYSlider.ControlObject.SetActive(activeLayerHasTattoo);

            if (!activeLayerHasTattoo)
                return;

            var data = UnsavedChanges[ActiveTattooLayer];
            OffsetXSlider.SetValue(data.UserOffset.x, false);
            OffsetYSlider.SetValue(data.UserOffset.y, false);
            ScaleXSlider.SetValue(data.UserScale.x, false);
            ScaleYSlider.SetValue(data.UserScale.y, false);
        }


        internal static void UpdateTattooTextureCache(TattooDataManager.Part part, RenderTexture renderTexture) {
            if (renderTexture == null)
                return;

            var cachedRt = TattooTextureCache.GetValueOrDefaultValue(part, null);
            if (cachedRt == null)
                cachedRt = new RenderTexture(
                    renderTexture.width,
                    renderTexture.height,
                    0,
                    RenderTextureFormat.ARGB32,
                    RenderTextureReadWrite.sRGB
                );

            Graphics.Blit(renderTexture, cachedRt);

            TattooTextureCache[part] = cachedRt;
        }

        internal static void DestroyTattooTextureCache() {
            foreach (var kvp in TattooTextureCache.Where(kvp => kvp.Value != null)) {
                kvp.Value.Release();
                Destroy(kvp.Value);
            }

            TattooTextureCache.Clear();
        }


        private void Start() {
            MakerAPI.MakerBaseLoaded += OnMakerBaseLoaded;
            MakerAPI.MakerFinishedLoading += OnMakerFinishedLoading;
            MakerAPI.MakerExiting += OnMakerExiting;

            TattooDataManager.ExtDataFromTheCard += UpdateDataAndGuiStates;
            TattooDataManager.ExtDataFromVanilla += UpdateDataAndGuiStates;

            SubscribeToTattooEvents();
        }

        private void OnDestroy() {
            MakerAPI.MakerBaseLoaded -= OnMakerBaseLoaded;
            MakerAPI.MakerFinishedLoading -= OnMakerFinishedLoading;
            MakerAPI.MakerExiting -= OnMakerExiting;

            TattooDataManager.ExtDataFromTheCard -= UpdateDataAndGuiStates;
            TattooDataManager.ExtDataFromVanilla -= UpdateDataAndGuiStates;

            UnsubscribeFromTattooEvents();
        }

        private void OnMakerBaseLoaded(object sender, RegisterCustomControlsEvent e) {
            UnsavedChanges = TattooDataManager.GetTattooDataListCopy(MakerAPI.GetMakerBase().human, Part)
                             ?? new TattooDataManager.SerializableTattooData[TattooDataManager.NumberOfTattooSlots];

            TattooLayerSelectorsGridLayout = new ExtGridLayout(
                MakerCategory,
                Ash.Instance,
                CreateTattooLayerSelectors()
            );

            OffsetXSlider = new MakerSlider(MakerCategory, "Offset X", OffsetMinValue, OffsetMaxValue, 0f, Ash.Instance);
            OffsetYSlider = new MakerSlider(MakerCategory, "Offset Y", OffsetMinValue, OffsetMaxValue, 0f, Ash.Instance);
            ScaleXSlider = new MakerSlider(MakerCategory, "Scale X", 0f, 4f, 1f, Ash.Instance);
            ScaleYSlider = new MakerSlider(MakerCategory, "Scale Y", 0f, 4f, 1f, Ash.Instance);

            var human = MakerAPI.GetMakerBase().human;
            var tattooData = TattooDataManager.GetTattooDataListCopy(human, Part)[ActiveTattooLayer];
            OffsetXSlider.SetValue(tattooData?.UserOffset.x ?? 0);
            OffsetYSlider.SetValue(tattooData?.UserOffset.y ?? 0);
            ScaleXSlider.SetValue(tattooData?.UserScale.x ?? 1);
            ScaleYSlider.SetValue(tattooData?.UserScale.y ?? 1);

            var offsetXObserver = Observer.Create<float>(val => {
                if (UnsavedChanges[ActiveTattooLayer] == null)
                    return;

                UnsavedChanges[ActiveTattooLayer].UserOffset = new Vector2(val, OffsetYSlider.Value);
                RenderSkinTexture();
            });
            var offsetYObserver = Observer.Create<float>(val => {
                if (UnsavedChanges[ActiveTattooLayer] == null)
                    return;

                UnsavedChanges[ActiveTattooLayer].UserOffset = new Vector2(OffsetXSlider.Value, val);
                RenderSkinTexture();
            });
            var scaleXObserver = Observer.Create<float>(val => {
                if (UnsavedChanges[ActiveTattooLayer] == null)
                    return;

                UnsavedChanges[ActiveTattooLayer].UserScale = new Vector2(val, ScaleYSlider.Value);
                RenderSkinTexture();
            });
            var scaleYObserver = Observer.Create<float>(val => {
                if (UnsavedChanges[ActiveTattooLayer] == null)
                    return;

                UnsavedChanges[ActiveTattooLayer].UserScale = new Vector2(ScaleXSlider.Value, val);
                RenderSkinTexture();
            });

            OffsetXSlider.ValueChanged.Subscribe(offsetXObserver).AddTo(MakerDisposables);
            OffsetYSlider.ValueChanged.Subscribe(offsetYObserver).AddTo(MakerDisposables);
            ScaleXSlider.ValueChanged.Subscribe(scaleXObserver).AddTo(MakerDisposables);
            ScaleYSlider.ValueChanged.Subscribe(scaleYObserver).AddTo(MakerDisposables);

            e.AddControl(TattooLayerSelectorsGridLayout);
            e.AddControl(OffsetXSlider);
            e.AddControl(OffsetYSlider);
            e.AddControl(ScaleXSlider);
            e.AddControl(ScaleYSlider);
        }

        private void OnMakerFinishedLoading(object sender, EventArgs e) {
            OffsetXSlider.ControlObject.GetComponent<InputSliderUI>().inputField.characterLimit = 8;
            OffsetYSlider.ControlObject.GetComponent<InputSliderUI>().inputField.characterLimit = 8;
            ScaleXSlider.ControlObject.GetComponent<InputSliderUI>().inputField.characterLimit = 8;
            ScaleYSlider.ControlObject.GetComponent<InputSliderUI>().inputField.characterLimit = 8;

            SelectFirstLayerAndUpdateGuiStates();
        }

        private void OnMakerExiting(object sender, EventArgs e) {
            MakerDisposables.Clear();

            OffsetXSlider = null;
            OffsetYSlider = null;
            ScaleXSlider = null;
            ScaleYSlider = null;

            TattooLayerSelectorsGridLayout = null;
        }


        private void UpdateDataAndGuiStates(TattooDataManager.Part part, TattooDataManager.SerializableTattooData[] tattooData)  {
            if (!MakerAPI.InsideMaker)
                return;

            if (Part != part)
                return;

            UnsavedChanges = tattooData;

            SelectFirstLayerAndUpdateGuiStates();
        }


        private List<GameObject> CreateTattooLayerSelectors() {
            var tattooSelectors = new List<GameObject>();
            var referenceToggle = GameObject.Find("EditMode/Canvas/File/Tabs/Toggle CharaSave");
            if (referenceToggle == null)
                referenceToggle = GameObject.Find("EditMode(Clone)/Canvas/File/Tabs/Toggle CharaSave");

            var buttonOnRef = referenceToggle.transform.FindChild("Button_on");
            var buttonOffRef = referenceToggle.transform.FindChild("Button_off");

            for (var i = 0; i < 10; i++) {
                var go = new GameObject("TattooLayerSelector");

                var buttonOn = Instantiate(buttonOnRef, go.transform, false);
                var buttonOff = Instantiate(buttonOffRef, go.transform, false);

                buttonOn.GetComponent<Button>().onClick.RemoveAllListeners();
                buttonOff.GetComponent<Button>().onClick.RemoveAllListeners();

                var layoutElement = go.AddComponent<LayoutElement>();
                layoutElement.minWidth = 40;
                layoutElement.minHeight = 40;
                layoutElement.preferredWidth = 40;
                layoutElement.preferredHeight = 40;

                go.AddComponent<ExtSelectionToggle>()
                    .Setup(
                        TattooLayerSelectorsGroupId,
                        buttonOn.GetComponent<Button>(),
                        buttonOff.GetComponent<Button>(),
                        (i + 1).ToString(),
                        OnTattooLayerButtonPressed
                    );

                tattooSelectors.Add(go);
            }

            return tattooSelectors;
        }

        private void SelectFirstLayerAndUpdateGuiStates() {
            TattooLayerSelectorsGridLayout.ChildControls[0].GetComponent<ExtSelectionToggle>().ChangeValue(true, true, false);

            ActiveTattooLayer = 0;

            UpdateGuiControlsState();
            UpdateNativeGuiControlsState();
        }
    }
}
