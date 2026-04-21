using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.GlobalUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components
{
    internal static class ItemButtonBase
    {
        internal static RawImage AddButtonBackground(GameObject buttonGameObj, Texture texture) {
            var iconBg = buttonGameObj.GetComponent<RawImage>();
            iconBg.rectTransform.SetParent(buttonGameObj.transform, false);
            iconBg.texture = texture;
            iconBg.SetNativeSize();

            return iconBg;
        }

        internal static void AddButtonComponent(GameObject buttonGameObj, RawImage buttonImage) {
            var buttonComp = buttonGameObj.GetComponent<Button>();
            buttonComp.targetGraphic = buttonImage;
            // disable persistent 'selected' state after click-and-drag
            buttonComp.navigation = new Navigation { mode = Navigation.Mode.None };
        }

        internal static RawImage AddHoverStrokeElement(GameObject root, Texture texture) {
            var hoverStrokeGameObj = new GameObject("Stroke", typeof(RectTransform), typeof(RawImage));
            hoverStrokeGameObj.transform.SetParent(root.transform, false);

            var stroke = hoverStrokeGameObj.GetComponent<RawImage>();
            stroke.texture = texture;
            stroke.SetNativeSize();

            IuiPositioningHelpers.AnchorsCenterIn(hoverStrokeGameObj);

            ImageUtils.SetRawImageTransparency(stroke, 0f);

            return stroke;
        }
    }
}
