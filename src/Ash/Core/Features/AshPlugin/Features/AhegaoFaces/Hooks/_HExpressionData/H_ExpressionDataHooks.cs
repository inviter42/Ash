using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.AhegaoFaces.Hooks._HExpressionData;

[HarmonyPatch]
internal class HExpressionDataHooks
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(H_ExpressionData), nameof(H_ExpressionData.ChangeExpression), typeof(Human), typeof(string), typeof(H_Parameter), typeof(float))]
    internal static IEnumerable<CodeInstruction> ChangeExpressionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il) {
        var skipEyesLabel = il.DefineLabel();
        var isEyesPresetActive = AccessTools.Method(typeof(GlobalGameData), nameof(GlobalGameData.IsCustomEyesExpressionPresetActive), new[] { typeof(Female) });
        var expressionPlay = AccessTools.Method(typeof(Human), nameof(Human.ExpressionPlay), new[] { typeof(int), typeof(string), typeof(float) });

        var matcher = new CodeMatcher(instructions, il);
        matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_1),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(i => i.IsLdloc()),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(i => i.IsLdarg()),
                new CodeMatch(i => i.Calls(expressionPlay))
            )
            .ThrowIfInvalid("Could not find start of eyes block")
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Castclass, typeof(Female)),
                new CodeInstruction(OpCodes.Call, isEyesPresetActive),
                new CodeInstruction(OpCodes.Brtrue, skipEyesLabel)
            );

        var skipMouthLabel = il.DefineLabel();
        var getLipSync = AccessTools.PropertyGetter(typeof(Human), nameof(Human.lipSync));
        var setTongueType = AccessTools.Method(typeof(Human), nameof(Human.SetTongueType));
        var isMouthPresetActive = AccessTools.Method(typeof(GlobalGameData), nameof(GlobalGameData.IsCustomMouthExpressionPresetActive), new[] { typeof(Female) });

        matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_1),
                new CodeMatch(i => i.Calls(getLipSync)),
                new CodeMatch(i => i.IsLdloc()),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_R4, 0.01f)
            )
            .ThrowIfInvalid("Could not find start of mouth block")
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(skipEyesLabel),
                new CodeInstruction(OpCodes.Castclass, typeof(Female)),
                new CodeInstruction(OpCodes.Call, isMouthPresetActive),
                new CodeInstruction(OpCodes.Brtrue, skipMouthLabel)
            );

        matcher.MatchEndForward(new CodeMatch(i => i.Calls(setTongueType)))
            .ThrowIfInvalid("Could not find SetTongueType call")
            .AddLabelsAt(matcher.Pos + 1, new[] { skipMouthLabel });

        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(H_ExpressionData), nameof(H_ExpressionData.ChangeExpression), typeof(Human), typeof(H_Expression.TYPE), typeof(H_Parameter), typeof(float))]
    public static IEnumerable<CodeInstruction> ChangeExpression2Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il) {
        var skipEyesLabel = il.DefineLabel();
        var isEyesPresetActive = AccessTools.Method(typeof(GlobalGameData), nameof(GlobalGameData.IsCustomEyesExpressionPresetActive), new[] { typeof(Female) });
        var expressionPlay = AccessTools.Method(typeof(Human), nameof(Human.ExpressionPlay), new[] { typeof(int), typeof(string), typeof(float) });
        var matcher = new CodeMatcher(instructions, il);

        matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_1),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(i => i.IsLdloc()),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(i => i.IsLdarg()),
                new CodeMatch(i => i.Calls(expressionPlay))
            )
            .ThrowIfInvalid("Could not find start of eyes block")
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Castclass, typeof(Female)),
                new CodeInstruction(OpCodes.Call, isEyesPresetActive),
                new CodeInstruction(OpCodes.Brtrue, skipEyesLabel)
            );

        var skipMouthLabel = il.DefineLabel();
        var isMouthPresetActive = AccessTools.Method(typeof(GlobalGameData), nameof(GlobalGameData.IsCustomMouthExpressionPresetActive), new[] { typeof(Female) });
        var getLipSync = AccessTools.PropertyGetter(typeof(Human), nameof(Human.lipSync));
        var setTongueType = AccessTools.Method(typeof(Human), nameof(Human.SetTongueType));

        matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_1),
                new CodeMatch(i => i.Calls(getLipSync)),
                new CodeMatch(i => i.IsLdloc()),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_R4, 0.01f)
            )
            .ThrowIfInvalid("Could not find start of mouth block")
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(skipEyesLabel),
                new CodeInstruction(OpCodes.Castclass, typeof(Female)),
                new CodeInstruction(OpCodes.Call, isMouthPresetActive),
                new CodeInstruction(OpCodes.Brtrue, skipMouthLabel)
            );

        matcher.MatchEndForward(
                new CodeMatch(i => i.Calls(setTongueType)),
                new CodeMatch(i => i.Branches(out _))
            )
            .ThrowIfInvalid("Could not find end of if block after SetTongueType")
            .AddLabels(new[] { skipMouthLabel });

        return matcher.InstructionEnumeration();
    }
}
