using Ash.Core.Features.ImmersiveUI;
using HarmonyLib;

namespace Ash.HarmonyHooks._Menus
{
    internal class ConfigMenuHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Config), nameof(Config.Close))]
        // ReSharper disable once InconsistentNaming
        internal static bool ClosePrefix() {
            if (Ash.AshUI.IuiMain == null || IuiMain.IsEditModeRendered)
                return true;

            Ash.AshUI.IuiMain.CanvasGameObj.SetActive(true);

            return true;
        }
    }
}
