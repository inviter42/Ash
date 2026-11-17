using Ash.Core.Features.Actions;
using H;
using HarmonyLib;

namespace Ash.Core.Features.Ahegao.Hooks.HStates._HStateXtcFIn;

[HarmonyPatch]
internal class HStateXtcFInHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_State_XTC_F_In), nameof(H_State_XTC_F_In.In), typeof(H_State.Message))]
    // ReSharper disable once InconsistentNaming
    internal static bool HStateXtcFInInPostfix(H_State_XTC_F_In __instance) {
        AhegaoFaceAction.TryActivateAhegaoExpression(__instance.members.GetFemale(0), Ash.PersistentSettings.AhegaoDecayValue.Value);
        return true;
    }
}
