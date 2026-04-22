using System;
using Ash.Core.SceneManagement;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.State
{
    internal class WearsMenuStateControl
    {
        internal bool IsVisible => Menu.WearsMenuRoot.activeSelf;

        private readonly WearsMenuMain Menu;

        internal WearsMenuStateControl(
            WearsMenuMain menu,
            IuiMain iuiMain
        ) {
            var scene = SceneTypeTracker.Scene as H_Scene;
            if (scene == null) {
                throw new Exception("Expected H_Scene is null.");
            }

            Menu = menu;
            iuiMain.FemaleHasBeenEdited += Menu.RecreateContainerForFemale;
        }

        internal void ToggleMenuVisibility() {
            if (!IuiMain.IsLegalScene)
                return;

            if (IsVisible) {
                HideUI();
            }
            else {
                ShowUI();
            }
        }

        private void ShowUI() {
            var scene = SceneTypeTracker.Scene as H_Scene;
            if (scene == null) {
                throw new Exception("Expected H_Scene is null.");
            }

            if (!Ash.AshUI.IuiMain.CanvasGameObj) {
                Ash.Logger.LogError("Unable to toggle Wears Menu visibility - CanvasGameObj not found.");
                return;
            }

            if (!Menu.WearsMenuRoot) {
                Ash.Logger.LogError("Unable to toggle Wears Menu visibility - WearsMenuRoot not found.");
                return;
            }

            Menu.UpdateFemaleText(true);
            Menu.UpdateContainerVisibilityAndRecalculateBounds(true);

            Menu.WearsMenuRoot.SetActive(true);
        }

        private void HideUI() {
            if (!Ash.AshUI.IuiMain.CanvasGameObj) {
                Ash.Logger.LogError("Unable to toggle Wears Menu visibility - Canvas not found.");
                return;
            }

            if (!Menu.WearsMenuRoot) {
                Ash.Logger.LogError("Unable to toggle Wears Menu visibility - WearsMenuRootGameObj not found.");
                return;
            }

            Menu.WearsMenuRoot.SetActive(false);
        }
    }
}
