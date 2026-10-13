using System.Diagnostics.CodeAnalysis;
using Character;
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

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(H_Members), nameof(H_Members.AddMale), typeof(MALE_ID))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static void AddMalePostfix(H_Members __instance, ref Male __result) {
            Ash.BoneInjector.TryInjectBones(__result);
            UpdateMaleBallsVisibility(__result);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(H_Members), nameof(H_Members.DelChara), typeof(Male))]
        internal static void DelCharaPostfix(Male male) {
            Ash.BoneInjector.PruneInjectorsList(male);
        }

        private static void UpdateMaleBallsVisibility(Male male) {
            male.tinRoot.transform
                .FindChild("cm_O_dan_f").gameObject
                .SetActive(Ash.PersistentSettings.ShowMaleSackValue.Value);
        }
    }
}
