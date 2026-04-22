using System;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures;
using Ash.GlobalUtils;
using H;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Components.ScrollView
{
    internal static class StylesScrollViewButton
    {
        private static readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;

        private static readonly Color TextNormalColor = ColorUtils.Color32Af(251, 244, 248, 0.55f);
        private static readonly Color TextHoverColor = ColorUtils.Color32Af(251, 244, 248, 0.85f);

        internal static GameObject CreateScrollListButton(
            StylesMenuTextures textures,
            RectTransform contentRectTransform,
            H_StyleData styleData,
            Action actionOnPointerOver,
            Action actionOnClick
        ) {
            var buttonGameObj = new GameObject($"StyleButton", typeof(Button), typeof(LayoutElement), typeof(Image), typeof(IuiTextState));

            buttonGameObj.transform.SetParent(contentRectTransform, false);

            AddLayoutElement(buttonGameObj);

            var buttonImage = AddButtonBackground(buttonGameObj, textures.ScrollListButtonBackgroundSprite);
            var buttonComp = AddButtonComponent(buttonGameObj, buttonImage, actionOnPointerOver, actionOnClick);

            SetColorBlock(buttonComp);

            var buttonRectTransform = AddButtonRectTransform(buttonComp);

            CreateTextComponent(buttonRectTransform, styleData, buttonGameObj);

            return buttonGameObj;
        }

        private static void AddLayoutElement(GameObject buttonGameObj) {
            var layoutElement = buttonGameObj.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = Config.ScrollListButtonSize.Width;
            layoutElement.preferredHeight = Config.ScrollListButtonSize.Height;
        }

        private static Button AddButtonComponent(
            GameObject buttonGameObj,
            Image buttonImage,
            Action actionOnPointerOver,
            Action actionOnClick
        ) {
            var buttonComp = buttonGameObj.GetComponent<Button>();
            buttonComp.transition = Selectable.Transition.ColorTint;
            buttonComp.targetGraphic = buttonImage;
            // disable persistent 'selected' state after click-and-drag
            buttonComp.navigation = new Navigation { mode = Navigation.Mode.None };

            var stateHandler = buttonGameObj.GetComponent<IuiTextState>();
            stateHandler.ActionOnPointerEnter.Add(eventData => actionOnPointerOver());

            buttonComp.onClick.AddListener(delegate { actionOnClick(); });

            return buttonComp;
        }

        private static Image AddButtonBackground(GameObject buttonGameObj, Sprite scrollListButtonBackgroundSprite) {
            var buttonImage = buttonGameObj.GetComponent<Image>();
            buttonImage.sprite = scrollListButtonBackgroundSprite;
            buttonImage.type = Image.Type.Sliced;
            buttonImage.raycastTarget = true;

            return buttonImage;
        }

        private static void SetColorBlock(Button buttonComponent) {
            var colorBlock = ColorBlock.defaultColorBlock;
            colorBlock.normalColor = Config.ScrollListButtonNormalColor;
            colorBlock.highlightedColor = Config.ScrollListButtonHighlightedColor;
            colorBlock.pressedColor = Config.ScrollListButtonPressedColor;
            colorBlock.fadeDuration = Config.ScrollListButtonFadeDuration;

            buttonComponent.colors = colorBlock;
        }

        private static RectTransform AddButtonRectTransform(Button buttonComponent) {
            var buttonRectTransform = buttonComponent.GetComponent<RectTransform>();
            buttonRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRectTransform.pivot = new Vector2(0.5f, 0.5f);
            buttonRectTransform.sizeDelta = new Vector2(Config.ScrollListButtonSize.Width, Config.ScrollListButtonSize.Height);

            return buttonRectTransform;
        }

        private static void CreateTextComponent(RectTransform buttonRectTransform, H_StyleData styleData, GameObject buttonGameObj) {
            var buttonLabelGameObj = new GameObject($"ButtonText", typeof(Text));
            buttonLabelGameObj.transform.SetParent(buttonRectTransform, false);

            var translatedLabel = AutoTranslatorIntegration.Translate(styleData.name);

            var textComp = buttonLabelGameObj.GetComponent<Text>();
            textComp.text = translatedLabel;
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/candara-light.ttf");
            textComp.fontSize = Config.ScrollListButtonLabelFontSize;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = TextNormalColor;
            textComp.raycastTarget = false;

            var textHoverComp = buttonGameObj.GetComponent<IuiTextState>();
            textHoverComp.TargetText = textComp;
            textHoverComp.NormalColor = TextNormalColor;
            textHoverComp.HoverColor = TextHoverColor;

            IuiPositioningHelpers.AnchorsFillParent(buttonLabelGameObj);
        }
    }
}
