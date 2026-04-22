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
    internal class GenreSelectionToggle : IuiToggle
    {
        private static readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;

        private RawImage ToggleStrokeRawImage;

        internal GenreSelectionToggle(
            StylesMenuTextures textures,
            GameObject root,
            ToggleGroup toggleGroup,
            H_StyleData.TYPE type,
            Action actionOnValueChanged
        ) : base(root, toggleGroup) {
            var toggleBackgroundRawImage = AddToggleBackgroundRawImage(ToggleGameObject, textures.GenreToggleTintTexture);

            var toggleStrokeGameObject = new GameObject("Stroke", typeof(RectTransform), typeof(RawImage));
            toggleStrokeGameObject.transform.SetParent(ToggleGameObject.transform, false);

            AddToggleStrokeRawImage(toggleStrokeGameObject, textures.GenreToggleStrokeTexture);

            InitializeToggleComponent(toggleBackgroundRawImage, actionOnValueChanged);

            AddToggleRectTransform(ToggleComponent);

            AddLayoutComponent(ToggleGameObject);

            CreateTextComponent(ToggleGameObject, type);

            ImageUtils.SetRawImageTransparency(ToggleStrokeRawImage,
                StylesMenuConfig.SideToggleInactiveStrokeTransparencyValue);
        }

        private RawImage AddToggleBackgroundRawImage(GameObject toggleGameObject, Texture2D genreToggleTintTexture) {
            var toggleBackgroundRawImage = toggleGameObject.GetComponent<RawImage>();
            toggleBackgroundRawImage.texture = genreToggleTintTexture;
            toggleBackgroundRawImage.SetNativeSize();

            return toggleBackgroundRawImage;
        }

        private void AddToggleStrokeRawImage(GameObject toggleStrokeGameObject, Texture2D genreToggleStrokeTexture) {
            ToggleStrokeRawImage = toggleStrokeGameObject.GetComponent<RawImage>();
            ToggleStrokeRawImage.texture = genreToggleStrokeTexture;
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
            toggleRectTransform.sizeDelta = new Vector2(Config.GenreToggleSize.Width, Config.GenreToggleSize.Height);
        }

        private void AddLayoutComponent(GameObject toggleGameObject) {
            var layoutElement = toggleGameObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = Config.GenreToggleSize.Width;
            layoutElement.preferredHeight = Config.GenreToggleSize.Height;
        }

        private void CreateTextComponent(GameObject toggleGameObject, H_StyleData.TYPE type) {
            var toggleLabelGameObj = new GameObject($"ToggleText", typeof(Text));
            toggleLabelGameObj.transform.SetParent(toggleGameObject.transform, false);

            var textNormalColor = ColorUtils.Color32Af(251, 244, 248, 0.55f);
            var textHoverColor = ColorUtils.Color32Af(251, 244, 248, 0.85f);
            var textSelectedColor = ColorUtils.Color32Af(251, 244, 248);

            var textComp = toggleLabelGameObj.GetComponent<Text>();
            textComp.text = HStylesLabels.GetValueOrDefaultValue(type, ErrorLabel);
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/corbel-regular.ttf");
            textComp.fontSize = Config.GenreToggleLabelFontSize;
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
