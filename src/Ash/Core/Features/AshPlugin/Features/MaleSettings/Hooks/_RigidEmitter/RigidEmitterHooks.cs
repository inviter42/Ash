using System.Collections.Generic;
using Ash.Core.Features.AshPlugin.Settings;
using Ash.Core.Tooling.SceneManagement;
using Ash.Utility.GlobalUtils;
using HarmonyLib;
using UnityEngine;

namespace Ash.Core.Features.AshPlugin.Features.MaleSettings.Hooks._RigidEmitter;

[HarmonyPatch]
internal class RigidEmitterHooks
{
    internal static readonly List<int> ActivePackPool = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RigidEmitter), nameof(RigidEmitter.Start))]
    // ReSharper disable once InconsistentNaming
    internal static void StartPrefix(RigidEmitter __instance) {
        UpdateSettingsForMale(__instance);

        ActivePackPool.Clear();
        if (Ash.PersistentSettings.EjaculatorPack0AddToPoolValue.Value) ActivePackPool.Add(0);
        if (Ash.PersistentSettings.EjaculatorPack1AddToPoolValue.Value) ActivePackPool.Add(1);
        if (Ash.PersistentSettings.EjaculatorPack2AddToPoolValue.Value) ActivePackPool.Add(2);

        __instance.key = Ash.ConfigEntryTriggerMaleEmitter.Value.MainKey;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RigidEmitter), nameof(RigidEmitter.Emit))]
    // ReSharper disable once InconsistentNaming
    internal static bool EmitPrefix(RigidEmitter __instance) {
        if (ActivePackPool.Count == 0) {
            __instance.emitCount = 0;
            __instance.timer = 0f;
            __instance.emitting = false;

            Ash.Logger.LogDebug("RigidEmitter active pool is empty.");

            return false;
        }

        var gameObject = Object.Instantiate(__instance.original[RandomUtils.RandomFromList(ActivePackPool)], __instance.setParent, true);
        gameObject.SetActive(true);
        gameObject.transform.position = __instance.transform.position;
        gameObject.transform.rotation = __instance.transform.rotation;

        foreach (var componentsInChild in gameObject.GetComponentsInChildren<Rigidbody>()) {
            var y = Random.Range(__instance.directionMin, __instance.directionMax);
            var x = Random.Range(__instance.elevationMin, __instance.elevationMax);
            var num = Random.Range(__instance.speedMin, __instance.speedMax);
            var vector3 = __instance.transform.rotation * Quaternion.Euler(x, y, 0.0f) * Vector3.forward * num;
            componentsInChild.velocity = vector3;
        }

        ++__instance.emitCount;

        return false;
    }

    internal static void UpdateSettingsForAllMales() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        foreach (var male in hScene.mainMembers.males)
            UpdateSettingsForMale(male.ejaculator);
    }

    private static void UpdateSettingsForMale(RigidEmitter rigidEmitter) {
        for (var i = 0; i < rigidEmitter.original.Length; i++)
            UpdateSettings(rigidEmitter, i);
    }

    private static void UpdateSettings(RigidEmitter rigidEmitter, int index) {
        var child = rigidEmitter.original[index].transform.GetChild(0);
        var tensileStrength = child.GetComponent<TensileStrength>();

        Setting<float> localScale = null;
        Setting<float> relaxDistance = null;
        Setting<float> minDistance = null;
        Setting<float> maxDistance = null;

        switch (index) {
            case 0: {
                localScale = Ash.PersistentSettings.EjaculatorPack0LocalScaleValue;
                relaxDistance = Ash.PersistentSettings.EjaculatorPack0TsRelaxDistanceValue;
                minDistance = Ash.PersistentSettings.EjaculatorPack0TsMinDistanceValue;
                maxDistance = Ash.PersistentSettings.EjaculatorPack0TsMaxDistanceValue;

                break;
            }
            case 1: {
                localScale = Ash.PersistentSettings.EjaculatorPack1LocalScaleValue;
                relaxDistance = Ash.PersistentSettings.EjaculatorPack1TsRelaxDistanceValue;
                minDistance = Ash.PersistentSettings.EjaculatorPack1TsMinDistanceValue;
                maxDistance = Ash.PersistentSettings.EjaculatorPack1TsMaxDistanceValue;

                break;
            }
            case 2: {
                localScale = Ash.PersistentSettings.EjaculatorPack2LocalScaleValue;
                relaxDistance = Ash.PersistentSettings.EjaculatorPack2TsRelaxDistanceValue;
                minDistance = Ash.PersistentSettings.EjaculatorPack2TsMinDistanceValue;
                maxDistance = Ash.PersistentSettings.EjaculatorPack2TsMaxDistanceValue;

                break;
            }
        }

        child.localScale = new Vector3(localScale!.Value, localScale.Value, localScale.Value);
        tensileStrength.relaxDistance = relaxDistance!.Value;
        tensileStrength.minDistance = minDistance!.Value;
        tensileStrength.maxDistance = maxDistance!.Value;

        rigidEmitter.emitNum = Ash.PersistentSettings.EjaculatorRigidEmitterEmitNumValue.Value;
    }
}
