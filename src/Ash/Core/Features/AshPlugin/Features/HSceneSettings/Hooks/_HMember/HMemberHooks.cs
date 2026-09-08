using System.Linq;
using H;
using HarmonyLib;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Ash.Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HMember
{
    internal class HMemberHooks
    {
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
    }
}
