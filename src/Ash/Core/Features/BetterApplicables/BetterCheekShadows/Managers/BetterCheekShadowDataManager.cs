using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedComponents;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using Character;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.BetterCheekShadows.Managers
{
    internal class BetterCheekShadowDataManager : ApplicableDataManagerBase
    {
        internal override AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ExtDataCheekShadows);

        protected override string PayloadId => Ash.GUID + "_CheekShadowExtData";

        internal override void RecordExtendedData() {
            var human = MakerAPI.GetMakerBase().human;

            if (MultiTextureData.TryGetValue(GetCustomParamByHuman(human), out var dict))
                dict[Part.Head] = Ash.BetterCheekShadowMakerExtensionHead.UnsavedChanges;

            ApplicablesTextureCaches.UpdateTextureCache(
                BetterCategory.CheekShadow,
                human,
                Part.Head,
                TextureUtils.SaveToTexture2D(Ash.BetterCheekShadowMakerExtensionHead.GetCachedRenderTexture(Part.Head))
            );

            Ash.BetterCheekShadowMakerExtensionHead.DestroyTextureCache();
        }

        protected override SerializableTextureData[] GetUnsavedChanges(Part part) {
            return Ash.BetterCheekShadowMakerExtensionHead.UnsavedChanges;
        }

        protected override Dictionary<Part, SerializableTextureData[]> CreateMultiTextureDataDict(CustomParameter param) {
            var combinedTextureDataFace = param.sex == SEX.FEMALE
                // ReSharper disable once ConditionalTernaryEqualBranch
                ? CustomDataManager.GetCheek(param.head.cheekTexID)
                : CustomDataManager.GetCheek(param.head.cheekTexID); // if we want to add cheek-shadows to males, it needs a custom getter!

            var headArray = new SerializableTextureData[NumberOfSlots];

            if (param.head.cheekTexID != 0 && combinedTextureDataFace != null)
                headArray[0] = new SerializableTextureData(
                    combinedTextureDataFace.id,
                    combinedTextureDataFace.assetbundleName,
                    combinedTextureDataFace.textureName,
                    param.head.cheekColor,
                    combinedTextureDataFace.pos.x,
                    combinedTextureDataFace.pos.y,
                    Vector2.zero,
                    Vector2.one
                );

            return new Dictionary<Part, SerializableTextureData[]> {
                [Part.Head] = headArray
            };
        }

        protected override void MigrateExtendedData(CustomParameter file) {
            var migrationData = file.sex == SEX.FEMALE
                ? Migrations.ApplicablesMigration.ApplicablesMigrationDataInstance.FemaleDataCheekShadows?.GetValueOrDefaultValue(GetHeroineIdByCustomParam(file), null)
                : Migrations.ApplicablesMigration.ApplicablesMigrationDataInstance.MaleDataCheekShadows?.GetValueOrDefaultValue(GetMaleIdByCustomParam(file), null);

            if (migrationData == null || migrationData.All(kvp => kvp.Value == null || kvp.Value.All(e => e == null))) {
                PluginDataLoaded = false;
                Logger.LogDebug("No PluginData is found in this card");
                InitializeDictionaryWithVanillaData(file);
                return;
            }



            MultiTextureData[file] = migrationData;

            Logger.LogDebug($"Extended texture data has been loaded");
        }
    }
}
