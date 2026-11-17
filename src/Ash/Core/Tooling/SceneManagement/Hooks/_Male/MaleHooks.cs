using HarmonyLib;

namespace Ash.Core.Tooling.SceneManagement.Hooks._Male
{
    [HarmonyPatch]
    internal class MaleHooks
    {
        // Female instance is ready
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Male), nameof(Male.Awake))]
        // ReSharper disable once InconsistentNaming
        internal static void FemaleAwakePostfix(Male __instance) {
            // add special component to track Female destruction
            var destroyTracker = __instance.gameObject.AddComponent<ObjectDestroyTracker>();
            destroyTracker.Initialize(__instance);
            destroyTracker.OnBeforeDestroy.Add(
                () => SceneComponentRegistry.UnregisterComponent(destroyTracker.Target)
            );

            SceneComponentRegistry.RegisterComponent(__instance);
        }
    }
}
