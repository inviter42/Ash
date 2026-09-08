using System.Diagnostics.CodeAnalysis;
using H;
using HarmonyLib;

namespace Ash.Core.Features.Fixes.Hooks._HMember
{
    internal class HMembersHooks
    {
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
                    // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
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
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                if ((__instance.param.detail & H_Parameter.DETAIL.SHOW_ORAL) == H_Parameter.DETAIL.NO)
                    female.ExpressionPlay(1, "Mouth_Def", 0.2f);
            }
            else {
                female.ExpressionPlay(1, "Mouth_Def", 0.2f);
            }
            __result = false;

            return false;
        }

    }
}
