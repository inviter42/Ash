using Ash.Core.Features.ImmersiveUI;
using HarmonyLib;

namespace Ash.HarmonyHooks._Menus
{
    public class PauseMenueHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PauseMenue), nameof(PauseMenue.Open))]
        // ReSharper disable once InconsistentNaming
        internal static bool OpenPrefix() {
            if (Ash.AshUI.IuiMain == null || IuiMain.IsConfigMenuRendered || IuiMain.IsEditModeRendered)
                return true;

            if (Ash.AshUI.IuiMain.IsAnySubMenuVisible || Ash.AshUI.IuiMain.IsRadialMenuVisible)
                return false;

            Ash.AshUI.IuiMain.CanvasGameObj.SetActive(false);

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PauseMenue), nameof(PauseMenue.Close))]
        // ReSharper disable once InconsistentNaming
        internal static bool ClosePrefix() {
            if (Ash.AshUI.IuiMain == null || IuiMain.IsConfigMenuRendered || IuiMain.IsEditModeRendered)
                return true;

            Ash.AshUI.IuiMain.CanvasGameObj.SetActive(true);

            return true;
        }
    }
}
