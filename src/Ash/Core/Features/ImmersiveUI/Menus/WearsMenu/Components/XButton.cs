using System;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures;
using Ash.GlobalUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components
{
    internal static class XButton
    {
        // todo: add frosted bg here
        internal static void CreateXButton(
            WearsMenuConfig config,
            WearsMenuTextures textures,
            Action onCloseButtonClicked,
            GameObject root
        ) {
            var buttonGameObj = new GameObject("XButton", typeof(RectTransform), typeof(RawImage), typeof(Button),
                typeof(IuiPointerEventsHandler));
            buttonGameObj.transform.SetParent(root.transform, false);

            var bgImage = buttonGameObj.GetComponent<RawImage>();
            bgImage.texture = textures.CloseButtonTintTexture;
            bgImage.SetNativeSize();

            var buttonStrokeGameObj = new GameObject("Stroke", typeof(RectTransform), typeof(RawImage));
            buttonStrokeGameObj.transform.SetParent(buttonGameObj.transform, false);

            var stroke = buttonStrokeGameObj.GetComponent<RawImage>();
            stroke.texture = textures.CloseButtonStrokeTexture;
            stroke.SetNativeSize();

            var button = buttonGameObj.GetComponent<Button>();
            button.targetGraphic = bgImage;
            // disable persistent 'selected' state after click-and-drag
            button.navigation = new Navigation { mode = Navigation.Mode.None };

            button.onClick.AddListener(delegate { onCloseButtonClicked(); });

            var xImageGameObj = new GameObject("XImage", typeof(RectTransform), typeof(RawImage));
            xImageGameObj.transform.SetParent(buttonGameObj.transform, false);

            var xImage = xImageGameObj.GetComponent<RawImage>();
            xImage.texture = textures.CloseButtonXTexture;
            xImage.SetNativeSize();

            ImageUtils.SetRawImageTransparency(xImage, 0.5f);

            var cursorHandler = buttonGameObj.GetComponent<IuiPointerEventsHandler>();
            cursorHandler.ActionOnPointerEnter.Add(eventData => ImageUtils.SetRawImageTransparency(xImage, 1));
            cursorHandler.ActionOnPointerExit.Add(eventData => ImageUtils.SetRawImageTransparency(xImage, 0.5f));

            IuiPositioningHelpers.AnchorsRightTop(buttonGameObj, config.XButtonOffset);
        }
    }
}
