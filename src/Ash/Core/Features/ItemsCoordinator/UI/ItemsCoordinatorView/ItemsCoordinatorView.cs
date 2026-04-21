using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.InterItemCoordination;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State;
using UnityEngine;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView
{
    internal class ItemsCoordinatorView
    {
        // Tab Label
        internal const string ItemsVisibilityCoordinatorTabLabel = "Item Rules";
        internal static string SelectedRuleType = HPosRuleItemSelectionComponent.HPosRuleTypeStateKey;

        private HPosRuleForm HPosRuleForm;
        private InterItemRuleForm InterItemRuleForm;

        // ReSharper disable once MemberCanBeMadeStatic.Global
        internal void DrawView() {
            switch (SelectedRuleType) {
                case HPosRuleItemSelectionComponent.HPosRuleTypeStateKey:
                    if (HPosRuleForm == null)
                        HPosRuleForm = new HPosRuleForm();

                    if (HPosRuleItemSelectionComponent.IsHPosItemSelected(HPosRuleForm)) {
                        HPosRuleDetailsComponent.DrawHPosRuleDetailsView(HPosRuleForm);
                    }
                    else {
                        HPosRuleItemSelectionComponent.DrawHPosRuleItemSelectionComponent(HPosRuleForm);
                        GUILayout.Space(10);
                        HPosRulesListComponent.DrawHPosRulesList();
                    }
                    break;
                case MasterItemSelectionComponent.InterItemRuleTypeStateKey:
                    if (InterItemRuleForm == null)
                        InterItemRuleForm = new InterItemRuleForm();

                    if (MasterItemSelectionComponent.IsMasterItemSelected(InterItemRuleForm)) {
                        InterItemRuleDetailsComponent.DrawInterItemRuleDetailsView(InterItemRuleForm);
                    }
                    else {
                        MasterItemSelectionComponent.DrawMasterItemSelectionComponent(InterItemRuleForm);
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
