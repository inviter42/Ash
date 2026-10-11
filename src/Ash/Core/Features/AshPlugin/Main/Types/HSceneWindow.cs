using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.AshPlugin.Features.AnimationControls.UI.AnimationControlsView;
using Ash.Core.Features.AshPlugin.Features.BetterPenetrationSettings.UI.BetterPenetrationSettingsView;
using Ash.Core.Features.AshPlugin.Features.GameUIControls.UI.GameUIControlsView;
using Ash.Core.Features.AshPlugin.Features.HSceneControls.UI.HSceneControlsView;
using Ash.Core.Features.AshPlugin.Features.HSceneSettings.UI.HSceneSettingsView;
using Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.UI.ItemsCoordinatorView;
using Ash.Core.Features.AshPlugin.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView;
using Ash.Core.Tooling.SceneManagement;

namespace Ash.Core.Features.AshPlugin.Main.Types
{
    internal class HSceneWindow : AshWindow<HSceneWindow>
    {
        internal Female ActiveFemale {
            get {
                if (_ActiveFemale == null)
                    InitializeActiveFemale();

                return _ActiveFemale;
            }
            set => _ActiveFemale = value;
        }

        internal H_Scene HScene => (H_Scene)SceneTypeTracker.Scene;


        private GameUIControlsView GameUIControlsViewInstance;
        private HSceneControlsView HSceneControlsViewInstance;
        private HSceneSettingsView HSceneSettingsViewInstance;
        private ItemsVisibilityControlsView ItemsVisibilityControlsViewInstance;
        private ItemsCoordinatorView ItemsCoordinatorViewInstance;
        private BetterPenetrationSettingsView BetterPenetrationSettingsViewInstance;
#if DEBUG
        private AnimationControlsView AnimationControlsViewInstance;
#endif

        private Female _ActiveFemale;

        private void Awake() {
            GameUIControlsViewInstance = new GameUIControlsView();
            ItemsVisibilityControlsViewInstance = new ItemsVisibilityControlsView();
            ItemsCoordinatorViewInstance = new ItemsCoordinatorView();
            HSceneControlsViewInstance = new HSceneControlsView(this);
            HSceneSettingsViewInstance = new HSceneSettingsView(this);
            BetterPenetrationSettingsViewInstance = new BetterPenetrationSettingsView();
#if DEBUG
            AnimationControlsViewInstance = new AnimationControlsView();
#endif
        }

        protected override string[] SetTabsModel() {
            return new[] {
                GameUIControlsView.UIControlsLabel,
                ItemsCoordinatorView.ItemsVisibilityCoordinatorTabLabel,
                ItemsVisibilityControlsView.ItemsVisibilityControlsTabLabel,
                HSceneControlsView.HSceneControlsViewTabLabel,
                HSceneSettingsView.HSceneSettingsViewTabLabel,
                BetterPenetrationSettingsView.AnimationControlsViewTabLabel,
#if DEBUG
                AnimationControlsView.AnimationControlsViewTabLabel,
#endif
            };
        }

        protected override List<Action> SetTabViewsModel() {
            return new List<Action> {
                GameUIControlsViewInstance.DrawView,
                ItemsCoordinatorViewInstance.DrawView,
                ItemsVisibilityControlsViewInstance.DrawView,
                HSceneControlsViewInstance.DrawView,
                HSceneSettingsViewInstance.DrawView,
                BetterPenetrationSettingsViewInstance.DrawView,
#if DEBUG
                AnimationControlsViewInstance.DrawView,
#endif
            };
        }

        private void InitializeActiveFemale() {
            var registeredFemales = SceneComponentRegistry.GetComponentsOfType<Female>().ToArray();

            if (registeredFemales.Length == 0) {
                Ash.Logger.LogError("Unable to find Female components in the SceneComponentRegistry");
                return;
            }

            ActiveFemale = registeredFemales[0];
        }
    }
}
