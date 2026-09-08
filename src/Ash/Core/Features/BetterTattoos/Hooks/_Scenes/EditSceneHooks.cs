using HarmonyLib;

namespace Ash.Core.Features.BetterTattoos.Hooks._Scenes
{
    internal class EditSceneHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EditScene), nameof(EditScene.RecordCustomData))]
        internal static bool RecordCustomDataPrefix() {
            TattooDataManager.RecordTattooExtData();
            return true;
        }
    }
}
