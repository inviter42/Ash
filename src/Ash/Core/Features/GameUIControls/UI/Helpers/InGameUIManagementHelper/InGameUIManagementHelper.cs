using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.InGameUIObjectManagement;
using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.InGameUIAdjustments;
using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.InGameUIResets;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper
{
    public static class InGameUIManagementHelper
    {
        // UI MODES
        public static void ImmersiveUIMode() {
            string[] namesOfGameObjectsToDeactivate = {
                InGameUIStrings.ConfigGameObjName,
                InGameUIStrings.EndSceneGameObjName,
                InGameUIStrings.MaleGameObjName,
                InGameUIStrings.MapGameObjName,
                InGameUIStrings.LightingGameObjName,
                InGameUIStrings.EditCharGameObjName,
                InGameUIStrings.BadgesGameObjName
            };

            foreach (var gameObjName in namesOfGameObjectsToDeactivate) {
                var gameObj = TargetGameObjects.Find(o => o != null && o.name == gameObjName);

                if (gameObj == null) {
                    Ash.Logger.LogWarning($"Cannot deactivate <GameObject> '{gameObjName}' - reference is not found.");
                    continue;
                }

                if (!InactiveGameObjects.Contains(gameObj))
                    InactiveGameObjects.Add(gameObj);


                gameObj.SetActive(false);
            }

            if (TalkImage != null) TalkImage.enabled = false;

            AdjustGages();
            AdjustXtcLocks();
            AdjustPad();
            AdjustActButtons();
            AdjustMiddleButtonsAlignment();
            AdjustMiddleButtonsSize();
            AdjustMiddleButtonsHideablePos();
            AdjustSwapButton();
        }

        public static void DefaultUIMode() {
            while (InactiveGameObjects.Count > 0) {
                InactiveGameObjects[0].SetActive(true);
                InactiveGameObjects.RemoveAt(0);
            }

            if (TalkImage != null) TalkImage.enabled = true;

            ResetGages();
            ResetXtcLocks();
            ResetPad();
            ResetActButtons();
            ResetMiddleButtonsAlignment();
            ResetMiddleButtonsSize();
            ResetMiddleButtonsHideablePos();
            ResetSwapButton();
        }
    }
}
