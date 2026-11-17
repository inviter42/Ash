using HarmonyLib;

namespace Ash.Core.Features.Ahegao.Hooks._Human;

[HarmonyPatch]
internal class HumanHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Human), nameof(Human.RePlayMouthExpression))]
    // ReSharper disable once InconsistentNaming
    internal static bool RePlayMouthExpressionPrefix(Human __instance) {
        if (!GlobalGameData.IsCustomMouthExpressionPresetActive((Female)__instance))
            return true;

        __instance.ExpressionPlay(1, __instance.mouthState, 0.4f);
        __instance.OpenMouth(__instance.mouthOpen);

        return false;
    }
}
