using UnityEngine;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper
{
    internal static class InGameUIAdjustments
    {
        internal static void AdjustGages(Canvas canvas) {
            var maleGage = canvas.transform.Find("MaleGage");
            var femaleGage = canvas.transform.Find("FemaleGage");

            if (maleGage == null || femaleGage == null) {
                Ash.Logger.LogWarning("One or more gage references are null");
                return;
            }

            maleGage.transform.localScale = new Vector3(0.3f, 0.5f, 1f);
            maleGage.transform.localPosition = new Vector3(634f, 354f, 0);
            femaleGage.transform.localScale = new Vector3(0.3f, 0.5f, 1f);
            femaleGage.transform.localPosition = new Vector3(634f, 343f, 0);
        }

        internal static void AdjustXtcLocks(Canvas canvas) {
            var maleXtcLock = canvas.transform.Find("MaleXTCLock");
            var femaleXtcLock = canvas.transform.Find("FemaleXTCLock");

            if (maleXtcLock == null || femaleXtcLock == null) {
                Ash.Logger.LogWarning("One or more XTCLock references are null");
                return;
            }

            maleXtcLock.transform.localPosition = new Vector3(840f, 358f, 0);
            femaleXtcLock.transform.localPosition = new Vector3(840f, 346f, 0);

            var maleImg = maleXtcLock.transform.Find("Checkmark");
            var femaleImg = femaleXtcLock.transform.Find("Checkmark");

            if (maleImg == null || femaleImg == null) return;

            maleImg.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            femaleImg.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }

        internal static void AdjustPad(Canvas canvas) {
            var pad = canvas.transform.Find("Pad");

            if (pad == null) {
                Ash.Logger.LogWarning("Pad reference is null");
                return;
            }

            pad.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }

        internal static void AdjustActButtons(Canvas canvas) {
            var actButtons = canvas.transform.Find("ActButtons");

            if (actButtons == null) {
                Ash.Logger.LogWarning("ActButtons reference is null");
                return;
            }

            actButtons.transform.localPosition = new Vector3(633f, -240f, 0);
            actButtons.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        }

        internal static void AdjustLeftMiddleToggles(GameObject leftMiddleToggles) {
            leftMiddleToggles.GetComponent<RectTransform>().anchoredPosition = new Vector2(40, 0);
        }
    }
}
