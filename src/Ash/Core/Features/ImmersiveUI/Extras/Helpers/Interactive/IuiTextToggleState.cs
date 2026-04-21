using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive
{
    internal class IuiTextToggleState : IuiTextState
    {
        internal Toggle TargetToggle;

        internal Color SelectedColor = Color.white;

        private bool IsSelected;

        private void Awake() {
            ActionOnPointerEnter.Add(eventData => {
                if (IsSelected)
                    return;

                TargetText.color = HoverColor;
            });

            ActionOnPointerExit.Add(eventData => {
                if (IsSelected)
                    return;

                TargetText.color = NormalColor;
            });
        }

        private void Start() {
            TargetToggle.onValueChanged.AddListener(delegate { OnValueChanged(); });

            if (TargetToggle.isOn)
                OnValueChanged();
        }

        private void OnValueChanged() {
            if (TargetToggle == null || TargetText == null)
                return;

            IsSelected = TargetToggle.isOn;

            TargetText.color = IsSelected ? SelectedColor : NormalColor;
        }

        private void OnEnable() {
            OnValueChanged();
        }
    }
}
