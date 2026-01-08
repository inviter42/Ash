using System;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator;
using Ash.Core.Features.ItemsCoordinator.Types;
using Character;
using H;
using HarmonyLib;

namespace Ash.HarmonyHooks
{
    public class HMembersHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Members), nameof(H_Members.Wear), new Type[] { })]
        // ReSharper disable once InconsistentNaming
        public static bool WearPrefix(H_Members __instance) {
            if (__instance.PoseData != null || __instance.StyleData == null)
                return false;

            foreach (var female in __instance.females) {
                PerformRuleBasedWearsStripping(__instance, female);
                ItemsCoordinator.ApplyRules(female, RulesManager.InterItemRuleSets);
            }

            return false;
        }

        private static WEAR_SHOW[] GetShowData(H_Members hMembers) {
            var show1 = WEAR_SHOW.ALL; // TOPUPPER, SWIM_TOPUPPER
            var show2 = WEAR_SHOW.ALL; // TOPLOWER, BOTTOM, SWIM_TOPLOWER, SWIM_BOTTOM
            var show3 = WEAR_SHOW.ALL; // BRA, SWIMUPPER
            var show4 = WEAR_SHOW.ALL; // SHORTS, SWIMLOWER
            var show5 = WEAR_SHOW.ALL; // PANST

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (hMembers.StyleData.type) {
                case H_StyleData.TYPE.INSERT:
                {
                    show2 = WEAR_SHOW.HALF;
                    show4 = WEAR_SHOW.HALF;
                    show5 = WEAR_SHOW.HALF;

                    if ((hMembers.StyleData.detailFlag & 32 /*0x20*/) != 0) // 32 = ANAL
                        show4 = WEAR_SHOW.HIDE;
                    break;
                }

                case H_StyleData.TYPE.PETTING
                    when (hMembers.StyleData.detailFlag & 16 /*0x10*/) != 0: // 16 = VAGINA
                {
                    show2 = WEAR_SHOW.HALF;
                    show4 = WEAR_SHOW.HALF;
                    show5 = WEAR_SHOW.HALF;
                    break;
                }

                case H_StyleData.TYPE.PETTING:
                {
                    if ((hMembers.StyleData.detailFlag & 32 /*0x20*/) != 0) { // 32 = ANAL
                        show2 = WEAR_SHOW.HALF;
                        show4 = WEAR_SHOW.HIDE;
                        show5 = WEAR_SHOW.HALF;
                    }

                    break;
                }

                case H_StyleData.TYPE.SERVICE
                    when (hMembers.StyleData.detailFlag & 128 /*0x80*/) != 0: // 128 = TITTY_FUCK
                {
                    show1 = WEAR_SHOW.HALF;
                    show3 = WEAR_SHOW.HALF;
                    show2 = WEAR_SHOW.HALF;
                    break;
                }

                case H_StyleData.TYPE.SERVICE:
                {
                    show2 = WEAR_SHOW.HALF;
                    break;
                }

                default:
                {
                    Ash.Logger.LogError($"Unknown style type {hMembers.StyleData.type}");
                    return null;
                }
            }

            return new[] { show1, show2, show3, show4, show5 };
        }

        private static bool AllowedToStripType(H_Members hMembers, Female female, WEAR_SHOW_TYPE type) {
            var activeWearObj = female.wears.wearObjs[(int)Wears.ShowToWearType[(int)type]];
            if (activeWearObj == null)
                return false;

            var hPosStyle = hMembers.StyleData.type;
            var detailFlag = hMembers.StyleData.detailFlag;

            var compareItems = new Func<BaseItem, WearObj, bool>((item1, item2) => {
                var i2WearData = female.wears.GetWearData(item2.type);
                return item1.Id == i2WearData.id
                       && item1.AssetbundleName == i2WearData.assetbundleName
                       && item1.Prefab == i2WearData.prefab
                       && item1.Name == i2WearData.name
                       && item1.ItemData.AsT0.ItemPart == type;
            });

            return !RulesManager.HPosRuleSets.Any(rule => {
                var itemCheck = rule.HPosItem.IsT0
                    ? compareItems(rule.HPosItem.AsT0, activeWearObj)
                    : rule.HPosItem.AsT1 == type;

                var styleCheck = rule.HPosStyle.IsT0
                    ? rule.HPosStyle.AsT0 == hPosStyle
                    : rule.HPosStyle.AsT1.Type == hPosStyle && ((int)rule.HPosStyle.AsT1.Detail & detailFlag) != 0
                      || (int)rule.HPosStyle.AsT1.Type == -2 && (int)rule.HPosStyle.AsT1.Detail == -2;

                return itemCheck && styleCheck;
            });
        }

        private static void PerformRuleBasedWearsStripping(H_Members instance, Female female) {
            var showData = GetShowData(instance);
            if (showData == null)
                return;

            var show1 = showData[0];
            var show2 = showData[1];
            var show3 = showData[2];
            var show4 = showData[3];
            var show5 = showData[4];

            // [Service > Titty_Fuck]
                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.TOPUPPER))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.TOPUPPER, show1);

                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.SWIM_TOPUPPER))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.SWIM_TOPUPPER, show1);

                // [Insert]
                // [Petting > Vagina, Petting > Anal]
                // [Service, Service > Titty_Fuck]
                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.TOPLOWER))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.TOPLOWER, show2);

                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.BOTTOM))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.BOTTOM, show2);

                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.SWIM_TOPLOWER))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.SWIM_TOPLOWER, show2);

                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.SWIM_BOTTOM))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.SWIM_BOTTOM, show2);

                // [Service > Titty_Fuck]
                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.BRA))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.BRA, show3);

                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.SWIMUPPER))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.SWIMUPPER, show3);

                // [Insert, Insert > Anal]
                // [Petting > Vagina, Petting > Anal]
                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.SHORTS))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.SHORTS, show4);

                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.SWIMLOWER))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.SWIMLOWER, show4);

                // [Insert]
                // [Petting > Vagina, Petting > Anal]
                if (AllowedToStripType(instance, female, WEAR_SHOW_TYPE.PANST))
                    female.wears.ChangeShow_StripOnly(WEAR_SHOW_TYPE.PANST, show5);

                female.CheckShow();
        }
    }
}
