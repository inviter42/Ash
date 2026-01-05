using Ash.Core.Features.ItemsCoordinator;
using HarmonyLib;

namespace Ash.HarmonyHooks
{
    internal class AccessoriesHooks
    {
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
