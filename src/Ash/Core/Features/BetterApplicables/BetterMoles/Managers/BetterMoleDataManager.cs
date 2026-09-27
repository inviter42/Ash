using System.Collections.Generic;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedComponents;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using Character;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.BetterMoles.Managers
{
    internal class BetterMoleDataManager : ApplicableDataManagerBase
    {
        internal override AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ExtDataMoles);

        protected override string PayloadId => Ash.GUID + "_MoleExtData";

        internal override void RecordExtendedData() {
            var human = MakerAPI.GetMakerBase().human;
            if (human.sex == SEX.FEMALE) {
                FemaleMultiTextureData[((Female)human).heroineID][Part.Head] = Ash.BetterMoleMakerExtensionHead.UnsavedChanges;
            } else {
                MaleMultiTextureData[((Male)human).maleID][Part.Head] = Ash.BetterMoleMakerExtensionHead.UnsavedChanges;
            }

            ApplicablesTextureCaches.UpdateTextureCache(
                BetterCategory.Mole,
                human,
                Part.Head,
                TextureUtils.SaveToTexture2D(Ash.BetterMoleMakerExtensionHead.GetCachedRenderTexture(Part.Head))
            );

            Ash.BetterMoleMakerExtensionHead.DestroyTextureCache();
        }

        protected override SerializableTextureData[] GetUnsavedChanges(Part part) {
            return Ash.BetterMoleMakerExtensionHead.UnsavedChanges;
        }

        protected override Dictionary<Part, SerializableTextureData[]> CreateMultiTextureDataDict(CustomParameter param) {
            var combinedTextureDataFace = param.sex == SEX.FEMALE
                // ReSharper disable once ConditionalTernaryEqualBranch
                ? CustomDataManager.GetMole(param.head.moleTexID)
                : CustomDataManager.GetMole(param.head.moleTexID); // if we want to add moles to males, it needs a custom getter!

            var headArray = new SerializableTextureData[NumberOfSlots];

            if (param.head.moleTexID != 0)
                headArray[0] = new SerializableTextureData(
                    combinedTextureDataFace.id,
                    combinedTextureDataFace.assetbundleName,
                    combinedTextureDataFace.textureName,
                    param.head.moleColor,
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
