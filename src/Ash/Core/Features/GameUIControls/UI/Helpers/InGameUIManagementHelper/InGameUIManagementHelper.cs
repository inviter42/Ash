using System.Collections.Generic;
using Ash.Core.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.InGameUIAdjustments;
using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.InGameUIResets;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper
{
    internal static class InGameUIManagementHelper
    {
        private static readonly List<string> LeftMiddleMenuWhitelist = new List<string> {
            "MaleShow",
            "Map",
            "Move",
            "Light",
            "Customs"
        };

        // UI MODES
        internal static void SwitchToImmersiveUIMode() {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null)
                return;

            // left middle toggles
            // scene.middleLeftCanvas.gameObject.SetActive(false); // todo: replace loop below with this when all ui's are done
            var leftMiddleTogglesTransform = hScene.middleLeftCanvas.transform.Find("LeftMiddleToggles");
            foreach (Transform child in leftMiddleTogglesTransform) {
                if (LeftMiddleMenuWhitelist.Contains(child.gameObject.name))
                    continue;

                child.gameObject.SetActive(false);
            }

            leftMiddleTogglesTransform.gameObject.SetActive(false);

            hScene.uiCanvas.transform.Find("Badges").gameObject.SetActive(false);
            hScene.uiCanvas.transform.Find("Swap").gameObject.SetActive(false);
            hScene.uiCanvas.transform.Find("Talk").transform.GetComponent<Image>().enabled = false;

            // left down buttons
            var leftDownButtonsGameObj = hScene.uiCanvas.transform.Find("LeftDownButtons");
            leftDownButtonsGameObj.transform.Find("Button_Config").gameObject.SetActive(false);
            leftDownButtonsGameObj.transform.Find("Button_End").gameObject.SetActive(false);

            AdjustGages(hScene.uiCanvas);
            AdjustXtcLocks(hScene.uiCanvas);
            AdjustPad(hScene.uiCanvas);
            AdjustActButtons(hScene.uiCanvas);

            AdjustLeftMiddleToggles(leftMiddleTogglesTransform.gameObject);
        }

        internal static void SwitchToDefaultUIMode() {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null)
                return;

            // left middle toggles
            // scene.middleLeftCanvas.gameObject.SetActive(true); // todo: replace loop below with this when all ui's are done
            var leftMiddleTogglesTransform = hScene.middleLeftCanvas.transform.Find("LeftMiddleToggles");
            foreach (Transform child in leftMiddleTogglesTransform) {
                if (child.gameObject.name == "Sperm")
                    continue;

                child.gameObject.SetActive(true);
            }

            leftMiddleTogglesTransform.gameObject.SetActive(true);

            hScene.uiCanvas.transform.Find("Badges").gameObject.SetActive(true);
            hScene.uiCanvas.transform.Find("Swap").gameObject.SetActive(true);
            hScene.uiCanvas.transform.Find("Talk").transform.GetComponent<Image>().enabled = true;

            // left down buttons
            var leftDownButtonsGameObj = hScene.uiCanvas.transform.Find("LeftDownButtons");
            leftDownButtonsGameObj.transform.Find("Button_Config").gameObject.SetActive(true);
            leftDownButtonsGameObj.transform.Find("Button_End").gameObject.SetActive(true);

            ResetGages(hScene.uiCanvas);
            ResetXtcLocks(hScene.uiCanvas);
            ResetPad(hScene.uiCanvas);
            ResetActButtons(hScene.uiCanvas);

            ResetLeftMiddleToggles(leftMiddleTogglesTransform.gameObject);
        }
    }
}
