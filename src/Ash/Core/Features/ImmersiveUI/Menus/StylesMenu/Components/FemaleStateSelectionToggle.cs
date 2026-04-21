using System;
using Ash.Core.Features.ImmersiveUI.Extras.Extensions;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures;
using Ash.GlobalUtils;
using H;
using UnityEngine;
using UnityEngine.UI;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Components
{
    internal class FemaleStateSelectionToggle : IuiToggle
    {
        private static readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;

        private RawImage ToggleStrokeRawImage;

        private static readonly Color TextNormalColor = ColorUtils.Color32Af(251, 244, 248, 0.55f);
        private static readonly Color TextHoverColor = ColorUtils.Color32Af(251, 244, 248, 0.85f);

        internal FemaleStateSelectionToggle(
            StylesMenuTextures textures,
            GameObject root,
            ToggleGroup toggleGroup,
            H_StyleData.STATE state,
            Action actionOnValueChanged
        ) : base(root, toggleGroup) {
            var toggleBackgroundRawImage = AddToggleBackgroundRawImage(ToggleGameObject, textures.FemaleStateToggleTintTexture);

            var toggleStrokeGameObject = new GameObject("Stroke", typeof(RectTransform), typeof(RawImage));
            toggleStrokeGameObject.transform.SetParent(ToggleGameObject.transform, false);

            AddToggleStrokeRawImage(toggleStrokeGameObject, textures.FemaleStateToggleStrokeTexture);

            InitializeToggleComponent(toggleBackgroundRawImage, actionOnValueChanged);

            AddToggleRectTransform(ToggleComponent);

            AddLayoutComponent(ToggleGameObject);

            CreateTextComponent(ToggleGameObject, state);

            ImageUtils.SetRawImageTransparency(ToggleStrokeRawImage,
                StylesMenuConfig.SideToggleInactiveStrokeTransparencyValue);
        }

        private RawImage AddToggleBackgroundRawImage(GameObject toggleGameObject, Texture2D femaleStateToggleTintTexture) {
            var toggleBackgroundRawImage = toggleGameObject.GetComponent<RawImage>();
            toggleBackgroundRawImage.texture = femaleStateToggleTintTexture;
            toggleBackgroundRawImage.SetNativeSize();

            return toggleBackgroundRawImage;
        }

        private void AddToggleStrokeRawImage(GameObject toggleStrokeGameObject, Texture2D femaleStateToggleStrokeTexture) {
            ToggleStrokeRawImage = toggleStrokeGameObject.GetComponent<RawImage>();
            ToggleStrokeRawImage.texture = femaleStateToggleStrokeTexture;
            ToggleStrokeRawImage.SetNativeSize();
        }

        private void InitializeToggleComponent(
            RawImage toggleBackgroundRawImage,
            Action actionOnValueChanged
        ) {
            ToggleComponent.targetGraphic = toggleBackgroundRawImage;
            ToggleComponent.onValueChanged.AddListener(delegate {
                if (ToggleComponent.isOn)
                    actionOnValueChanged();

                ImageUtils.SetRawImageTransparency(
                    ToggleStrokeRawImage,
                    ToggleComponent.isOn
                        ? StylesMenuConfig.SideToggleActiveStrokeTransparencyValue
                        : StylesMenuConfig.SideToggleInactiveStrokeTransparencyValue
                );
            });
        }

        private void AddToggleRectTransform(Toggle toggleComponent) {
            var toggleRectTransform = toggleComponent.GetComponent<RectTransform>();
            toggleRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            toggleRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            toggleRectTransform.pivot = new Vector2(0.5f, 0.5f);
            toggleRectTransform.sizeDelta = new Vector2(Config.FemaleStateToggleSize.Width, Config.FemaleStateToggleSize.Height);
        }

        private void AddLayoutComponent(GameObject toggleGameObject) {
            var layoutElement = toggleGameObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = Config.FemaleStateToggleSize.Width;
            layoutElement.preferredHeight = Config.FemaleStateToggleSize.Height;
        }

        private void CreateTextComponent(GameObject toggleGameObject, H_StyleData.STATE state) {
            var toggleLabelGameObj = new GameObject($"ToggleText", typeof(Text));
            toggleLabelGameObj.transform.SetParent(toggleGameObject.transform, false);

            var textComp = toggleLabelGameObj.GetComponent<Text>();
            textComp.text = HFemaleStateLabels.GetValueOrDefaultValue(state, ErrorLabel);
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/corbel-regular.ttf");
            textComp.fontSize = Config.FemaleStateToggleLabelFontSize;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = TextNormalColor;
            textComp.raycastTarget = false;

            var toggle = toggleGameObject.GetComponent<Toggle>();

            var textHoverComp = toggleGameObject.GetComponent<IuiTextToggleState>();
            textHoverComp.TargetToggle = toggle;
            textHoverComp.TargetText = textComp;
            textHoverComp.NormalColor = TextNormalColor;
            textHoverComp.HoverColor = TextHoverColor;

            IuiPositioningHelpers.AnchorsFillParent(toggleLabelGameObj);
        }
    }
}
