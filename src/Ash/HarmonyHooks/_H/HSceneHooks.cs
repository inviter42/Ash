using HarmonyLib;

namespace Ash.HarmonyHooks._H
{
    internal class HSceneHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Scene), nameof(H_Scene.CustomEnd_Record))]
        // ReSharper disable once InconsistentNaming
        internal static bool CustomEnd_RecordPrefix(H_Scene __instance) {
            if (Ash.AshUI.IuiMain == null)
                return true;

            Ash.AshUI.IuiMain.RaiseEventCharacterHasBeenEdited(__instance.editMode.human);

            Ash.AshUI.IuiMain.CanvasGameObj.SetActive(true);

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Scene), nameof(H_Scene.CustomEnd_Revert))]
        // ReSharper disable once InconsistentNaming
        internal static bool CustomEnd_RevertPrefix() {
            if (Ash.AshUI.IuiMain == null)
                return true;

            Ash.AshUI.IuiMain.CanvasGameObj.SetActive(true);

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Scene), nameof(H_Scene.OpenConfig))]
        // ReSharper disable once InconsistentNaming
        internal static bool OpenConfigPrefix() {
            if (Ash.AshUI.IuiMain == null)
                return true;

            Ash.AshUI.IuiMain.CanvasGameObj.SetActive(false);

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_Scene), nameof(H_Scene.ToCharaCustom_Female), typeof(bool))]
        // ReSharper disable once InconsistentNaming
        internal static bool ToCharaCustom_FemalePrefix(H_Scene __instance, bool destroyRecord) {
            if (Ash.AshUI.IuiMain == null)
                return true;

            Ash.AshUI.IuiMain.CanvasGameObj.SetActive(false);

            return true;
        }
    }
}
