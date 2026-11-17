using H;
using HarmonyLib;

namespace Ash.HarmonyHooks._H._HState
{
    internal class HStateExitHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_State_Exit), nameof(H_State_Exit.In), typeof(H_State.Message))]
        // ReSharper disable once InconsistentNaming
        internal static bool InPrefix(H_State_Exit __instance, H_State.Message msg) {
            var styleData = __instance.members.StyleData;
            if (styleData != null) {
                var flag = false;
                switch (styleData.type) {
                    case H_StyleData.TYPE.PETTING:
                        break;
                    case H_StyleData.TYPE.INSERT:
                        flag = true;
                        break;
                    case H_StyleData.TYPE.SERVICE:
                        flag = styleData.IsInMouth();
                        break;
                }

                if (flag) {
                    if ((styleData.detailFlag & 4096 /*0x1000*/) != 0)
                        __instance.members.PlayAnime("InEja_Base", 0.5f);
                    else if (__instance.members.StateMgr.PrevStateID == H_STATE.PRE_INSERT_WAIT)
                        __instance.members.PlayAnime("PreInsertWait", 0.5f);
                    else
                        __instance.members.PlayAnime("OutEja_Base", 0.5f);
                }
                else
                    __instance.members.PlayAnime("Wait", 0.5f);
            }

            __instance.members.param.mouth = H_MOUTH.FREE;
            __instance.members.EnableTinIK = false;

            if (!Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneEnd.Value)
                __instance.members.VoiceExpression(H_Voice.TYPE.EXIT);

            __instance.members.MaleExpression(H_Expression_Male.TYPE.NORMAL);

            __instance.exit = false;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_State_Exit), nameof(H_State_Exit.Main))]
        // ReSharper disable once InconsistentNaming
        internal static bool MainPrefix(H_State_Exit __instance) {
            // this should ideally check if Breath and nothing else is playing, but the voice system is so complicated,
            // and the code quality is so bad, I just can't make sense of it. No idea how to check for it.
            if (__instance.exit
                || !__instance.members.CheckEndVoice()
                   && !Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneEnd.Value
            )
                return false;

            __instance.members.h_scene.Exit();
            __instance.exit = true;

            return false;
        }
    }
}
