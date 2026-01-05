using Ash.Core.Features.ItemsCoordinator;
using Character;
using HarmonyLib;

namespace Ash.HarmonyHooks
{
    internal class WearsHooks
    {
        // Change wear show state hook
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Wears), nameof(Wears.ChangeShow), typeof(WEAR_SHOW_TYPE), typeof(WEAR_SHOW))]
        // ReSharper disable once InconsistentNaming
        public static void ChangeShowPostfix(Wears __instance, WEAR_SHOW_TYPE type) {
            if (__instance.human is Female female)
                ItemsCoordinator.ApplyRuleset(female, type);
        }
    }
}
