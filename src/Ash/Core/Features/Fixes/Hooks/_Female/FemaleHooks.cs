using System.Diagnostics.CodeAnalysis;
using Ash.Core.Tooling.SceneManagement;
using Character;
using HarmonyLib;

namespace Ash.Core.Features.Fixes.Hooks._Female
{
    internal class FemaleHooks
    {
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
