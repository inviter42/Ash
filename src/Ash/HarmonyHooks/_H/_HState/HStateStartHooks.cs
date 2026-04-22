using H;
using HarmonyLib;

namespace Ash.HarmonyHooks._H._HState
{
    internal class HStateStartHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_State_Start), nameof(H_State_Start.In), typeof(H_State.Message))]
        // ReSharper disable once InconsistentNaming
        internal static bool InPrefix(H_State_Start __instance, H_State.Message msg) {
            if (!Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneStart.Value)
                __instance.members.VoiceExpression(H_Voice.TYPE.START);

            __instance.members.MaleExpression(H_Expression_Male.TYPE.NORMAL);

            return false;
        }
    }
}
