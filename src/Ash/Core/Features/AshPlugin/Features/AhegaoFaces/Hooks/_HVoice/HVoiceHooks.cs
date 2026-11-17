using H;
using HarmonyLib;
using UnityEngine;

namespace Ash.Core.Features.Ahegao.Hooks._HVoice;

[HarmonyPatch]
internal class HVoiceHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_Voice), nameof(H_Voice.Voice), typeof(Female), typeof(H_VoiceLog), typeof(AssetBundleController), typeof(H_Voice.TYPE), typeof(H_Members))]
    // ReSharper disable InconsistentNaming
    internal static bool Voice(H_Voice __instance, Female female, H_VoiceLog log, AssetBundleController voiceABC, H_Voice.TYPE type, H_Members members, ref H_Voice.Data __result) {
        if (!Ash.PersistentSettings.AhegaoFacesEnabled.Value)
            return true;

        var data = __instance.GetData(female, log, type, members);
        if (data != null) {
            var fileName = __instance.GetFileName(female.HeroineID, data);
            var clip = voiceABC.LoadAsset<AudioClip>(fileName);
            if (clip == null)
                Debug.LogError("音声クリップがない：" + fileName);

            female.PhonationVoice(clip, false);
            female.ExpressionPlay(
                2,
                female.personality.ahe || GlobalGameData.IsCustomMouthExpressionPresetActive(female)
                    ? H_Voice.KindAnimes[3]
                    : H_Voice.KindAnimes[(int)data.kind],
                0.2f
            );
        }
        else
            Debug.LogWarning("ふさわしい音声がない:" + type);

        __result = data;

        return false;
    }
}
