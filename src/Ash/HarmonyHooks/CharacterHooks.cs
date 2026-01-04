using Ash.Core.SceneManagement;
using Character;
using HarmonyLib;
using System;
using Ash.Core.Features.ItemsCoordinator;

namespace Ash.HarmonyHooks
{
    internal class CharacterHooks
    {
        // Female instance is ready
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Female), nameof(Female.Awake), new Type[] { })]
        // ReSharper disable once InconsistentNaming
        public static void FemaleAwakePostfix(Female __instance) {
            // add special component to track Female destruction
            __instance.gameObject.AddComponent<ObjectDestroyTracker>().Initialize(__instance);
            SceneComponentRegistry.RegisterComponent(__instance);
        }

        // Female instance is ready
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Female), nameof(Female.Start), new Type[] { })]
        // ReSharper disable once InconsistentNaming
        public static void FemaleStartPostfix(Female __instance) {
            ItemsCoordinator.ApplyRules(__instance, RulesManager.RuleSets);
        }

        // Change wear show state hook
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Wears), nameof(Wears.ChangeShow), typeof(WEAR_SHOW_TYPE), typeof(WEAR_SHOW))]
        // ReSharper disable once InconsistentNaming
        public static void ChangeShowPostfix(Wears __instance, WEAR_SHOW_TYPE type) {
            if (__instance.human is Female female)
                ItemsCoordinator.ApplyRuleset(female, type);
        }

        // Change accessories show state hook
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Accessories), nameof(Accessories.SetShow), typeof(int), typeof(bool))]
        // ReSharper disable once InconsistentNaming
        public static void ChangeShowPostfix(Accessories __instance, int slotNo, bool show) {
            var f = __instance.wearsRoot.transform.parent.gameObject.GetComponent<Female>();
            if (f == null) {
                if (__instance.wearsRoot.transform.parent.gameObject.GetComponent<Male>() == null)
                    Ash.Logger.LogWarning(
                        "[Accessories.SetShow] The state of an accessory has changed, but the search for its Female has failed.");
                return;
            }

            // showFlags is a lookup for Accessories.GetShow() - update showFlags manually, because Accessories.SetShow() doesn't do it
            if (slotNo < AccessoryCustom.SLOT_NUM)
                __instance.showFlags[slotNo] = show;

            ItemsCoordinator.ApplyRuleset(f, slotNo);
        }

        // Change all accessories show state hook
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Accessories), nameof(Accessories.ChangeAllShow), typeof(bool))]
        // ReSharper disable once InconsistentNaming
        public static void ChangeShowPostfix(Accessories __instance, bool show) {
            if (__instance.wearsRoot.transform.parent.gameObject.GetComponent<Female>() == null) {
                if (__instance.wearsRoot.transform.parent.gameObject.GetComponent<Male>() == null)
                    Ash.Logger.LogWarning(
                        "[Accessories.ChangeAllShow] The state of an accessory has changed, but the search for its Female has failed.");
                return;
            }

            // showFlags is a lookup for Accessories.GetShow() - update showFlags manually, because Accessories.SetShow() doesn't do it
            for (var i = 0; i < __instance.showFlags.Length; i++) {
                __instance.showFlags[i] = show;
            }
        }
    }
}
