using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator;
using Ash.Core.Features.ItemsCoordinator.Types;
using Character;
using H;
using HarmonyLib;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Ash.HarmonyHooks._H
{
    internal class HMembersHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Members), nameof(H_Members.Wear))]
        // ReSharper disable once InconsistentNaming
        internal static bool WearPrefix(H_Members __instance) {
            if (__instance.PoseData != null || __instance.StyleData == null)
                return false;

            foreach (var female in __instance.females) {
                PerformRuleBasedWearsStripping(__instance, female);
                ItemsCoordinator.ApplyRules(female, RulesManager.InterItemRuleSets);
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Members), nameof(H_Members.ChangeStyle), typeof(H_StyleData))]
        // ReSharper disable once InconsistentNaming
        internal static bool ChangeStylePrefix(H_Members __instance, H_StyleData data) {
            if (!Ash.PersistentSettings.DisableFemaleHVoiceBark.Value)
                return true;

            __instance.PoseData = null;
            foreach (var itemObject in __instance._itemObjects.Where(itemObject => itemObject != null))
                UObject.Destroy(itemObject.gameObject);

            __instance._itemObjects.Clear();
            if ((data.detailFlag & 1024 /*0x0400*/) != 0) {
                if ((data.detailFlag & 16 /*0x10*/) != 0) {
                    var gameObject =
                        AssetBundleLoader.LoadAndInstantiate<GameObject>(GlobalData.assetBundlePath, "h/h_item",
                            "p_item_vibe_01");
                    var transform =
                        Transform_Utility.FindTransform(__instance.females[0].body.Anime.transform, "k_f_kokan_00");
                    var component = gameObject.GetComponent<H_Item>();
                    component.SetTarget(transform);
                    __instance._itemObjects.Add(component);
                }

                if ((data.detailFlag & 32 /*0x20*/) != 0) {
                    var gameObject =
                        AssetBundleLoader.LoadAndInstantiate<GameObject>(GlobalData.assetBundlePath, "h/h_item",
                            "p_item_analvibe");
                    var transform =
                        Transform_Utility.FindTransform(__instance.females[0].body.Anime.transform, "k_f_ana_00");
                    var component = gameObject.GetComponent<H_Item>();
                    component.SetTarget(transform);
                    __instance._itemObjects.Add(component);
                }
            }

            var set1 = (data.detailFlag & 2048 /*0x0800*/) != 0;
            foreach (var male in __instance.males)
                male.ChangeRestrict(set1);

            if ((data.detailFlag & 8192 /*0x2000*/) != 0) {
                var component = AssetBundleLoader
                    .LoadAndInstantiate<GameObject>(GlobalData.assetBundlePath, "h/h_item", "p_item_holder")
                    .GetComponent<H_Item>();
                component.SetTarget(__instance.Transform);
                __instance._itemObjects.Add(component);
            }

            var set2 = (data.detailFlag & 16384 /*0x4000*/) != 0;
            foreach (var female in __instance.females)
                female.ChangeRestrict(set2);

            __instance.MemberAdjust(data.member);

            var strArray1 = new[] { "M", "N", "O", "P" };
            var strArray2 = new[] { "F", "G" };
            var bundleController = new AssetBundleController(false);
            bundleController.OpenFromFile(GlobalData.assetBundlePath, data.assetBundle);

            for (var index = 0; index < __instance.males.Count; ++index)
                __instance.males[index].body.Anime.runtimeAnimatorController =
                    bundleController.LoadAsset<RuntimeAnimatorController>($"AC_{data.id}_{strArray1[index]}");

            for (var index = 0; index < __instance.females.Count; ++index)
                __instance.females[index].body.Anime.runtimeAnimatorController =
                    bundleController.LoadAsset<RuntimeAnimatorController>($"AC_{data.id}_{strArray2[index]}");

            __instance.mapIK.runtimeAnimatorController =
                bundleController.LoadAsset<RuntimeAnimatorController>($"AC_{data.id}_H");
            var strArray3 = new[] { "_I", "_J" };
            for (var index = 0; index < __instance._itemObjects.Count; ++index) {
                var itemObject = __instance._itemObjects[index];
                if (itemObject == null)
                    continue;

                if (index >= strArray3.Length)
                    Debug.LogError("予想外です");
                itemObject.animator.runtimeAnimatorController =
                    bundleController.LoadAsset<RuntimeAnimatorController>($"AC_{data.id}{strArray3[index]}");
            }

            bundleController.Close();
            Resources.UnloadUnusedAssets();

            var flag1 = __instance.StyleData != null && __instance.StyleData.position == data.position;
            __instance.StyleData = data;
            if (!flag1) {
                __instance.h_scene.VisitorPos(__instance.SetDataPos());
                __instance.h_scene.CharaMove.SetDef(__instance.Transform.position, __instance.Transform.rotation);
            }

            if (__instance.StyleData.hasLight)
                __instance.h_scene.SetLightDir(__instance.StyleData.lightEuler);

            __instance.param.mouth = H_MOUTH.FREE;
            __instance.param.style = data;
            __instance.param.continuanceXTC_F = 0;

            foreach (var female in __instance.females) {
                var flag2 = true;
                if (data.type != H_StyleData.TYPE.PETTING)
                    flag2 = (data.detailFlag & H_StyleData.DetailMasking_UseMouth) == 0;
                female.ChangeShowGag(flag2);
            }

            __instance.VoiceExpression(H_Voice.TYPE.BREATH);

            __instance.Wear();

            var flag3 = (__instance.StyleData.detailFlag & 384) != 0;
            foreach (var female in __instance.females) {
                female.body.bustDynamicBone_L.enabled = !flag3;
                female.body.bustDynamicBone_R.enabled = !flag3;
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Members), nameof(H_Members.VoiceExpression), typeof(Female), typeof(H_VoiceLog), typeof(H_Voice.TYPE))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static bool VoiceExpressionPrefix(H_Members __instance, ref bool __result, Female female, H_VoiceLog voiceLog, H_Voice.TYPE voice) {
            var voice1 = __instance.h_scene.Voice(female, voiceLog, voice, __instance);

            if (voice1 != null) {
                if (voice1.priority > 0)
                    voiceLog.AddPriorityTalk(voice1.File);
                else
                    voiceLog.AddPant(voice1.File);

                if (Ash.PersistentSettings.FixIncorrectShowMouthLiquidState.Value) {
                    if ((__instance.param.detail & H_Parameter.DETAIL.SHOW_ORAL) == H_Parameter.DETAIL.NO)
                        __instance.ExpressionFromVoice(female, voice1);
                }
                else {
                    __instance.ExpressionFromVoice(female, voice1);
                }
                __result = true;

                return false;
            }

            if (Ash.PersistentSettings.FixIncorrectShowMouthLiquidState.Value) {
                if ((__instance.param.detail & H_Parameter.DETAIL.SHOW_ORAL) == H_Parameter.DETAIL.NO)
                    female.ExpressionPlay(1, "Mouth_Def", 0.2f);
            }
            else {
                female.ExpressionPlay(1, "Mouth_Def", 0.2f);
            }
            __result = false;

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
