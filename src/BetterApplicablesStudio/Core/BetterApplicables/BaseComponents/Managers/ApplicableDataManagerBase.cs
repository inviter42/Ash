using System;
using System.Collections.Generic;
using System.Linq;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared;
using BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Female;
using BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Male;
using BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Studio;
using BetterApplicablesStudio.GlobalUtils;
using Character;
using ExtensibleSaveFormat;
using UnityEngine;

namespace BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers;

internal class ApplicableDataManagerBase : MonoBehaviour
{
    internal virtual int NumberOfSlots => 10;

    private readonly Dictionary<CustomParameter, Dictionary<Part, SerializableTextureData[]>> MultiTextureData = new();

    // ReSharper disable once InconsistentNaming
    private readonly List<CustomParameter> CpFromOICharInfo = new();

    protected virtual string PayloadId { get; }

    private void Awake() {
        ExtendedSave.CardBeingLoaded += OnCardBeingLoaded;
        ExtendedSave.CardBeingSaved += OnCardBeingSaved;

        FemaleHooks.FemaleIsAwaken += OnFemaleIsAwaken;
        MaleHooks.MaleIsAwaken += OnMaleIsAwaken;

        StudioHooks.StudioSceneStartedLoading += OnStudioSceneStartedLoading;
    }

    private void OnDestroy() {
        ExtendedSave.CardBeingLoaded -= OnCardBeingLoaded;
        ExtendedSave.CardBeingSaved -= OnCardBeingSaved;

        FemaleHooks.FemaleIsAwaken -= OnFemaleIsAwaken;
        MaleHooks.MaleIsAwaken -= OnMaleIsAwaken;

        StudioHooks.StudioSceneStartedLoading -= OnStudioSceneStartedLoading;
    }


    /// <summary>
    /// CustomParameter passed to this method is from Male/Female instance.
    /// </summary>
    internal SerializableTextureData[] GetSerializableTextureDataList(CustomParameter parameter, Part part) {
        return MultiTextureData.GetValueOrDefaultValue(parameter, null)
            ?.GetValueOrDefaultValue(part, null);
    }


    protected virtual Dictionary<Part, SerializableTextureData[]> CreateMultiTextureDataDict(CustomParameter param) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// If Studio.LoadScene() invokes it - CustomParameter passed to this callback is from OICharaInfo.
    /// If CharaList.LoadChara*() invokes it - CustomParameter passed to this callback is from Human.customParam.
    /// </summary>
    private void OnCardBeingLoaded(CustomParameter file) {
        BetterApplicablesStudio.Logger.LogDebug($"[{PayloadId}] OnCardBeingLoaded ({file.Sex})");

        if (BetterApplicablesStudio.StudioSceneIsBeingLoaded || BetterApplicablesStudio.StudioSceneIsBeingImported)
            CpFromOICharInfo.Add(file);

        var pluginData = ExtendedSave.GetExtendedDataById(file, PayloadId);
        if (pluginData == null) {
            BetterApplicablesStudio.Logger.LogDebug($"[{PayloadId}] No PluginData is found in this card");
            InitializeDictionaryWithVanillaData(file);
            return;
        }

        MultiTextureData[file] = new Dictionary<Part, SerializableTextureData[]>();

        foreach (var kvp in pluginData.data) {
            var key = (Part)Enum.Parse(typeof(Part), kvp.Key);
            var value = SerializationUtils.JsonDeserializeFromString<SerializableTextureData[]>(kvp.Value.ToString());

            MultiTextureData[file].Add(key, value);
        }

        BetterApplicablesStudio.Logger.LogDebug($"[{PayloadId}] Extended texture data has been loaded");
    }

    /// <summary>
    /// CustomParameter always comes from Human.customParam in this method
    /// </summary>
    private void OnCardBeingSaved(CustomParameter file) {
        BetterApplicablesStudio.Logger.LogDebug($"OnCardBeingSaved");

        var pluginData = new PluginData {
            version = 0,
            data = new Dictionary<string, object>()
        };

        var headData = MultiTextureData[file]?.GetValueOrDefaultValue(Part.Head, null);
        var bodyData = MultiTextureData[file]?.GetValueOrDefaultValue(Part.Body, null);

        if (headData != null)
            pluginData.data.Add(nameof(Part.Head), SerializationUtils.JsonSerializeToString(headData));

        if (bodyData != null)
            pluginData.data.Add(nameof(Part.Body), SerializationUtils.JsonSerializeToString(bodyData));

        ExtendedSave.SetExtendedDataById(file, PayloadId, pluginData);
    }


    /// <summary>
    /// If Studio.LoadScene() invokes it - it is called last, after CustomParameter.Load().
    /// If CharaList.LoadChara*() invokes it - it is called first, before CustomParameter.Load().
    /// </summary>
    private void OnFemaleIsAwaken(Female female) {
        BetterApplicablesStudio.Logger.LogDebug($"[{PayloadId}] OnFemaleIsAwaken");

        if (!BetterApplicablesStudio.StudioSceneIsBeingLoaded && !BetterApplicablesStudio.StudioSceneIsBeingImported)
            return;

        var cp = CpFromOICharInfo.ElementAt(0);
        MultiTextureData[female.customParam] = MultiTextureData[cp];
        MultiTextureData.Remove(cp);
        CpFromOICharInfo.RemoveAt(0);
    }

    /// <summary>
    /// If Studio.LoadScene() invokes it - it is called last, after CustomParameter.Load().
    /// If CharaList.LoadChara*() invokes it - it is called first, before CustomParameter.Load().
    /// </summary>
    private void OnMaleIsAwaken(Male male) {
        BetterApplicablesStudio.Logger.LogDebug($"[{PayloadId}] OnMaleIsAwaken");

        if (!BetterApplicablesStudio.StudioSceneIsBeingLoaded && !BetterApplicablesStudio.StudioSceneIsBeingImported)
            return;

        var cp = CpFromOICharInfo.ElementAt(0);
        MultiTextureData[male.customParam] = MultiTextureData[cp];
        MultiTextureData.Remove(cp);
        CpFromOICharInfo.RemoveAt(0);
    }


    private void OnStudioSceneStartedLoading() {
        ResetDictionaries();
    }

    private void ResetDictionaries() {
        BetterApplicablesStudio.Logger.LogDebug($"[{PayloadId}] Resetting MultiTextureData dictionaries");
        MultiTextureData.Clear();
    }

    private void InitializeDictionaryWithVanillaData(CustomParameter file) {
        BetterApplicablesStudio.Logger.LogDebug($"[{PayloadId}] Initializing dictionaries with vanilla data");
        MultiTextureData[file] = CreateMultiTextureDataDict(file);
    }
}
