using Ash.Utility.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.InputHandler
{
    internal static class IuiInputHandler
    {
        internal static bool IsEscapeReleased() {
            return HotkeyUtils.HotkeyIsUp(KeyCode.Escape);
        }
    }
}
