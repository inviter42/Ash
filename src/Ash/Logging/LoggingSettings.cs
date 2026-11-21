using System.Collections.Generic;
using Ash.Utility.GlobalUtils;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace Ash.Logging;

internal static class LoggingSettings
{
    private static readonly Dictionary<LoggingModules, ConfigEntry<LogLevel>> LoggingState = new() {
        [LoggingModules.Actions] = Ash.ConfigEntryActionsModuleLogLevel,
        [LoggingModules.AnimationControls] = Ash.ConfigEntryAnimationControlsModuleLogLevel,
        [LoggingModules.ApplicablesMigrations] = Ash.ConfigEntryApplicablesMigrationsModuleLogLevel,
        [LoggingModules.BetterPenetration] = Ash.ConfigEntryBetterPenetrationModuleLogLevel,
        [LoggingModules.BugFixes] = Ash.ConfigEntryBugFixesModuleLogLevel,
        [LoggingModules.DebuggingTesting] = Ash.ConfigEntryDebugModuleLogLevel,
        [LoggingModules.ExtDataCheekShadows] = Ash.ConfigEntryExtDataCheekShadowsModuleLogLevel,
        [LoggingModules.ExtDataEyeShadows] = Ash.ConfigEntryExtDataEyeShadowsModuleLogLevel,
        [LoggingModules.ExtDataMoles] = Ash.ConfigEntryExtDataMolesModuleLogLevel,
        [LoggingModules.ExtDataTattoos] = Ash.ConfigEntryExtDataTattoosModuleLogLevel,
        [LoggingModules.FemaleSettings] = Ash.ConfigEntryFemaleSettingsModuleLogLevel,
        [LoggingModules.Global] = Ash.ConfigEntryGlobalModuleLogLevel,
        [LoggingModules.HMembersHooks] = Ash.ConfigEntryHMembersHooksModuleLogLevel,
        [LoggingModules.HSceneSettings] = Ash.ConfigEntryHSceneSettingsModuleLogLevel,
        [LoggingModules.HumanHooks] = Ash.ConfigEntryHumanHooksModuleLogLevel,
        [LoggingModules.ImmersiveUI] = Ash.ConfigEntryImmersiveUIModuleLogLevel,
        [LoggingModules.ItemsCoordinator] = Ash.ConfigEntryItemsCoordinatorModuleLogLevel,
        [LoggingModules.ItemsVisibilityControls] = Ash.ConfigEntryItemsVisibilityControlsModuleLogLevel,
        [LoggingModules.MaleSettings] = Ash.ConfigEntryMaleSettingsModuleLogLevel,
        [LoggingModules.SceneManagement] = Ash.ConfigEntrySceneManagementModuleLogLevel,
        [LoggingModules.UISettings] = Ash.ConfigEntryUISettingsModuleLogLevel,
        [LoggingModules.WearablesHooks] = Ash.ConfigEntryWearablesHooksModuleLogLevel,

    };

    internal enum LoggingModules
    {
        Actions,
        AnimationControls,
        ApplicablesMigrations,
        BetterPenetration,
        BugFixes,
        DebuggingTesting,
        ExtDataCheekShadows,
        ExtDataEyeShadows,
        ExtDataMoles,
        ExtDataTattoos,
        FemaleControls,
        FemaleSettings,
        Global,
        HMembersHooks,
        HSceneSettings,
        HumanHooks,
        ImmersiveUI,
        ItemsCoordinator,
        ItemsVisibilityControls,
        MaleSettings,
        SceneManagement,
        UISettings,
        WearablesHooks,
    }

    internal static LogLevel GetLogLevel(LoggingModules module) {
        return LoggingState.GetValueOrDefaultValue(module, null)?.Value ?? LogLevel.None;
    }
}
