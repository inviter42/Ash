using Ash.Core.SceneManagement;
using HarmonyLib;

namespace Ash.HarmonyHooks._Scene
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
