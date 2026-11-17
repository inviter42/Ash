using HarmonyLib;

namespace Ash.Core.Features.Ahegao.Hooks._Female;

[HarmonyPatch]
internal class FemaleHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Female), nameof(Female.ChangeShowGag), typeof(bool))]
    // ReSharper disable once InconsistentNaming
    internal static void ChangeShowGagPrefix(Female __instance, bool flag) {
        __instance.gagShow = flag;
        var reset = false;

        if (__instance.gagItem != null && __instance.gagItem.obj != null) {
            __instance.gagItem.obj.SetActive(__instance.gagShow);
            reset = __instance.gagShow;
        }

        if (__instance.lipSync != null)
            __instance.lipSync.Hold = !GlobalGameData.IsCustomMouthExpressionPresetActive(__instance) && reset;

        __instance.head?.ResetMouth(reset);
        __instance.RePlayMouthExpression();
    }
}
