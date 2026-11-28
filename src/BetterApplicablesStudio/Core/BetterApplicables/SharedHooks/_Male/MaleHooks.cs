using System;
using HarmonyLib;

namespace BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Male;

[HarmonyPatch]
internal class MaleHooks
{
    internal static event Action<Male> MaleIsAwaken;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Male), nameof(Male.Awake))]
    // ReSharper disable once InconsistentNaming
    internal static void MaleAwakePostfix(Male __instance) {
        MaleIsAwaken?.Invoke(__instance);
    }
}
