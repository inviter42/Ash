using System.Collections.Generic;
using BepInEx.Logging;

namespace Ash.Logging
{
    internal static class LoggingSettings
    {
        internal static readonly Dictionary<LoggingModules, LogLevel> LoggingState = new Dictionary<LoggingModules, LogLevel> {
            [LoggingModules.ItemsCoordinator] = LogLevel.Warning | LogLevel.Error | LogLevel.Fatal,
            // [LoggingModules.HumanHooks] = LogLevel.Warning | LogLevel.Error | LogLevel.Fatal,
        };

        internal enum LoggingModules
        {
            Global,
            ItemsCoordinator,
            ExtDataTattoos,
            Actions,
            GameUIControls,
            HSceneControls,
            HSceneSettings,
            ImmersiveUI,
            ItemsVisibilityControls,
            SceneManagement,
            PhExtendedSaveFiles,
            HMembersHooks,
            HumanHooks,
            WearablesHooks
        }
    }
}
