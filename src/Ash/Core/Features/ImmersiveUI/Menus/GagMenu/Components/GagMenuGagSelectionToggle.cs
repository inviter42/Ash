using System;
using Ash.Core.Features.ImmersiveUI.Extras.Extensions;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Textures;
using Ash.GlobalUtils;
using Character;
using UnityEngine;
using UnityEngine.UI;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Components
{
    internal class GagMenuGagSelectionToggle : IuiToggle
    {
        private RawImage ToggleStrokeRawImage;

        private static readonly GagMenuConfig Config = IuiMain.GagMenuConfig;

        internal GagMenuGagSelectionToggle(
            GagMenuTextures textures,
            GameObject root,
            ToggleGroup toggleGroup,
            GAG_ITEM item,
            Action actionOnValueChanged
        ) : base(root, toggleGroup) {
            var toggleStrokeGameObject = new GameObject("Stroke", typeof(RectTransform), typeof(RawImage));
            toggleStrokeGameObject.transform.SetParent(ToggleGameObject.transform, false);

            AddToggleStrokeRawImage(toggleStrokeGameObject, textures.GagToggleStrokeTexture);

            InitializeToggleComponent(actionOnValueChanged);

            AddToggleRectTransform(ToggleComponent);

            AddLayoutComponent(ToggleGameObject);

            CreateTextComponent(ToggleGameObject, item);

            ImageUtils.SetRawImageTransparency(ToggleStrokeRawImage,
                GagMenuConfig.ToggleInactiveStrokeTransparencyValue);
        }

        private void AddToggleStrokeRawImage(GameObject toggleStrokeGameObject, Texture2D gagToggleStrokeTexture) {
            ToggleStrokeRawImage = toggleStrokeGameObject.GetComponent<RawImage>();
            ToggleStrokeRawImage.texture = gagToggleStrokeTexture;
            ToggleStrokeRawImage.SetNativeSize();
        }

        private void InitializeToggleComponent(
            Action actionOnValueChanged
        ) {
            ToggleGameObject.GetComponent<RawImage>().color = Color.clear;
            ToggleComponent.onValueChanged.AddListener(delegate {
                if (ToggleComponent.isOn)
                    actionOnValueChanged();

                ImageUtils.SetRawImageTransparency(
                    ToggleStrokeRawImage,
                    ToggleComponent.isOn
                        ? GagMenuConfig.ToggleActiveStrokeTransparencyValue
                        : GagMenuConfig.ToggleInactiveStrokeTransparencyValue
                );
            });
        }

        private void AddToggleRectTransform(Toggle toggleComponent) {
            var toggleRectTransform = toggleComponent.GetComponent<RectTransform>();
            toggleRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            toggleRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            toggleRectTransform.pivot = new Vector2(0.5f, 0.5f);
            toggleRectTransform.sizeDelta = new Vector2(Config.GagToggleSize.Width, Config.GagToggleSize.Height);
        }

        private void AddLayoutComponent(GameObject toggleGameObject) {
            var layoutElement = toggleGameObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = Config.GagToggleSize.Width;
            layoutElement.preferredHeight = Config.GagToggleSize.Height;
        }

        private void CreateTextComponent(GameObject toggleGameObject, GAG_ITEM item) {
            var toggleLabelGameObj = new GameObject($"ToggleText", typeof(Text));
            toggleLabelGameObj.transform.SetParent(toggleGameObject.transform, false);

            var textNormalColor = ColorUtils.Color32Af(251, 244, 248, 0.55f);
            var textHoverColor = ColorUtils.Color32Af(251, 244, 248, 0.85f);
            var textSelectedColor = ColorUtils.Color32Af(251, 244, 248);

            var textComp = toggleLabelGameObj.GetComponent<Text>();
            textComp.text = GagItemLabels.GetValueOrDefaultValue(item, ErrorLabel);
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/corbel-regular.ttf");
            textComp.fontSize = Config.GagToggleLabelFontSize;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = textNormalColor;
            textComp.raycastTarget = false;

            var toggle = toggleGameObject.GetComponent<Toggle>();

            var textHoverComp = toggleGameObject.GetComponent<IuiTextToggleState>();
            textHoverComp.TargetToggle = toggle;
            textHoverComp.TargetText = textComp;
            textHoverComp.NormalColor = textNormalColor;
            textHoverComp.HoverColor = textHoverColor;
            textHoverComp.SelectedColor = textSelectedColor;

            IuiPositioningHelpers.AnchorsFillParent(toggleLabelGameObj);
        }
    }
}
