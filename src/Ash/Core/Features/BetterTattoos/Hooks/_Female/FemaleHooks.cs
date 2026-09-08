using System;
using HarmonyLib;

namespace Ash.Core.Features.BetterTattoos.Hooks._Female
{
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
