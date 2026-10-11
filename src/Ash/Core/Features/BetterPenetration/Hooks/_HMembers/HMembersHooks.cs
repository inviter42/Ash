using Ash.Core.Tooling.SceneManagement;
using H;
using HarmonyLib;

namespace Ash.Core.Features.BetterPenetration.Hooks._HMembers
{
    [HarmonyPatch]
    internal class HMembersHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Members), nameof(H_Members.PlayAnime), typeof(string), typeof(float))]
        // ReSharper disable once InconsistentNaming
        internal static bool PlayAnimePrefix(H_Members __instance, string name, float transitionDuration) {
            Ash.Logger.LogDebug($"PlayAnime '{name}', transitionDuration: {transitionDuration}");

            foreach(var male in __instance.males)
                Ash.BoneInjector.ResetBoneTransforms(male);

            UpdateMaleBallsVisibility();

            return true;
        }

        private static void UpdateMaleBallsVisibility() {
            if (SceneTypeTracker.Scene is H_Scene hScene) {
                foreach (var male in hScene.mainMembers.males)
                    male.tinRoot.transform.FindChild("cm_O_dan_f").gameObject.SetActive(Ash.PersistentSettings.ShowMaleSackValue.Value);
            }
        }
    }
}
