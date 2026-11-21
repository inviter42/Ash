using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.UI.ItemsCoordinatorView;
using Ash.Core.Features.AshPlugin.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView;
using Ash.Core.Tooling.SceneManagement;

namespace Ash.Core.Features.AshPlugin.Main.Types;

internal class EditSceneWindow : AshWindow<EditSceneWindow>
{
    internal Female ActiveFemale {
        get {
            if (_ActiveFemale == null)
                InitializeActiveFemale();

            return _ActiveFemale;
        }
        set => _ActiveFemale = value;
    }

    internal Male ActiveMale {
        get {
            if (_ActiveMale == null)
                InitializeActiveMale();

            return _ActiveMale;
        }
        set => _ActiveMale = value;
    }


    private ItemsVisibilityControlsView ItemsVisibilityControlsViewInstance;
    private ItemsCoordinatorView ItemsCoordinatorViewInstance;
    // ReSharper disable once InconsistentNaming
    private Female _ActiveFemale;
    // ReSharper disable once InconsistentNaming
    private Male _ActiveMale;

    private void Awake() {
        ItemsVisibilityControlsViewInstance = new ItemsVisibilityControlsView();
        ItemsCoordinatorViewInstance = new ItemsCoordinatorView();

        WindowData = new Dictionary<string, Dictionary<string, Action>> {
            [string.Empty] = new() {
                [ItemsVisibilityControlsView.ItemsVisibilityControlsTabLabel] = ItemsVisibilityControlsViewInstance.DrawView,
                [ItemsCoordinatorView.ItemsVisibilityCoordinatorTabLabel] = ItemsCoordinatorViewInstance.DrawView,
            }
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

    private void InitializeActiveMale() {
        var registeredMales = SceneComponentRegistry.GetComponentsOfType<Male>().ToArray();

        if (registeredMales.Length == 0) {
            Ash.Logger.LogError("Unable to find Male components in the SceneComponentRegistry");
            return;
        }

        ActiveMale = registeredMales[0];
    }
}
