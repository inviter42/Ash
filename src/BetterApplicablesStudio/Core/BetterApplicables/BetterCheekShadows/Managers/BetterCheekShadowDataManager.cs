using System.Collections.Generic;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared;
using Character;
using UnityEngine;

namespace BetterApplicablesStudio.Core.BetterApplicables.BetterCheekShadows.Managers
{
    internal class BetterCheekShadowDataManager : ApplicableDataManagerBase
    {
        protected override string PayloadId => BetterApplicablesStudio.AshGUID + "_CheekShadowExtData";

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
