using System.Collections.Generic;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared;
using Character;
using UnityEngine;
using SerializableTextureData = BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared.SerializableTextureData;

namespace BetterApplicablesStudio.Core.BetterApplicables.BetterTattoos.Managers
{
    internal class BetterTattooDataManager : ApplicableDataManagerBase
    {
        protected override string PayloadId => BetterApplicablesStudio.AshGUID + "_TattooExtData";

        protected override Dictionary<Part, SerializableTextureData[]> CreateMultiTextureDataDict(CustomParameter param) {
            var combinedTextureDataFace = param.sex == SEX.FEMALE
                ? CustomDataManager.GetFaceTattoo_Female(param.head.tattooID)
                : CustomDataManager.GetFaceTattoo_Male(param.head.tattooID);

            var combinedTextureDataBody = param.sex == SEX.FEMALE
                ? CustomDataManager.GetBodyTattoo_Female(param.body.tattooID)
                : CustomDataManager.GetBodyTattoo_Male(param.body.tattooID);

            var headArray = new SerializableTextureData[NumberOfSlots];
            var bodyArray = new SerializableTextureData[NumberOfSlots];

            if (param.head.tattooID != 0)
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

            if (param.body.tattooID != 0)
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
    }
}
