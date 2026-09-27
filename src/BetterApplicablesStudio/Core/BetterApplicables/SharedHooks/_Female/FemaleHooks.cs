using System;
using HarmonyLib;

namespace BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Female
{
    [HarmonyPatch]
    internal class FemaleHooks
    {
        internal static event Action<Female> FemaleIsBeingApplied;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Female), nameof(Female.Apply))]
        internal static bool FemaleApplyPrefix(Female __instance) {
            FemaleIsBeingApplied?.Invoke(__instance);
            return true;
        }
    }
}
