using Ash.Core.Features.Actions;
using H;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.AhegaoFaces.Hooks.HStates._HStateXtcWin;

[HarmonyPatch]
internal class HStateXtcWinHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_State_XTC_W_In), nameof(H_State_XTC_W_In.In), typeof(H_State.Message))]
    // ReSharper disable once InconsistentNaming
    internal static bool HStateXtcWInInPostfix(H_State_XTC_W_In __instance) {
        AhegaoFaceAction.TryActivateAhegaoExpression(__instance.members.GetFemale(0), Ash.PersistentSettings.AhegaoDecayValue.Value);
        return true;
    }
}
