using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Components
{
    internal static class StyleThumbnail
    {
        private static readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;

        internal static GameObject AddThumbnailComponent(GameObject root) {
            var imgGameObject = new GameObject("Thumbnail", typeof(RawImage));
            imgGameObject.transform.SetParent(root.transform, false);

            var rt = IuiPositioningHelpers.AnchorsFillParent(imgGameObject);
            rt.sizeDelta = Config.ThumbnailPadding;

            return imgGameObject;
        }
    }
}
