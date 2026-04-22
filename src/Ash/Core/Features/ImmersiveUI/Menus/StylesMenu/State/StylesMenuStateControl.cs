using System;
using System.Collections;
using System.Collections.Generic;
using Ash.Core.Features.ImmersiveUI.Extras.Extensions;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Data;
using Ash.Core.SceneManagement;
using Ash.GlobalUtils;
using H;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.State
{
    internal class StylesMenuStateControl
    {
        internal bool IsVisible => StylesMenuMain.StylesMenuRootGameObj.activeSelf;

        // ui refs

        internal readonly List<GameObject> ListButtons = new List<GameObject>();

        private H_StyleData.TYPE SelectedStyleType = H_StyleData.TYPE.INSERT;
        private H_StyleData.STATE SelectedFemaleState = H_StyleData.STATE.UNKNOWN;

        private readonly List<IuiToggle> GenreToggles;
        private readonly List<IuiToggle> FemaleStateToggles;

        private readonly StylesMenuMain StylesMenuMain;
        private readonly StylesMenuStylesData  StylesData;

        // callbacks

        private readonly Action OnUpdateScrollList;

        internal StylesMenuStateControl(
            IuiMain iuiMain,
            StylesMenuMain stylesMenuMain,
            StylesMenuStylesData stylesData,
            List<IuiToggle> genreToggles,
            List<IuiToggle> femaleStateToggles,
            Action onUpdateScrollList,
            Func<IEnumerator> onAfterScrollListUpdatedRoutine
        ) {
            StylesMenuMain = stylesMenuMain;
            StylesData = stylesData;
            GenreToggles = genreToggles;
            FemaleStateToggles = femaleStateToggles;
            OnUpdateScrollList = onUpdateScrollList;

            // post-state-update routines
            iuiMain.StateControl.AddPostStateUpdateRoutine(onAfterScrollListUpdatedRoutine);

            UpdateFemaleStateFromSceneData();
        }

        internal void ToggleMenuVisibility() {
            if (!IuiMain.IsLegalScene)
                return;

            if (IsVisible) {
                HideUI();
            }
            else {
                SetActiveGenreToggle();
                SetActiveFemaleStateToggle();
                ShowUI();
            }
        }

        internal void UpdateThumbnailImage(H_StyleData styleData, RawImage thumbnailRawImage) {
            var texture = StylesMenuMain.Thumbnails.ThumbnailsDictionary.GetValueOrDefaultValue(
                styleData.id.ToLower(), null);

            if (texture == null) {
                thumbnailRawImage.enabled = false;
                Ash.Logger.LogDebug($"Unable to find thumbnail for {styleData.id}");
                return;
            }

            thumbnailRawImage.texture = texture;
            thumbnailRawImage.enabled = true;
        }

        internal void OnUpdateStyleType(H_StyleData.TYPE type) {
            SelectedStyleType = type;
            StylesData.OnSelectedStyleTypeChanged(SelectedStyleType, SelectedFemaleState);
            OnUpdateScrollList();
            SystemSE.Play(SystemSE.SE.CHOICE);
        }

        internal void OnUpdateFemaleState(H_StyleData.STATE state) {
            SelectedFemaleState = state;
            StylesData.OnSelectedFemaleStateChanged(SelectedStyleType, SelectedFemaleState);
            OnUpdateScrollList();
            SystemSE.Play(SystemSE.SE.CHOICE);
        }

        internal void OnStyleSelected(string id) {
            var scene = SceneTypeTracker.Scene as H_Scene;
            if (scene == null) {
                Ash.Logger.LogError($"Invalid scene type - H_Scene expected");
                return;
            }

            ScreenFade.StartFade(ScreenFade.TYPE.OUT_IN, Color.black, 0.5f,
                action: () => scene.ChangeStyle(id));

            SystemSE.Play(SystemSE.SE.YES);
        }

        private void ShowUI() {
            if (!Ash.AshUI.IuiMain.CanvasGameObj) {
                Ash.Logger.LogError("Unable to toggle Styles Menu visibility - Canvas not found.");
                return;
            }

            if (!StylesMenuMain.StylesMenuRootGameObj) {
                Ash.Logger.LogError("Unable to toggle Styles Menu visibility - StylesMenuRootGameObj not found.");
                return;
            }

            StylesMenuMain.StylesMenuRootGameObj.SetActive(true);
        }

        private void HideUI() {
            if (!Ash.AshUI.IuiMain.CanvasGameObj) {
                Ash.Logger.LogError("Unable to toggle Styles Menu visibility - Canvas not found.");
                return;
            }

            if (!StylesMenuMain.StylesMenuRootGameObj) {
                Ash.Logger.LogError("Unable to toggle Styles Menu visibility - StylesMenuRootGameObj not found.");
                return;
            }

            StylesMenuMain.StylesMenuRootGameObj.SetActive(false);
        }

        private void UpdateFemaleStateFromSceneData() {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null) {
                Ash.Logger.LogError($"Unable to cast Scene to H_Scene.");
                return;
            }

            var female = hScene.mainMembers.GetFemale(0);
            if (female.personality.weakness) {
                SelectedFemaleState = H_StyleData.STATE.WEAKNESS;
            }
            else if (female.IsFloped()) {
                SelectedFemaleState = H_StyleData.STATE.FLOP;
            }
            else {
                SelectedFemaleState = H_StyleData.STATE.RESIST;
            }

            StylesData.OnSelectedFemaleStateChanged(SelectedStyleType, SelectedFemaleState);
        }

        private void SetActiveGenreToggle() {
            var idx = Array.IndexOf(Enum.GetValues(typeof(H_StyleData.TYPE)), SelectedStyleType);
            GenreToggles[idx].ToggleOn();
        }

        private void SetActiveFemaleStateToggle() {
            var newIdx = Array.IndexOf(Enum.GetValues(typeof(H_StyleData.STATE)), SelectedFemaleState);
            FemaleStateToggles[newIdx].ToggleOn();
        }
    }
}
