using HarmonyLib;

namespace Ash.Core.Features.BetterApplicables.SharedHooks._EditMode
{
    [HarmonyPatch]
    internal static class EditModeHooks
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EditMode), nameof(EditMode.RecordCustomData))]
        internal static bool RecordCustomDataPrefix() {
            Ash.BetterTattooDataManager.RecordExtendedData();
            Ash.BetterEyeShadowDataManager.RecordExtendedData();
            return true;
        }
    }
}
