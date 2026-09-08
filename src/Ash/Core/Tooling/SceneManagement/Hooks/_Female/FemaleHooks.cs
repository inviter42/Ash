using HarmonyLib;

namespace Ash.Core.Tooling.SceneManagement.Hooks._Female
{
    internal class FemaleHooks
    {
        // Female instance is ready
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Female), nameof(Female.Awake))]
        // ReSharper disable once InconsistentNaming
        internal static void FemaleAwakePostfix(Female __instance) {
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
