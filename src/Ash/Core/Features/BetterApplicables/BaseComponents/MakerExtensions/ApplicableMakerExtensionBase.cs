using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions.CustomUIElements;
using Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions.ExtendedControls;
using Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions.ExtendedLayouts;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions
{
    internal abstract class ApplicableMakerExtensionBase : MonoBehaviour
    {
        protected abstract AshLogger Logger { get; }

        internal SerializableTextureData[] UnsavedChanges { get; private set; }


        protected int ActiveLayer { get; set; }

        protected abstract MakerCategory MakerCategory { get; }
        protected virtual int SubCategoryIndex => 0;
        protected virtual string UICategoryRootGameObjectPath => string.Empty;

        protected abstract float OffsetMinValue { get; }
        protected abstract float OffsetMaxValue { get; }
        protected abstract Part Part { get; }
        protected abstract string LayerSelectorsGroupId { get; }

        protected abstract Action RenderSkinTexture { get; }

        protected abstract SerializableTextureData[] InitializeUnsavedChanges();
        protected abstract SerializableTextureData CreateSerializableData(CustomSelectSet set);
        protected abstract void UpdateNativeGuiControlsState();

        protected abstract void SubscribeToEvents();
        protected abstract void UnsubscribeFromEvents();


        private ExtGridLayout LayerSelectorsGridLayout;
        private MakerSlider OffsetXSlider;
        private MakerSlider OffsetYSlider;
        private MakerSlider ScaleXSlider;
        private MakerSlider ScaleYSlider;

        private readonly Dictionary<Part, RenderTexture> TextureCache =
            new Dictionary<Part, RenderTexture>();

        private readonly CompositeDisposable MakerDisposables = new CompositeDisposable();


        protected void OnSelectionChanged(CustomSelectSet set) {
            UnsavedChanges[ActiveLayer] = CreateSerializableData(set);
            UpdateGuiControlsState();
        }

        protected void OnTintSelectionChanged(Color tint) {
            if (UnsavedChanges[ActiveLayer] == null)
                return;

            UnsavedChanges[ActiveLayer].Tint = tint;
        }

        protected void UpdateGuiControlsState() {
            var activeLayerHasData = UnsavedChanges[ActiveLayer] != null;

            OffsetXSlider.ControlObject.SetActive(activeLayerHasData);
            OffsetYSlider.ControlObject.SetActive(activeLayerHasData);
            ScaleXSlider.ControlObject.SetActive(activeLayerHasData);
            ScaleYSlider.ControlObject.SetActive(activeLayerHasData);

            LayerSelectorsGridLayout
                .ChildControls[ActiveLayer]
                .GetComponent<SelectionToggle>().IsEmpty = !activeLayerHasData;

            if (!activeLayerHasData)
                return;

            var data = UnsavedChanges[ActiveLayer];
            OffsetXSlider.SetValue(data.UserOffset.x, false);
            OffsetYSlider.SetValue(data.UserOffset.y, false);
            ScaleXSlider.SetValue(data.UserScale.x, false);
            ScaleYSlider.SetValue(data.UserScale.y, false);
        }


        internal RenderTexture GetCachedRenderTexture(Part part) {
            return TextureCache.GetValueOrDefaultValue(part, null);
        }

        internal void UpdateTextureCache(Part part, RenderTexture renderTexture) {
            if (renderTexture == null)
                return;

            var cachedRt = TextureCache.GetValueOrDefaultValue(part, null);
            if (cachedRt == null)
                cachedRt = new RenderTexture(
                    renderTexture.width,
                    renderTexture.height,
                    0,
                    RenderTextureFormat.ARGB32,
                    RenderTextureReadWrite.sRGB
                );

            Graphics.Blit(renderTexture, cachedRt);

            TextureCache[part] = cachedRt;
        }

        internal void DestroyTextureCache() {
            foreach (var kvp in TextureCache.Where(kvp => kvp.Value != null)) {
                kvp.Value.Release();
                Destroy(kvp.Value);
            }

            TextureCache.Clear();
        }


        private void Start() {
            MakerAPI.MakerBaseLoaded += OnMakerBaseLoaded;
            MakerAPI.MakerFinishedLoading += OnMakerFinishedLoading;
            MakerAPI.MakerExiting += OnMakerExiting;

            SubscribeToEvents();
        }

        private void OnDestroy() {
            MakerAPI.MakerBaseLoaded -= OnMakerBaseLoaded;
            MakerAPI.MakerFinishedLoading -= OnMakerFinishedLoading;
            MakerAPI.MakerExiting -= OnMakerExiting;

            UnsubscribeFromEvents();
        }

        private void OnMakerBaseLoaded(object sender, RegisterCustomControlsEvent e) {
            UnsavedChanges = InitializeUnsavedChanges();

            LayerSelectorsGridLayout = new ExtGridLayout(
                MakerCategory,
                Ash.Instance,
                CreateLayerSelectors()
            );

            OffsetXSlider = new MakerSlider(MakerCategory, "Offset X", OffsetMinValue, OffsetMaxValue, 0f, Ash.Instance);
            OffsetYSlider = new MakerSlider(MakerCategory, "Offset Y", OffsetMinValue, OffsetMaxValue, 0f, Ash.Instance);
            ScaleXSlider = new MakerSlider(MakerCategory, "Scale X",  0.01f, 4f, 1f, Ash.Instance);
            ScaleYSlider = new MakerSlider(MakerCategory, "Scale Y",  0.01f, 4f, 1f, Ash.Instance);


            var activeLayerData = UnsavedChanges[ActiveLayer];
            OffsetXSlider.SetValue(activeLayerData?.UserOffset.x ?? 0);
            OffsetYSlider.SetValue(activeLayerData?.UserOffset.y ?? 0);
            ScaleXSlider.SetValue(activeLayerData?.UserScale.x ?? 1);
            ScaleYSlider.SetValue(activeLayerData?.UserScale.y ?? 1);

            var offsetXObserver = Observer.Create<float>(val => {
                if (UnsavedChanges[ActiveLayer] == null)
                    return;

                UnsavedChanges[ActiveLayer].UserOffset = new Vector2(val, OffsetYSlider.Value);
                RenderSkinTexture();
            });
            var offsetYObserver = Observer.Create<float>(val => {
                if (UnsavedChanges[ActiveLayer] == null)
                    return;

                UnsavedChanges[ActiveLayer].UserOffset = new Vector2(OffsetXSlider.Value, val);
                RenderSkinTexture();
            });
            var scaleXObserver = Observer.Create<float>(val => {
                if (UnsavedChanges[ActiveLayer] == null)
                    return;

                UnsavedChanges[ActiveLayer].UserScale = new Vector2(val, ScaleYSlider.Value);
                RenderSkinTexture();
            });
            var scaleYObserver = Observer.Create<float>(val => {
                if (UnsavedChanges[ActiveLayer] == null)
                    return;

                UnsavedChanges[ActiveLayer].UserScale = new Vector2(ScaleXSlider.Value, val);
                RenderSkinTexture();
            });

            OffsetXSlider.ValueChanged.Subscribe(offsetXObserver).AddTo(MakerDisposables);
            OffsetYSlider.ValueChanged.Subscribe(offsetYObserver).AddTo(MakerDisposables);
            ScaleXSlider.ValueChanged.Subscribe(scaleXObserver).AddTo(MakerDisposables);
            ScaleYSlider.ValueChanged.Subscribe(scaleYObserver).AddTo(MakerDisposables);

            e.AddControl(LayerSelectorsGridLayout);
            e.AddControl(OffsetXSlider);
            e.AddControl(OffsetYSlider);
            e.AddControl(ScaleXSlider);
            e.AddControl(ScaleYSlider);
        }

        private void OnMakerFinishedLoading(object sender, EventArgs e) {
            SelectFirstLayerAndUpdateGuiStates();

            SetupSlider(OffsetXSlider.ControlObject.GetComponent<InputSliderUI>());
            SetupSlider(OffsetYSlider.ControlObject.GetComponent<InputSliderUI>());
            SetupSlider(ScaleXSlider.ControlObject.GetComponent<InputSliderUI>());
            SetupSlider(ScaleYSlider.ControlObject.GetComponent<InputSliderUI>());

            CreateSpoilerAndReparentControls(SubCategoryIndex); // put spoiler after Nth (index) color button, 0 by default

            // because MakerFinishedLoading is invoked before the internal flag is set, MakerApi.InsideAndLoaded is
            // going to be FALSE at this moment, so we cannot use it in most places or risk getting skin rendering borked
            RenderSkinTexture();
        }

        private void OnMakerExiting(object sender, EventArgs e) {
            MakerDisposables.Clear();

            OffsetXSlider = null;
            OffsetYSlider = null;
            ScaleXSlider = null;
            ScaleYSlider = null;

            LayerSelectorsGridLayout = null;
        }


        private void OnLayerButtonPressed(int index) {
            ActiveLayer = index;

            UpdateGuiControlsState();
            UpdateNativeGuiControlsState();

            RenderSkinTexture();
        }

        protected void UpdateDataAndGuiStates(Part part, SerializableTextureData[] data)  {
            if (!MakerAPI.InsideMaker)
                return;

            if (Part != part)
                return;

            UnsavedChanges = data;

            SelectFirstLayerAndUpdateGuiStates();
        }

        private void UpdateTogglesIsEmptyStatus() {
            for (var i = 0; i < LayerSelectorsGridLayout.ChildControls.Count; i++) {
                LayerSelectorsGridLayout.ChildControls[i].GetComponent<SelectionToggle>().IsEmpty =
                    UnsavedChanges[i] == null;
            }
        }

        private List<GameObject> CreateLayerSelectors() {
            var selectors = new List<GameObject>();
            var referenceToggle = GameObject.Find("EditMode/Canvas/File/Tabs/Toggle CharaSave");
            if (referenceToggle == null)
                referenceToggle = GameObject.Find("EditMode(Clone)/Canvas/File/Tabs/Toggle CharaSave");

            var buttonOnRef = referenceToggle.transform.FindChild("Button_on");
            var buttonOffRef = referenceToggle.transform.FindChild("Button_off");

            for (var i = 0; i < 10; i++) {
                var go = new GameObject("LayerSelector");

                var buttonOn = Instantiate(buttonOnRef, go.transform, false);
                var buttonOff = Instantiate(buttonOffRef, go.transform, false);

                buttonOn.FindChild("Frame").GetComponent<Image>().enabled = false;
                buttonOff.FindChild("Frame").GetComponent<Image>().enabled = false;

                var layoutElement = go.AddComponent<LayoutElement>();
                layoutElement.preferredWidth = 40;
                layoutElement.preferredHeight = 40;

                var toggle = go.AddComponent<SelectionToggle>();

                toggle.Setup(
                    LayerSelectorsGroupId,
                    buttonOn.GetComponent<Button>(),
                    buttonOff.GetComponent<Button>(),
                    (i + 1).ToString(),
                    OnLayerButtonPressed
                );

                toggle.IsEmpty = UnsavedChanges[ActiveLayer] == null;

                selectors.Add(go);
            }

            return selectors;
        }

        private void SelectFirstLayerAndUpdateGuiStates() {
            LayerSelectorsGridLayout.ChildControls[0].GetComponent<SelectionToggle>().ChangeValue(true, true, false);

            ActiveLayer = 0;

            UpdateGuiControlsState();
            UpdateTogglesIsEmptyStatus();
            UpdateNativeGuiControlsState();
        }

        private static void SetupSlider(InputSliderUI inputSliderUI) {
            if (inputSliderUI == null)
                return;

            var rootHorizontalLayoutGroup = inputSliderUI.gameObject.AddComponent<HorizontalLayoutGroup>();
            rootHorizontalLayoutGroup.childForceExpandWidth = false;
            rootHorizontalLayoutGroup.childForceExpandHeight = false;
            rootHorizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            var text = inputSliderUI.title.gameObject;
            var slider = inputSliderUI.slider.gameObject;
            var inputField = inputSliderUI.inputField.gameObject;

            text.AddComponent<LayoutElement>();

            var sliderLayoutElement = slider.AddComponent<LayoutElement>();
            sliderLayoutElement.preferredWidth = 104;
            sliderLayoutElement.preferredHeight = 20;

            var inputFieldLayoutElement = inputField.AddComponent<LayoutElement>();
            inputFieldLayoutElement.preferredWidth = 25;
            inputFieldLayoutElement.preferredHeight = 25;

            inputSliderUI.inputField.characterLimit = 8;
        }

        private void CreateSpoilerAndReparentControls(int subCategoryByIndex) {
            var categoryRoot = GameObject.Find($"EditMode{(SceneTypeTracker.Scene is H_Scene ? "(Clone)" : "")}/{UICategoryRootGameObjectPath}");
            if (categoryRoot == null)
                return;

            var colorButtonsIndices = categoryRoot.transform
                .Cast<Transform>()
                .ToList()
                .Select((value, index) => new { value, index })
                .Where(t => t.value.gameObject && t.value.gameObject.GetComponent<ColorChangeButton>() != null)
                .Select(x => x.index)
                .ToList();

            var colorChangeButtonIndex = colorButtonsIndices[subCategoryByIndex];

            if (colorChangeButtonIndex == -1)
                return;

            var spoiler = Spoiler.Create(categoryRoot.transform, "Controls");
            spoiler.SpoilerGroup.transform.SetParent(categoryRoot.transform, false);
            spoiler.SpoilerGroup.transform.SetSiblingIndex(colorChangeButtonIndex + 1);

            LayerSelectorsGridLayout.ControlObject.transform.SetParent(spoiler.ContentBody.transform, false);
            OffsetXSlider.ControlObject.transform.SetParent(spoiler.ContentBody.transform, false);
            OffsetYSlider.ControlObject.transform.SetParent(spoiler.ContentBody.transform, false);
            ScaleXSlider.ControlObject.transform.SetParent(spoiler.ContentBody.transform, false);
            ScaleYSlider.ControlObject.transform.SetParent(spoiler.ContentBody.transform, false);

            // overflow fix
            categoryRoot.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            categoryRoot.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
        }
    }
}
