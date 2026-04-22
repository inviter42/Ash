using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Extras.Extensions
{
    internal class IuiToggle
    {
        protected readonly Toggle ToggleComponent;
        protected readonly GameObject ToggleGameObject;

        protected IuiToggle(GameObject root, ToggleGroup toggleGroup) {
            ToggleGameObject = new GameObject("Toggle", typeof(RectTransform), typeof(RawImage),
                typeof(LayoutElement), typeof(Toggle), typeof(IuiTextToggleState));
            ToggleGameObject.transform.SetParent(root.transform, false);

            ToggleComponent = ToggleGameObject.GetComponent<Toggle>();
            ToggleComponent.group = toggleGroup;
        }

        internal void ToggleOn() {
            ToggleComponent.isOn = true;
        }

        internal void ToggleOff() {
            ToggleComponent.isOn = false;
        }
    }
}
