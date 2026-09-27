using System.Collections.Generic;
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
            if (human.sex == SEX.FEMALE) {
                FemaleMultiTextureData[((Female)human).heroineID][Part.Head] = Ash.BetterCheekShadowMakerExtensionHead.UnsavedChanges;
            } else {
                MaleMultiTextureData[((Male)human).maleID][Part.Head] = Ash.BetterCheekShadowMakerExtensionHead.UnsavedChanges;
            }

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

            if (param.head.cheekTexID != 0)
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
    }
}
