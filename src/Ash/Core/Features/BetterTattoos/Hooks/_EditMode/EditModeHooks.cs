using HarmonyLib;

namespace Ash.Core.Features.BetterTattoos.Hooks._EditMode
{
    internal static class EditModeHooks
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EditMode), nameof(EditMode.RecordCustomData))]
        internal static bool RecordCustomDataPrefix() {
            TattooDataManager.RecordTattooExtData();
            return true;
        }
    }
}
