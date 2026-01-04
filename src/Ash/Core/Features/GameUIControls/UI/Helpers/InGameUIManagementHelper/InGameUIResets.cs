using UnityEngine;
using UnityEngine.UI;
using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.InGameUIObjectManagement;
using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.Utils.InGameUIButtonsSearch;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper
{
    public class InGameUIResets
    {
        public static void ResetGages() {
            if (MaleGage == null || FemaleGage == null) {
                Ash.Logger.LogWarning("One or more gage references are null");
                return;
            }

            MaleGage.transform.localScale = Vector3.one;
            MaleGage.transform.localPosition = new Vector3(630f, 350f, 0);
            FemaleGage.transform.localScale = Vector3.one;
            FemaleGage.transform.localPosition = new Vector3(630f, 330f, 0);
        }

        public static void ResetXtcLocks() {
            if (MaleXTCLock == null || FemaleXTCLock == null) {
                Ash.Logger.LogWarning("One or more XTCLock references are null");
                return;
            }

            MaleXTCLock.transform.localPosition = new Vector3(630f, 350f, 0);
            FemaleXTCLock.transform.localPosition = new Vector3(630f, 330f, 0);

            var maleImg = MaleXTCLock.transform.Find("Checkmark");
            var femaleImg = FemaleXTCLock.transform.Find("Checkmark");

            if (maleImg == null || femaleImg == null) return;

            maleImg.transform.localScale = Vector3.one;
            femaleImg.transform.localScale = Vector3.one;
        }

        public static void ResetPad() {
            if (Pad == null) {
                Ash.Logger.LogWarning("Pad reference is null");
                return;
            }

            Pad.transform.localScale = Vector3.one;
        }

        public static void ResetActButtons() {
            if (ActButtons == null) {
                Ash.Logger.LogWarning("ActButtons reference is null");
                return;
            }

            ActButtons.transform.localPosition = new Vector3(630f, -160f, 0);
            ActButtons.transform.localScale = Vector3.one;
        }

        public static void ResetMiddleButtonsAlignment() {
            if (LeftMiddleToggles == null) {
                Ash.Logger.LogWarning("LeftMiddleToggles game object reference is null");
                return;
            }

            var vlg = LeftMiddleToggles.GetComponent<VerticalLayoutGroup>();

            if (vlg == null) {
                Ash.Logger.LogWarning("LeftMiddleToggles > VerticalLayoutGroup component reference is null");
                return;
            }

            vlg.childAlignment = TextAnchor.MiddleLeft;
        }

        public static void ResetMiddleButtonsSize() {
            var posBtnRectTransform = FindButtonRectTransform(PositionsGameObj, "PositionsButton");
            if (posBtnRectTransform == null)
                return;

            posBtnRectTransform.sizeDelta = new Vector2(80f, 30f);

            var clothBtnRectTransform = FindButtonRectTransform(ClothingGameObj, "ClothingButton");
            if (clothBtnRectTransform == null)
                return;

            clothBtnRectTransform.sizeDelta = new Vector2(80f, 30f);

            var gagBtnRectTransform = FindButtonRectTransform(GagGameObj, "GagButton");
            if (gagBtnRectTransform == null)
                return;

            gagBtnRectTransform.sizeDelta = new Vector2(80f, 30f);

            var moveBtnRectTransform = FindButtonRectTransform(MoveGameObj, "MoveButton");
            if (moveBtnRectTransform == null)
                return;

            moveBtnRectTransform.sizeDelta = new Vector2(80f, 30f);
        }

        public static void ResetMiddleButtonsHideablePos() {
            var posBtnHideable = FindButtonHideable(PositionsGameObj, "PositionButton");
            if (posBtnHideable != null)
                posBtnHideable.transform.localPosition = new Vector2(100f, 0f);

            var clothBtnHideable = FindButtonHideable(ClothingGameObj, "ClothingButton");
            if (clothBtnHideable != null)
                clothBtnHideable.transform.localPosition = new Vector2(-10f, 40f);

            var gagBtnHideable = FindButtonHideable(GagGameObj, "GagButton");
            if (gagBtnHideable != null)
                gagBtnHideable.transform.localPosition = new Vector2(-10f, 30f);

            var moveBtnHideable = FindButtonHideable(MoveGameObj, "MoveButton");
            if (moveBtnHideable != null)
                moveBtnHideable.transform.localPosition = new Vector2(85f, 179f);
        }

        public static void ResetSwapButton() {
            if (SwapGameObj == null) {
                Ash.Logger.LogWarning("Swap game object reference is null");
                return;
            }

            var img = SwapGameObj.transform.Find("Image").gameObject;
            if (img != null)
                img.SetActive(true);

            var btn = LeftMiddleToggles.transform.Find("Button_base_green");
            if (btn != null) {
                btn.gameObject.transform.SetParent(SwapGameObj.transform);
                btn.GetComponent<LayoutElement>().preferredWidth = 160;
            }
        }
    }
}
