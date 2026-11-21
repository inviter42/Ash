using System;
using System.Collections.Generic;
using Ash.Core.Features.AshPlugin.Features.AhegaoFaces.UI.AhegaoFacesView;
using Ash.Core.Features.AshPlugin.Features.BetterPenetrationSettings.UI.BetterPenetrationSettingsView;
using Ash.Core.Features.AshPlugin.Features.BugFixes.UI.BugFixesView;
using Ash.Core.Features.AshPlugin.Features.FemaleControls.UI.FemaleControlsView;
using Ash.Core.Features.AshPlugin.Features.FemaleSettings.UI.FemaleSettingsView;
using Ash.Core.Features.AshPlugin.Features.HSceneCustomization.UI.HSceneCustomizationView;
using Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.UI.ItemsCoordinatorView;
using Ash.Core.Features.AshPlugin.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView;
using Ash.Core.Features.AshPlugin.Features.MaleSettings.UI.MaleSettingsView;
using Ash.Core.Features.AshPlugin.Features.UISettings.UI.UISettingsView;
using Ash.Core.Tooling.SceneManagement;
#if DEBUG
using Ash.Core.Features.AshPlugin.Features.DebuggingTesting.UI.DebugView;
#endif

namespace Ash.Core.Features.AshPlugin.Main.Types;

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

    internal Male ActiveMale {
        get {
            if (_ActiveMale == null)
                InitializeActiveMale();

            return _ActiveMale;
        }
        set => _ActiveMale = value;
    }
    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal H_Scene HScene => (H_Scene)SceneTypeTracker.Scene;


    private UISettingsView UISettingsViewInstance;
    private ItemsVisibilityControlsView ItemsVisibilityControlsViewInstance;
    private ItemsCoordinatorView ItemsCoordinatorViewInstance;
    private FemaleControlsView FemaleControlsViewInstance;
    private BetterPenetrationSettingsView BetterPenetrationSettingsViewInstance;
    private AhegaoFacesView AhegaoFacesViewInstance;
    private BugFixesView BugFixesViewInstance;
    private HSceneCustomizationView HSceneCustomizationViewInstance;
    private FemaleSettingsView FemaleSettingsViewInstance;
    private MaleSettingsView MaleSettingsViewInstance;
#if DEBUG
    private DebugView DebugViewInstance;
#endif

    // ReSharper disable once InconsistentNaming
    private Female _ActiveFemale;
    // ReSharper disable once InconsistentNaming
    private Male _ActiveMale;

    private void Awake() {
        UISettingsViewInstance = new UISettingsView();
        ItemsVisibilityControlsViewInstance = new ItemsVisibilityControlsView();
        ItemsCoordinatorViewInstance = new ItemsCoordinatorView();
        FemaleControlsViewInstance = new FemaleControlsView(this);
        BetterPenetrationSettingsViewInstance = new BetterPenetrationSettingsView();
        AhegaoFacesViewInstance = new AhegaoFacesView();
        BugFixesViewInstance = new BugFixesView();
        HSceneCustomizationViewInstance = new HSceneCustomizationView();
        FemaleSettingsViewInstance = new FemaleSettingsView(this);
        MaleSettingsViewInstance = new MaleSettingsView(this);
#if DEBUG
        DebugViewInstance = new DebugView(this);
#endif

        WindowData = new Dictionary<string, Dictionary<string, Action>> {
            ["Scene"] = new() {
                    [FemaleControlsView.FemaleControlsViewTabLabel] = FemaleControlsViewInstance.DrawView,
                    [HSceneCustomizationView.HSceneCustomizationViewTabLabel] = HSceneCustomizationViewInstance.DrawView,
                    [AhegaoFacesView.AhegaoFacesViewTabLabel] = AhegaoFacesViewInstance.DrawView,
            },
            ["Items"] = new() {
                [ItemsCoordinatorView.ItemsVisibilityCoordinatorTabLabel] = ItemsCoordinatorViewInstance.DrawView,
                [ItemsVisibilityControlsView.ItemsVisibilityControlsTabLabel] = ItemsVisibilityControlsViewInstance.DrawView,
            },
            ["Settings"] = new() {
                [FemaleSettingsView.FemaleSettingsViewTabLabel] = FemaleSettingsViewInstance.DrawView,
                [MaleSettingsView.MaleSettingsViewTabLabel] = MaleSettingsViewInstance.DrawView,
                [BetterPenetrationSettingsView.BetterPenetrationViewTabLabel] = BetterPenetrationSettingsViewInstance.DrawView,
                [UISettingsView.UISettingsLabel] = UISettingsViewInstance.DrawView,
            },
            ["Other"] = new() {
                [BugFixesView.BugFixesViewTabLabel] = BugFixesViewInstance.DrawView,
            },
#if DEBUG
            ["Debug"] = new() {
                [DebugView.DebugViewTabLabel] = DebugViewInstance.DrawView,
            },
#endif
        };
    }

    private void InitializeActiveFemale() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        ActiveFemale = hScene.mainMembers.GetFemale(0);
    }

    private void InitializeActiveMale() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        ActiveMale = hScene.mainMembers.GetMale(0);
    }
}
