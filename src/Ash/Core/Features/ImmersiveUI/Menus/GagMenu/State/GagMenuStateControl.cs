using System;
using System.Collections.Generic;
using Ash.Core.Features.ImmersiveUI.Extras.Extensions;
using Ash.Core.SceneManagement;
using Character;

namespace Ash.Core.Features.ImmersiveUI.Menus.GagMenu.State
{
    internal class GagMenuStateControl
    {
        internal bool IsVisible => GagMenuMain.GagMenuRootGameObj.activeSelf;

        private GAG_ITEM SelectedGagItemMain = GAG_ITEM.NONE;
        private GAG_ITEM SelectedGagItemVisitor = GAG_ITEM.NONE;

        private readonly List<IuiToggle> GagTogglesMain;
        private readonly List<IuiToggle> GagTogglesVisitor;

        private readonly Female MainFemale;
        private readonly Female Visitor;

        private readonly GagMenuMain GagMenuMain;

        internal GagMenuStateControl(
            GagMenuMain gagMenuMain,
            List<IuiToggle> gagTogglesMain,
            List<IuiToggle> gagTogglesVisitor = null
        ) {
            GagMenuMain = gagMenuMain;
            GagTogglesMain = gagTogglesMain;
            GagTogglesVisitor = gagTogglesVisitor;

            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null) {
                Ash.Logger.LogError($"Unable to cast Scene to H_Scene.");
                return;
            }

            MainFemale = hScene.mainMembers.GetFemale(0);
            Visitor = hScene.visitor?.GetFemale();

            SelectedGagItemMain = MainFemale.GagType;
            if (Visitor != null)
                SelectedGagItemVisitor = Visitor.GagType;
        }

        internal void ToggleMenuVisibility() {
            if (!IuiMain.IsLegalScene)
                return;

            if (IsVisible) {
                HideUI();
            }
            else {
                SetActiveGagToggles();
                ShowUI();
            }
        }

        private void ShowUI() {
            if (!Ash.AshUI.IuiMain.CanvasGameObj) {
                Ash.Logger.LogError("Unable to toggle Gag Menu visibility - CanvasGameObj not found.");
                return;
            }

            if (!GagMenuMain.GagMenuRootGameObj) {
                Ash.Logger.LogError("Unable to toggle Gag Menu visibility - GagMenuRootGameObj not found.");
                return;
            }

            GagMenuMain.GagMenuRootGameObj.SetActive(true);
        }

        private void HideUI() {
            if (!Ash.AshUI.IuiMain.CanvasGameObj) {
                Ash.Logger.LogError("Unable to toggle Gag Menu visibility - Canvas not found.");
                return;
            }

            if (!GagMenuMain.GagMenuRootGameObj) {
                Ash.Logger.LogError("Unable to toggle Gag Menu visibility - GagMenuRootGameObj not found.");
                return;
            }

            GagMenuMain.GagMenuRootGameObj.SetActive(false);
        }

        internal void OnUpdateGagItem(GAG_ITEM item, bool isVisitor) {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null) {
                Ash.Logger.LogError($"Unable to cast Scene to H_Scene.");
                return;
            }

            if (isVisitor) {
                if (Visitor == null) {
                    Ash.Logger.LogDebug($"Unable to update gag item - Visitor is null");
                    return;
                }

                if (item == Visitor.personality.gagItem)
                    return;

                if (Ash.PersistentSettings.InterruptVoiceClipImmediatelyUponGagChange.Value &&
                    (item == GAG_ITEM.NONE || MainFemale.personality.gagItem == GAG_ITEM.NONE)
                ) {
                    Visitor.VoiceShutUp();
                }

                SelectedGagItemVisitor = item;
                Visitor.personality.gagItem = SelectedGagItemVisitor;
                Visitor.ChangeGagItem();
            }
            else {
                if (item == MainFemale.personality.gagItem)
                    return;

                if (Ash.PersistentSettings.InterruptVoiceClipImmediatelyUponGagChange.Value &&
                    (item == GAG_ITEM.NONE || MainFemale.personality.gagItem == GAG_ITEM.NONE)
                ) {
                    MainFemale.VoiceShutUp();
                }

                SelectedGagItemMain = item;
                MainFemale.personality.gagItem = SelectedGagItemMain;
                MainFemale.ChangeGagItem();
            }

            SystemSE.Play(SystemSE.SE.CHOICE);
        }

        private void SetActiveGagToggles() {
            var idx = Array.IndexOf(Enum.GetValues(typeof(GAG_ITEM)), SelectedGagItemMain);
            GagTogglesMain[idx].ToggleOn();

            if (GagTogglesVisitor is null)
                return;

            var idx2 = Array.IndexOf(Enum.GetValues(typeof(GAG_ITEM)), SelectedGagItemVisitor);
            GagTogglesVisitor[idx2].ToggleOn();
        }
    }
}
