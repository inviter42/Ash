using Ash.GlobalUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive
{
    internal class IuiTextState : IuiPointerEventsHandler
    {
        internal Text TargetText;

        internal Color NormalColor = ColorUtils.Color32Af(255, 255, 255, 0.5f);
        internal Color HoverColor = ColorUtils.Color32Af(255, 255, 255, 0.85f);

        private void Awake() {
            ActionOnPointerEnter.Add(eventData => {
                if (TargetText == null)
                    return;
                TargetText.color = HoverColor;
            });

            ActionOnPointerExit.Add(eventData => {
                if (TargetText == null)
                    return;

                TargetText.color = NormalColor;
            });

            ActionOnPointerClick.Add(eventData => {
                if (eventData.dragging)
                    return;

                TargetText.color = NormalColor;
            });
        }
    }
}
