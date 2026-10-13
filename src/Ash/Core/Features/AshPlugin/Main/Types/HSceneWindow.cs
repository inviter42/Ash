using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable once RedundantUsingDirective
using Ash.Core.Features.AshPlugin.Features.AnimationControls.UI.AnimationControlsView;
using Ash.Core.Features.AshPlugin.Features.BetterPenetrationSettings.UI.BetterPenetrationSettingsView;
using Ash.Core.Features.AshPlugin.Features.FemaleControls.UI.FemaleControlsView;
using Ash.Core.Features.AshPlugin.Features.GameUIControls.UI.GameUIControlsView;
using Ash.Core.Features.AshPlugin.Features.HSceneCustomization.UI.HSceneCustomizationView;
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
        private FemaleControlsView FemaleControlsViewInstance;
        private HSceneCustomizationView HSceneCustomizationViewInstance;
        private ItemsVisibilityControlsView ItemsVisibilityControlsViewInstance;
        private ItemsCoordinatorView ItemsCoordinatorViewInstance;
        private BetterPenetrationSettingsView BetterPenetrationSettingsViewInstance;
#if DEBUG
        private AnimationControlsView AnimationControlsViewInstance;
#endif

        // ReSharper disable once InconsistentNaming
        private Female _ActiveFemale;

        private void Awake() {
            GameUIControlsViewInstance = new GameUIControlsView();
            ItemsVisibilityControlsViewInstance = new ItemsVisibilityControlsView();
            ItemsCoordinatorViewInstance = new ItemsCoordinatorView();
            FemaleControlsViewInstance = new FemaleControlsView(this);
            HSceneCustomizationViewInstance = new HSceneCustomizationView(this);
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
                FemaleControlsView.HSceneControlsViewTabLabel,
                HSceneCustomizationView.HSceneSettingsViewTabLabel,
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
                FemaleControlsViewInstance.DrawView,
                HSceneCustomizationViewInstance.DrawView,
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
