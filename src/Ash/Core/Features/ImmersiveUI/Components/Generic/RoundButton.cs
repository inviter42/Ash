using System;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Components.Generic
{
    internal static class RoundButton
    {
        internal static GameObject CreateRoundButton(
            GameObject root,
            Action onClick
        ) {
            var buttonGameObj = new GameObject("RoundButton", typeof(RectTransform), typeof(RawImage), typeof(Button),
                typeof(IuiPointerEventsHandler));
            buttonGameObj.transform.SetParent(root.transform, false);

            var bgImage = buttonGameObj.GetComponent<RawImage>();
            bgImage.texture = IuiMain.SharedTextures.RoundButtonTintTexture;
            bgImage.SetNativeSize();

            var buttonStrokeGameObj = new GameObject("Stroke", typeof(RectTransform), typeof(RawImage));
            buttonStrokeGameObj.transform.SetParent(buttonGameObj.transform, false);

            var stroke = buttonStrokeGameObj.GetComponent<RawImage>();
            stroke.texture = IuiMain.SharedTextures.RoundButtonStrokeTexture;
            stroke.SetNativeSize();

            var button = buttonGameObj.GetComponent<Button>();
            button.targetGraphic = bgImage;
            // disable persistent 'selected' state after click-and-drag
            button.navigation = new Navigation { mode = Navigation.Mode.None };

            button.onClick.AddListener(delegate{ onClick?.Invoke(); });

            return buttonGameObj;
        }
    }
}
