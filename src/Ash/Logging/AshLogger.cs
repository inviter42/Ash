using Ash.Utility.GlobalUtils;
using BepInEx;
using BepInEx.Logging;
using IllusionUtility.GetUtility;

namespace Ash.Logging
{
    internal class AshLogger
    {
        private static readonly ManualLogSource LogSource =
            Logger.CreateLogSource(MetadataHelper.GetMetadata(Ash.Instance).Name);

        private readonly LoggingSettings.LoggingModules LoggingModule;

        internal AshLogger(LoggingSettings.LoggingModules loggingModule) {
            LoggingModule = loggingModule;
        }


        internal void Log(LogLevel level, object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(LoggingModule, LogLevel.All).HasFlag(level))
                return;

            LogSource.Log(level, $"[{LoggingModule}] {data}");
        }

        internal void LogFatal(object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(LoggingModule, LogLevel.All)
                    .HasFlag(LogLevel.Fatal))
                return;

            LogSource.Log(LogLevel.Fatal, $"[{LoggingModule}] {data}");
        }

        internal void LogError(object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(LoggingModule, LogLevel.All)
                    .HasFlag(LogLevel.Error))
                return;

            LogSource.Log(LogLevel.Error, $"[{LoggingModule}] {data}");
        }

        internal void LogWarning(object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(LoggingModule, LogLevel.All)
                    .HasFlag(LogLevel.Warning))
                return;

            LogSource.Log(LogLevel.Warning, $"[{LoggingModule}] {data}");
        }

        internal void LogMessage(object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(LoggingModule, LogLevel.All)
                    .HasFlag(LogLevel.Message))
                return;

            LogSource.Log(LogLevel.Message, $"[{LoggingModule}] {data}");
        }

        internal void LogInfo(object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(LoggingModule, LogLevel.All)
                    .HasFlag(LogLevel.Info))
                return;

            LogSource.Log(LogLevel.Info, $"[{LoggingModule}] {data}");
        }

        internal void LogDebug(object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(LoggingModule, LogLevel.All)
                .HasFlag(LogLevel.Debug))
                return;

            LogSource.Log(LogLevel.Debug, $"[{LoggingModule}] {data}");
        }


        internal static void Log(LoggingSettings.LoggingModules module, LogLevel level, object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(module, LogLevel.All).HasFlag(level))
                return;

            LogSource.Log(level, $"[{module}] {data}");
        }

        internal static void LogFatal(LoggingSettings.LoggingModules module, object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(module, LogLevel.All).HasFlag(LogLevel.Fatal))
                return;

            LogSource.Log(LogLevel.Fatal, $"[{module}] {data}");
        }

        internal static void LogError(LoggingSettings.LoggingModules module, object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(module, LogLevel.All).HasFlag(LogLevel.Error))
                return;

            LogSource.Log(LogLevel.Error, $"[{module}] {data}");
        }

        internal static void LogWarning(LoggingSettings.LoggingModules module, object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(module, LogLevel.All).HasFlag(LogLevel.Warning))
                return;

            LogSource.Log(LogLevel.Warning, $"[{module}] {data}");
        }

        internal static void LogMessage(LoggingSettings.LoggingModules module, object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(module, LogLevel.All).HasFlag(LogLevel.Message))
                return;

            LogSource.Log(LogLevel.Message, $"[{module}] {data}");
        }

        internal static void LogInfo(LoggingSettings.LoggingModules module, object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(module, LogLevel.All).HasFlag(LogLevel.Info))
                return;

            LogSource.Log(LogLevel.Info, $"[{module}] {data}");
        }

        internal static void LogDebug(LoggingSettings.LoggingModules module, object data) {
            if (!LoggingSettings.LoggingState.GetValueOrDefaultValue(module, LogLevel.All).HasFlag(LogLevel.Debug))
                return;

            LogSource.Log(LogLevel.Debug, $"[{module}] {data}");
        }
    }
}
