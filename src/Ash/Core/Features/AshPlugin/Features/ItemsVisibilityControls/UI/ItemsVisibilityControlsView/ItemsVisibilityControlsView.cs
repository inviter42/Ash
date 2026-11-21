using Ash.Core.Features.AshPlugin.Common.Components;
using Ash.Core.Features.AshPlugin.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components;
using Ash.Core.Features.AshPlugin.Main;
using Ash.Core.Features.AshPlugin.Main.Types;
using Ash.Logging;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;

namespace Ash.Core.Features.AshPlugin.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView;

internal class ItemsVisibilityControlsView
{
    internal static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.ItemsVisibilityControls);

    // Tab Label
    internal const string ItemsVisibilityControlsTabLabel = "Item Visibility";

    internal Female ActiveFemale => GetActiveFemale();

    internal void DrawView() {
        if (ActiveFemale == null) {
            GUILayout.Label("No active female");
            return;
        }

        using (new GUILayout.VerticalScope("box")) {
            Title(ActiveFemale.heroineID.ToString());

            FemaleSelectionComponent.Component(ActiveFemale, SetActiveFemale);

            GUILayout.Space(12);

            AllClothingVisibilitySelectionComponent.Component(ActiveFemale);
            IndividualWearsVisibilitySelectionComponent.Component(ActiveFemale);

            GUILayout.Space(12);

            AllAccessoriesVisibilitySelectionComponent.Component(ActiveFemale);
            IndividualAccessoriesVisibilitySelectionComponent.Component(ActiveFemale);
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
