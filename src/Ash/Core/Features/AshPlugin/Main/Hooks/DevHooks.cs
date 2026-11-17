using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Tooling.SceneManagement;
using Character;
using H;
using HarmonyLib;
using UnityEngine;
using Utility;

namespace Ash.Core.Features.AshPlugin.Main.Hooks
{
#if DEBUG
    [HarmonyPatch]
#endif
    internal class DevHooks
    {
        //////////////////////////////// --- DEBUGGING SECTION -- ////////////////////////////////

        // H_Members state machine hook
        [HarmonyPostfix]
        [HarmonyPatch(
            typeof(StateManagerBase_WithMsg<H_STATE, H_State, H_State.Message>),
            nameof(StateManagerBase_WithMsg<H_STATE, H_State, H_State.Message>.ChangeState),
            typeof(H_STATE),
            typeof(H_State.Message))
        ]
        // ReSharper disable once InconsistentNaming
        internal static void ChangeStatePostfix(StateManagerBase_WithMsg<H_STATE, H_State, H_State.Message> __instance,
            H_STATE next, H_State.Message msg) {
            Ash.Logger.LogDebug($"Change state is called, next: {next}, msg: {msg}");
            Ash.Logger.LogDebug($"NowStateID is {__instance.NowStateID}");
        }

        //////////////////////////////// --- CONVENIENCE SECTION -- ////////////////////////////////

        // load directly into the scene upon starting the game
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CautionScene), "Start")]
        // ReSharper disable once InconsistentNaming
        internal static bool StartPrefix(CautionScene __instance) {
            Ash.Logger.LogDebug($"Skipping to H scene.");

            __instance.InScene(false); // instantiates GC
            GlobalData.PlayData.Load(GlobalData.GetContinueSaveFile());
            __instance.GC.ChangeScene("H", "Load", 1f);

            return false;
        }

        //////////////////////////////// --- TESTING SECTION -- ////////////////////////////////

        ///////////////////////////////// ANIMATION BAKING /////////////////////////////////
        [HarmonyPrefix]
        [HarmonyPatch(typeof(H_State_Loop), nameof(H_State_Loop.In), typeof(H_State.Message))]
        // ReSharper disable once InconsistentNaming
        internal static bool HStateLoopInPrefix(H_State_Loop __instance, H_State.Message msg) {
            if (Ash.PersistentSettings.RemoveAnimationMinSpeedCapValue.Value)
                __instance.minSpeed = 0;

            return true;
        }

        internal class AnimationBaker : MonoBehaviour
        {
            internal static event Action<AnimationClip> BakeCompleteCallback;

            internal static void ReconstructAnimation(Human human, string targetClipName, float sampleRate) {
                var animator = human.GetComponentInChildren<Animator>();
                if (animator == null)
                    return;

                if (targetClipName.Length == 0)
                    return;

                var rootPath = human.sex == SEX.FEMALE
                    ? Body.name_baseBoneRoot_F
                    : Body.name_baseBoneRoot_M;

                var liveSkeletonRoot = human.transform.Find(rootPath);
                if (liveSkeletonRoot == null)
                    return;

                AnimationClip sourceClip = null;
                foreach (var clipInfo in animator.GetCurrentAnimatorClipInfo(0)) {
                    if (clipInfo.clip.name == targetClipName) {
                        sourceClip = clipInfo.clip;
                        Ash.Logger.LogDebug($"Target clip found!");
                        break;
                    }

                    Ash.Logger.LogDebug($"Searching for {targetClipName}, found {clipInfo.clip.name}");
                }

                if (sourceClip == null)
                    return;

                Sampler(human, animator, liveSkeletonRoot, sourceClip, sampleRate);
            }

            private static void Sampler(
                Human human,
                Animator animator,
                Transform sourceRoot,
                AnimationClip sourceClip,
                float sampleRate
            ) {
                var shadowContainer = new GameObject("ShadowRig_Bake_Container");
                shadowContainer.SetActive(false);

                var shadowRoot = Instantiate(sourceRoot, shadowContainer.transform);

                var sourceBones = new List<Transform>();
                var shadowBones = new List<Transform>();
                PopulateBoneLists(sourceRoot, shadowRoot, sourceBones, shadowBones);

                var posXCurves = new Dictionary<string, List<Keyframe>>();
                var posYCurves = new Dictionary<string, List<Keyframe>>();
                var posZCurves = new Dictionary<string, List<Keyframe>>();

                var rotXCurves = new Dictionary<string, List<Keyframe>>();
                var rotYCurves = new Dictionary<string, List<Keyframe>>();
                var rotZCurves = new Dictionary<string, List<Keyframe>>();

                var duration = sourceClip.length;
                var timeStep = 1f / sampleRate;
                var currentTime = 0f;

                var hScene = SceneTypeTracker.Scene as H_Scene;
                if (hScene) hScene.mainMembers.ClearIK();

                animator.Play(sourceClip.name, 0, 0f);
                animator.Update(0f);

                while (currentTime <= duration) {
                    animator.Update(timeStep);

                    for (var i = 0; i < sourceBones.Count; i++) {
                        shadowBones[i].localPosition = sourceBones[i].localPosition;
                        shadowBones[i].localEulerAngles = sourceBones[i].localEulerAngles;

                        var boneRelativePath = GetHierarchyPath(sourceBones[i], human.transform);

                        if (currentTime == 0f) {
                            posXCurves[boneRelativePath] = new List<Keyframe>();
                            posYCurves[boneRelativePath] = new List<Keyframe>();
                            posZCurves[boneRelativePath] = new List<Keyframe>();
                            rotXCurves[boneRelativePath] = new List<Keyframe>();
                            rotYCurves[boneRelativePath] = new List<Keyframe>();
                            rotZCurves[boneRelativePath] = new List<Keyframe>();
                        }

                        posXCurves[boneRelativePath].Add(new Keyframe(currentTime, shadowBones[i].localPosition.x));
                        posYCurves[boneRelativePath].Add(new Keyframe(currentTime, shadowBones[i].localPosition.y));
                        posZCurves[boneRelativePath].Add(new Keyframe(currentTime, shadowBones[i].localPosition.z));

                        rotXCurves[boneRelativePath].Add(new Keyframe(currentTime, shadowBones[i].localEulerAngles.x));
                        rotYCurves[boneRelativePath].Add(new Keyframe(currentTime, shadowBones[i].localEulerAngles.y));
                        rotZCurves[boneRelativePath].Add(new Keyframe(currentTime, shadowBones[i].localEulerAngles.z));
                    }

                    currentTime += timeStep;
                }

                if (hScene) hScene.mainMembers.SetStyleIK(sourceClip.name);

                var editableLegacyClip = new AnimationClip {
                    legacy = true,
                    name = sourceClip.name
                };

                const float posTolerance = 0.001f;
                const float rotTolerance = 0.01f;

                foreach (var bonePath in posXCurves.Keys) {
                    var curveX = new AnimationCurve(RecalcLinearTangents(RamerDouglasPeucker(posXCurves[bonePath], posTolerance)).ToArray());
                    var curveY = new AnimationCurve(RecalcLinearTangents(RamerDouglasPeucker(posYCurves[bonePath], posTolerance)).ToArray());
                    var curveZ = new AnimationCurve(RecalcLinearTangents(RamerDouglasPeucker(posZCurves[bonePath], posTolerance)).ToArray());

                    var curveRotX = new AnimationCurve(RecalcLinearTangents(RamerDouglasPeucker(rotXCurves[bonePath], rotTolerance)).ToArray());
                    var curveRotY = new AnimationCurve(RecalcLinearTangents(RamerDouglasPeucker(rotYCurves[bonePath], rotTolerance)).ToArray());
                    var curveRotZ = new AnimationCurve(RecalcLinearTangents(RamerDouglasPeucker(rotZCurves[bonePath], rotTolerance)).ToArray());

                    // edit data here
                    // example: delete keyframes: curveX.RemoveKey(index);
                    // example: alter values: curveX.MoveKey(index, new Keyframe(t, val + offset));

                    var transformType = typeof(Transform);
                    editableLegacyClip.SetCurve(bonePath, transformType, "localPosition.x", curveX);
                    editableLegacyClip.SetCurve(bonePath, transformType, "localPosition.y", curveY);
                    editableLegacyClip.SetCurve(bonePath, transformType, "localPosition.z", curveZ);

                    editableLegacyClip.SetCurve(bonePath, transformType, "localEulerAngles.x", curveRotX);
                    editableLegacyClip.SetCurve(bonePath, transformType, "localEulerAngles.y", curveRotY);
                    editableLegacyClip.SetCurve(bonePath, transformType, "localEulerAngles.z", curveRotZ);
                }

                Destroy(shadowContainer);

                BakeCompleteCallback?.Invoke(editableLegacyClip);
            }

            private static void PopulateBoneLists(
                Transform source,
                Transform shadow,
                List<Transform> sourceList,
                List<Transform> shadowList
            ) {
                sourceList.Add(source);
                shadowList.Add(shadow);
                for (var i = 0; i < source.childCount; i++) {
                    PopulateBoneLists(source.GetChild(i), shadow.GetChild(i), sourceList, shadowList);
                }
            }

            private static string GetHierarchyPath(Transform transform, Transform root) {
                if (transform == root) return "";
                var path = transform.name;
                while (transform.parent && transform.parent != root) {
                    transform = transform.parent;
                    path = transform.name + "/" + path;
                }

                return path;
            }

            private static List<Keyframe> RamerDouglasPeucker(List<Keyframe> keys, float tolerance) {
                if (keys == null || keys.Count <= 2)
                    return keys;

                var keepFlags = new bool[keys.Count];

                keepFlags[0] = true;
                keepFlags[keys.Count - 1] = true;

                // Run the recursive split-and-check pass across the whole list
                ExecuteRdp(keys, 0, keys.Count - 1, tolerance, ref keepFlags);

                // Rebuild the optimized collection based on the surviving flags
                return keys.Where((t, i) => keepFlags[i]).ToList();
            }

            private static void ExecuteRdp(List<Keyframe> keys, int startIdx, int endIdx, float tolerance,
                ref bool[] keepFlags) {
                if (endIdx <= startIdx + 1) return;

                var maxDistance = 0f;
                var furthestIdx = startIdx;

                var startKey = keys[startIdx];
                var endKey = keys[endIdx];

                // Find the point with the maximum perpendicular/vertical deviation from the chord line
                for (var i = startIdx + 1; i < endIdx; i++) {
                    var midKey = keys[i];

                    // Linear interpolation to find where the line passes at this specific time
                    var timeAlpha = (midKey.time - startKey.time) / (endKey.time - startKey.time);
                    var lineValue = Mathf.Lerp(startKey.value, endKey.value, timeAlpha);

                    var distance = Mathf.Abs(midKey.value - lineValue);

                    if (distance > maxDistance) {
                        maxDistance = distance;
                        furthestIdx = i;
                    }
                }

                // If the worst point drifts past our tolerance boundary, it's a critical inflection point
                if (maxDistance > tolerance) {
                    keepFlags[furthestIdx] = true;

                    // Recursively split the curve at this critical point and check both halves
                    ExecuteRdp(keys, startIdx, furthestIdx, tolerance, ref keepFlags);
                    ExecuteRdp(keys, furthestIdx, endIdx, tolerance, ref keepFlags);
                }
            }

            private static List<Keyframe> RecalcLinearTangents(List<Keyframe> optimizedKeys) {
                if (optimizedKeys == null || optimizedKeys.Count <= 2)
                    return optimizedKeys;

                for (var i = 0; i < optimizedKeys.Count; i++) {
                    var current = optimizedKeys[i];
                    var inTangent = 0f;
                    var outTangent = 0f;

                    // 1. Calculate the incoming slope from the previous keyframe
                    if (i > 0) {
                        var prev = optimizedKeys[i - 1];
                        var timeDelta = current.time - prev.time;
                        if (timeDelta > 0.0001f) {
                            inTangent = (current.value - prev.value) / timeDelta;
                        }
                    }

                    // 2. Calculate the outgoing slope leading to the next keyframe
                    if (i < optimizedKeys.Count - 1) {
                        var next = optimizedKeys[i + 1];
                        var timeDelta = next.time - current.time;
                        if (timeDelta > 0.0001f) {
                            outTangent = (next.value - current.value) / timeDelta;
                        }
                    }

                    // 3. Write the exact velocities directly into the keyframe structure
                    current.inTangent = inTangent;
                    current.outTangent = outTangent;

                    // In Unity 5.5, tangentMode flags dictate how the curve engine treats handles.
                    // 1 = Linear mode in the native unmanaged enumeration.
                    current.tangentMode = 1 | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4);

                    optimizedKeys[i] = current;
                }

                return optimizedKeys;
            }
        }
    }
}
