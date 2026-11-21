using Ash.Utility.GlobalUtils;
using H;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.HSceneCustomization.Hooks._HState;

[HarmonyPatch]
internal class HStateVomit
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_State_Vomit), nameof(H_State_Vomit.Vomit))]
    // ReSharper disable once InconsistentNaming
    internal static bool VomitPrefix(H_State_Vomit __instance) {
        ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(__instance.members.GetFemale(0).dripParticleVagina);
        return true;
    }
}
