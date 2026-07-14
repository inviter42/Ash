using H;
using HarmonyLib;

namespace Ash.HarmonyHooks._H._HState
{
    internal class HStateInXtcAfterBase
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_State_AfterBase), nameof(H_State_AfterBase.In), typeof(H_State.Message))]
        // ReSharper disable once InconsistentNaming
        internal static bool InPrefixBase(H_State_AfterBase __instance, H_State.Message msg) {
            var females = __instance.members.GetFemales();
            if (__instance.breath.Length != females.Count)
                __instance.breath = new bool[females.Count];
            var flag1 = __instance.VisitorVoice();
            for (var femaleNo = 0; femaleNo < females.Count; ++femaleNo) {
                var flag2 = false;
                if (!flag1 && !Ash.PersistentSettings.DisableFemaleVoiceBarkAfterEjaIn.Value)
                    flag2 = __instance.members.VoiceExpression(femaleNo, H_Voice.TYPE.XTC_AFTER_TALK);
                if (!flag2) {
                    __instance.members.VoiceExpression(femaleNo, H_Voice.TYPE.XTC_AFTER_BREATH);
                    __instance.breath[femaleNo] = true;
                }
            }

            __instance.members.MaleExpression(H_Expression_Male.TYPE.NORMAL);

            return false;
        }
    }
}
