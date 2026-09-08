using System;
using System.Collections.Generic;
using BetterTattoosStudio.GlobalUtils;
using BetterTattoosStudio.Hooks._Female;
using BetterTattoosStudio.Hooks._Male;
using Character;
using ExtensibleSaveFormat;
using Newtonsoft.Json;
using PhExtendedSaveFiles.Utils;
using UnityEngine;

namespace BetterTattoosStudio.Core.ExtDataManagement
{
    internal class TattooDataManager : MonoBehaviour
    {
        internal static readonly AssetBundle MultipleTattoosShadersAssetBundle =
            AssetBundleUtils.LoadBundleFromResource("BetterTattoosStudio.Resources.multiple_tattoos_shaders");

        internal static readonly Dictionary<Part, SerializableTattooData[]> TattooDataDict =
            new Dictionary<Part, SerializableTattooData[]>();

        private const string PayloadId = Ash.Ash.GUID + "_TattooExtData";

        private static bool PluginDataLoaded;

        private const int NumberOfTattooSlots = 10;


        private void Awake() {
            ExtendedSave.CardBeingLoaded += OnCardBeingLoaded;

            FemaleHooks.FemaleIsBeingApplied += CreateExtDataFromVanilla;
            MaleHooks.MaleIsBeingApplied += CreateExtDataFromVanilla;
        }

        private void OnDestroy() {
            ExtendedSave.CardBeingLoaded -= OnCardBeingLoaded;

            FemaleHooks.FemaleIsBeingApplied -= CreateExtDataFromVanilla;
            MaleHooks.MaleIsBeingApplied -= CreateExtDataFromVanilla;
        }


        private static void OnCardBeingLoaded(CustomParameter file) {
            var pluginData = ExtendedSave.GetExtendedDataById(file, PayloadId);
            if (pluginData == null) {
                PluginDataLoaded = false;
                BetterTattoosStudio.Logger.LogDebug("No PluginData is found in this card");
                return;
            }

            foreach (var kvp in pluginData.data) {
                var value = SerializationUtils.JsonDeserializeFromString<SerializableTattooData[]>((string)kvp.Value);
                TattooDataDict[(Part)Enum.Parse(typeof(Part), kvp.Key)] = value;
            }

            PluginDataLoaded = true;

            BetterTattoosStudio.Logger.LogDebug("Extended tattoo data has been loaded");
        }

        private static void CreateExtDataFromVanilla(Human human) {
            if (PluginDataLoaded)
                return;

            var vanillaData = CreateTattooDataDict(human.customParam);
            if (vanillaData.TryGetValue(Part.Head, out var headData))
                TattooDataDict[Part.Head] = headData;

            if (vanillaData.TryGetValue(Part.Body, out var bodyData))
                TattooDataDict[Part.Body] = bodyData;
        }

        private static Dictionary<Part, SerializableTattooData[]> CreateTattooDataDict(CustomParameter param) {
            var combinedTextureDataFace = param.sex == SEX.FEMALE
                ? CustomDataManager.GetFaceTattoo_Female(param.head.tattooID)
                : CustomDataManager.GetFaceTattoo_Male(param.head.tattooID);

            var combinedTextureDataBody = param.sex == SEX.FEMALE
                ? CustomDataManager.GetBodyTattoo_Female(param.body.tattooID)
                : CustomDataManager.GetBodyTattoo_Male(param.body.tattooID);

            var headArray = new SerializableTattooData[NumberOfTattooSlots];
            var bodyArray = new SerializableTattooData[NumberOfTattooSlots];

            if (param.head.tattooID != 0)
                headArray[0] = new SerializableTattooData(
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
                bodyArray[0] = new SerializableTattooData(
                    combinedTextureDataBody.id,
                    combinedTextureDataBody.assetbundleName,
                    combinedTextureDataBody.textureName,
                    param.body.tattooColor,
                    combinedTextureDataBody.pos.x,
                    combinedTextureDataBody.pos.y,
                    Vector2.zero,
                    Vector2.one
                );

            return new Dictionary<Part, SerializableTattooData[]> {
                [Part.Head] = headArray,
                [Part.Body] = bodyArray
            };
        }


        internal class SerializableTattooData
        {
            [JsonIgnore]
            internal Vector2 AbOffset {
                get => new Vector2(AbOffsetX, AbOffsetY);
                set {
                    AbOffsetX = value.x;
                    AbOffsetY = value.y;
                }
            }

            [JsonIgnore]
            internal Vector2 UserOffset {
                get => new Vector2(UserOffsetX, UserOffsetY);
                set {
                    UserOffsetX = value.x;
                    UserOffsetY = value.y;
                }
            }

            [JsonIgnore]
            internal Vector2 UserScale {
                get => new Vector2(UserScaleX, UserScaleY);
                set {
                    UserScaleX = value.x;
                    UserScaleY = value.y;
                }
            }

            [JsonIgnore]
            internal Color TattooColor {
                get {
                    var c = new Color {
                        a = TattooColorA,
                        r = TattooColorR,
                        g = TattooColorG,
                        b = TattooColorB
                    };
                    return c;
                }
                set {
                    TattooColorA = value.a;
                    TattooColorR = value.r;
                    TattooColorG = value.g;
                    TattooColorB = value.b;
                }
            }

            [JsonProperty] internal int Id;
            [JsonProperty] internal string AssetBundleName;
            [JsonProperty] internal string TextureName;

            [JsonProperty] private float TattooColorA;
            [JsonProperty] private float TattooColorR;
            [JsonProperty] private float TattooColorG;
            [JsonProperty] private float TattooColorB;

            [JsonProperty] private float AbOffsetX;
            [JsonProperty] private float AbOffsetY;

            [JsonProperty] private float UserOffsetX;
            [JsonProperty] private float UserOffsetY;
            [JsonProperty] private float UserScaleX;
            [JsonProperty] private float UserScaleY;

            [JsonConstructor]
            internal SerializableTattooData(
                int id,
                string assetBundleName,
                string textureName,
                Color tattooColor,
                float abOffsetX,
                float abOffsetY,
                Vector2 offset,
                Vector2 scale
            ) {
                Id = id;
                AssetBundleName = assetBundleName;
                TextureName = textureName;
                TattooColor = tattooColor;

                AbOffsetX = abOffsetX;
                AbOffsetY = abOffsetY;

                UserOffsetX = offset.x;
                UserOffsetY = offset.y;

                UserScaleX = scale.x;
                UserScaleY = scale.y;
            }
        }

        internal enum Part
        {
            Head,
            Body,
        }
    }
}
