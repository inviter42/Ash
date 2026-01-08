using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.InterItemCoordination;
using UnityEngine;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.InterItemRuleForm;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView
{
    public class ItemsCoordinatorView
    {
        // Tab Label
        public const string ItemsVisibilityCoordinatorTabLabel = "Items Visibility Coordinator";
        public static string SelectedRuleType = HPosRuleItemSelectionComponent.HPosRuleTypeStateKey;

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void DrawView() {
            switch (SelectedRuleType) {
                case HPosRuleItemSelectionComponent.HPosRuleTypeStateKey:
                    if (HPosRuleItemSelectionComponent.IsHPosItemSelected()) {
                        HPosRuleDetailsComponent.DrawHPosRuleDetailsView();
                    }
                    else {
                        HPosRuleItemSelectionComponent.DrawHPosRuleItemSelectionComponent();
                        GUILayout.Space(10);
                        HPosRulesListComponent.DrawHPosRulesList();
                    }
                    break;
                case MasterItemSelectionComponent.InterItemRuleTypeStateKey:
                    if (MasterItemSelectionComponent.IsMasterItemSelected()) {
                        InterItemRuleDetailsComponent.DrawInterItemRuleDetailsView();
                    }
                    else {
                        MasterItemSelectionComponent.DrawMasterItemSelectionComponent();
                        GUILayout.Space(10);
                        InterItemRulesListComponent.DrawInterItemRulesList();
                    }
                    break;
                default:
                    Ash.Logger.LogWarning("Unknown Rule Type");
                    break;
            }
        }
    }
}
