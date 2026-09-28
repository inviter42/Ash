using HarmonyLib;

namespace Ash.Core.Features.BetterApplicables.SharedHooks._Scenes
{
    [HarmonyPatch]
    internal class EditSceneHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EditScene), nameof(EditScene.RecordCustomData))]
        internal static bool RecordCustomDataPrefix() {
            Ash.BetterTattooDataManager.RecordExtendedData();
            Ash.BetterEyeShadowDataManager.RecordExtendedData();
            Ash.BetterCheekShadowDataManager.RecordExtendedData();
            Ash.BetterMoleDataManager.RecordExtendedData();
            return true;
        }
    }
}
