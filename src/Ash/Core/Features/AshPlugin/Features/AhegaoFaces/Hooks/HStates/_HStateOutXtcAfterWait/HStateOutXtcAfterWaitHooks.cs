using Ash.Core.Features.Actions;
using H;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.AhegaoFaces.Hooks.HStates._HStateOutXtcAfterWait;

[HarmonyPatch]
internal class HStateOutXtcAfterWaitHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_State_OutXtcAfterWait), nameof(H_State_OutXtcAfterWait.In), typeof(H_State.Message))]
    // ReSharper disable once InconsistentNaming
    internal static bool HStateOutXtcAfterWaitInPostfix(H_State_OutXtcAfterWait __instance) {
        AhegaoFaceAction.DeactivateAhegaoExpression(__instance.members.GetFemale(0));
        return true;
    }
}
