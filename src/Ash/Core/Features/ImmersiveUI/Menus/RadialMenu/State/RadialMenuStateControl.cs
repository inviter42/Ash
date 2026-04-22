using System;
using System.Collections.Generic;
using System.IO;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.InputHandler;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu;
using Ash.Core.SceneManagement;
using Ash.GlobalUtils;
using UnityEngine;
using UnityEngine.UI;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.State
{
    internal class RadialMenuStateControl
    {
        internal bool IsVisible => RadialMenuMain.RadialMenuRootGameObject.activeSelf;

        private int SelectedIndex = -1;

        private readonly GameObject CanvasGameObj;
        private readonly RadialMenuMain RadialMenuMain;

        private readonly RadialMenuInputHandler RadialMenuInputHandler;

        // private bool WasVisible;

        private bool WasInDeadZone = true;

        private bool IsCanceled;

        private readonly Dictionary<string, Action> Actions;

        internal RadialMenuStateControl(
            IuiMain iuiMain,
            GameObject canvasGameObj,
            RadialMenuMain radialMenuMain,
            StylesMenuMain stylesMenuMain,
            GagMenuMain gagMenuMain,
            WearsMenuMain wearsMenuMain
        ) {
            CanvasGameObj = canvasGameObj;
            RadialMenuMain = radialMenuMain;

            RadialMenuInputHandler = new RadialMenuInputHandler(radialMenuMain);

            iuiMain.StateControl.AddStateUpdateRequest(UpdateState);

            Actions = new Dictionary<string, Action> {
                ["piemenu-heart"] = stylesMenuMain.StateControl.ToggleMenuVisibility,
                ["piemenu-message-circle-x"] = gagMenuMain.StateControl.ToggleMenuVisibility,
                ["piemenu-shirt"] = wearsMenuMain.StateControl.ToggleMenuVisibility,
                ["piemenu-cog"] = ActionOpenConfig,
                ["piemenu-house"] = ActionOpenMapMenu,
                ["piemenu-log-out"] = ActionSceneExit,
                ["piemenu-mars"] = ActionOpenMaleMenu,
                ["piemenu-move-3d"] = ActionOpenMoveMenu,
                ["piemenu-palette"] = ActionOpenEditCharacterMenu,
                ["piemenu-repeat-2"] = ActionReplaceActiveFemale,
                ["piemenu-spotlight"] = ActionOpenLightingMenu
            };
        }

        private void UpdateState() {
            if (Ash.AshUI.IuiMain.IsAnySubMenuVisible)
                return;

            if (Ash.AshUI.IuiMain.IsAnyCanvasRenderedOnTop)
                return;

            RadialMenuInputHandler.UpdateCursorPosition();

            UpdateRadialMenuVisibility();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateRadialMenuVisibility() {
            if (!IuiMain.IsLegalScene)
                return;

            var isHotkeyReleased = RadialMenuInputHandler.IsHotkeyReleased();

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (IsCanceled && isHotkeyReleased) {
                IsCanceled = false; // reset IsCanceled if it was set to TRUE and hotkey got released
                return;
            }

            if (IsCanceled)
                return;

            var isHotkeyHeld = RadialMenuInputHandler.IsHotkeyHeld();
            var isCancelPressed = RadialMenuInputHandler.IsCanceled();

            if (!isCancelPressed) {
                if (isHotkeyHeld)
                    UpdateMenuFromCurrentCursorPos();

                if (isHotkeyReleased && SelectedIndex >= 0) {
                    try {
                        ActivateActionByName(GetIconNameByIndex(SelectedIndex));
                    }
                    catch (Exception e) {
                        Ash.Logger.LogError(e);
                    }
                }
            }

            if (isHotkeyReleased || isCancelPressed) {
                HideUI(isHotkeyHeld, isCancelPressed);
                return;
            }

            if (!Ash.PersistentSettings.IsImmersiveUiEnabled.Value) {
                Ash.Logger.LogDebug($"Immersive UI is disabled");
                return;
            }

            if (isHotkeyHeld)
                ShowUI();
        }

        private void ActivateActionByName(string key) {
            if (!Actions.TryGetValue(key, out var action)) {
                throw new NotImplementedException($"Action for {key} is not implemented.");
            }

            action();
        }

        private void ShowUI() {
            if (!CanvasGameObj) {
                Ash.Logger.LogError("Unable to toggle Radial Menu visibility - Canvas not found.");
                return;
            }

            if (!RadialMenuMain.RadialMenuRootGameObject) {
                Ash.Logger.LogError("Unable to toggle Radial Menu visibility - RadialMenuRootGameObject not found.");
                return;
            }

            if (RadialMenuMain.RadialMenuRootGameObject.activeSelf)
                return;

            SetInitialState();

            RadialMenuMain.RadialMenuRootGameObject.SetActive(true);
        }

        private void HideUI(bool isHotkeyHeld, bool isCanceled) {
            if (!CanvasGameObj) {
                Ash.Logger.LogError("Unable to toggle Radial Menu visibility - Canvas not found.");
                return;
            }

            if (!RadialMenuMain.RadialMenuRootGameObject) {
                Ash.Logger.LogError("Unable to toggle Radial Menu visibility - RadialMenuRootGameObject not found.");
                return;
            }

            RadialMenuMain.RadialMenuRootGameObject.SetActive(false);

            IsCanceled = isHotkeyHeld && isCanceled;
            SelectedIndex = -1;

            Cursor.visible = true;
        }

        private void SetInitialState() {
            for (var n = 0; n < RadialMenuMain.IconActiveStrokeGameObjects.Count; n++) {
                RadialMenuMain.IconActiveStrokeGameObjects[n].SetActive(false);
                ImageUtils.SetRawImageTransparency(
                    RadialMenuMain.IconActiveStrokeGameObjects[n].GetComponent<RawImage>(), 0);
                ImageUtils.SetRawImageTransparency(RadialMenuMain.IconTextures[n],
                    RadialMenuConfig.InactiveIconTransparencyValue);
            }

            RadialMenuMain.ActiveElementTextComponent.text = "";

            RadialMenuMain.SectorHighlightGameObject.SetActive(false);
            RadialMenuMain.CancelGroupGameObject.SetActive(false);

            SelectedIndex = -1;
        }

        private void SetActiveElement(int angle) {
            var increment = 360f / RadialMenuMain.IconPathsFromBundle.Count;
            var index = (RadialMenuMain.IconPathsFromBundle.Count - (int)Math.Round(angle / increment)) %
                        RadialMenuMain.IconPathsFromBundle.Count;

            for (var n = 0; n < RadialMenuMain.IconActiveStrokeGameObjects.Count; n++) {
                RadialMenuMain.IconActiveStrokeGameObjects[n].SetActive(n == index);

                var iconActiveRingRi = RadialMenuMain.IconActiveStrokeGameObjects[n].GetComponent<RawImage>();
                ImageUtils.SetRawImageTransparency(iconActiveRingRi, n == index ? RadialMenuConfig.ActiveStrokeTransparencyValue : 0);

                var iconTextureRi = RadialMenuMain.IconTextures[n];
                ImageUtils.SetRawImageTransparency(iconTextureRi,
                    n == index
                        ? RadialMenuConfig.ActiveIconTransparencyValue
                        : RadialMenuConfig.InactiveIconTransparencyValue);
            }

            var iconPaths = RadialMenuMain.IconPathsFromBundle;
            var iconPath = iconPaths[index];
            var iconFileName = Path.GetFileNameWithoutExtension(iconPath);

            RadialMenuMain.ActiveElementTextComponent.text = IconLabels.GetValueOrDefaultValue(iconFileName, ErrorLabel);

            IuiPositioningHelpers.RotateToIconAtIndex(RadialMenuMain.IconPathsFromBundle, index,
                RadialMenuMain.SectorHighlightRectTransform);
            IuiPositioningHelpers.RotateToAngle(RadialMenuMain.MiddleHighlightRectTransform, angle);

            RadialMenuMain.MiddleHighlightGameObject.SetActive(true);
            RadialMenuMain.SectorHighlightGameObject.SetActive(true);
            RadialMenuMain.CancelGroupGameObject.SetActive(true);

            SelectedIndex = index;
        }

        private void UnsetActiveElement() {
            for (var n = 0; n < RadialMenuMain.IconActiveStrokeGameObjects.Count; n++) {
                RadialMenuMain.IconActiveStrokeGameObjects[n].SetActive(false);
                ImageUtils.SetRawImageTransparency(
                    RadialMenuMain.IconActiveStrokeGameObjects[n].GetComponent<RawImage>(),
                    0);
                ImageUtils.SetRawImageTransparency(RadialMenuMain.IconTextures[n],
                    RadialMenuConfig.InactiveIconTransparencyValue);
            }

            RadialMenuMain.ActiveElementTextComponent.text = "";

            IuiPositioningHelpers.RotateToAngle(RadialMenuMain.MiddleHighlightRectTransform, 0);
            RadialMenuMain.MiddleHighlightGameObject.SetActive(false);

            IuiPositioningHelpers.RotateToIconAtIndex(RadialMenuMain.IconPathsFromBundle, 0,
                RadialMenuMain.SectorHighlightRectTransform);
            RadialMenuMain.SectorHighlightGameObject.SetActive(false);

            RadialMenuMain.CancelGroupGameObject.SetActive(false);

            SelectedIndex = -1;
        }

        private void UpdateMenuFromCurrentCursorPos() {
            Cursor.visible = false;

            var angle = RadialMenuInputHandler.GetAngleByVector();
            if (angle < 0) {
                UnsetActiveElement();
                WasInDeadZone = true;
            }
            else {
                // if prev frame was in deadzone - skip sector angle animation
                if (WasInDeadZone)
                    IuiPositioningHelpers.RotateToAngle(RadialMenuMain.SectorHighlightRectTransform, angle);

                SetActiveElement(angle);
                RadialMenuMain.SectorHighlightGameObject.SetActive(true);
                RadialMenuMain.CancelGroupGameObject.SetActive(true);

                WasInDeadZone = false;
            }
        }

        private string GetIconNameByIndex(int index) {
            var path = RadialMenuMain.IconPathsFromBundle[index];
            return Path.GetFileNameWithoutExtension(path);
        }


        // MENU ACTIONS

        private void ActionOpenConfig() {
            if (SceneTypeTracker.Scene is H_Scene scene) {
                scene.OpenConfig();
            }
        }

        private void ActionOpenMapMenu() {
            throw new NotImplementedException();
        }

        private void ActionSceneExit() {
            if (SceneTypeTracker.Scene is H_Scene scene) {
                scene.Button_Exit();
            }
        }

        private void ActionOpenMaleMenu() {
            throw new NotImplementedException();
        }

        private void ActionOpenMoveMenu() {
            throw new NotImplementedException();
        }

        private void ActionOpenEditCharacterMenu() {
            throw new NotImplementedException();
        }

        private void ActionReplaceActiveFemale() {
            if (SceneTypeTracker.Scene is H_Scene scene) {
                scene.SwapVisitor();
            }
        }

        private void ActionOpenLightingMenu() {
            throw new NotImplementedException();
        }
    }
}
