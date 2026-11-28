using System;
using HarmonyLib;

namespace BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Female;

[HarmonyPatch]
internal class FemaleHooks
{
    internal static event Action<Female> FemaleIsAwaken;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Female), nameof(Female.Awake))]
    // ReSharper disable once InconsistentNaming
    internal static void FemaleAwakePostfix(Female __instance) {
        FemaleIsAwaken?.Invoke(__instance);
    }
}
