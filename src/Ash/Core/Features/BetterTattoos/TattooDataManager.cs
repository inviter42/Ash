using System;
using System.Collections.Generic;
using Ash.Core.Features.BetterTattoos.MakerExtensions;
using Ash.Core.Features.BetterTattoos.Hooks._Female;
using Ash.Core.Features.BetterTattoos.Hooks._Male;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using Character;
using ExtensibleSaveFormat;
using KKAPI.Maker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using PhExtendedSaveFiles;
using PhExtendedSaveFiles.Utils;

namespace Ash.Core.Features.BetterTattoos
{
    internal class TattooDataManager : MonoBehaviour
    {
        internal static readonly AssetBundle MultipleTattoosShadersAssetBundle =
            AssetBundleUtils.LoadBundleFromResource("Ash.Resources.multiple_tattoos_shaders");

        internal static readonly AshLogger Logger = new AshLogger(LoggingSettings.LoggingModules.ExtDataTattoos);

        internal const int NumberOfTattooSlots = 10;

        internal static Dictionary<HEROINE, Dictionary<Part, SerializableTattooData[]>> FemaleMultiTattooData =
            new Dictionary<HEROINE, Dictionary<Part, SerializableTattooData[]>> {
                [HEROINE.RITSUKO] = new Dictionary<Part, SerializableTattooData[]>(),
                [HEROINE.AKIKO] = new Dictionary<Part, SerializableTattooData[]>(),
                [HEROINE.YUKIKO] = new Dictionary<Part, SerializableTattooData[]>(),
                [HEROINE.MARIKO] = new Dictionary<Part, SerializableTattooData[]>(),
            };

        internal static Dictionary<MALE_ID, Dictionary<Part, SerializableTattooData[]>> MaleMultiTattooData =
            new Dictionary<MALE_ID, Dictionary<Part, SerializableTattooData[]>> {
                [MALE_ID.HERO] = new Dictionary<Part, SerializableTattooData[]>(),
                [MALE_ID.KOUICHI] = new Dictionary<Part, SerializableTattooData[]>(),
                [MALE_ID.MOB_A] = new Dictionary<Part, SerializableTattooData[]>(),
                [MALE_ID.MOB_B] = new Dictionary<Part, SerializableTattooData[]>(),
                [MALE_ID.MOB_C] = new Dictionary<Part, SerializableTattooData[]>(),
            };


        private static readonly Dictionary<HEROINE, Dictionary<Part, Texture2D>> FemaleTattooTextureCache =
            new Dictionary<HEROINE, Dictionary<Part, Texture2D>> {
                [HEROINE.RITSUKO] = new Dictionary<Part, Texture2D>(),
                [HEROINE.AKIKO] = new Dictionary<Part, Texture2D>(),
                [HEROINE.YUKIKO] = new Dictionary<Part, Texture2D>(),
                [HEROINE.MARIKO] = new Dictionary<Part, Texture2D>(),
            };

        private static readonly Dictionary<MALE_ID, Dictionary<Part, Texture2D>> MaleTattooTextureCache =
            new Dictionary<MALE_ID, Dictionary<Part, Texture2D>> {
                [MALE_ID.HERO] = new Dictionary<Part, Texture2D>(),
                [MALE_ID.KOUICHI] = new Dictionary<Part, Texture2D>(),
                [MALE_ID.MOB_A] = new Dictionary<Part, Texture2D>(),
                [MALE_ID.MOB_B] = new Dictionary<Part, Texture2D>(),
                [MALE_ID.MOB_C] = new Dictionary<Part, Texture2D>(),
            };


        internal static event Action<Part, SerializableTattooData[]> ExtDataFromTheCard;
        internal static event Action<Part, SerializableTattooData[]> ExtDataFromVanilla;

        private const string PayloadId = Ash.GUID + "_TattooExtData";

        private static bool PluginDataLoaded;


        private void Awake() {
            ExtendedSaveFiles.SaveFileBeingLoaded += OnSaveFileBeingLoaded;
            ExtendedSaveFiles.SaveFileBeingWritten += OnSaveFileBeingWritten;

            ExtendedSave.CardBeingLoaded += OnCardBeingLoaded;
            ExtendedSave.CardBeingSaved += OnCardBeingSaved;

            FemaleHooks.FemaleIsBeingApplied += CreateExtDataFromVanilla;
            MaleHooks.MaleIsBeingApplied += CreateExtDataFromVanilla;
        }

        private void OnDestroy() {
            ExtendedSaveFiles.SaveFileBeingLoaded -= OnSaveFileBeingLoaded;
            ExtendedSaveFiles.SaveFileBeingWritten -= OnSaveFileBeingWritten;
            ExtendedSave.CardBeingLoaded -= OnCardBeingLoaded;
            ExtendedSave.CardBeingSaved -= OnCardBeingSaved;

            FemaleHooks.FemaleIsBeingApplied -= CreateExtDataFromVanilla;
            MaleHooks.MaleIsBeingApplied -= CreateExtDataFromVanilla;
        }


        internal static Texture2D GetCachedTexture2D(Human human, Part part) {
            return human.sex == SEX.FEMALE
                ? FemaleTattooTextureCache[((Female)human).heroineID].GetValueOrDefaultValue(part, null)
                : MaleTattooTextureCache[((Male)human).maleID].GetValueOrDefaultValue(part, null);
        }


        internal static SerializableTattooData[] GetTattooDataList(Human human, Part part) {
            return human.sex == SEX.FEMALE
                ? FemaleMultiTattooData[((Female)human).heroineID].GetValueOrDefaultValue(part, null)
                : MaleMultiTattooData[((Male)human).maleID].GetValueOrDefaultValue(part, null);
        }

        internal static SerializableTattooData[] GetTattooDataListCopy(Human human, Part part) {
            return GetTattooDataList(human, part)?.Clone() as SerializableTattooData[];
        }


        internal static void RecordTattooExtData() {
            var human = MakerAPI.GetMakerBase().human;
            if (human.sex == SEX.FEMALE) {
                FemaleMultiTattooData[((Female)human).heroineID][Part.Head] = Ash.TattooExtensionHead.UnsavedChanges;
                FemaleMultiTattooData[((Female)human).heroineID][Part.Body] = Ash.TattooExtensionBody.UnsavedChanges;
            } else {
                MaleMultiTattooData[((Male)human).maleID][Part.Head] = Ash.TattooExtensionHead.UnsavedChanges;
                MaleMultiTattooData[((Male)human).maleID][Part.Body] = Ash.TattooExtensionBody.UnsavedChanges;
            }

            UpdateTattooTextureCache(
                human,
                Part.Head,
                TextureUtils.SaveToTexture2D(TattooExtensionBase.GetCachedRenderTexture(Part.Head))
            );

            UpdateTattooTextureCache(
                human,
                Part.Body,
                TextureUtils.SaveToTexture2D(TattooExtensionBase.GetCachedRenderTexture(Part.Body))
            );

            TattooExtensionBase.DestroyTattooTextureCache();
        }


        internal static void UpdateTattooTextureCache(Human human, Part part, Texture2D texture) {
            if (texture == null)
                return;

            if (human.sex == SEX.FEMALE)
                FemaleTattooTextureCache[((Female)human).heroineID][part] = texture;
            else
                MaleTattooTextureCache[((Male)human).maleID][part] = texture;
        }


        private static void OnSaveFileBeingLoaded() {
            ResetMultiTattooDictionaries();

            var pluginData = ExtendedSaveFiles.GetDataById(PayloadId);
            if (pluginData == null) {
                Logger.LogDebug("No PluginData is found in this save file");
                InitializeDictionariesWithVanillaData();
                return;
            }

            foreach (var kvp in pluginData.Data) {
                switch (kvp.Key) {
                    case nameof(FemaleMultiTattooData):
                        FemaleMultiTattooData = ((JObject)kvp.Value)
                            .ToObject<Dictionary<HEROINE, Dictionary<Part, SerializableTattooData[]>>>();
                        break;
                    case nameof(MaleMultiTattooData):
                        MaleMultiTattooData = ((JObject)kvp.Value)
                            .ToObject<Dictionary<MALE_ID, Dictionary<Part, SerializableTattooData[]>>>();
                        break;
                    default:
                        Logger.LogWarning($"Unknown data type {kvp.Key}");
                        break;
                }
            }

            Logger.LogDebug($"FemaleMultiTattooData {SerializationUtils.JsonSerializeToString(FemaleMultiTattooData)}");

            Logger.LogDebug("Extended tattoo data has been loaded");
        }

        private static void OnSaveFileBeingWritten() {
            var data = new ExtendedSaveFiles.PluginData {
                Version = 0,
                Data = new Dictionary<string, object> {
                    [nameof(FemaleMultiTattooData)] = FemaleMultiTattooData,
                    [nameof(MaleMultiTattooData)] = MaleMultiTattooData
                }
            };

            ExtendedSaveFiles.SetPayload(PayloadId, data);
        }


        private static void OnCardBeingLoaded(CustomParameter file) {
            if (!MakerAPI.InsideAndLoaded)
                return;

            var pluginData = ExtendedSave.GetExtendedDataById(file, PayloadId);
            if (pluginData == null) {
                PluginDataLoaded = false;
                Logger.LogDebug("No PluginData is found in this card");
                return;
            }

            foreach (var kvp in pluginData.data) {
                var value = SerializationUtils.JsonDeserializeFromString<SerializableTattooData[]>((string)kvp.Value);
                ExtDataFromTheCard?.Invoke((Part)Enum.Parse(typeof(Part), kvp.Key), value);
            }

            PluginDataLoaded = true;

            Logger.LogDebug("Extended tattoo data has been loaded");
        }

        private static void OnCardBeingSaved(CustomParameter file) {
            if (!MakerAPI.InsideAndLoaded)
                return;

            var data = new PluginData {
                version = 0,
                data = new Dictionary<string, object> {
                    [nameof(Part.Head)] = SerializationUtils.JsonSerializeToString(Ash.TattooExtensionHead.UnsavedChanges),
                    [nameof(Part.Body)] = SerializationUtils.JsonSerializeToString(Ash.TattooExtensionBody.UnsavedChanges)
                }
            };

            ExtendedSave.SetExtendedDataById(file, PayloadId, data);
        }


        private static void CreateExtDataFromVanilla(Human human) {
            if (!MakerAPI.InsideAndLoaded || PluginDataLoaded)
                return;

            var convertedVanillaData = CreateTattooDataDict(human.customParam);
            ExtDataFromVanilla?.Invoke(Part.Body, convertedVanillaData?.GetValueOrDefaultValue(Part.Body, null));
            ExtDataFromVanilla?.Invoke(Part.Head, convertedVanillaData?.GetValueOrDefaultValue(Part.Head, null));
        }


        private static void ResetMultiTattooDictionaries() {
            Logger.LogDebug("Resetting MultiTattooData dictionaries");
            FemaleMultiTattooData =
                new Dictionary<HEROINE, Dictionary<Part, SerializableTattooData[]>> {
                    [HEROINE.RITSUKO] = new Dictionary<Part, SerializableTattooData[]>(),
                    [HEROINE.AKIKO] = new Dictionary<Part, SerializableTattooData[]>(),
                    [HEROINE.YUKIKO] = new Dictionary<Part, SerializableTattooData[]>(),
                    [HEROINE.MARIKO] = new Dictionary<Part, SerializableTattooData[]>(),
                };

            MaleMultiTattooData =
                new Dictionary<MALE_ID, Dictionary<Part, SerializableTattooData[]>> {
                    [MALE_ID.HERO] = new Dictionary<Part, SerializableTattooData[]>(),
                    [MALE_ID.KOUICHI] = new Dictionary<Part, SerializableTattooData[]>(),
                    [MALE_ID.MOB_A] = new Dictionary<Part, SerializableTattooData[]>(),
                    [MALE_ID.MOB_B] = new Dictionary<Part, SerializableTattooData[]>(),
                    [MALE_ID.MOB_C] = new Dictionary<Part, SerializableTattooData[]>(),
                };
        }

        private static void InitializeDictionariesWithVanillaData() {
            Logger.LogDebug("Initializing dictionaries with vanilla data");

            foreach (var kvp in FemaleMultiTattooData)
                InitFemaleTattooData(GetCustomParamByHeroineId(kvp.Key), kvp.Key);

            foreach (var kvp in MaleMultiTattooData)
                InitMaleTattooData(GetCustomParamByMaleId(kvp.Key), kvp.Key);

            return;

            void InitFemaleTattooData(CustomParameter param, HEROINE heroineId) {
                var data = CreateTattooDataDict(param);
                if (data == null) return;

                FemaleMultiTattooData[heroineId][Part.Head] = data[Part.Head];
                FemaleMultiTattooData[heroineId][Part.Body] = data[Part.Body];
            }

            void InitMaleTattooData(CustomParameter param, MALE_ID maleId) {
                var data = CreateTattooDataDict(param);
                if (data == null) return;

                MaleMultiTattooData[maleId][Part.Head] = data[Part.Head];
                MaleMultiTattooData[maleId][Part.Body] = data[Part.Body];
            }
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


        private static CustomParameter GetCustomParamByHeroineId(HEROINE heroineId) {
            if (MakerAPI.InsideAndLoaded)
                return MakerAPI.GetMakerBase().human.customParam;

            switch (heroineId) {
                case HEROINE.RITSUKO:
                    return GlobalData.PlayData.custom_ritsuko;

                case HEROINE.AKIKO:
                    return GlobalData.PlayData.custom_akiko;

                case HEROINE.YUKIKO:
                    return GlobalData.PlayData.custom_yukiko;

                case HEROINE.MARIKO:
                    return GlobalData.PlayData.custom_mariko;

                default:
                    return null;
            }
        }

        private static CustomParameter GetCustomParamByMaleId(MALE_ID maleId) {
            if (MakerAPI.InsideAndLoaded)
                return MakerAPI.GetMakerBase().human.customParam;

            switch (maleId) {
                case MALE_ID.HERO:
                    return GlobalData.PlayData.custom_hero;

                case MALE_ID.KOUICHI:
                    return GlobalData.PlayData.custom_kouichi;

                case MALE_ID.MOB_A:
                    return GlobalData.PlayData.custom_h_maleMobA;

                case MALE_ID.MOB_B:
                    return GlobalData.PlayData.custom_h_maleMobB;

                case MALE_ID.MOB_C:
                    return GlobalData.PlayData.custom_h_maleMobC;

                default:
                    return null;
            }
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
