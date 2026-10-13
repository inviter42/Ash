using H;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.AnimationControls.Hooks._MixController
{
    [HarmonyPatch]
    internal class MixControllerHooks
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MixController), nameof(MixController.Start))]
        // ReSharper disable once InconsistentNaming
        internal static void MixControllerPostfix(MixController __instance) {
            __instance.wheelSpeed = Ash.PersistentSettings.AnimationWheelSpeedValue.Value;
        }
    }
}
