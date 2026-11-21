using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.AhegaoFaces.Hooks._Female;

[HarmonyPatch]
internal class FemaleHooks
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Female), nameof(Female.ChangeShowGag))]
    internal static IEnumerable<CodeInstruction> ChangeShowGagTranspiler(IEnumerable<CodeInstruction> instructions) {
        var setHold = AccessTools.PropertySetter(typeof(AnimeLipSync), nameof(AnimeLipSync.Hold));
        var isPresetActive = AccessTools.Method(
            typeof(GlobalGameData),
            nameof(GlobalGameData.IsCustomMouthExpressionPresetActive),
            new[] { typeof(Female) }
        );

        return new CodeMatcher(instructions)
            .MatchStartForward(new CodeMatch(i => i.Calls(setHold)))
            .ThrowIfInvalid("Human.lipSync.Hold setter call not found")
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, isPresetActive),
                new CodeInstruction(OpCodes.Or))
            .InstructionEnumeration();
    }
}
