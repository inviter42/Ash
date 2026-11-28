using System;
using HarmonyLib;

namespace BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Studio;

[HarmonyPatch]
internal class StudioHooks
{
    internal static event Action StudioSceneStartedLoading;
    internal static event Action StudioSceneFinishedLoading;

    internal static event Action StudioSceneStartedImporting;
    internal static event Action StudioSceneFinishedImporting;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.LoadScene), typeof(string))]
    // ReSharper disable InconsistentNaming
    internal static void StudioLoadScenePrefix(string _path) {
        StudioSceneStartedLoading?.Invoke();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.LoadScene), typeof(string))]
    // ReSharper disable InconsistentNaming
    internal static void StudioLoadScenePostfix(string _path) {
        StudioSceneFinishedLoading?.Invoke();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.ImportScene), typeof(string))]
    // ReSharper disable InconsistentNaming
    internal static void StudioImportScenePrefix(string _path) {
        StudioSceneStartedImporting?.Invoke();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.ImportScene), typeof(string))]
    // ReSharper disable InconsistentNaming
    internal static void StudioImportScenePostfix(string _path) {
        StudioSceneFinishedImporting?.Invoke();
    }
}
