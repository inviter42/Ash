using Ash.Core.Features.Actions;
using H;
using HarmonyLib;

namespace Ash.Core.Features.Ahegao.Hooks.HStates._HStateInXtcAfterWait;

[HarmonyPatch]
internal class HStateInXtcAfterWaitHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_State_InXtcAfterWait), nameof(H_State_InXtcAfterWait.In), typeof(H_State.Message))]
    // ReSharper disable once InconsistentNaming
    internal static bool HStateInXtcAfterWaitInPostfix(H_State_InXtcAfterWait __instance) {
        AhegaoFaceAction.DeactivateAhegaoExpression(__instance.members.GetFemale(0));
        return true;
    }
}
