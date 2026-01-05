using Ash.Core.SceneManagement;
using Character;
using HarmonyLib;
using System;
using Ash.Core.Features.Common.Misc;
using Ash.Core.Features.HSceneControls;
using Ash.Core.Features.ItemsCoordinator;

namespace Ash.HarmonyHooks
{
    internal class FemaleHooks
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

        // this fixes incorrect original return result for Mariko
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Female), nameof(Female.GetVolume), new Type[] { })]
        // ReSharper disable once InconsistentNaming
        public static bool GetVolumePrefix(Female __instance, ref float __result) {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (__instance.heroineID) {
                case HEROINE.RITSUKO:
                    __result = ConfigData.VolumeVoice_Ritsuko();
                    break;
                case HEROINE.AKIKO:
                    __result = ConfigData.VolumeVoice_Akiko();
                    break;
                case HEROINE.YUKIKO:
                    __result = ConfigData.VolumeVoice_Yukiko();
                    break;
                case HEROINE.MARIKO:
                    __result = ConfigData.VolumeVoice_Mariko();
                    break;
                default:
                    __result = 0.0f;
                    break;
            }

            // apply custom volume modifier
            __result *= SceneDataModifiers.GetFemaleVolumeModifier(__instance.heroineID);

            // return false to skip the original method
            return false;
        }
    }
}
