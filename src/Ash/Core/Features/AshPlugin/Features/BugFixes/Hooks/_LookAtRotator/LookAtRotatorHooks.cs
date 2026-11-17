using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Utility;

namespace Ash.Core.Features.AshPlugin.Features.BugFixes.Hooks._LookAtRotator;

[HarmonyPatch]
internal class LookAtRotatorHooks
{
    private static readonly Dictionary<LookAtRotator.RotateBone, BoneState> States = new();

    private static int PruneThreshold = 64;


    [HarmonyPrefix]
    [HarmonyPatch(typeof(LookAtRotator), nameof(LookAtRotator.Calc))]
    // ReSharper disable once InconsistentNaming
    internal static bool CalcPrefix(LookAtRotator __instance) {
        if (PresetOverrides.TryGetValue(__instance, out var presetRotations)) {
            ApplyPreset(__instance, presetRotations);
            return false;
        }

        if (!Ash.PersistentSettings.ApplyNewLarCalculationLogic.Value)
            return true;

        if (__instance.calcType == LookAtRotator.TYPE.NO)
            return false;

        foreach (var rotateBone in __instance.rotateBones)
            rotateBone.ResetRotate();

        __instance.baseBone.transform.localRotation = __instance.baseLocalRot;
        if ((bool)(Object)__instance.target && __instance.calcType is LookAtRotator.TYPE.TARGET or LookAtRotator.TYPE.AWAY)
            __instance.targetPos = __instance.target.transform.position;

        var dir = ResolveDirection(__instance);
        foreach (var bone in __instance.rotateBones) {
            var state = GetState(bone);
            if (__instance.calcType != LookAtRotator.TYPE.HOLD)
                Interpolate(state, Desired(__instance, bone, dir), __instance.turnPower, __instance.turnMinSpeed,
                    __instance.turnMaxSpeed);
            bone.Rotate(state.Rotation);
        }

        return false;
    }


    private static BoneState GetState(LookAtRotator.RotateBone bone) {
        if (States.TryGetValue(bone, out var state))
            return state;

        if (States.Count >= PruneThreshold)
            Prune();

        state = new BoneState { Rotation = Quaternion.Euler(bone.Euler) };
        States[bone] = state;
        return state;
    }

    private static void Prune() {
        var dead = (from pair in States where pair.Key.bone == null select pair.Key).ToList();

        foreach (var rotateBone in dead)
            States.Remove(rotateBone);

        PruneThreshold = Mathf.Max(64, States.Count * 2);
    }


    private static void Interpolate(BoneState state, Quaternion target, float speed, float min, float max) {
        var delta = target * Quaternion.Inverse(state.Rotation);
        delta.ToAngleAxis(out var angle, out var axis);

        if (angle < 1e-4f || float.IsNaN(axis.x) || float.IsInfinity(axis.x)) {
            state.Rotation = target;
            return;
        }

        if (angle > 180.0f)
            angle -= 360.0f;

        var step = Tween.Spring(0.0f, angle, speed, Time.deltaTime, min, max);
        state.Rotation = Normalized(Quaternion.AngleAxis(step, axis) * state.Rotation);
    }

    private static Quaternion Normalized(Quaternion q) {
#pragma warning disable Harmony003
        var n = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
#pragma warning restore Harmony003
        if (n < 1e-6f)
            return Quaternion.identity;

        var inv = 1.0f / n;

#pragma warning disable Harmony003
        return new Quaternion(q.x * inv, q.y * inv, q.z * inv, q.w * inv);
#pragma warning restore Harmony003
    }

    private static Vector3 ResolveDirection(LookAtRotator lookAtRotator) {
        if (lookAtRotator.calcType != LookAtRotator.TYPE.TARGET && lookAtRotator.calcType != LookAtRotator.TYPE.AWAY)
            return Vector3.forward;

        var v = lookAtRotator.baseBone.InverseTransformPoint(lookAtRotator.targetPos);
        var degenerate = v.sqrMagnitude < 1e-8f;
        var angle = degenerate ? 0.0f : Vector3.Angle(Vector3.forward, v);

        bool outOfRange;
        if (lookAtRotator.calcType == LookAtRotator.TYPE.TARGET)
            outOfRange = angle >= lookAtRotator.ignoreYaw;
        else
            outOfRange = angle <= lookAtRotator.awayIgnoreYaw;

        if (lookAtRotator.prevTarget.HasValue && (degenerate || outOfRange))
            return lookAtRotator.prevTarget.Value;

        var dir = degenerate ? Vector3.forward : v.normalized;
        lookAtRotator.prevTarget = dir;
        return dir;
    }

    private static Quaternion Desired(LookAtRotator lookAtRotator, LookAtRotator.RotateBone bone, Vector3 dir) {
        return lookAtRotator.calcType switch {
            LookAtRotator.TYPE.TARGET => Swing(lookAtRotator, dir, bone, lookAtRotator.turnRate) * Quaternion.Euler(lookAtRotator.offsetPitch, 0.0f, 0.0f),
            LookAtRotator.TYPE.AWAY => SwingAway(lookAtRotator, dir, bone), LookAtRotator.TYPE.FORWARD => Swing(lookAtRotator, Quaternion.Euler(lookAtRotator.offsetPitch, 0.0f, 0.0f) * Vector3.forward, bone, 1f),
            LookAtRotator.TYPE.DIRECTION => Swing(lookAtRotator, Quaternion.Euler(lookAtRotator.targetPos.x + lookAtRotator.offsetPitch, lookAtRotator.targetPos.y, 0.0f) * Vector3.forward, bone, 1f),
            LookAtRotator.TYPE.DIRECTION_UNLIMITED => Quaternion.Euler(lookAtRotator.targetPos.x + lookAtRotator.offsetPitch, lookAtRotator.targetPos.y, 0.0f),
            _ => Quaternion.identity
        };
    }

    private static float LimitAt(LookAtRotator lookAtRotator, LookAtRotator.RotateBone bone, Vector2 p) {
#pragma warning disable Harmony003
        var a = p.x >= 0.0f ? bone.maxRight : bone.maxLeft;
#pragma warning restore Harmony003
#pragma warning disable Harmony003
        var b = p.y >= 0.0f ? lookAtRotator.maxUp : lookAtRotator.maxDown;
#pragma warning restore Harmony003
#pragma warning disable Harmony003
        var denominator = Mathf.Sqrt(b * b * p.x * p.x + a * a * p.y * p.y);
#pragma warning restore Harmony003

        return denominator < 1e-6f ? 0.0f : a * b / denominator;
    }

    private static Quaternion Swing(LookAtRotator lookAtRotator, Vector3 dir, LookAtRotator.RotateBone bone, float rate) {
        var angle = Vector3.Angle(Vector3.forward, dir);
        var p = (Vector2)dir;
        if (p.sqrMagnitude < 1e-8f) {
            if (angle < 90.0f)
                return Quaternion.identity;
            p = Vector2.right;
        }

        p.Normalize();
        angle = Mathf.Min(angle * rate, LimitAt(lookAtRotator, bone, p));

        return Quaternion.AngleAxis(angle, new Vector3(-p.y, p.x, 0.0f));
    }

    private static Quaternion SwingAway(LookAtRotator lookAtRotator, Vector3 dir, LookAtRotator.RotateBone bone) {
#pragma warning disable Harmony003
        var p = new Vector2(-dir.x, -dir.y);
#pragma warning restore Harmony003
        p = p.sqrMagnitude < 1e-8f ? Vector2.right : p.normalized;

        return Quaternion.AngleAxis(LimitAt(lookAtRotator, bone, p), new Vector3(-p.y, p.x, 0.0f));
    }


    private sealed class BoneState
    {
        public Quaternion Rotation = Quaternion.identity;
    }

    /// <summary>
    /// Asymmetrical per-eye vectors support
    /// </summary>

    private static readonly Dictionary<LookAtRotator, Quaternion[]> PresetOverrides = new();


    internal static void SetPresetOverride(LookAtRotator lookAtRotator, Quaternion[] localRotations) {
        // Drop entries for destroyed rotators so the table can't grow without bound.
        var dead = PresetOverrides.Keys.Where(key => key == null).ToList();

        foreach (var key in dead)
            PresetOverrides.Remove(key);

        PresetOverrides[lookAtRotator] = localRotations;
    }

    internal static void ClearPresetOverride(LookAtRotator lookAtRotator) {
        PresetOverrides.Remove(lookAtRotator);
    }

    private static void ApplyPreset(LookAtRotator lookAtRotator, Quaternion[] localRotations) {
        lookAtRotator.baseBone.transform.localRotation = lookAtRotator.baseLocalRot;

        var index = 0;
        foreach (var bone in lookAtRotator.rotateBones) {
            if (index >= localRotations.Length)
                break;

            var state = GetState(bone);
            // State is stored relative to the bone's rest rotation, presets are absolute local rotations.
            var target = Quaternion.Inverse(bone.baseRot) * localRotations[index];
            Interpolate(state, target, lookAtRotator.turnPower, lookAtRotator.turnMinSpeed, lookAtRotator.turnMaxSpeed);
            bone.Rotate(state.Rotation);
            index++;
        }
    }
}
