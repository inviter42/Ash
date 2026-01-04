using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components;
using UnityEngine;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.FormState;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView
{
    public class ItemsCoordinatorView
    {
        // Tab Label
        public const string ItemsVisibilityCoordinatorTabLabel = "Items Visibility Coordinator";

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void DrawView() {
            if (IsMasterItemSelected()) {
                RuleDetailsComponent.DrawRuleDetailsView();
            }
            else {
                MasterItemSelectionComponent.DrawMasterItemSelectionComponent();
                GUILayout.Space(10);
                RulesListComponent.DrawRulesList();
            }
        }
    }
}
