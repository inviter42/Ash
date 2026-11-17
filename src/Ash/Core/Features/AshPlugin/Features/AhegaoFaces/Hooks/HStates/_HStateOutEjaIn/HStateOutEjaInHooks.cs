using Ash.Core.Features.Actions;
using H;
using HarmonyLib;

namespace Ash.Core.Features.Ahegao.Hooks.HStates._HStateOutEjaIn;

[HarmonyPatch]
internal class HStateOutEjaInHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_State_OutEjaIn), nameof(H_State_OutEjaIn.In), typeof(H_State.Message))]
    // ReSharper disable once InconsistentNaming
    internal static bool HStateOutEjaInInPostfix(H_State_OutEjaIn __instance) {
        AhegaoFaceAction.TryActivateAhegaoExpression(__instance.members.GetFemale(0), Ash.PersistentSettings.AhegaoDecayValue.Value);
        return true;
    }
}
