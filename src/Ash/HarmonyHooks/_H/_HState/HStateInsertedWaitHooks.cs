using H;
using HarmonyLib;

namespace Ash.HarmonyHooks._H._HState
{
    internal class HStateInsertedWaitHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_State_InsertedWait), nameof(H_State_InsertedWait.Main))]
        // ReSharper disable once InconsistentNaming
        internal static bool MainPrefix(H_State_PreInsertWait __instance) {
            if (!Ash.PersistentSettings.DisableFemaleInactionBark.Value)
                return true;

            __instance.UpdateVoice();
            if (!__instance.endInVoice || __instance.members.h_scene.MixCtrl.mode != MixController.MODE.FULL_AUTO)
                return false;

            __instance.ChangeState(H_STATE.LOOP);

            return false;
        }
    }
}
