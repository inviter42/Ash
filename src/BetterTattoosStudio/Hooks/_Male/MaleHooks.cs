using System;
using HarmonyLib;

namespace BetterTattoosStudio.Hooks._Male
{
    internal class MaleHooks
    {
        internal static event Action<Male> MaleIsBeingApplied;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Male), nameof(Male.Apply))]
        internal static bool MaleApplyPrefix(Male __instance) {
            MaleIsBeingApplied?.Invoke(__instance);
            return true;
        }
    }
}
