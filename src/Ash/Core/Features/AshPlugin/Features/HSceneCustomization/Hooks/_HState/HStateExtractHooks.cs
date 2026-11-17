using Ash.Utility.GlobalUtils;
using H;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.HSceneCustomization.Hooks._HState;

[HarmonyPatch]
internal class HStateExtractHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_State_Extract), nameof(H_State_Extract.Extract))]
    // ReSharper disable once InconsistentNaming
    internal static bool ExtractPrefix(H_State_Extract __instance) {
        if (!Ash.PersistentSettings.DisableFemaleVoiceBarkAfterExtract.Value)
            return true;

        var type = H_Expression.TYPE.EXTRACT;
        var style = __instance.members.param.style;
        if ((style.detailFlag & 4) != 0)
            type = H_Expression.TYPE.EXTRACT_FELLATIO;
        else if ((style.detailFlag & 8) != 0)
            type = H_Expression.TYPE.EXTRACT_IRRUMATIO;
        __instance.members.param.mouth = H_MOUTH.FREE;
        __instance.members.Expression(type);
        var flag1 = (style.detailFlag & 16 /*0x10*/) != 0;
        var flag2 = (style.detailFlag & 32 /*0x20*/) != 0;
        var females = __instance.members.GetFemales();
        foreach (var female in females) {
            if (female.personality.spermInCntV > 0 && flag1) {
                female.personality.spermInCntV = 0;
                if (!Ash.PersistentSettings.DisableFemaleSpermDripAfterExtract.Value) {
                    ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(female.dripParticleVagina);
                    female.dripParticleVagina.Play();
                }
            }

            if (female.personality.spermInCntA > 0 && flag2) {
                female.personality.spermInCntA = 0;
                if (!Ash.PersistentSettings.DisableFemaleSpermDripAfterExtract.Value) {
                    ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(female.dripParticleAnus);
                    female.dripParticleAnus.Play();
                }
            }
        }

        __instance.extracted = true;

        return false;
    }
}
