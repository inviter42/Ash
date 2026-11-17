using HarmonyLib;

namespace Ash.HarmonyHooks._Scene
{
    internal class IllusionCameraHooks
    {
        // disable default code exec if RadialMenu is visible
        [HarmonyPrefix]
        [HarmonyPatch(typeof(IllusionCamera), "LateUpdate")]
        // ReSharper disable once InconsistentNaming
        internal static bool LateUpdatePrefix(IllusionCamera __instance) {
            return Ash.AshUI.IuiMain != null && !Ash.AshUI.IuiMain.IsRadialMenuVisible
                   || Ash.AshUI.IuiMain == null;
        }
    }
}
