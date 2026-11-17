using UnityEngine;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper
{
    internal static class InGameUIResets
    {
        internal static void ResetGages(Canvas canvas) {
            var maleGage = canvas.transform.Find("MaleGage");
            var femaleGage = canvas.transform.Find("FemaleGage");

            if (maleGage == null || femaleGage == null) {
                Ash.Logger.LogWarning("One or more gage references are null");
                return;
            }

            maleGage.transform.localScale = Vector3.one;
            maleGage.transform.localPosition = new Vector3(630f, 350f, 0);
            femaleGage.transform.localScale = Vector3.one;
            femaleGage.transform.localPosition = new Vector3(630f, 330f, 0);
        }

        internal static void ResetXtcLocks(Canvas canvas) {
            var maleXtcLock = canvas.transform.Find("MaleXTCLock");
            var femaleXtcLock = canvas.transform.Find("FemaleXTCLock");

            if (maleXtcLock == null || femaleXtcLock == null) {
                Ash.Logger.LogWarning("One or more XTCLock references are null");
                return;
            }

            maleXtcLock.transform.localPosition = new Vector3(630f, 350f, 0);
            femaleXtcLock.transform.localPosition = new Vector3(630f, 330f, 0);

            var maleImg = maleXtcLock.transform.Find("Checkmark");
            var femaleImg = femaleXtcLock.transform.Find("Checkmark");

            if (maleImg == null || femaleImg == null) return;

            maleImg.transform.localScale = Vector3.one;
            femaleImg.transform.localScale = Vector3.one;
        }

        internal static void ResetPad(Canvas canvas) {
            var pad = canvas.transform.Find("Pad");

            if (pad == null) {
                Ash.Logger.LogWarning("Pad reference is null");
                return;
            }

            pad.transform.localScale = Vector3.one;
        }

        internal static void ResetActButtons(Canvas canvas) {
            var actButtons = canvas.transform.Find("ActButtons");

            if (actButtons == null) {
                Ash.Logger.LogWarning("ActButtons reference is null");
                return;
            }

            actButtons.transform.localPosition = new Vector3(630f, -160f, 0);
            actButtons.transform.localScale = Vector3.one;
        }

        internal static void ResetLeftMiddleToggles(GameObject leftMiddleToggles) {
            leftMiddleToggles.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
}
