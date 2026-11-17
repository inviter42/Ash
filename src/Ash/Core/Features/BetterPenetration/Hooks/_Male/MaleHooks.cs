using HarmonyLib;

namespace Ash.Core.Features.BetterPenetration.Hooks._Male
{
    [HarmonyPatch]
    internal class MaleHooks
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Male), nameof(Male.Update))]
        // ReSharper disable once InconsistentNaming
        internal static void MaleUpdatePostfix(Male __instance) {
            Ash.BoneInjector.UpdateIKIndependentBonesTransforms(__instance);
        }
    }
}
