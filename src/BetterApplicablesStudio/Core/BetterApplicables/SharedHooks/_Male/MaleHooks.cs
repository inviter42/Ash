using System;
using HarmonyLib;

namespace BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Male
{
    [HarmonyPatch]
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
