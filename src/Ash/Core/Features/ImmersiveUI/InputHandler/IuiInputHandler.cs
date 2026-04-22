using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.InputHandler
{
    internal static class IuiInputHandler
    {
        internal static bool IsEscapeReleased() {
            return GlobalUtils.HotkeyUtils.HotkeyIsUp(KeyCode.Escape);
        }
    }
}
