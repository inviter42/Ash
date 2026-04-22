using H;
using HarmonyLib;
using UnityEngine;

namespace Ash.HarmonyHooks._H._HState
{
    internal class HStateLoopHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_State_Loop), "OnInput")]
        // ReSharper disable once InconsistentNaming
        internal static bool OnInputPrefix(H_State_Loop __instance, H_INPUT input) {
            if (!Ash.PersistentSettings.SkippingSpurtStateEnabled.Value)
                return true;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (input) {
                case H_INPUT.EJA_IN:
                    __instance.members.param.xtcType = XTC_TYPE.EJA_IN;
                    __instance.ChangeState(H_STATE.IN_EJA_IN, null);
                    break;
                case H_INPUT.EJA_OUT:
                    __instance.members.param.xtcType = XTC_TYPE.EJA_OUT;
                    __instance.ChangeState(H_STATE.OUT_EJA_IN, null);
                    break;
                case H_INPUT.XTC_F:
                    __instance.members.param.xtcType = XTC_TYPE.XTC_F;
                    __instance.ChangeState(H_STATE.XTC_F_IN, null);
                    break;
                case H_INPUT.XTC_W:
                    __instance.members.param.xtcType = XTC_TYPE.XTC_W;
                    __instance.ChangeState(H_STATE.XTC_W_IN, null);
                    break;
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_State_Loop), "Main")]
        // ReSharper disable once InconsistentNaming
        internal static bool MainPrefix(H_State_Loop __instance) {
            var speed = __instance.members.input.Speed;
            var num = speed <= 0.0
                ? Mathf.Lerp(1f, __instance.minSpeed, -speed)
                : Mathf.Lerp(1f, __instance.maxSpeed, speed);
            var pose = __instance.members.input.Pose;
            var stroke = __instance.members.input.Stroke;

            __instance.members.SetLoopPose(pose);
            __instance.members.SetLoopStroke(stroke);
            __instance.members.SetSpeed(num);

            var styleData = __instance.members.StyleData;
            __instance.members.AddGage(
                styleData.type == H_StyleData.TYPE.INSERT || styleData.type == H_StyleData.TYPE.PETTING,
                styleData.type == H_StyleData.TYPE.INSERT || styleData.type == H_StyleData.TYPE.SERVICE
            );

            if (__instance.members.FemaleGageVal >= 1.0 && !Ash.PersistentSettings.DisableFemaleAutoEjaculation.Value) {
                if (Ash.PersistentSettings.SkippingSpurtStateEnabled.Value)
                    __instance.ChangeState(H_STATE.XTC_F_IN, null);
                else
                    __instance.ChangeState(H_STATE.SPURT, new H_State_Spurt.SpurtMsg(XTC_TYPE.XTC_F));
            } else {
                var forced = false;
                if (__instance.members.param.speed != H_SPEED.NORMAL &&
                    __instance.checkSpeed != __instance.members.param.speed) {
                    forced = true;
                    __instance.checkSpeed = __instance.members.param.speed;
                }

                __instance.UpdateVoice(forced);
                __instance.UpdateSE(num);
                __instance.UpdateVisitor();
            }

            return false;
        }
    }
}
