using HarmonyLib;

namespace Ash.HarmonyHooks._Scene
{
    internal class SceneLoadingHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CautionScene), "Start")]
        // ReSharper disable once InconsistentNaming
        internal static bool StartPrefix(CautionScene __instance) {
            if (!Ash.ConfigEntrySkipToTitleSceneEnabled.Value)
                return true;

            __instance.InScene(false); // instantiates GC
            GlobalData.PlayData.Load(GlobalData.GetContinueSaveFile());
            __instance.GC.ChangeScene("TitleScene", string.Empty, 1f);

            return false;
        }
    }
}
