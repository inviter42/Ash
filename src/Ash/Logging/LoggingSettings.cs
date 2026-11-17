using System.Collections.Generic;
using Ash.Utility.GlobalUtils;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace Ash.Logging;

internal static class LoggingSettings
{
    private static readonly Dictionary<LoggingModules, ConfigEntry<LogLevel>> LoggingState = new() {
        [LoggingModules.Global] = Ash.ConfigEntryGlobalModuleLogLevel,
        [LoggingModules.ItemsCoordinator] = Ash.ConfigEntryItemsCoordinatorModuleLogLevel,
        [LoggingModules.ExtDataTattoos] = Ash.ConfigEntryExtDataTattoosModuleLogLevel,
        [LoggingModules.ExtDataEyeShadows] = Ash.ConfigEntryExtDataEyeShadowsModuleLogLevel,
        [LoggingModules.ExtDataCheekShadows] = Ash.ConfigEntryExtDataCheekShadowsModuleLogLevel,
        [LoggingModules.ExtDataMoles] = Ash.ConfigEntryExtDataMolesModuleLogLevel,
        [LoggingModules.Actions] = Ash.ConfigEntryActionsModuleLogLevel,
        [LoggingModules.GameUIControls] = Ash.ConfigEntryGameUIControlsModuleLogLevel,
        [LoggingModules.HSceneControls] = Ash.ConfigEntryHSceneControlsModuleLogLevel,
        [LoggingModules.HSceneSettings] = Ash.ConfigEntryHSceneSettingsModuleLogLevel,
        [LoggingModules.BugFixes] = Ash.ConfigEntryBugFixesModuleLogLevel,
        [LoggingModules.AnimationControls] = Ash.ConfigEntryAnimationControlsModuleLogLevel,
        [LoggingModules.ImmersiveUI] = Ash.ConfigEntryImmersiveUIModuleLogLevel,
        [LoggingModules.ItemsVisibilityControls] = Ash.ConfigEntryItemsVisibilityControlsModuleLogLevel,
        [LoggingModules.SceneManagement] = Ash.ConfigEntrySceneManagementModuleLogLevel,
        [LoggingModules.HMembersHooks] = Ash.ConfigEntryHMembersHooksModuleLogLevel,
        [LoggingModules.HumanHooks] = Ash.ConfigEntryHumanHooksModuleLogLevel,
        [LoggingModules.WearablesHooks] = Ash.ConfigEntryWearablesHooksModuleLogLevel,
        [LoggingModules.BetterPenetration] = Ash.ConfigEntryBetterPenetrationModuleLogLevel,
        [LoggingModules.ApplicablesMigrations] = Ash.ConfigEntryApplicablesMigrationsModuleLogLevel,
    };

    internal enum LoggingModules
    {
        Global,
        ItemsCoordinator,
        ExtDataTattoos,
        ExtDataEyeShadows,
        ExtDataCheekShadows,
        ExtDataMoles,
        Actions,
        GameUIControls,
        HSceneControls,
        HSceneSettings,
        BugFixes,
        AnimationControls,
        ImmersiveUI,
        ItemsVisibilityControls,
        SceneManagement,
        HMembersHooks,
        HumanHooks,
        WearablesHooks,
        BetterPenetration,
        ApplicablesMigrations
    }

    internal static LogLevel GetLogLevel(LoggingModules module) {
        return LoggingState.GetValueOrDefaultValue(module, null)?.Value ?? LogLevel.None;
    }
}
