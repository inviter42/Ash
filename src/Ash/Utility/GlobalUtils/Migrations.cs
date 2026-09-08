using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
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
    }
}
