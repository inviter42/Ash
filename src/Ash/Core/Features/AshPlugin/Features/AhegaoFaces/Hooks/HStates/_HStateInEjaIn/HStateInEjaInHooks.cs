using Ash.Core.Features.Actions;
using H;
using HarmonyLib;

namespace Ash.Core.Features.Ahegao.Hooks.HStates._HStateInEjaIn;

[HarmonyPatch]
internal class HStateInEjaInHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_State_InEjaIn), nameof(H_State_InEjaIn.In), typeof(H_State.Message))]
    // ReSharper disable once InconsistentNaming
    internal static bool HStateInEjaInInPostfix(H_State_InEjaIn __instance) {
        AhegaoFaceAction.TryActivateAhegaoExpression(__instance.members.GetFemale(0), Ash.PersistentSettings.AhegaoDecayValue.Value);
        return true;
    }
}
