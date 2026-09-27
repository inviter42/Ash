using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedHooks._Female;
using Ash.Core.Features.BetterApplicables.SharedHooks._Male;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using Character;
using ExtensibleSaveFormat;
using KKAPI.Maker;
using Newtonsoft.Json.Linq;
using PhExtendedSaveFiles;
using PhExtendedSaveFiles.Utils;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.BaseComponents.Managers
{
    internal class ApplicableDataManagerBase : MonoBehaviour
    {
        internal virtual AshLogger Logger { get; }

        internal virtual int NumberOfSlots => 10;

        protected Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> FemaleMultiTextureData =
            new Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> {
                [HEROINE.RITSUKO] = new Dictionary<Part, SerializableTextureData[]>(),
                [HEROINE.AKIKO] = new Dictionary<Part, SerializableTextureData[]>(),
                [HEROINE.YUKIKO] = new Dictionary<Part, SerializableTextureData[]>(),
                [HEROINE.MARIKO] = new Dictionary<Part, SerializableTextureData[]>(),
            };

        protected Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> MaleMultiTextureData =
            new Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> {
                [MALE_ID.HERO] = new Dictionary<Part, SerializableTextureData[]>(),
                [MALE_ID.KOUICHI] = new Dictionary<Part, SerializableTextureData[]>(),
                [MALE_ID.MOB_A] = new Dictionary<Part, SerializableTextureData[]>(),
                [MALE_ID.MOB_B] = new Dictionary<Part, SerializableTextureData[]>(),
                [MALE_ID.MOB_C] = new Dictionary<Part, SerializableTextureData[]>(),
            };


        internal event Action<Part, SerializableTextureData[]> ExtDataFromTheCard;
        internal event Action<Part, SerializableTextureData[]> ExtDataFromVanilla;

        protected virtual string PayloadId { get; }

        private bool PluginDataLoaded;


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


        internal SerializableTextureData[] GetSerializableTextureDataList(Human human, Part part) {
            return human.sex == SEX.FEMALE
                ? FemaleMultiTextureData[((Female)human).heroineID].GetValueOrDefaultValue(part, null)
                : MaleMultiTextureData[((Male)human).maleID].GetValueOrDefaultValue(part, null);
        }

        internal SerializableTextureData[] GetSerializableTextureDataListCopy(Human human, Part part) {
            return GetSerializableTextureDataList(human, part)?.Clone() as SerializableTextureData[];
        }


        internal virtual void RecordExtendedData() {
            throw new NotImplementedException();
        }

        protected virtual SerializableTextureData[] GetUnsavedChanges(Part part) {
            throw new NotImplementedException();
        }

        protected virtual Dictionary<Part, SerializableTextureData[]> CreateMultiTextureDataDict(CustomParameter param) {
            throw new NotImplementedException();
        }


        private void OnSaveFileBeingLoaded() {
            ResetDictionaries();

            var pluginData = ExtendedSaveFiles.GetDataById(PayloadId);
            if (pluginData == null) {
                Logger.LogDebug("No PluginData is found in this save file");
                InitializeDictionariesWithVanillaData();
                return;
            }

            foreach (var kvp in pluginData.Data) {
                Ash.Logger.LogDebug($"PluginData key - {kvp.Key}");
                switch (kvp.Key) {
                    case nameof(FemaleMultiTextureData):
                        FemaleMultiTextureData = ((JObject)kvp.Value)
                            .ToObject<Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>>>();
                        break;
                    case nameof(MaleMultiTextureData):
                        MaleMultiTextureData = ((JObject)kvp.Value)
                            .ToObject<Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>>>();
                        break;
                    default:
                        Logger.LogWarning($"Unknown data type {kvp.Key}");
                        Logger.LogDebug($"Attempting to deserialize using known aliases for key \"{kvp.Key}\"");
                        var femaleKeyAliases = new[] { "FemaleMultiEyeShadowData", "FemaleMultiTattooData" };
                        var maleKeyAliases = new[] { "MaleMultiEyeShadowData", "MaleMultiTattooData" };
                        if (femaleKeyAliases.Contains(kvp.Key)) {
                            FemaleMultiTextureData = ((JObject)kvp.Value)
                                .ToObject<Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>>>();

                            Ash.Logger.LogDebug($"Successfully deserialized female data for key \"{kvp.Key}\"");
                        }

                        if (maleKeyAliases.Contains(kvp.Key)) {
                            MaleMultiTextureData = ((JObject)kvp.Value)
                                .ToObject<Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>>>();

                            Ash.Logger.LogDebug($"Successfully deserialized female data for key \"{kvp.Key}\"");
                        }
                        break;
                }
            }

            Logger.LogDebug("////////////////////////// FemaleMultiTextureData /////////////////////////////");
            Logger.LogDebug($"FemaleMultiTextureData {SerializationUtils.JsonSerializeToString(FemaleMultiTextureData)}\n\n");

            Logger.LogDebug("/////////////////////////// MaleMultiTextureData //////////////////////////////");
            Logger.LogDebug($"MaleMultiTextureData {SerializationUtils.JsonSerializeToString(MaleMultiTextureData)}\n\n");

            Logger.LogDebug("Extended data has been loaded");
        }

        private void OnSaveFileBeingWritten() {
            var data = new ExtendedSaveFiles.PluginData {
                Version = 0,
                Data = new Dictionary<string, object> {
                    [nameof(FemaleMultiTextureData)] = FemaleMultiTextureData,
                    [nameof(MaleMultiTextureData)] = MaleMultiTextureData
                }
            };

            ExtendedSaveFiles.SetPayload(PayloadId, data);
        }


        private void OnCardBeingLoaded(CustomParameter file) {
            if (!MakerAPI.InsideMaker)
                return;

            var pluginData = ExtendedSave.GetExtendedDataById(file, PayloadId);
            if (pluginData == null) {
                PluginDataLoaded = false;
                Logger.LogDebug("No PluginData is found in this card");
                return;
            }

            foreach (var kvp in pluginData.data) {
                var value = SerializationUtils.JsonDeserializeFromString<SerializableTextureData[]>((string)kvp.Value);
                ExtDataFromTheCard?.Invoke((Part)Enum.Parse(typeof(Part), kvp.Key), value);
            }

            PluginDataLoaded = true;

            Logger.LogDebug($"Extended texture data has been loaded");
        }

        private void OnCardBeingSaved(CustomParameter file) {
            if (!MakerAPI.InsideMaker)
                return;

            var dict = new Dictionary<string, object>();
            var headData = GetUnsavedChanges(Part.Head);
            var bodyData = GetUnsavedChanges(Part.Body);

            if (headData != null)
                dict.Add(nameof(Part.Head), SerializationUtils.JsonSerializeToString(headData));

            if (bodyData != null)
                dict.Add(nameof(Part.Body), SerializationUtils.JsonSerializeToString(bodyData));

            var data = new PluginData {
                version = 0,
                data = dict
            };

            ExtendedSave.SetExtendedDataById(file, PayloadId, data);
        }


        private void CreateExtDataFromVanilla(Human human) {
            if (!MakerAPI.InsideMaker || PluginDataLoaded)
                return;

            var convertedVanillaData = CreateMultiTextureDataDict(human.customParam);
            ExtDataFromVanilla?.Invoke(Part.Head, convertedVanillaData?.GetValueOrDefaultValue(Part.Head, null));
        }


        private void ResetDictionaries() {
            Logger.LogDebug($"Resetting MultiTextureData dictionaries");
            FemaleMultiTextureData =
                new Dictionary<HEROINE, Dictionary<Part, SerializableTextureData[]>> {
                    [HEROINE.RITSUKO] = new Dictionary<Part, SerializableTextureData[]>(),
                    [HEROINE.AKIKO] = new Dictionary<Part, SerializableTextureData[]>(),
                    [HEROINE.YUKIKO] = new Dictionary<Part, SerializableTextureData[]>(),
                    [HEROINE.MARIKO] = new Dictionary<Part, SerializableTextureData[]>(),
                };

            MaleMultiTextureData =
                new Dictionary<MALE_ID, Dictionary<Part, SerializableTextureData[]>> {
                    [MALE_ID.HERO] = new Dictionary<Part, SerializableTextureData[]>(),
                    [MALE_ID.KOUICHI] = new Dictionary<Part, SerializableTextureData[]>(),
                    [MALE_ID.MOB_A] = new Dictionary<Part, SerializableTextureData[]>(),
                    [MALE_ID.MOB_B] = new Dictionary<Part, SerializableTextureData[]>(),
                    [MALE_ID.MOB_C] = new Dictionary<Part, SerializableTextureData[]>(),
                };
        }

        private void InitializeDictionariesWithVanillaData() {
            Logger.LogDebug("Initializing dictionaries with vanilla data");

            foreach (var kvp in FemaleMultiTextureData)
                InitFemaleEyeShadowData(GetCustomParamByHeroineId(kvp.Key), kvp.Key);

            foreach (var kvp in MaleMultiTextureData)
                InitMaleEyeShadowData(GetCustomParamByMaleId(kvp.Key), kvp.Key);

            return;

            void InitFemaleEyeShadowData(CustomParameter param, HEROINE heroineId) {
                var data = CreateMultiTextureDataDict(param);
                if (data == null) return;

                FemaleMultiTextureData[heroineId][Part.Head] = data[Part.Head];
            }

            void InitMaleEyeShadowData(CustomParameter param, MALE_ID maleId) {
                var data = CreateMultiTextureDataDict(param);
                if (data == null) return;

                MaleMultiTextureData[maleId][Part.Head] = data[Part.Head];
            }
        }


        private static CustomParameter GetCustomParamByHeroineId(HEROINE heroineId) {
            if (MakerAPI.InsideMaker)
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
            if (MakerAPI.InsideMaker)
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
    }
}
