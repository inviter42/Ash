using System;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.GlobalUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Components.Generic
{
    internal static class RoundButtonWithIcon
    {
        internal static GameObject CreateRoundButtonWithIcon(
            GameObject root,
            Action onClick,
            Texture2D iconTexture,
            Vector2 iconSize,
            Vector2 iconPosition,
            float iconTransparency
        ) {
            var buttonGameObj = RoundButton.CreateRoundButton(root, onClick);

            var iconGameObj = new GameObject("Icon", typeof(RectTransform), typeof(RawImage));
            iconGameObj.transform.SetParent(buttonGameObj.transform, false);

            var icon = iconGameObj.GetComponent<RawImage>();
            icon.texture = iconTexture;

            iconGameObj.GetComponent<RectTransform>().sizeDelta = iconSize;

            IuiPositioningHelpers.AnchorsCenterIn(iconGameObj, iconPosition);

            ImageUtils.SetRawImageTransparency(icon, iconTransparency);

            var cursorHandler = buttonGameObj.GetComponent<IuiPointerEventsHandler>();
            cursorHandler.ActionOnPointerEnter.Add(eventData => ImageUtils.SetRawImageTransparency(icon, 1));
            cursorHandler.ActionOnPointerExit.Add(eventData => ImageUtils.SetRawImageTransparency(icon, iconTransparency));

            return buttonGameObj;
        }
    }
}
