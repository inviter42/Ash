using System;
using System.Collections.Generic;
using Ash.Core.Features.ImmersiveUI.Extras.Extensions;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Components;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.State;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Textures;
using Ash.Core.SceneManagement;
using Ash.GlobalUtils;
using Character;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.GagMenu
{
    internal class GagMenuMain
    {
        internal readonly GameObject GagMenuRootGameObj;

        internal readonly GagMenuStateControl StateControl;

        private Material FrostedGlassMaterial;

        private readonly GagMenuTextures Textures;

        private readonly GagMenuConfig Config = IuiMain.GagMenuConfig;

        private const string HeaderText = "Mouth Gag";

        private static readonly int FrostTex = Shader.PropertyToID("_FrostTex");
        private static readonly int FrostIntensity = Shader.PropertyToID("_FrostIntensity");

        internal GagMenuMain(IuiMain iuiMain, GameObject canvasGameObj) {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null) {
                throw new Exception("Expected H_Scene is null.");
            }

            var hasVisitor = hScene.visitor != null;

            var bgTextureSize = new IuiTextureGen.Size(
                Config.BackgroundRectSize.Width,
                (Config.GagToggleSize.Height
                 + Config.SubtitleFontSize
                 + Config.GroupVerticalLayoutSpacing
                 + Config.MainVerticalLayoutSpacing
                ) * (hasVisitor ? 2 : 1)
                + Config.BackgroundContainerMargins.top
                + Config.BackgroundContainerMargins.bottom
                + Config.HeaderFontSize
            );

            Textures = new GagMenuTextures(bgTextureSize);

            GagMenuRootGameObj = new GameObject("GagMenuRoot", typeof(RectTransform));
            GagMenuRootGameObj.transform.SetParent(canvasGameObj.transform, false);

            IuiPositioningHelpers.AnchorsFillParent(GagMenuRootGameObj);


            // frosted glass bg
            var frostedGlassGameObj = CreateFrostedGlass(GagMenuRootGameObj);

            var containerGameObj = CreateContentContainer(frostedGlassGameObj, bgTextureSize);

            var containerSafeMarginsGameObj = CreateContainerWithSafeMargins(containerGameObj);

            AddVerticalLayoutComponent(containerSafeMarginsGameObj);

            // x button
            CreateXButton(containerGameObj);

            CreateHeader(containerSafeMarginsGameObj);

            // main group
            var mainFemaleToggles = CreateGagSelectionGroup(
                containerSafeMarginsGameObj,
                hScene.mainMembers.GetFemale(0),
                hScene.visitor != null,
                false
            );

            // visitor group
            List<IuiToggle> visitorToggles = null;
            if (hScene.visitor != null)
                visitorToggles = CreateGagSelectionGroup(
                    containerSafeMarginsGameObj,
                    hScene.visitor.female,
                    hScene.visitor != null,
                    true
                );

            StateControl = new GagMenuStateControl(this, mainFemaleToggles, visitorToggles);

            GagMenuRootGameObj.SetActive(false);
        }

        ~GagMenuMain() {
            UnityEngine.Object.Destroy(FrostedGlassMaterial);
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
            buttonRectTransform.anchoredPosition = new Vector2(
                -Config.XButtonSize.Width,
                -Config.XButtonSize.Height
            ) * 1.1f;
            buttonRectTransform.sizeDelta = new Vector2(Config.XButtonSize.Width, Config.XButtonSize.Height);
        }

        private GameObject CreateContentContainer(GameObject root, IuiTextureGen.Size bgTextureSize) {
            var containerGameObject = new GameObject("ContentContainer", typeof(RectTransform));
            containerGameObject.transform.SetParent(root.transform, false);

            var containerRectTransform = IuiPositioningHelpers.AnchorsLeftBottom(containerGameObject);
            containerRectTransform.pivot = Vector2.zero;
            containerRectTransform.sizeDelta = new Vector2(bgTextureSize.Width, bgTextureSize.Height);

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
                    "assets/frostedglass/fonts/corbel-regular.ttf");
            textComp.fontSize = Config.HeaderFontSize;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.LowerCenter;
            textComp.color = ColorUtils.Color32Af(251, 244, 248);
        }

        private void CreateSubtitle(GameObject root, Female female) {
            var textGameObj = new GameObject("SubtitleText", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            textGameObj.transform.SetParent(root.transform, false);

            var layoutElement = textGameObj.GetComponent<LayoutElement>();
            layoutElement.minHeight = Config.HeaderFontSize;

            var textComp = textGameObj.GetComponent<Text>();
            textComp.text = female.heroineID.ToString().ToLower().ToTitleCase();
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/myriadpro-regular.otf");
            textComp.fontSize = Config.SubtitleFontSize;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.LowerLeft;
            textComp.color = ColorUtils.Color32Af(251, 244, 248, 0.5f);
        }

        private List<IuiToggle> CreateGagSelectionToggles(GameObject root, bool hasVisitor, bool isVisitor) {
            var containerGameObject =
                new GameObject("GagSelectionToggles", typeof(RectTransform), typeof(LayoutElement),
                    typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter), typeof(ToggleGroup));
            containerGameObject.transform.SetParent(root.transform, false);

            var layoutElement = containerGameObject.GetComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;
            layoutElement.minHeight = Config.GagToggleSize.Height;

            var hLayout = containerGameObject.GetComponent<HorizontalLayoutGroup>();
            hLayout.childControlWidth = true;
            hLayout.childControlHeight = true;
            hLayout.childForceExpandWidth = false;
            hLayout.childForceExpandHeight = false;
            hLayout.spacing = Config.GagToggleSpacing;

            var contentSizeFitter = containerGameObject.GetComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var toggleGroup = containerGameObject.GetComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = false;

            var items = (GAG_ITEM[])Enum.GetValues(typeof(GAG_ITEM));
            var toggles = new List<IuiToggle>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in items)
                toggles.Add(
                    new GagMenuGagSelectionToggle(
                        Textures,
                        containerGameObject,
                        toggleGroup,
                        item,
                        () => {
                            StateControl.OnUpdateGagItem(item, isVisitor);
                            if (!hasVisitor) StateControl.ToggleMenuVisibility();
                        }
                    )
                );

            return toggles;
        }

        private List<IuiToggle> CreateGagSelectionGroup(GameObject root, Female female, bool hasVisitor, bool isVisitor) {
            var groupGameObject = new GameObject("GagSelectionGroup", typeof(VerticalLayoutGroup));
            groupGameObject.transform.SetParent(root.transform, false);

            var vLayout = groupGameObject.GetComponent<VerticalLayoutGroup>();
            vLayout.childControlWidth = true;
            vLayout.childControlHeight = true;
            vLayout.childForceExpandWidth = false;
            vLayout.childForceExpandHeight = false;
            vLayout.spacing = Config.GroupVerticalLayoutSpacing;

            CreateSubtitle(groupGameObject, female);
            return CreateGagSelectionToggles(groupGameObject, hasVisitor, isVisitor);
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
    }
}
