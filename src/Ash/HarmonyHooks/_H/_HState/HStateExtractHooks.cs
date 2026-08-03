using Ash.GlobalUtils;
using H;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Ash.HarmonyHooks._H._HState
{
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
            for (var index = 0; index < females.Count; ++index) {
                if (females[index].personality.spermInCntV > 0 && flag1) {
                    females[index].personality.spermInCntV = 0;
                    if (!Ash.PersistentSettings.DisableFemaleSpermDripAfterExtract.Value) {
                        ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(females[index].dripParticleVagina);
                        females[index].dripParticleVagina.Play();
                    }
                }

                if (females[index].personality.spermInCntA > 0 && flag2) {
                    females[index].personality.spermInCntA = 0;
                    if (!Ash.PersistentSettings.DisableFemaleSpermDripAfterExtract.Value) {
                        ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(females[index].dripParticleAnus);
                        females[index].dripParticleAnus.Play();
                    }
                }
            }

            __instance.extracted = true;

            return false;
        }
    }
}
