using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.GameUIControls.UI.GameUIControlsView;
using Ash.Core.Features.HSceneControls.UI.HSceneControlsView;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView;
using Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView;
using Ash.Core.SceneManagement;

namespace Ash.Core.UI.Types
{
    public class HSceneWindow : AshWindow<HSceneWindow>
    {
        private GameUIControlsView GameUIControlsViewInstance;
        private HSceneControlsView HSceneControlsViewInstance;
        private ItemsVisibilityControlsView ItemsVisibilityControlsViewInstance;
        private ItemsCoordinatorView ItemsCoordinatorViewInstance;

        public Female ActiveFemale { get; set; }

        public void Awake() {
            InitializeActiveFemale();
            GameUIControlsViewInstance = new GameUIControlsView();
            ItemsVisibilityControlsViewInstance = new ItemsVisibilityControlsView();
            ItemsCoordinatorViewInstance = new ItemsCoordinatorView();
            HSceneControlsViewInstance = new HSceneControlsView();
        }

        protected override string[] SetTabsModel() {
            return new[] {
                GameUIControlsView.UIControlsLabel,
                ItemsVisibilityControlsView.ItemsVisibilityControlsTabLabel,
                ItemsCoordinatorView.ItemsVisibilityCoordinatorTabLabel,
                HSceneControlsView.HSceneControlsViewTabLabel
            };
        }

        protected override List<Action> SetTabViewsModel() {
            return new List<Action> {
                GameUIControlsViewInstance.DrawView,
                ItemsVisibilityControlsViewInstance.DrawView,
                ItemsCoordinatorViewInstance.DrawView,
                () => HSceneControlsViewInstance.DrawView()
            };
        }

        public Female GetActiveFemale() {
            return ActiveFemale;
        }

        public void SetActiveFemale(Female female) {
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
