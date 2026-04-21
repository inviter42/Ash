using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.SwitchFemale
{
    internal static class SwitchFemaleButton
    {
        private const string SwitchButtonLabel = "Switch";

        // todo: add frosted bg here
        internal static void CreateSwitchFemaleButton(
            WearsMenuMain menu,
            WearsMenuConfig config,
            WearsMenuTextures textures,
            GameObject root
        ) {
            var buttonGameObj = new GameObject("SwitchFemaleButton", typeof(RectTransform), typeof(RawImage),
                typeof(Button), typeof(IuiTextState), typeof(LayoutElement));
            buttonGameObj.transform.SetParent(root.transform, false);

            var layoutElement = buttonGameObj.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = config.SwitchFemaleButtonSize.Width;
            layoutElement.preferredHeight = config.SwitchFemaleButtonSize.Height;

            var bgImage = buttonGameObj.GetComponent<RawImage>();
            bgImage.texture = textures.SwitchFemaleButtonTintTexture;
            bgImage.SetNativeSize();

            var buttonStrokeGameObj = new GameObject("Stroke", typeof(RectTransform), typeof(RawImage));
            buttonStrokeGameObj.transform.SetParent(buttonGameObj.transform, false);

            var stroke = buttonStrokeGameObj.GetComponent<RawImage>();
            stroke.texture = textures.SwitchFemaleButtonStrokeTexture;
            stroke.SetNativeSize();

            var button = buttonGameObj.GetComponent<Button>();
            button.targetGraphic = bgImage;
            // disable persistent 'selected' state after click-and-drag
            button.navigation = new Navigation { mode = Navigation.Mode.None };

            button.onClick.AddListener(delegate {
                menu.UpdateFemaleText();
                menu.UpdateContainerVisibilityAndRecalculateBounds();
            });

            IuiPositioningHelpers.AnchorsCenterIn(buttonGameObj, new Vector2(130, 0));

            var labelGameObj = new GameObject($"ButtonText", typeof(Text));
            labelGameObj.transform.SetParent(buttonGameObj.transform, false);

            var textComp = labelGameObj.GetComponent<Text>();
            textComp.text = SwitchButtonLabel;
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/corbel-regular.ttf");
            textComp.fontSize = config.SwitchFemaleButtonLabelFontSize;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = config.TextNormalColor;
            textComp.raycastTarget = false;

            var textHoverComp = buttonGameObj.GetComponent<IuiTextState>();
            textHoverComp.TargetText = textComp;
            textHoverComp.NormalColor = config.TextNormalColor;
            textHoverComp.HoverColor = config.TextHoverColor;

            IuiPositioningHelpers.AnchorsCenterIn(labelGameObj, new Vector2(0, -1));
        }
    }
}
