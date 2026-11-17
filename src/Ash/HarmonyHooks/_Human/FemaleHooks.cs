using System.Diagnostics.CodeAnalysis;
using Ash.Core.Features.ItemsCoordinator;
using Ash.Core.SceneManagement;
using Character;
using HarmonyLib;

namespace Ash.HarmonyHooks._Human
{
    internal class FemaleHooks
    {
        // Female instance is ready
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Female), nameof(Female.Awake))]
        // ReSharper disable once InconsistentNaming
        internal static void FemaleAwakePostfix(Female __instance) {
            // add special component to track Female destruction
            var destroyTracker = __instance.gameObject.AddComponent<ObjectDestroyTracker>();
            destroyTracker.Initialize(__instance);
            destroyTracker.OnBeforeDestroy.Add(
                    () => SceneComponentRegistry.UnregisterComponent(destroyTracker.Target)
            );

            SceneComponentRegistry.RegisterComponent(__instance);
        }

        // Female instance is ready
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Female), nameof(Female.Start))]
        // ReSharper disable once InconsistentNaming
        internal static void FemaleStartPostfix(Female __instance) {
            ItemsCoordinator.ApplyRules(__instance, RulesManager.InterItemRuleSets);
        }

        // this fixes incorrect original return result for Mariko
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Female), nameof(Female.GetVolume))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static bool GetVolumePrefix(Female __instance, ref float __result) {
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
