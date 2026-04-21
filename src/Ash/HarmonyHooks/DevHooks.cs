using H;
using HarmonyLib;
using Utility;

namespace Ash.HarmonyHooks
{
    internal class DevHooks
    {
        //////////////////////////////// --- DEBUGGING SECTION -- ////////////////////////////////

        // H_Members state machine hook
        [HarmonyPostfix]
        [HarmonyPatch(
            typeof(StateManagerBase_WithMsg<H_STATE, H_State, H_State.Message>),
            nameof(StateManagerBase_WithMsg<H_STATE, H_State, H_State.Message>.ChangeState),
            typeof(H_STATE),
            typeof(H_State.Message))
        ]
        // ReSharper disable once InconsistentNaming
        internal static void ChangeStatePostfix(StateManagerBase_WithMsg<H_STATE, H_State, H_State.Message> __instance,
            H_STATE next, H_State.Message msg) {
            Ash.Logger.LogDebug($"Change state is called, next: {next}, msg: {msg}");
            Ash.Logger.LogDebug($"NowStateID is {__instance.NowStateID}");
        }

        // H_Members animation method hook
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Members), nameof(H_Members.PlayAnime), typeof(string), typeof(float))]
        // ReSharper disable once InconsistentNaming
        internal static bool PlayAnimePrefix(H_Members __instance, string name, float transitionDuration) {
            Ash.Logger.LogDebug($"PlayAnime '{name}', transitionDuration: {transitionDuration}");
            return true;
        }

        //////////////////////////////// --- CONVENIENCE SECTION -- ////////////////////////////////

        // load directly into the scene upon starting the game
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CautionScene), "Start")]
        // ReSharper disable once InconsistentNaming
        internal static bool StartPrefix(CautionScene __instance) {
            Ash.Logger.LogDebug($"Skipping to H scene.");

            __instance.InScene(false); // instantiates GC
            GlobalData.PlayData.Load(GlobalData.GetContinueSaveFile());
            __instance.GC.ChangeScene("H", "Load", 1f);
            // __instance.GC.ChangeScene("SelectScene", "Load", 1f);

            return false;
        }

        //////////////////////////////// --- TESTING SECTION -- ////////////////////////////////
    }
}
