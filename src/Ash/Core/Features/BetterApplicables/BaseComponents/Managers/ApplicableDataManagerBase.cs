using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedHooks._Female;
using Ash.Core.Features.BetterApplicables.SharedHooks._Male;
using Ash.Core.Tooling.SaveLoadManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using Character;
using ExtensibleSaveFormat;
using KKAPI.Maker;
using UnityEngine;
using SerializationUtils = Ash.Utility.GlobalUtils.SerializationUtils;

namespace Ash.Core.Features.BetterApplicables.BaseComponents.Managers
{
    internal class ApplicableDataManagerBase : MonoBehaviour
    {
        internal virtual AshLogger Logger { get; }

        internal virtual int NumberOfSlots => 10;

        protected readonly Dictionary<CustomParameter, Dictionary<Part, SerializableTextureData[]>> MultiTextureData =
            new Dictionary<CustomParameter, Dictionary<Part, SerializableTextureData[]>> ();


        internal event Action<Part, SerializableTextureData[]> ExtDataFromTheCard;
        internal event Action<Part, SerializableTextureData[]> ExtDataFromVanilla;

        protected virtual string PayloadId { get; }

        protected bool PluginDataLoaded;


        private void Awake() {
            Migrations.ApplicablesMigration.LoadingIsStarted += OnLoadingIsStarted;
            Migrations.ApplicablesMigration.LoadingIsComplete += OnLoadingIsComplete;

            ExtendedSave.CardBeingLoaded += OnCardBeingLoaded;
            ExtendedSave.CardBeingSaved += OnCardBeingSaved;

            FemaleHooks.FemaleIsBeingApplied += CreateExtDataFromVanilla;
            MaleHooks.MaleIsBeingApplied += CreateExtDataFromVanilla;
        }

        private void OnDestroy() {
            Migrations.ApplicablesMigration.LoadingIsStarted -= OnLoadingIsStarted;
            Migrations.ApplicablesMigration.LoadingIsComplete -= OnLoadingIsComplete;

            ExtendedSave.CardBeingLoaded -= OnCardBeingLoaded;
            ExtendedSave.CardBeingSaved -= OnCardBeingSaved;

            FemaleHooks.FemaleIsBeingApplied -= CreateExtDataFromVanilla;
            MaleHooks.MaleIsBeingApplied -= CreateExtDataFromVanilla;
        }


        internal SerializableTextureData[] GetSerializableTextureDataList(Human human, Part part) {
            return MultiTextureData
                .GetValueOrDefaultValue(GetCustomParamByHuman(human), null)
                ?.GetValueOrDefaultValue(part, null);
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

        protected virtual void MigrateExtendedData(CustomParameter file) {
            throw new NotImplementedException();
        }


        private void OnCardBeingLoaded(CustomParameter file) {
            Logger.LogDebug($"OnCardBeingLoaded {(SaveLoadDataTracker.SexOfLoadingCharacter == SEX.FEMALE ? SaveLoadDataTracker.FemaleIdOfLoadingCharacter.ToString() : SaveLoadDataTracker.MaleIdOfLoadingCharacter.ToString())}");

            if (Migrations.ApplicablesMigration.MigrationIsRequired) {
                Logger.LogDebug($"Migrating extended data.");
                MigrateExtendedData(file);
                return;
            }

            var pluginData = ExtendedSave.GetExtendedDataById(file, PayloadId);
            if (pluginData == null) {
                PluginDataLoaded = false;
                Logger.LogDebug("No PluginData is found in this card");
                if (!MakerAPI.InsideMaker) InitializeDictionaryWithVanillaData(file);
                return;
            }

            if (MakerAPI.InsideMaker) {
                foreach (var kvp in pluginData.data) {
                    var value = SerializationUtils.JsonDeserializeFromString<SerializableTextureData[]>((string)kvp.Value);
                    ExtDataFromTheCard?.Invoke((Part)Enum.Parse(typeof(Part), kvp.Key), value);
                }
            }
            else {
                var kvp = pluginData.data.ElementAt(0);
                MultiTextureData[file] = SerializationUtils
                    .JsonDeserializeFromString<Dictionary<Part, SerializableTextureData[]>>(kvp.Value.ToString());

                PluginDataLoaded = true;

                Logger.LogDebug($"Extended texture data has been loaded");
            }
        }

        private void OnCardBeingSaved(CustomParameter file) {
            Logger.LogDebug($"OnCardBeingSaved {(SaveLoadDataTracker.SexOfSavingCharacter == SEX.FEMALE ? SaveLoadDataTracker.FemaleIdOfSavingCharacter.ToString() : SaveLoadDataTracker.MaleIdOfSavingCharacter.ToString())}");

            var pluginData = new PluginData {
                version = 0,
                data = new Dictionary<string, object>()
            };
            if (MakerAPI.InsideMaker) {
                var headData = GetUnsavedChanges(Part.Head);
                var bodyData = GetUnsavedChanges(Part.Body);

                if (headData != null)
                    pluginData.data.Add(nameof(Part.Head), SerializationUtils.JsonSerializeToString(headData));

                if (bodyData != null)
                    pluginData.data.Add(nameof(Part.Body), SerializationUtils.JsonSerializeToString(bodyData));
            }
            else {
                pluginData.data.Add(
                    SaveLoadDataTracker.SexOfSavingCharacter == SEX.FEMALE
                        ? SaveLoadDataTracker.FemaleIdOfSavingCharacter.ToString()
                        : SaveLoadDataTracker.MaleIdOfSavingCharacter.ToString(),
                    SerializationUtils.JsonSerializeToString(MultiTextureData.GetValueOrDefaultValue(GetCustomParamOfSavingCharacter(), null))
                );
            }

            ExtendedSave.SetExtendedDataById(file, PayloadId, pluginData);
        }

        private void OnLoadingIsStarted() {
            Logger.LogDebug($"OnLoadingIsStarted");
            ResetDictionaries();
        }

        private void OnLoadingIsComplete() {
            Logger.LogDebug($"OnLoadingIsComplete");
            Logger.LogDebug("/////////////////////////// MultiTextureData //////////////////////////////");
            Logger.LogDebug($"MaleMultiTextureData {SerializationUtils.JsonSerializeToString(MultiTextureData)}\n\n");
        }


        private void CreateExtDataFromVanilla(Human human) {
            if (!MakerAPI.InsideMaker || PluginDataLoaded)
                return;

            var convertedVanillaData = CreateMultiTextureDataDict(human.customParam);
            ExtDataFromVanilla?.Invoke(Part.Head, convertedVanillaData?.GetValueOrDefaultValue(Part.Head, null));
        }

        private void ResetDictionaries() {
            Logger.LogDebug($"Resetting MultiTextureData dictionaries");
            MultiTextureData.Clear();
        }

        protected void InitializeDictionaryWithVanillaData(CustomParameter file) {
            Logger.LogDebug("Initializing dictionaries with vanilla data");
            MultiTextureData[file] = CreateMultiTextureDataDict(file);
        }


        protected static CustomParameter GetCustomParamByHuman(Human human) {
            return human.sex == SEX.FEMALE
                ? GetCustomParamByHeroineId(((Female)human).heroineID)
                : GetCustomParamByMaleId(((Male)human).maleID);
        }

        protected static HEROINE GetHeroineIdByCustomParam(CustomParameter customParam) {
            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_ritsuko))
                return HEROINE.RITSUKO;

            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_akiko))
                return HEROINE.AKIKO;

            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_yukiko))
                return HEROINE.YUKIKO;

            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_mariko))
                return HEROINE.MARIKO;

            return HEROINE.NUM;
        }

        protected static MALE_ID GetMaleIdByCustomParam(CustomParameter customParam) {
            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_hero))
                return MALE_ID.HERO;

            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_kouichi))
                return MALE_ID.KOUICHI;

            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_h_maleMobA))
                return MALE_ID.MOB_A;

            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_h_maleMobB))
                return MALE_ID.MOB_B;

            if (ReferenceEquals(customParam, GlobalData.PlayData.custom_h_maleMobC))
                return MALE_ID.MOB_C;

            return MALE_ID.NUM;
        }


        private static CustomParameter GetCustomParamOfSavingCharacter() {
            return SaveLoadDataTracker.SexOfSavingCharacter == SEX.FEMALE
                ? GetCustomParamByHeroineId(SaveLoadDataTracker.FemaleIdOfSavingCharacter)
                : GetCustomParamByMaleId(SaveLoadDataTracker.MaleIdOfSavingCharacter);
        }

        private static CustomParameter GetCustomParamByHeroineId(HEROINE heroineId) {
            return GlobalData.PlayData.GetHeroineCustomParam(heroineId);
        }

        private static CustomParameter GetCustomParamByMaleId(MALE_ID maleId) {
            return GlobalData.PlayData.GetMaleCustomParam(maleId);
        }
    }
}
