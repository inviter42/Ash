using UnityEngine;

namespace Ash.Core.Features.AshPlugin.Main.State
{
    internal static class AshUIStateControl
    {
        internal static void UpdateState() {
            UpdateCursorLockMode();
        }

        private static void UpdateCursorLockMode() {
            Cursor.lockState = Ash.PersistentSettings.ConfineCursorToWindowEnabled.Value
                ? CursorLockMode.Confined
                :  CursorLockMode.None;
        }
    }
}
