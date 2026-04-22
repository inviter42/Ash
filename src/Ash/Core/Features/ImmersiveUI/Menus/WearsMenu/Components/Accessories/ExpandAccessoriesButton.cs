using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures;
using Ash.GlobalUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.Accessories
{
    internal static class ExpandAccessoriesButton
    {
        // todo: add frosted bg here
        internal static void CreateExpandAccessoriesButton(
            WearsMenuMain menu,
            WearsMenuConfig config,
            WearsMenuTextures textures,
            GameObject root,
            GameObject accessoryContainer,
            GameObject frostedGlassGameObject) {
            var buttonGameObj = new GameObject("ExpandAccessoriesButton", typeof(RectTransform), typeof(RawImage), typeof(Button),
                typeof(IuiPointerEventsHandler));
            buttonGameObj.transform.SetParent(root.transform, false);

            var bgImage = buttonGameObj.GetComponent<RawImage>();
            bgImage.texture = textures.ExpandAccessoriesButtonTintTexture;
            bgImage.SetNativeSize();

            var buttonStrokeGameObj = new GameObject("Stroke", typeof(RectTransform), typeof(RawImage));
            buttonStrokeGameObj.transform.SetParent(buttonGameObj.transform, false);

            var stroke = buttonStrokeGameObj.GetComponent<RawImage>();
            stroke.texture = textures.ExpandAccessoriesStrokeTexture;
            stroke.SetNativeSize();

            var button = buttonGameObj.GetComponent<Button>();
            button.targetGraphic = bgImage;
            // disable persistent 'selected' state after click-and-drag
            button.navigation = new Navigation { mode = Navigation.Mode.None };

            button.onClick.AddListener(delegate {
                var newValue = !accessoryContainer.activeSelf;
                accessoryContainer.SetActive(newValue);
                frostedGlassGameObject.SetActive(newValue);

                menu.RecalculateButtonsContainerBoundingBox();
            });

            var iconGameObj = new GameObject("Icon", typeof(RectTransform), typeof(RawImage));
            iconGameObj.transform.SetParent(buttonGameObj.transform, false);

            var icon = iconGameObj.GetComponent<RawImage>();
            icon.texture = Ash.AshUI.ImmersiveUIIconsAssetBundle.LoadAsset<Texture2D>(
                "assets/frostedglass/icons/small-arrow-right.png");

            iconGameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(
                config.ExpandAccessoriesButtonIconSize.Width,
                config.ExpandAccessoriesButtonIconSize.Height
            );

            IuiPositioningHelpers.AnchorsCenterIn(iconGameObj, new Vector2(1, 0));

            ImageUtils.SetRawImageTransparency(icon, 0.5f);

            var cursorHandler = buttonGameObj.GetComponent<IuiPointerEventsHandler>();
            cursorHandler.ActionOnPointerEnter.Add(eventData => ImageUtils.SetRawImageTransparency(icon, 1));
            cursorHandler.ActionOnPointerExit.Add(eventData => ImageUtils.SetRawImageTransparency(icon, 0.5f));

            IuiPositioningHelpers.AnchorsCenterIn(buttonGameObj, config.ExpandAccessoriesButtonOffset);
        }
    }
}
