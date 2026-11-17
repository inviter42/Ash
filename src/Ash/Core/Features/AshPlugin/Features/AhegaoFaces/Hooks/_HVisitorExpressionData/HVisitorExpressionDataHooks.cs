using System.Linq;
using Character;
using HarmonyLib;
using UnityEngine;

namespace Ash.Core.Features.Ahegao.Hooks._HVisitorExpressionData;

[HarmonyPatch]
internal class HVisitorExpressionDataHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_VisitorExpressionData), nameof(H_VisitorExpressionData.ChangeExpression), typeof(Human), typeof(string), typeof(float))]
    // ReSharper disable InconsistentNaming
    internal static bool ChangeExpressionVisitorPrefix(H_VisitorExpressionData __instance, Human human, string voiceFile, float duration, ref H_VisitorExpressionData.Data __result) {
        if (!__instance.voice_datas.TryGetValue(voiceFile, out var voiceData)) {
            __result = null;
            return false;
        }

        var no = Random.Range(0, voiceData.Count);
        H_VisitorExpressionData.Data data = voiceData[no];
        if (data != null) {
            if (!GlobalGameData.IsCustomEyesExpressionPresetActive((Female)human)) {
                human.ExpressionPlay(0, data.eyeType, duration);
                human.blink.LimitMax = data.eyeOpen * 0.01f;
            }

            if (!GlobalGameData.IsCustomMouthExpressionPresetActive((Female)human)) {
                human.lipSync.RelaxOpen = data.mouthOpen * 0.01f;
                var mouthType = data.mouthType;
                human.ExpressionPlay(1, mouthType, duration);
                human.lipSync.Hold = human.Gag;
                human.head.ResetMouth(human.Gag);
                const TONGUE_TYPE type = TONGUE_TYPE.FACE;
                human.SetTongueType(type);
            }
        }

        __result = data;

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_VisitorExpressionData), nameof(H_VisitorExpressionData.ChangeExpression), typeof(Human), typeof(H_VisitorExpression.TYPE), typeof(float))]
    // ReSharper disable InconsistentNaming
    internal static bool ChangeExpressionVisitorPrefix(H_VisitorExpressionData __instance, Human human, H_VisitorExpression.TYPE type, float duration, ref H_VisitorExpressionData.Data __result) {
        if (human is not Female female)
            return false;

        H_VisitorExpressionData.Data data = null;
        var state = (H_VisitorExpression.STATE) 0;
        if (female.sex == SEX.FEMALE)
            state = !female.IsFloped() ? H_VisitorExpression.STATE.RESIST : H_VisitorExpression.STATE.FLIP;

        var dataList = __instance.type_datas
            .Where(typeData => (typeData.state & state) != 0 && typeData.type == type)
            .Cast<H_VisitorExpressionData.Data>()
            .ToList();

        if (dataList.Count > 0) {
            var index = Random.Range(0, dataList.Count);
            data = dataList[index];
        }

        if (data != null) {
            if (!GlobalGameData.IsCustomEyesExpressionPresetActive(female)) {
                human.ExpressionPlay(0, data.eyeType, duration);
                human.blink.LimitMax = data.eyeOpen * 0.01f;
            }

            if (!GlobalGameData.IsCustomMouthExpressionPresetActive(female)) {
                human.lipSync.Hold = human.Gag;
                human.head.ResetMouth(human.Gag);
                human.SetTongueType(TONGUE_TYPE.FACE);
                human.lipSync.RelaxOpen = data.mouthOpen * 0.01f;
                human.ExpressionPlay(1, data.mouthType, duration);
            }
        }
        else
            Debug.LogError("ふさわしい表情がなかった:" + state);

        __result = data;

        return false;
    }
}
