using System.Collections.Generic;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared;
using Character;
using UnityEngine;
using SerializableTextureData = BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared.SerializableTextureData;

namespace BetterApplicablesStudio.Core.BetterApplicables.BetterMoles.Managers
{
    internal class BetterMoleDataManager : ApplicableDataManagerBase
    {
        protected override string PayloadId => BetterApplicablesStudio.AshGUID + "_MoleExtData";

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
