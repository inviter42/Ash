using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.GameUIControls.UI.GameUIControlsView;
using Ash.Core.Features.HSceneControls.UI.HSceneControlsView;
using Ash.Core.Features.HSceneSettings.UI.HSceneSettingsView;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView;
using Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView;
using Ash.Core.SceneManagement;

namespace Ash.Core.UI.Types
{
    internal class HSceneWindow : AshWindow<HSceneWindow>
    {
        private GameUIControlsView GameUIControlsViewInstance;
        private HSceneControlsView HSceneControlsViewInstance;
        private HSceneSettingsView HSceneSettingsViewInstance;
        private ItemsVisibilityControlsView ItemsVisibilityControlsViewInstance;
        private ItemsCoordinatorView ItemsCoordinatorViewInstance;

        private Female ActiveFemale { get; set; }

        private void Awake() {
            GameUIControlsViewInstance = new GameUIControlsView();
            ItemsVisibilityControlsViewInstance = new ItemsVisibilityControlsView();
            ItemsCoordinatorViewInstance = new ItemsCoordinatorView();
            HSceneControlsViewInstance = new HSceneControlsView();
            HSceneSettingsViewInstance = new HSceneSettingsView();
        }

        protected override string[] SetTabsModel() {
            return new[] {
                GameUIControlsView.UIControlsLabel,
                ItemsCoordinatorView.ItemsVisibilityCoordinatorTabLabel,
                ItemsVisibilityControlsView.ItemsVisibilityControlsTabLabel,
                HSceneControlsView.HSceneControlsViewTabLabel,
                HSceneSettingsView.HSceneSettingsViewTabLabel,
            };
        }

        protected override List<Action> SetTabViewsModel() {
            return new List<Action> {
                GameUIControlsViewInstance.DrawView,
                ItemsCoordinatorViewInstance.DrawView,
                ItemsVisibilityControlsViewInstance.DrawView,
                HSceneControlsViewInstance.DrawView,
                HSceneSettingsViewInstance.DrawView,
            };
        }

        internal Female GetActiveFemale() {
            if (ActiveFemale == null)
                InitializeActiveFemale();

            return ActiveFemale;
        }

        internal void SetActiveFemale(Female female) {
            ActiveFemale = female;
        }

        private void InitializeActiveFemale() {
            var registeredFemales = SceneComponentRegistry.GetComponentsOfType<Female>().ToArray();

            if (registeredFemales.Length == 0) {
                Ash.Logger.LogError("Unable to find Female components in the SceneComponentRegistry");
                return;
            }

            ActiveFemale = SceneComponentRegistry.GetComponentsOfType<Female>().ToArray()[0];
        }
    }
}
