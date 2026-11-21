using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.AhegaoFaces.Hooks._HVisitorExpressionData;

[HarmonyPatch]
internal class HVisitorExpressionDataHooks
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(H_VisitorExpressionData), nameof(H_VisitorExpressionData.ChangeExpression), typeof(Human), typeof(string), typeof(float))]
    internal static IEnumerable<CodeInstruction> ChangeExpressionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il) {
        var instructionsList = instructions.ToList();
        var matcher = new CodeMatcher(instructionsList, il);

        matcher.MatchForward(false,
            new CodeMatch(i => i.IsLdloc()),
            new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S),
            new CodeMatch(OpCodes.Ldarg_1)
        );

        if (matcher.IsInvalid)
            return instructionsList;

        matcher.Advance(2);

        var skipEyesLabel = il.DefineLabel();
        var labelsA = matcher.Labels;
        var isCustomEyesActive = AccessTools.Method(typeof(GlobalGameData), nameof(GlobalGameData.IsCustomEyesExpressionPresetActive), new[] { typeof(Female) });

        matcher.Labels = new List<Label>();

        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_1) { labels = labelsA },
            new CodeInstruction(OpCodes.Isinst, typeof(Female)),
            new CodeInstruction(OpCodes.Call, isCustomEyesActive),
            new CodeInstruction(OpCodes.Brtrue, skipEyesLabel)
        ).Advance(4);

        var setBlinkLimitMax = AccessTools.PropertySetter(typeof(AnimeParamBlink), nameof(AnimeParamBlink.LimitMax));

        matcher.MatchForward(false,
            new CodeMatch(i => i.Calls(setBlinkLimitMax)),
            new CodeMatch(OpCodes.Ldarg_1)
        );

        if (matcher.IsInvalid)
            return instructionsList;

        var skipMouthLabel = il.DefineLabel();
        var isCustomMouthActive = AccessTools.Method(typeof(GlobalGameData), nameof(GlobalGameData.IsCustomMouthExpressionPresetActive), new[] { typeof(Female) });

        matcher.Advance(1)
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_1) { labels = new List<Label> { skipEyesLabel } },
                new CodeInstruction(OpCodes.Isinst, typeof(Female)),
                new CodeInstruction(OpCodes.Call, isCustomMouthActive),
                new CodeInstruction(OpCodes.Brtrue, skipMouthLabel)
            )
            .Advance(4);

        var setTongueType = AccessTools.Method(typeof(Human), nameof(Human.SetTongueType));

        matcher.MatchForward(false,
            new CodeMatch(i => i.Calls(setTongueType))
        );

        if (matcher.IsInvalid)
            return instructionsList;

        matcher.Advance(1)
            .Insert(new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { skipMouthLabel } });

        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(H_VisitorExpressionData), nameof(H_VisitorExpressionData.ChangeExpression), typeof(Human), typeof(H_VisitorExpression.TYPE), typeof(float))]
    internal static IEnumerable<CodeInstruction> ChangeExpression2Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il) {
        var instructionsList = instructions.ToList();
        var matcher = new CodeMatcher(instructionsList, il);

        matcher.MatchForward(false,
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S),
            new CodeMatch(OpCodes.Ldarg_1)
        );

        if (matcher.IsInvalid)
            return instructionsList;

        matcher.Advance(2);

        var skipCb1Label = il.DefineLabel();
        var labelsA = matcher.Labels;
        matcher.Labels = new List<Label>();
        var isCustomMouthActive = AccessTools.Method(typeof(GlobalGameData), nameof(GlobalGameData.IsCustomMouthExpressionPresetActive), new[] { typeof(Female) });

        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_1) { labels = labelsA },
            new CodeInstruction(OpCodes.Isinst, typeof(Female)),
            new CodeInstruction(OpCodes.Call, isCustomMouthActive),
            new CodeInstruction(OpCodes.Brtrue, skipCb1Label)
        ).Advance(4);

        var setTongueType = AccessTools.Method(typeof(Human), nameof(Human.SetTongueType));

        matcher.MatchForward(false,
            new CodeMatch(OpCodes.Callvirt, setTongueType),
            new CodeMatch(OpCodes.Ldarg_1)
        );

        if (matcher.IsInvalid)
            return instructionsList;

        var skipCb2Label = il.DefineLabel();
        var isCustomEyesActive = AccessTools.Method(typeof(GlobalGameData), nameof(GlobalGameData.IsCustomEyesExpressionPresetActive), new[] { typeof(Female) });

        matcher.Advance(1)
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_1) { labels = new List<Label> { skipCb1Label } },
                new CodeInstruction(OpCodes.Isinst, typeof(Female)),
                new CodeInstruction(OpCodes.Call, isCustomEyesActive),
                new CodeInstruction(OpCodes.Brtrue, skipCb2Label)
            )
            .Advance(4);

        var setBlinkLimitMax = AccessTools.PropertySetter(typeof(AnimeParamBlink), nameof(AnimeParamBlink.LimitMax));

        matcher.MatchForward(false,
            new CodeMatch(i => i.Calls(setBlinkLimitMax)),
            new CodeMatch(OpCodes.Ldarg_1)
        );

        if (matcher.IsInvalid)
            return instructionsList;

        var skipCb3Label = il.DefineLabel();

        matcher.Advance(1)
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_1) { labels = new List<Label> { skipCb2Label } },
                new CodeInstruction(OpCodes.Isinst, typeof(Female)),
                new CodeInstruction(OpCodes.Call, isCustomMouthActive),
                new CodeInstruction(OpCodes.Brtrue, skipCb3Label)
            )
            .Advance(4);

        var expressionPlay = AccessTools.Method(typeof(Human), nameof(Human.ExpressionPlay));

        matcher.MatchForward(false,
            new CodeMatch(OpCodes.Callvirt, expressionPlay)
        );

        if (matcher.IsInvalid)
            return instructionsList;

        matcher.Advance(1)
            .Insert(new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { skipCb3Label } });

        return matcher.InstructionEnumeration();
    }
}
