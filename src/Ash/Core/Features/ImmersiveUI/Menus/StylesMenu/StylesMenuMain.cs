using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.ImmersiveUI.Extras.Extensions;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Components;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Components.ScrollView;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Data;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.State;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures;
using Ash.GlobalUtils;
using H;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu
{
    internal class StylesMenuMain
    {

        internal readonly GameObject StylesMenuRootGameObj;

        internal readonly StylesMenuStateControl StateControl;
        internal readonly StylesMenuThumbnailCollection Thumbnails;

        private Material FrostedGlassMaterial;

        private bool IsScrollPositionDirty;

        private readonly RectTransform ScrollViewContentRectTransform;
        private readonly RawImage ThumbnailRawImage;
        private readonly ScrollRect ScrollViewRect;

        private readonly StylesMenuTextures Textures;

        private readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;
        private readonly StylesMenuStylesData StylesData;

        private const string HeaderText = "Positions";

        private static readonly int FrostTex = Shader.PropertyToID("_FrostTex");
        private static readonly int FrostIntensity = Shader.PropertyToID("_FrostIntensity");

        internal StylesMenuMain(IuiMain iuiMain, GameObject canvasGameObj) {
            StylesData = canvasGameObj.GetComponent<StylesMenuStylesData>();
            Textures = new StylesMenuTextures();
            Thumbnails = new StylesMenuThumbnailCollection();

            StylesMenuRootGameObj = new GameObject("StylesMenuRoot");
            StylesMenuRootGameObj.transform.SetParent(canvasGameObj.transform, false);

            IuiPositioningHelpers.AnchorsFillParent(StylesMenuRootGameObj);

            // frosted glass bg
            var frostedGlassGameObj = CreateFrostedGlass(StylesMenuRootGameObj);

            var containerGameObj = CreateContentContainer(frostedGlassGameObj);

            var containerSafeMarginsGameObj = CreateContainerWithSafeMargins(containerGameObj);

            AddVerticalLayoutComponent(containerSafeMarginsGameObj);

            // x button
            CreateXButton(containerGameObj);

            // header
            CreateHeader(containerSafeMarginsGameObj);

            var hLayoutGameObj = AddHorizontalLayoutComponent(containerSafeMarginsGameObj);
            var leftGameObj = AddLeftContainer(hLayoutGameObj);
            var rightGameObj = AddRightContainer(hLayoutGameObj);

            // scroll view
            var scrollViewData = StylesScrollView.CreateScrollView(leftGameObj, Textures);

            ScrollViewRect = scrollViewData[0].GetComponent<ScrollRect>();
            ScrollViewContentRectTransform = scrollViewData[1].GetComponent<RectTransform>();

            // create style selection buttons
            var genreTogglesList = CreateGenreSelectionToggles(frostedGlassGameObj);

            // create female state selection buttons
            var femaleStateTogglesList = CreateFemaleStateSelectionToggles(frostedGlassGameObj);

            // thumbnails
            var thumbnailGameObj = StyleThumbnail.AddThumbnailComponent(rightGameObj);
            ThumbnailRawImage = thumbnailGameObj.GetComponent<RawImage>();
            ThumbnailRawImage.enabled = false;

            StylesMenuRootGameObj.SetActive(false);

            StateControl = new StylesMenuStateControl(
                iuiMain,
                this,
                StylesData,
                genreTogglesList,
                femaleStateTogglesList,
                OnUpdateScrollList,
                OnAfterScrollListUpdatedRoutine
            );

            // refers to StateControl, should be called last
            PopulateScrollList();
        }

        ~StylesMenuMain() {
            Object.Destroy(FrostedGlassMaterial);
        }

        private void OnUpdateScrollList() {
            ClearScrollList();
            PopulateScrollList();

            IsScrollPositionDirty = true;
        }

        private IEnumerator OnAfterScrollListUpdatedRoutine() {
            yield return new WaitForEndOfFrame();

            if (!IsScrollPositionDirty)
                yield break;

            ScrollViewRect.verticalNormalizedPosition = 1;
            IsScrollPositionDirty = false;
        }

        private GameObject CreateFrostedGlass(GameObject root) {
            var frostedGlassShader =
                Ash.AshUI.ImmersiveUIShadersAssetBundle.LoadAsset<Shader>(
                    "assets/frostedglass/shaders/frostedglass.shader");

            if (frostedGlassShader == null) {
                Ash.Logger.LogError($"{nameof(frostedGlassShader)} is null");
                return null;
            }

            if (Textures.FrostedGlassAtlasMask == null) {
                Ash.Logger.LogError($"{nameof(Textures.FrostedGlassAtlasMask)} is null");
                return null;
            }

            var frostedGlassGameObj = new GameObject("FrostedGlass", typeof(RectTransform), typeof(RawImage));
            frostedGlassGameObj.transform.SetParent(root.transform, false);

            var frostedGlassRectTransform = frostedGlassGameObj.GetComponent<RectTransform>();
            frostedGlassRectTransform.sizeDelta = new Vector2(
                Textures.FrostedGlassAtlasMask.width, Textures.FrostedGlassAtlasMask.height
            );

            FrostedGlassMaterial = new Material(frostedGlassShader);
            FrostedGlassMaterial.SetTexture(FrostTex, Textures.FrostedGlassAtlasMask);
            FrostedGlassMaterial.SetFloat(FrostIntensity, IuiSettings.SubMenusFrostIntensity);

            var frostedGlassBackground = frostedGlassGameObj.GetComponent<RawImage>();
            frostedGlassBackground.material = FrostedGlassMaterial;

            IuiPositioningHelpers.AnchorsCenterIn(frostedGlassGameObj);

            var tintGameObj = new GameObject("FrostedGlassTint", typeof(RawImage));
            tintGameObj.transform.SetParent(frostedGlassGameObj.transform, false);

            var frostGlassTint = tintGameObj.GetComponent<RawImage>();
            frostGlassTint.texture = Textures.FrostGlassTintTexture;
            frostGlassTint.SetNativeSize();

            var tintRectTransform = IuiPositioningHelpers.AnchorsLeftBottom(tintGameObj);
            tintRectTransform.pivot = Vector2.zero;

            return frostedGlassGameObj;
        }

        private void CreateXButton(GameObject root) {
            var xButtonGameObject = new GameObject("XButton", typeof(RectTransform), typeof(RawImage), typeof(Button));
            xButtonGameObject.transform.SetParent(root.transform, false);

            var buttonRawImage = xButtonGameObject.GetComponent<RawImage>();
            buttonRawImage.texture = Textures.XButtonTexture;

            var buttonComp = xButtonGameObject.GetComponent<Button>();
            buttonComp.targetGraphic = buttonRawImage;

            buttonComp.onClick.AddListener(delegate { StateControl.ToggleMenuVisibility(); });

            var buttonRectTransform = buttonComp.GetComponent<RectTransform>();
            buttonRectTransform.anchorMin = Vector2.one;
            buttonRectTransform.anchorMax = Vector2.one;
            buttonRectTransform.pivot = Vector2.one;
            buttonRectTransform.anchoredPosition =
                new Vector2(-Config.XButtonSize.Width, -Config.XButtonSize.Height) * 1.1f;
            buttonRectTransform.sizeDelta = new Vector2(Config.XButtonSize.Width, Config.XButtonSize.Height);
        }

        private GameObject CreateContentContainer(GameObject root) {
            var containerGameObject = new GameObject("ContentContainer", typeof(RectTransform));
            containerGameObject.transform.SetParent(root.transform, false);

            var containerRectTransform = IuiPositioningHelpers.AnchorsLeftBottom(containerGameObject);
            containerRectTransform.pivot = Vector2.zero;
            containerRectTransform.sizeDelta = new Vector2(
                Config.BackgroundRectSize.Width,
                Config.BackgroundRectSize.Height
            );

            return containerGameObject;
        }

        private GameObject CreateContainerWithSafeMargins(GameObject root) {
            var containerGameObject = new GameObject("ContentContainerSafeMargins", typeof(RectTransform),
                typeof(VerticalLayoutGroup));
            containerGameObject.transform.SetParent(root.transform, false);

            var containerRectTransform = IuiPositioningHelpers.AnchorsFillParent(containerGameObject);
            containerRectTransform.offsetMin =
                new Vector2(Config.BackgroundContainerMargins.left, Config.BackgroundContainerMargins.bottom);
            containerRectTransform.offsetMax =
                new Vector2(-Config.BackgroundContainerMargins.right, -Config.BackgroundContainerMargins.top);

            return containerGameObject;
        }

        private void CreateHeader(GameObject root) {
            var textGameObj = new GameObject("HeaderText", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            textGameObj.transform.SetParent(root.transform, false);

            var layoutElement = textGameObj.GetComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;
            layoutElement.minHeight = Config.HeaderFontSize;

            var textComp = textGameObj.GetComponent<Text>();
            textComp.text = HeaderText;
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/tangerine-regular.ttf");
            textComp.fontSize = Config.HeaderFontSize;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.LowerCenter;
            textComp.color = ColorUtils.Color32Af(251, 244, 248);
        }

        private List<IuiToggle> CreateGenreSelectionToggles(GameObject root) {
            var containerGameObject =
                new GameObject("GenreSelectionToggles", typeof(RectTransform), typeof(HorizontalLayoutGroup),
                    typeof(ContentSizeFitter), typeof(ToggleGroup));
            containerGameObject.transform.SetParent(root.transform, false);

            var genreTogglesContainerOffset = new Vector2(Config.GenreTogglesContainerLeftMargin,
                Config.BackgroundRectSize.Height + Config.GenreTogglesContainerBottomMargin);

            var containerRectTransform =
                IuiPositioningHelpers.AnchorsLeftBottom(containerGameObject, genreTogglesContainerOffset);
            containerRectTransform.pivot = Vector2.zero;

            var hLayout = containerGameObject.GetComponent<HorizontalLayoutGroup>();
            hLayout.childControlWidth = true;
            hLayout.childControlHeight = true;
            hLayout.childForceExpandWidth = false;
            hLayout.childForceExpandHeight = false;
            hLayout.spacing = Config.GenreToggleSpacing;

            var contentSizeFitter = containerGameObject.GetComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var toggleGroup = containerGameObject.GetComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = false;

            var types = (H_StyleData.TYPE[])Enum.GetValues(typeof(H_StyleData.TYPE));
            var genreTogglesList = new List<IuiToggle>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var type in types.Where(t => t != H_StyleData.TYPE.NUM))
                genreTogglesList.Add(
                    new GenreSelectionToggle(
                        Textures,
                        containerGameObject,
                        toggleGroup,
                        type,
                        () => StateControl.OnUpdateStyleType(type)
                    )
                );

            return genreTogglesList;
        }

        private List<IuiToggle> CreateFemaleStateSelectionToggles(GameObject root) {
            var containerGameObject =
                new GameObject("FemaleStateSelectionToggles", typeof(RectTransform), typeof(VerticalLayoutGroup),
                    typeof(ContentSizeFitter), typeof(ToggleGroup));
            containerGameObject.transform.SetParent(root.transform, false);

            var femaleStateTogglesContainerOffset = new Vector2(Config.BackgroundRectSize.Width + Config.FemaleStateButtonsContainerRightMargin,
                Config.FemaleStateButtonsContainerBottomMargin);

            var containerRectTransform =
                IuiPositioningHelpers.AnchorsLeftBottom(containerGameObject, femaleStateTogglesContainerOffset);
            containerRectTransform.pivot = Vector2.zero;

            var vLayout = containerGameObject.GetComponent<VerticalLayoutGroup>();
            vLayout.childControlWidth = true;
            vLayout.childControlHeight = true;
            vLayout.childForceExpandWidth = false;
            vLayout.childForceExpandHeight = false;
            vLayout.spacing = StylesMenuConfig.FemaleStateToggleSpacing;

            var contentSizeFitter = containerGameObject.GetComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var toggleGroup = containerGameObject.GetComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = false;

            var states = (H_StyleData.STATE[])Enum.GetValues(typeof(H_StyleData.STATE));
            var femaleStateTogglesList = new List<IuiToggle>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var state in states.Where(t => t != H_StyleData.STATE.UNKNOWN))
                femaleStateTogglesList.Add(
                    new FemaleStateSelectionToggle(
                        Textures,
                        containerGameObject,
                        toggleGroup,
                        state,
                        () => StateControl.OnUpdateFemaleState(state)
                    )
                );

            return femaleStateTogglesList;
        }

        // populate

        private void PopulateScrollList() {
            if (StateControl.ListButtons.Count > 0)
                return;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var hStyleData in StylesData.ValidStyles) {
                // Ash.Logger.LogDebug($"name: {hStyleData.name}, id: {hStyleData.id}");

                // ReSharper disable once MoveLocalFunctionAfterJumpStatement
                var buttonGameObj = StylesScrollViewButton.CreateScrollListButton(
                    Textures,
                    ScrollViewContentRectTransform,
                    hStyleData,
                    () => StateControl.UpdateThumbnailImage(hStyleData, ThumbnailRawImage),
                    () => {
                        StateControl.OnStyleSelected(hStyleData.id);
                        StateControl.ToggleMenuVisibility();
                    }
                );

                StateControl.ListButtons.Add(buttonGameObj);
            }
        }

        private void ClearScrollList() {
            foreach (var buttonGameObj in StateControl.ListButtons)
                Object.Destroy(buttonGameObj);

            StateControl.ListButtons.Clear();
        }

        // layouts
        private void AddVerticalLayoutComponent(GameObject root) {
            var vLayout = root.GetComponent<VerticalLayoutGroup>();
            vLayout.childControlWidth = true;
            vLayout.childControlHeight = true;
            vLayout.childForceExpandWidth = true;
            vLayout.childForceExpandHeight = false;
            vLayout.spacing = Config.MainVerticalLayoutSpacing;
        }

        private GameObject AddHorizontalLayoutComponent(GameObject root) {
            var hLayoutGameObj =
                new GameObject("HorizontalLayout", typeof(LayoutElement), typeof(HorizontalLayoutGroup));
            hLayoutGameObj.transform.SetParent(root.transform, false);

            var hLayoutElement = hLayoutGameObj.GetComponent<LayoutElement>();
            hLayoutElement.minHeight = 100f;
            hLayoutElement.flexibleHeight = 1f;

            var hLayout = hLayoutGameObj.GetComponent<HorizontalLayoutGroup>();
            hLayout.childControlWidth = true;
            hLayout.childForceExpandWidth = false;
            hLayout.spacing = Config.MainHorizontalLayoutSpacing;

            return hLayoutGameObj;
        }

        private GameObject AddLeftContainer(GameObject root) {
            var leftGameObj = new GameObject("LeftContainer", typeof(RectTransform), typeof(LayoutElement));
            leftGameObj.transform.SetParent(root.transform, false);

            var leftLayoutElement = leftGameObj.GetComponent<LayoutElement>();
            leftLayoutElement.flexibleWidth =
                (float)Config.StylesListWidth / Config.BackgroundRectSize.Width;
            leftLayoutElement.flexibleHeight = 1f;

            return leftGameObj;
        }

        private GameObject AddRightContainer(GameObject root) {
            var rightGameObj = new GameObject("RightContainer", typeof(RectTransform), typeof(LayoutElement));
            rightGameObj.transform.SetParent(root.transform, false);

            var rightLayoutElement = rightGameObj.GetComponent<LayoutElement>();
            rightLayoutElement.flexibleWidth =
                1 - (float)Config.StylesListWidth / Config.BackgroundRectSize.Width;
            rightLayoutElement.flexibleHeight = 1f;

            return rightGameObj;
        }
    }
}
