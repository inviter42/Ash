using Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination;
using Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.InterItemCoordination;
using Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State;
using Ash.Core.Features.AshPlugin.Main;
using Ash.Core.Features.AshPlugin.Main.Types;
using Ash.Logging;
using UnityEngine;

namespace Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.UI.ItemsCoordinatorView;

internal class ItemsCoordinatorView
{
    internal static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.ItemsCoordinator);

    // Tab Label
    internal const string ItemsVisibilityCoordinatorTabLabel = "Item Rules";
    internal static string SelectedRuleType = HPosRuleItemSelectionComponent.HPosRuleTypeStateKey;

    internal Female ActiveFemale => GetActiveFemale();

    private HPosRuleForm HPosRuleForm;
    private InterItemRuleForm InterItemRuleForm;

    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal void DrawView() {
        if (ActiveFemale == null) {
            GUILayout.Label("No active female");
            return;
        }

        switch (SelectedRuleType) {
            case HPosRuleItemSelectionComponent.HPosRuleTypeStateKey:
                HPosRuleForm ??= new HPosRuleForm();

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
                InterItemRuleForm ??= new InterItemRuleForm();

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
                ItemsCoordinator.Logger.LogWarning("Unknown Rule Type");
                break;
        }
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private Female GetActiveFemale() {
        switch (WindowManager.Window) {
            case EditSceneWindow editSceneWindow:
                return editSceneWindow.ActiveFemale;
            case HSceneWindow hSceneWindow:
                return hSceneWindow.ActiveFemale;
            default:
                Logger.LogError($"View ItemsVisibilityControlsView is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                return null;
        }
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private void SetActiveFemale(Female female) {
        switch (WindowManager.Window) {
            case EditSceneWindow editSceneWindow:
                editSceneWindow.ActiveFemale = female;
                break;
            case HSceneWindow hSceneWindow:
                hSceneWindow.ActiveFemale = female;
                break;
            default:
                Logger.LogError($"Component FemaleSelectionComponent is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                return;
        }
    }
}
