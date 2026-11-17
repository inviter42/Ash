using System.Linq;
using Character;
using H;
using HarmonyLib;
using UnityEngine;

namespace Ash.Core.Features.Ahegao.Hooks._HExpressionData;

[HarmonyPatch]
internal class HExpressionDataHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_ExpressionData), nameof(H_ExpressionData.ChangeExpression), typeof(Human), typeof(string), typeof(H_Parameter), typeof(float))]
    // ReSharper disable InconsistentNaming
    internal static bool ChangeExpressionPrefix(H_ExpressionData __instance, Human human, string voiceFile, H_Parameter param, float duration, ref H_ExpressionData.Data __result) {
        if (!__instance.voice_datas.TryGetValue(voiceFile, out var voiceData)) {
            __result = null;
            return false;
        }

        var no = Random.Range(0, voiceData.Count);
        H_ExpressionData.Data data = voiceData[no];
        if (data != null) {
            if (!GlobalGameData.IsCustomEyesExpressionPresetActive((Female)human)) {
                human.ExpressionPlay(0, data.eyeType, duration);
                human.blink.LimitMax = data.eyeOpen * 0.01f;
            }

            if (!GlobalGameData.IsCustomMouthExpressionPresetActive((Female)human)) {
                human.lipSync.RelaxOpen = data.mouthOpen * 0.01f;
                var flag = param.UseMouth(human.sex);
                var name = H_ExpressionData.MouthNo(human, data, param);
                human.ExpressionPlay(1, name, duration);
                human.lipSync.Hold = flag || human.Gag;
                human.head.ResetMouth(flag || human.Gag);
                var type = !flag ? TONGUE_TYPE.FACE : TONGUE_TYPE.BODY;
                human.SetTongueType(type);
            }
        }

        __result = data;

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_ExpressionData), nameof(H_ExpressionData.ChangeExpression), typeof(Human), typeof(H_Expression.TYPE), typeof(H_Parameter), typeof(float))]
    // ReSharper disable InconsistentNaming
    internal static bool ChangeExpressionPrefix(H_ExpressionData __instance, Human human, H_Expression.TYPE type, H_Parameter param, float duration, ref H_ExpressionData.Data __result) {
        H_ExpressionData.Data data = null;
        var state1 = (H_Expression.STATE)0;
        var state2 = (H_Expression.STATE)0;
        if (human.sex == SEX.FEMALE) {
            if (human is Female female) {
                state1 = !female.IsFloped() ? H_Expression.STATE.RESIST : H_Expression.STATE.FLIP;
                state2 = state1;
                if (female.personality.ahe)
                    state2 = H_Expression.STATE.AHE;
                else if (female.personality.weakness)
                    state2 = H_Expression.STATE.WEAKNESS;
                else if (param.style is { type: H_StyleData.TYPE.INSERT }) {
                    if ((param.style.detailFlag & 16 /*0x10*/) != 0 && female.personality.vaginaVirgin)
                        state2 |= H_Expression.STATE.PAIN;
                    if ((param.style.detailFlag & 32 /*0x20*/) != 0 && female.personality.analVirgin)
                        state2 |= H_Expression.STATE.PAIN;
                }
            }
        }

        var dataList = __instance.type_datas
            .Where(typeData => typeData.state == state2 && typeData.type == type)
            .Cast<H_ExpressionData.Data>()
            .ToList();

        if (state2 != state1 && dataList.Count == 0)
            dataList.AddRange(
                __instance.type_datas
                    .Where(typeData => typeData.state == state1 && typeData.type == type)
                    .Cast<H_ExpressionData.Data>()
            );

        if (dataList.Count > 0) {
            var index = Random.Range(0, dataList.Count);
            data = dataList[index];
        }

        if (data != null) {
            if (!GlobalGameData.IsCustomEyesExpressionPresetActive((Female)human)) {
                human.ExpressionPlay(0, data.eyeType, duration);
                human.blink.LimitMax = data.eyeOpen * 0.01f;
            }

            if (!GlobalGameData.IsCustomMouthExpressionPresetActive((Female)human)) {
                human.lipSync.RelaxOpen = data.mouthOpen * 0.01f;
                var flag = param.UseMouth(human.sex);
                var name = H_ExpressionData.MouthNo(human, data, param);
                human.ExpressionPlay(1, name, duration);
                human.lipSync.Hold = flag || human.Gag;
                human.head.ResetMouth(flag || human.Gag);
                var type1 = !flag ? TONGUE_TYPE.FACE : TONGUE_TYPE.BODY;
                human.SetTongueType(type1);
            }
        } else {
            Debug.LogError("ふさわしい表情がなかった:" + state2);
        }

        __result = data;

        return false;
    }
}
