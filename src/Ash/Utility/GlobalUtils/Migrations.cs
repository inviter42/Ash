using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Logging;
using BepInEx;
using Character;
using HarmonyLib;
using MessagePack;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Ash.Utility.GlobalUtils
{
    public static class Migrations
    {
        private static readonly List<MigrationData> DataMigrationMap = new List<MigrationData> {
            new MigrationData(
                new MigrationVersions("1.1.0", "1.2.0"),
                new List<MigrationFile> {
                    new MigrationFile("Ash.ItemRules.json", new List<TypeMigration> {
                        new TypeMigration(
                            "Ash.Core.Features.ItemsCoordinator.RulesManager+RuleSet",
                            "Ash.Core.Features.ItemsCoordinator.Types.InterItemRuleSet"
                        )
                    })
                }
            ),
            new MigrationData(
                new MigrationVersions("1.3.2", "1.4.0"),
                new List<MigrationFile> {
                    new MigrationFile("Ash.Settings.json", new List<TypeMigration> {
                        new TypeMigration(
                            "Ash.GlobalUtils.IO+PersistenceEnvelope`1[[Ash.Core.Settings.PersistentSettings, Ash]]",
                            "Ash.Utility.GlobalUtils.IO+PersistenceEnvelope`1[[Ash.Core.Features.AshPlugin.Settings.PersistentSettings, Ash]]"
                        )
                    }),
                    new MigrationFile("Ash.InterItemRules.json", new List<TypeMigration> {
                        new TypeMigration(
                            "Ash.GlobalUtils.IO+PersistenceEnvelope`1[[System.Collections.Generic.List`1[[Ash.Core.Features.ItemsCoordinator.Types.InterItemRuleSet, Ash]], mscorlib]]",
                            "Ash.Utility.GlobalUtils.IO+PersistenceEnvelope`1[[System.Collections.Generic.List`1[[Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Types.InterItemRuleSet, Ash]], mscorlib]]"
                        ),
                        new TypeMigration(
                            "Ash.Core.Features.ItemsCoordinator.Types.BaseItem",
                            "Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Types.BaseItem"
                        ),
                        new TypeMigration(
                            "Ash.Core.Features.ItemsCoordinator.Types.DataOfAccessoryItem",
                            "Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Types.DataOfAccessoryItem"
                        ),
                        new TypeMigration(
                            "Ash.Core.Features.ItemsCoordinator.Types.DataOfWearItem",
                            "Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Types.DataOfWearItem"
                        ),
                    }),
                    new MigrationFile("Ash.HPosItemRules.json", new List<TypeMigration> {
                        new TypeMigration(
                            "Ash.GlobalUtils.IO+PersistenceEnvelope`1[[System.Collections.Generic.List`1[[Ash.Core.Features.ItemsCoordinator.Types.HPosRuleSet, Ash]], mscorlib]]",
                            "Ash.Utility.GlobalUtils.IO+PersistenceEnvelope`1[[System.Collections.Generic.List`1[[Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Types.HPosRuleSet, Ash]], mscorlib]]"
                        ),
                        new TypeMigration(
                            "Ash.Core.Features.ItemsCoordinator.Types.BaseItem",
                            "Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Types.BaseItem"
                        ),
                        new TypeMigration(
                            "Ash.Core.Features.ItemsCoordinator.Types.DataOfWearItem",
                            "Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Types.DataOfWearItem"
                        ),
                        new TypeMigration(
                            "Ash.Core.Features.ItemsCoordinator.Types.HStyleDetail",
                            "Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Types.HStyleDetail"
                        )
                    }),
                }
            ),
        };

        //------------------------------ IMPORTANT! ---------------------------//
        // The latest entry here MUST be in sync with `IO.*FileName` property! //
        //---------------------------------------------------------------------//
        private static readonly List<FileNameMigrationData> FilesMigrationMap =
            new List<FileNameMigrationData> {
                new FileNameMigrationData(
                    new MigrationVersions("1.1.0", "1.2.0"),
                    new List<FileNameMigration> {
                        new FileNameMigration(
                            "Ash.ItemRules.json",
                            "Ash.InterItemRules.json"
                        )
                    }
                )
            };

        public class MigrationBinder : DefaultSerializationBinder
        {
            private readonly string VersionFromFile;
            private readonly string FileName;

            public MigrationBinder(string fileName, string versionFromFile) {
                FileName = fileName;
                VersionFromFile = versionFromFile;
            }

            public override Type BindToType(string assemblyName, string typeName) {
                var currentVersion = VersionFromFile;
                MigrationData migrationData;

                do {
                    // find migration entry where 'FromVersion' matches the current version
                    migrationData = DataMigrationMap.FirstOrDefault(e => e.VersionData.FromVersion == currentVersion);

                    if (migrationData?.VersionData.FromVersion == null)
                        continue;

                    // check if entry contains migrations for our file
                    var fileMigration = migrationData.Files.Find(f => f.FileName == FileName);
                    if (fileMigration?.FileName != null) {
                        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                        foreach (var typeMap in fileMigration.TypeMigrations) {
                            if (typeName.Contains(typeMap.FromType)) {
                                typeName = typeName.Replace(typeMap.FromType, typeMap.ToType);
                                break;
                            }
                        }
                    }

                    currentVersion = migrationData.VersionData.ToVersion;
                } while (migrationData != null);

                return base.BindToType(assemblyName, typeName);
            }
        }

        private class MigrationData
        {
            public readonly MigrationVersions VersionData;
            public readonly List<MigrationFile> Files;

            public MigrationData(MigrationVersions versionData, List<MigrationFile> files) {
                VersionData = versionData;
                Files = files;
            }
        }

        private class FileNameMigrationData
        {
            // ReSharper disable once NotAccessedField.Local
            public readonly MigrationVersions VersionData;
            public readonly List<FileNameMigration> FileNameMigrations;

            public FileNameMigrationData(MigrationVersions versionData, List<FileNameMigration> fileNameMigrations) {
                VersionData = versionData;
                FileNameMigrations = fileNameMigrations;
            }
        }

        // ReSharper disable once StructCanBeMadeReadOnly
        private class MigrationVersions : IEquatable<MigrationVersions>
        {
            public readonly string FromVersion;
            public readonly string ToVersion;

            public MigrationVersions(string fromVersion, string toVersion) {
                FromVersion = fromVersion;
                ToVersion = toVersion;
            }

            public bool Equals(MigrationVersions other) {
                return other != null && FromVersion == other.FromVersion && ToVersion == other.ToVersion;
            }

            public override bool Equals(object obj) {
                return obj is MigrationVersions other && Equals(other);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((FromVersion != null ? FromVersion.GetHashCode() : 0) * 397) ^
                           (ToVersion != null ? ToVersion.GetHashCode() : 0);
                }
            }
        }

        private class MigrationFile
        {
            public readonly string FileName;
            public readonly List<TypeMigration> TypeMigrations;

            public MigrationFile(string fileName, List<TypeMigration> typeMigrations) {
                FileName = fileName;
                TypeMigrations = typeMigrations;
            }
        }

        private class TypeMigration
        {
            public readonly string FromType;
            public readonly string ToType;

            public TypeMigration(string fromType, string toType) {
                FromType = fromType;
                ToType = toType;
            }
        }

        private class FileNameMigration
        {
            public readonly string FromFileName;
            public readonly string ToFileName;

            public FileNameMigration(string fromFileName, string toFileName) {
                FromFileName = fromFileName;
                ToFileName = toFileName;
            }
        }

        public static KeyValuePair<string, string> GetVersionSpecificFileData(string latestFileName) {
            var latestPath = Path.Combine(Paths.ConfigPath, latestFileName);
            if (File.Exists(latestPath))
                return new KeyValuePair<string, string>(latestPath, latestFileName);

            var currentFileName = latestFileName;
            for (var i = FilesMigrationMap.Count - 1; i >= 0; i--) {
                var entry = FilesMigrationMap[i];
                var migrationData = entry.FileNameMigrations.Find(e => e.ToFileName == currentFileName);
                if (migrationData == null) {
                    // FileNameMigrationData exists, but it doesn't contain data for this file - continue
                    continue;
                }

                currentFileName = migrationData.FromFileName;

                var path = Path.Combine(Paths.ConfigPath, currentFileName);
                if (File.Exists(path))
                    return new KeyValuePair<string, string>(path, currentFileName);
            }

            return new KeyValuePair<string, string>("", "");
        }


        internal static class ApplicablesMigration
        {
            internal static ApplicablesMigrationData ApplicablesMigrationDataInstance;

            internal static event Action LoadingIsStarted;
            internal static event Action LoadingIsComplete;

            internal static bool MigrationIsRequired;

            private static AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ApplicablesMigrations);

            private const uint AppendixMagicBytes = 0x415348;
            private const uint AppendixEndMagicBytes = 0x454E44;

            private static readonly Dictionary<string, PluginData> PluginDataDictFromSave = new Dictionary<string, PluginData>();


            private static void FillExtDataDictionary(BinaryReader reader) {
                var position = reader.BaseStream.Position;

                if (!ReaderUtils.SeekByteSequence(reader, AppendixMagicBytes)) {
                    Logger.LogWarning("Unable to find appendix magic bytes");
                    return;
                }

                try {
                    var version = reader.ReadUInt32();
                    Logger.LogDebug($"Version: {version}");

                    do {
                        var kv = ReadSection(reader);
                        if (kv.Key == null)
                            break;
                        PluginDataDictFromSave.Add(kv.Key, kv.Value);
                    } while (true);

                    MigrationIsRequired = true;
                }
                catch (Exception e) {
                    Logger.LogError(e);

                    MigrationIsRequired = false;
                }

                reader.BaseStream.Position = position;
            }

            private static ApplicablesMigrationData PrepareDictionaries() {
                Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> femaleMultiTextureDataCheekShadows = null;
                Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> maleMultiTextureDataCheekShadows = null;
                Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> femaleMultiTextureDataEyeShadows = null;
                Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> maleMultiTextureDataEyeShadows = null;
                Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> femaleMultiTextureDataMoles = null;
                Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> maleMultiTextureDataMoles = null;
                Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> femaleMultiTextureDataTattoos = null;
                Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> maleMultiTextureDataTattoos = null;

                foreach (var payloadPair in PluginDataDictFromSave) {
                    switch (payloadPair.Key) {
                        case Ash.GUID + "_CheekShadowExtData":
                        {
                            RetrieveDictionariesByPayloadId(payloadPair.Key, out femaleMultiTextureDataCheekShadows, out maleMultiTextureDataCheekShadows);
                            break;
                        }

                        case Ash.GUID + "_EyeShadowExtData":
                        {
                            RetrieveDictionariesByPayloadId(payloadPair.Key, out femaleMultiTextureDataEyeShadows, out maleMultiTextureDataEyeShadows);
                            break;
                        }

                        case Ash.GUID + "_MoleExtData":
                        {
                            RetrieveDictionariesByPayloadId(payloadPair.Key, out femaleMultiTextureDataMoles, out maleMultiTextureDataMoles);
                            break;
                        }

                        case Ash.GUID + "_TattooExtData":
                        {
                            RetrieveDictionariesByPayloadId(payloadPair.Key, out femaleMultiTextureDataTattoos, out maleMultiTextureDataTattoos);
                            break;
                        }

                        default:
                            throw new Exception("Unknown payload identifier, migration failed.");
                    }
                }

                return new ApplicablesMigrationData(
                    femaleMultiTextureDataCheekShadows,
                    maleMultiTextureDataCheekShadows,
                    femaleMultiTextureDataEyeShadows,
                    maleMultiTextureDataEyeShadows,
                    femaleMultiTextureDataMoles,
                    maleMultiTextureDataMoles,
                    femaleMultiTextureDataTattoos,
                    maleMultiTextureDataTattoos
                );
            }

            private static void RetrieveDictionariesByPayloadId(
                string payloadId,
                out Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> fDict,
                out Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> mDict
            ) {
                fDict = null;
                mDict = null;

                var pluginData = PluginDataDictFromSave.TryGetValue(payloadId, out var data) ? data : null;
                if (pluginData == null) {
                    Logger.LogDebug($"No PluginData is found by this is id {payloadId}");
                    return;
                }

                foreach (var kvp in pluginData.Data) {
                    switch (kvp.Key) {
                        case "FemaleMultiTextureData":
                            fDict = ((JObject)kvp.Value)
                                .ToObject<Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>>>();
                            break;
                        case "MaleMultiTextureData":
                            mDict = ((JObject)kvp.Value)
                                .ToObject<Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>>>();
                            break;
                        default:
                        {
                            Logger.LogWarning($"Unknown data type {kvp.Key}");
                            Logger.LogDebug($"Attempting to deserialize using known aliases for key \"{kvp.Key}\"");
                            var femaleKeyAliases = new[] { "FemaleMultiEyeShadowData", "FemaleMultiTattooData" };
                            var maleKeyAliases = new[] { "MaleMultiEyeShadowData", "MaleMultiTattooData" };
                            if (femaleKeyAliases.Contains(kvp.Key)) {
                                fDict = ((JObject)kvp.Value)
                                    .ToObject<Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>>>();

                                Ash.Logger.LogDebug($"Successfully deserialized female data for key \"{kvp.Key}\"");
                            }

                            if (maleKeyAliases.Contains(kvp.Key)) {
                                mDict = ((JObject)kvp.Value)
                                    .ToObject<Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>>>();

                                Ash.Logger.LogDebug($"Successfully deserialized male data for key \"{kvp.Key}\"");
                            }

                            break;
                        }
                    }
                }
            }

            private static KeyValuePair<string, PluginData> ReadSection(BinaryReader reader) {
                try {
                    var closingBytes = reader.ReadUInt32();
                    if (closingBytes == AppendixEndMagicBytes) {
                        Logger.LogDebug($"Reached closing bytes");
                        return default;
                    }

                    reader.BaseStream.Position -= sizeof(uint);

                    var sectionName = reader.ReadString();
                    var length = reader.ReadInt32();

                    if (sectionName.Length == 0 || length == 0) {
                        Logger.LogDebug($"Reached end of file");
                        return default;
                    }

                    var data = SerializationUtils.Deserialize<PluginData>(reader.ReadBytes(length));

                    Logger.LogDebug($"Section '{sectionName}' deserialized successfully");

                    return new KeyValuePair<string, PluginData>(sectionName, data);
                }
                catch (EndOfStreamException) {
                    Logger.LogDebug($"Last section has been read");
                    return default;
                }
                catch (Exception e) {
                    throw new Exception($"Unexpected exception has occured while trying to read magic bytes: {e}");
                }
            }

            internal class ApplicablesMigrationData
            {
                internal readonly Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> FemaleDataCheekShadows;
                internal readonly Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> MaleDataCheekShadows;

                internal readonly Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> FemaleDataEyeShadows;
                internal readonly Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> MaleDataEyeShadows;

                internal readonly Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> FemaleDataMoles;
                internal readonly Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> MaleDataMoles;

                internal readonly Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> FemaleDataTattoos;
                internal readonly Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> MaleDataTattoos;

                internal ApplicablesMigrationData(
                    Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> femaleDataCheekShadows,
                    Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> maleDataCheekShadows,
                    Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> femaleDataEyeShadows,
                    Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> maleDataEyeShadows,
                    Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> femaleDataMoles,
                    Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> maleDataMoles,
                    Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> femaleDataTattoos,
                    Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> maleDataTattoos
                ) {
                    FemaleDataCheekShadows = femaleDataCheekShadows;
                    MaleDataCheekShadows = maleDataCheekShadows;
                    FemaleDataEyeShadows = femaleDataEyeShadows;
                    MaleDataEyeShadows = maleDataEyeShadows;
                    FemaleDataMoles = femaleDataMoles;
                    MaleDataMoles = maleDataMoles;
                    FemaleDataTattoos = femaleDataTattoos;
                    MaleDataTattoos = maleDataTattoos;
                }
            }

            // ReSharper disable once ClassNeverInstantiated.Local
            private class PluginData
            {
                [Key(0)] public int Version = 0;
                [Key(1)] public Dictionary<string, object> Data = new Dictionary<string, object>();
            }

            [HarmonyPatch]
            internal class ApplicablesMigrationHooks
            {
                [HarmonyPrefix]
                [HarmonyPatch(typeof(GamePlayData), nameof(GamePlayData.Load), typeof(string))]
                // ReSharper disable once InconsistentNaming
                internal static bool GamePlayDataLoadPrefix(GamePlayData __instance, string path) {
                    LoadingIsStarted?.Invoke();

                    PluginDataDictFromSave.Clear();
                    MigrationIsRequired = false;

                    FileStream input;
                    try {
                        input = File.OpenRead(path);
                    }
                    catch {
                        return false;
                    }

                    var reader = new BinaryReader(input);

                    if (!__instance.header.Load(reader)) {
                        input.Close();
                        return false;
                    }

                    Logger.LogDebug($"Path {path}, Comment {__instance.header.comment}");

                    FillExtDataDictionary(reader);

                    input.Close();

                    if (!MigrationIsRequired)
                        return true;

                    Logger.LogDebug("Initiating applicables migration");

                    ApplicablesMigrationDataInstance = PrepareDictionaries();

                    return true;
                }

                [HarmonyPostfix]
                [HarmonyPatch(typeof(GamePlayData), nameof(GamePlayData.Load), typeof(string))]
                // ReSharper disable once InconsistentNaming
                internal static void GamePlayDataLoadPostfix(GamePlayData __instance, string path) {
                    LoadingIsComplete?.Invoke();

                    if (MigrationIsRequired) {
                        Ash.Logger.LogDebug($"Resaving the file {__instance.header.comment}");
                        GlobalData.PlayData.Save(path, __instance.header.comment); // trigger save to migrate data
                    }
                }

                /// Replaces File.OpenWrite with File.Create to ensure file is flushed prior to writing
                [HarmonyTranspiler]
                [HarmonyPatch(typeof(GamePlayData), nameof(GamePlayData.Save), typeof(string), typeof(string))]
                internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
                    var codes = new List<CodeInstruction>(instructions);
                    foreach (var code in codes) {
                        if (code.opcode == OpCodes.Call &&
                            code.operand is MethodInfo method &&
                            method.DeclaringType == typeof(File) &&
                            method.Name == nameof(File.OpenWrite))
                        {
                            code.operand = AccessTools.Method(typeof(File), nameof(File.Create), new[] { typeof(string) });
                            break;
                        }
                    }

                    return codes.AsEnumerable();
                }
            }
        }
    }
}
