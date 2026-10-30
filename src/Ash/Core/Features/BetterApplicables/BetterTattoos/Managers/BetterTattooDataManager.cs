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

namespace Ash.Core.Features.BetterApplicables.BetterTattoos.Managers
{
    internal class BetterTattooDataManager : ApplicableDataManagerBase
    {
        internal override AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ExtDataTattoos);

        protected override string PayloadId => Ash.GUID + "_TattooExtData";

        internal override void RecordExtendedData() {
            var human = MakerAPI.GetMakerBase().human;

            if (MultiTextureData.TryGetValue(GetCustomParamByHuman(human), out var dictHead))
                dictHead[Part.Head] = Ash.BetterTattooMakerExtensionHead.UnsavedChanges;

            if (MultiTextureData.TryGetValue(GetCustomParamByHuman(human), out var dictBody))
                dictBody[Part.Body] = Ash.BetterTattooMakerExtensionBody.UnsavedChanges;

            ApplicablesTextureCaches.UpdateTextureCache(
                BetterCategory.Tattoo,
                human,
                Part.Head,
                TextureUtils.SaveToTexture2D(Ash.BetterTattooMakerExtensionHead.GetCachedRenderTexture(Part.Head))
            );

            ApplicablesTextureCaches.UpdateTextureCache(
                BetterCategory.Tattoo,
                human,
                Part.Body,
                TextureUtils.SaveToTexture2D(Ash.BetterTattooMakerExtensionBody.GetCachedRenderTexture(Part.Head))
            );

            Ash.BetterTattooMakerExtensionHead.DestroyTextureCache();
            Ash.BetterTattooMakerExtensionBody.DestroyTextureCache();
        }

        protected override SerializableTextureData[] GetUnsavedChanges(Part part) {
            return part == Part.Head
                ? Ash.BetterTattooMakerExtensionHead.UnsavedChanges
                : Ash.BetterTattooMakerExtensionBody.UnsavedChanges;
        }

        protected override Dictionary<Part, SerializableTextureData[]> CreateMultiTextureDataDict(CustomParameter param) {
            var combinedTextureDataFace = param.sex == SEX.FEMALE
                ? CustomDataManager.GetFaceTattoo_Female(param.head.tattooID)
                : CustomDataManager.GetFaceTattoo_Male(param.head.tattooID);

            var combinedTextureDataBody = param.sex == SEX.FEMALE
                ? CustomDataManager.GetBodyTattoo_Female(param.body.tattooID)
                : CustomDataManager.GetBodyTattoo_Male(param.body.tattooID);

            var headArray = new SerializableTextureData[NumberOfSlots];
            var bodyArray = new SerializableTextureData[NumberOfSlots];

            if (param.head.tattooID != 0 && combinedTextureDataFace != null)
                headArray[0] = new SerializableTextureData(
                    combinedTextureDataFace.id,
                    combinedTextureDataFace.assetbundleName,
                    combinedTextureDataFace.textureName,
                    param.head.tattooColor,
                    combinedTextureDataFace.pos.x,
                    combinedTextureDataFace.pos.y,
                    Vector2.zero,
                    Vector2.one
                );

            if (param.body.tattooID != 0 && combinedTextureDataBody != null)
                bodyArray[0] = new SerializableTextureData(
                    combinedTextureDataBody.id,
                    combinedTextureDataBody.assetbundleName,
                    combinedTextureDataBody.textureName,
                    param.body.tattooColor,
                    combinedTextureDataBody.pos.x,
                    combinedTextureDataBody.pos.y,
                    Vector2.zero,
                    Vector2.one
                );

            return new Dictionary<Part, SerializableTextureData[]> {
                [Part.Head] = headArray,
                [Part.Body] = bodyArray
            };
        }

        protected override void MigrateExtendedData(CustomParameter file) {
            var migrationData = file.sex == SEX.FEMALE
                ? Migrations.ApplicablesMigration.ApplicablesMigrationDataInstance.FemaleDataTattoos.GetValueOrDefaultValue(GetHeroineIdByCustomParam(file), null)
                : Migrations.ApplicablesMigration.ApplicablesMigrationDataInstance.MaleDataTattoos.GetValueOrDefaultValue(GetMaleIdByCustomParam(file), null);

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
