using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView;
using Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView;
using Ash.Core.SceneManagement;

namespace Ash.Core.UI.Types
{
    public class EditSceneWindow : AshWindow<EditSceneWindow>
    {
        private ItemsVisibilityControlsView ItemsVisibilityControlsViewInstance;
        private ItemsCoordinatorView ItemsCoordinatorViewInstance;

        public Female ActiveFemale { get; set; }

        public void Awake() {
            InitializeActiveFemale();
            ItemsVisibilityControlsViewInstance = new ItemsVisibilityControlsView();
            ItemsCoordinatorViewInstance = new ItemsCoordinatorView();
        }

        protected override string[] SetTabsModel() {
            return new[] {
                ItemsVisibilityControlsView.ItemsVisibilityControlsTabLabel,
                ItemsCoordinatorView.ItemsVisibilityCoordinatorTabLabel
            };
        }

        protected override List<Action> SetTabViewsModel() {
            return new List<Action> {
                ItemsVisibilityControlsViewInstance.DrawView,
                ItemsCoordinatorViewInstance.DrawView
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
