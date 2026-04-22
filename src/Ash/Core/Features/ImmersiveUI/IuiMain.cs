using System;
using System.Linq;
using Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper;
using Ash.Core.Features.ImmersiveUI.Components.Config;
using Ash.Core.Features.ImmersiveUI.Components.Generic;
using Ash.Core.Features.ImmersiveUI.Components.Textures;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Data;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.State;
using Ash.Core.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.ImmersiveUI
{
    internal class IuiMain
    {
        internal GameObject CanvasGameObj;

        internal readonly IuiMainStateControl StateControl;

        internal bool IsRadialMenuVisible => RadialMenuMain.StateControl.IsVisible;

        // todo: add all future submenus visibility flags here
        internal bool IsAnySubMenuVisible =>
            StylesMenuMain.StateControl.IsVisible
            || GagMenuMain.StateControl.IsVisible
            || WearsMenuMain.StateControl.IsVisible;

        internal bool IsAnyCanvasRenderedOnTop =>
            IsPauseMenuRendered || IsConfigMenuRendered || IsEditModeRendered;

        // ReSharper disable once MemberCanBePrivate.Global
        internal static bool IsPauseMenuRendered => PauseMenuCanvas.gameObject.activeSelf && PauseMenuCanvas.enabled;
        internal static bool IsConfigMenuRendered => ConfigCanvas.gameObject.activeSelf && ConfigCanvas.enabled;
        internal static bool IsEditModeRendered {
            get {
                if (SceneTypeTracker.Scene is H_Scene hScene)
                    return hScene.editMode != null;

                return false;
            }
        }

        internal static bool IsLegalScene => LegalScenes.Contains(SceneTypeTracker.TypeOfCurrentScene);

        internal event Action<Female> FemaleHasBeenEdited;

        internal static readonly RadialMenuConfig RadialMenuConfig = new RadialMenuConfig();
        internal static readonly StylesMenuConfig StylesMenuConfig = new StylesMenuConfig();
        internal static readonly GagMenuConfig GagMenuConfig = new GagMenuConfig();
        internal static readonly WearsMenuConfig WearsMenuConfig = new WearsMenuConfig();

        internal static readonly GenericComponentsConfig GenericComponentsConfig = new GenericComponentsConfig();
        internal static readonly SharedTextures SharedTextures = new SharedTextures();

        private readonly RadialMenuMain RadialMenuMain;
        private readonly StylesMenuMain StylesMenuMain;
        private readonly GagMenuMain GagMenuMain;
        private readonly WearsMenuMain WearsMenuMain;

        private static Canvas ConfigCanvas;
        private static Canvas PauseMenuCanvas;

        private static readonly SceneTypeTracker.SceneTypes[] LegalScenes =
            { SceneTypeTracker.SceneTypes.H };

        internal IuiMain() {
            if (!IsLegalScene)
                return;

            if (SceneTypeTracker.Scene is H_Scene hScene) {
                ConfigCanvas = hScene.config.GetComponent<Canvas>();
                PauseMenuCanvas = hScene.pauseMenue.GetComponent<Canvas>();
            }

            // initialize data
            CreateCanvas();
            AddCommandBufferBlurComponentToCamera();

            InGameUIManagementHelper.SwitchToImmersiveUIMode();

            StateControl = new IuiMainStateControl(this);

            StylesMenuMain = new StylesMenuMain(this, CanvasGameObj);
            GagMenuMain = new GagMenuMain(this, CanvasGameObj);
            WearsMenuMain = new WearsMenuMain(this, CanvasGameObj);
            RadialMenuMain = new RadialMenuMain(
                this,
                CanvasGameObj,
                StylesMenuMain,
                GagMenuMain,
                WearsMenuMain
            );

            CreateAdditionalUIElements();
        }

        internal void RaiseEventCharacterHasBeenEdited(Human human) {
            if (human is Female female)
                FemaleHasBeenEdited?.Invoke(female);
        }

        private void AddCommandBufferBlurComponentToCamera() {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (SceneTypeTracker.TypeOfCurrentScene) {
                // todo: in SelectScene backbuffer contents are v-flipped
                case SceneTypeTracker.SceneTypes.H:
                {
                    Ash.Logger.LogDebug($"Instantiating CommandBufferBlur in {SceneTypeTracker.TypeOfCurrentScene}");
                    var illusionCamera = Object.FindObjectOfType<IllusionCamera>();
                    if (illusionCamera == null) {
                        Ash.Logger.LogWarning(
                            $"Unable to find IllusionCamera in the scene {SceneTypeTracker.TypeOfCurrentScene}");
                        return;
                    }

                    illusionCamera.gameObject.AddComponent<CommandBufferBlur>();
                    break;
                }
            }
        }

        private void CreateCanvas() {
            if (CanvasGameObj != null)
                return;

            CanvasGameObj = new GameObject("Ash.IuiCanvas", typeof(Canvas), typeof(StylesMenuStylesData));

            var canvas = CanvasGameObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -1;
        }

        private void CreateAdditionalUIElements() {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null)
                return;

            var leftMiddleToggles = hScene.middleLeftCanvas.transform.Find("LeftMiddleToggles").gameObject;

            // ReSharper disable once ConvertToLocalFunction
            Action expandLeftMiddleToggles = () => { leftMiddleToggles.SetActive(!leftMiddleToggles.activeSelf); };
            var expandLeftMiddleTogglesButtonIcon = Ash.AshUI.ImmersiveUIIconsAssetBundle.LoadAsset<Texture2D>(
                "assets/frostedglass/icons/small-arrow-right.png");

            var expandLeftMiddleTogglesButton = RoundButtonWithIcon.CreateRoundButtonWithIcon(
                CanvasGameObj,
                expandLeftMiddleToggles,
                expandLeftMiddleTogglesButtonIcon,
                new Vector2(16, 16),
                new Vector2(1, 0),
                0.5f
            );

            IuiPositioningHelpers.AnchorsLeftCenter(expandLeftMiddleTogglesButton, new Vector2(20, 0), new Vector2(0, 0.5f));
        }
    }
}
