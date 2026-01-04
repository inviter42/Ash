using UnityEngine;
using UnityEngine.UI;
using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.InGameUIObjectManagement;
using static Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.Utils.InGameUIButtonsSearch;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper
{
    public class InGameUIAdjustments
    {
        public static void AdjustGages() {
            if (MaleGage == null || FemaleGage == null) {
                Ash.Logger.LogWarning("One or more gage references are null");
                return;
            }

            MaleGage.transform.localScale = new Vector3(0.3f, 0.5f, 1f);
            MaleGage.transform.localPosition = new Vector3(634f, 354f, 0);
            FemaleGage.transform.localScale = new Vector3(0.3f, 0.5f, 1f);
            FemaleGage.transform.localPosition = new Vector3(634f, 343f, 0);
        }

        public static void AdjustXtcLocks() {
            if (MaleXTCLock == null || FemaleXTCLock == null) {
                Ash.Logger.LogWarning("One or more XTCLock references are null");
                return;
            }

            MaleXTCLock.transform.localPosition = new Vector3(840f, 358f, 0);
            FemaleXTCLock.transform.localPosition = new Vector3(840f, 346f, 0);

            var maleImg = MaleXTCLock.transform.Find("Checkmark");
            var femaleImg = FemaleXTCLock.transform.Find("Checkmark");

            if (maleImg == null || femaleImg == null) return;

            maleImg.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            femaleImg.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }

        public static void AdjustPad() {
            if (Pad == null) {
                Ash.Logger.LogWarning("Pad reference is null");
                return;
            }

            Pad.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }

        public static void AdjustActButtons() {
            if (ActButtons == null) {
                Ash.Logger.LogWarning("ActButtons reference is null");
                return;
            }

            ActButtons.transform.localPosition = new Vector3(633f, -240f, 0);
            ActButtons.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        }

        public static void AdjustMiddleButtonsAlignment() {
            if (LeftMiddleToggles == null) {
                Ash.Logger.LogWarning("LeftMiddleToggles game object reference is null");
                return;
            }

            var vlg = LeftMiddleToggles.GetComponent<VerticalLayoutGroup>();

            if (vlg == null) {
                Ash.Logger.LogWarning("LeftMiddleToggles > VerticalLayoutGroup component reference is null");
                return;
            }

            vlg.childAlignment = TextAnchor.UpperLeft;
        }

        public static void AdjustMiddleButtonsSize() {
            var posBtnRectTransform = FindButtonRectTransform(PositionsGameObj, "PositionsButton");
            if (posBtnRectTransform != null)
                posBtnRectTransform.sizeDelta = new Vector2(30f, 30f);

            var clothBtnRectTransform = FindButtonRectTransform(ClothingGameObj, "ClothingButton");
            if (clothBtnRectTransform != null)
                clothBtnRectTransform.sizeDelta = new Vector2(30f, 30f);

            var gagBtnRectTransform = FindButtonRectTransform(GagGameObj, "GagButton");
            if (gagBtnRectTransform != null)
                gagBtnRectTransform.sizeDelta = new Vector2(30f, 30f);

            var moveBtnRectTransform = FindButtonRectTransform(MoveGameObj, "MoveButton");
            if (moveBtnRectTransform != null)
                moveBtnRectTransform.sizeDelta = new Vector2(30f, 30f);
        }

        public static void AdjustMiddleButtonsHideablePos() {
            var posBtnHideable = FindButtonHideable(PositionsGameObj, "PositionButton");
            if (posBtnHideable != null)
                posBtnHideable.transform.localPosition = new Vector2(40f, 0f);

            var clothBtnHideable = FindButtonHideable(ClothingGameObj, "ClothingButton");
            if (clothBtnHideable != null)
                clothBtnHideable.transform.localPosition = new Vector2(-55f, 0f);

            var gagBtnHideable = FindButtonHideable(GagGameObj, "GagButton");
            if (gagBtnHideable != null)
                gagBtnHideable.transform.localPosition = new Vector2(-55f, 30f);

            var moveBtnHideable = FindButtonHideable(MoveGameObj, "MoveButton");
            if (moveBtnHideable != null)
                moveBtnHideable.transform.localPosition = new Vector2(40f, 170f);
        }

        public static void AdjustSwapButton() {
            if (SwapGameObj == null) {
                Ash.Logger.LogWarning("Swap game object reference is null");
                return;
            }

            var img = SwapGameObj.transform.Find("Image").gameObject;
            if (img != null)
                img.SetActive(false);

            var btn = SwapGameObj.transform.Find("Button_base_green");
            // ReSharper disable once InvertIf
            if (btn != null) {
                btn.gameObject.transform.SetParent(LeftMiddleToggles.transform);
                btn.GetComponent<LayoutElement>().preferredWidth = 30;
            }
        }

    }
}
