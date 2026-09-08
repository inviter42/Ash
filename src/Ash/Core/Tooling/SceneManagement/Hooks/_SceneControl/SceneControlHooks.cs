using HarmonyLib;

namespace Ash.Core.Tooling.SceneManagement.Hooks._SceneControl
{
    internal class SceneControlHooks
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneControl), nameof(SceneControl.SetScene), typeof(Scene))]
        // ReSharper disable once InconsistentNaming
        internal static void SetScenePostfix(SceneControl __instance) {
            SceneTypeTracker.Scene = __instance.nowScene;
        }
    }
}
