using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedHooks._Female;
using Ash.Core.Features.BetterApplicables.SharedHooks._Male;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared;
using BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Female;
using BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Male;
using BetterApplicablesStudio.GlobalUtils;
using Character;
using ExtensibleSaveFormat;
using KKAPI.Maker;
using Newtonsoft.Json.Linq;
using PhExtendedSaveFiles;
using PhExtendedSaveFiles.Utils;
using UnityEngine;
using SerializableTextureData = BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared.SerializableTextureData;

namespace BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers
{
    internal class ApplicableDataManagerBase : MonoBehaviour
    {
        internal virtual int NumberOfSlots => 10;

        protected Dictionary<Part, SerializableTextureData[]> MultiTextureData = new Dictionary<Part, SerializableTextureData[]>();

        protected virtual string PayloadId { get; }

        private bool PluginDataLoaded;


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


        internal SerializableTextureData[] GetSerializableTextureDataList(Part part) {
            return MultiTextureData.GetValueOrDefaultValue(part, null);
        }


        protected virtual Dictionary<Part, SerializableTextureData[]> CreateMultiTextureDataDict(CustomParameter param) {
            throw new NotImplementedException();
        }


        private void OnCardBeingLoaded(CustomParameter file) {
            ResetDictionaries();

            var pluginData = ExtendedSave.GetExtendedDataById(file, PayloadId);
            if (pluginData == null) {
                PluginDataLoaded = false;
                BetterApplicablesStudio.Logger.LogDebug("No PluginData is found in this card");
                return;
            }

            foreach (var kvp in pluginData.data) {
                var key = (Part)Enum.Parse(typeof(Part), kvp.Key);
                var value = SerializationUtils.JsonDeserializeFromString<SerializableTextureData[]>((string)kvp.Value);
                MultiTextureData[key] = value;
            }

            PluginDataLoaded = true;

            BetterApplicablesStudio.Logger.LogDebug($"Extended texture data has been loaded");
        }

        private void CreateExtDataFromVanilla(Human human) {
            if (PluginDataLoaded)
                return;

            MultiTextureData = CreateMultiTextureDataDict(human.customParam);
        }

        private void ResetDictionaries() {
            BetterApplicablesStudio.Logger.LogDebug($"Resetting MultiTextureData dictionaries");
            MultiTextureData.Clear();
        }
    }
}
