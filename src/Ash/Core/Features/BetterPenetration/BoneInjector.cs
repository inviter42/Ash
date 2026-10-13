using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.BetterPenetration
{
    internal class BoneInjector : MonoBehaviour
    {
        internal List<Injector> ActiveInjectors { get; private set; } = new List<Injector>();

        private static readonly AshLogger Logger = new AshLogger(LoggingSettings.LoggingModules.BetterPenetration);

        private void Awake() {
            SceneTypeTracker.SceneUnloaded += DestroyAllInjectors;
        }

        private void OnDestroy() {
            SceneTypeTracker.SceneUnloaded -= DestroyAllInjectors;
        }

        internal void DestroyAllInjectors() {
            ActiveInjectors.Clear();
        }

        internal void RemoveInjectedBonesAndResetMesh() {
            foreach (var injector in ActiveInjectors)
                injector.RemoveInjectedBonesAndResetMesh();
        }

        internal void PruneInjectorsList(Male male) {
            ActiveInjectors.Remove(ActiveInjectors.FirstOrDefault(injector => injector.Male == male));
            Logger.LogDebug($"male's {male.maleID} injector is pruned");
        }


        internal void TryInjectBones(Male male) {
            if (ActiveInjectors.Find(inj => inj.Male == male) != null)
                return;

            var injector = new Injector(male);
            injector.TryInjectBones();

            ActiveInjectors.Add(injector);


        }

        internal void UpdateIKBonesTransforms(Male male) {
            ActiveInjectors.Find(inj => inj.Male == male)?.UpdateIKBonesTransforms();
        }

        internal void UpdateIKIndependentBonesTransforms(Male male) {
            ActiveInjectors.Find(inj => inj.Male == male)?.UpdateIKIndependentBonesTransforms();

        }

        internal void ResetBoneTransforms(Male male) {
            ActiveInjectors.Find(inj => inj.Male == male)?.ResetBoneTransforms();
        }


        internal class Injector
        {
            internal Transform[] NewBones { get; private set; }
            internal Male Male { get; }
            internal float SegmentLength { get; private set; }

            private Transform TinRoot;
            private Transform TinTip;
            private Mesh OriginalMesh;
            private SkinnedMeshRenderer Renderer;

            private bool Initialized;


            internal Injector(Male male) {
                Male = male;
            }

            internal void TryInjectBones() {
                if (Initialized)
                    return;

                TinRoot = Male.ik.tinRoot;
                TinTip = Male.ik.tinTip;
                Renderer = Male.tinRoot.GetComponentInChildren<SkinnedMeshRenderer>();

                if (TinRoot == null) {
                    Logger.LogDebug($"TinRoot {TinRoot}{(TinRoot == null ? "(null)" : "")}");
                    return;
                }

                if (TinTip == null) {
                    Logger.LogDebug($"TinTip {TinTip}{(TinTip == null ? "(null)" : "")}");
                    return;
                }

                if (Renderer == null) {
                    Logger.LogDebug($"Renderer {Renderer}{(Renderer == null ? "(null)" : "")}");
                    return;
                }

                var boneCount = Ash.PersistentSettings.BoneDensity.Value;
                var totalMeshLength = TinTip.localPosition.z;
                SegmentLength = totalMeshLength / boneCount;
                NewBones = new Transform[boneCount];

                for (var i = 0; i < boneCount; i++) {
                    var boneObj = new GameObject("BetterPenetrationBone" + i);
                    var boneTransform = boneObj.transform;
                    boneTransform.SetParent(TinRoot, false);
                    boneTransform.localRotation = Quaternion.identity;
                    boneTransform.localScale = Vector3.one;
                    boneTransform.localPosition = new Vector3(0f, 0f, i * SegmentLength);
                    NewBones[i] = boneTransform;
                }

                var originalBonesArray = Renderer.bones;
                var rootIndex = -1;

                for (var i = 0; i < originalBonesArray.Length; i++) {
                    if (originalBonesArray[i] == TinRoot) {
                        rootIndex = i;
                        break;
                    }
                }

                if (rootIndex == -1)
                    return;

                var originalBoneCount = originalBonesArray.Length;
                var newBonesArray = new Transform[originalBoneCount + boneCount];
                Array.Copy(originalBonesArray, newBonesArray, originalBoneCount);
                var newBonesIndices = new int[boneCount];

                for (var i = 0; i < boneCount; i++) {
                    var assignedIndex = originalBoneCount + i;
                    newBonesArray[assignedIndex] = NewBones[i];
                    newBonesIndices[i] = assignedIndex;
                }

                OriginalMesh = Renderer.sharedMesh;
                var newMesh = Instantiate(OriginalMesh);
                newMesh.name = OriginalMesh.name + "_BetterPenetration";
                var vertices = newMesh.vertices;
                var newBoneWeights = newMesh.boneWeights;
                var bindPoses = newMesh.bindposes;
                var meshToRootLocalSpace = bindPoses[rootIndex];

                for (var i = 0; i < vertices.Length; i++) {
                    var rootLocalPos = meshToRootLocalSpace.MultiplyPoint3x4(vertices[i]);
                    var zValue = Mathf.Clamp(rootLocalPos.z, 0f, totalMeshLength); // safety clamp
                    var normalizedFractionalPos = zValue / totalMeshLength;
                    var bucketIndex = Mathf.FloorToInt(normalizedFractionalPos * boneCount);
                    if (bucketIndex >= boneCount) //  array oob safety
                        bucketIndex = boneCount - 1;

                    var boneWeight = newBoneWeights[i];
                    boneWeight.boneIndex0 = newBonesIndices[bucketIndex];
                    boneWeight.weight0 = 1f;
                    boneWeight.boneIndex1 = 0;
                    boneWeight.weight1 = 0f;
                    boneWeight.boneIndex2 = 0;
                    boneWeight.weight2 = 0f;
                    boneWeight.boneIndex3 = 0;
                    boneWeight.weight3 = 0f;
                    newBoneWeights[i] = boneWeight;
                }

                var newBindPoses = new Matrix4x4[originalBoneCount + boneCount];
                Array.Copy(bindPoses, newBindPoses, bindPoses.Length);
                for (var i = 0; i < boneCount; i++) {
                    var distFromRoot = i * SegmentLength;
                    var translationVector = new Vector3(0f, 0f, -distFromRoot);
                    var translationOffset = Matrix4x4.TRS(translationVector, Quaternion.identity, Vector3.one);
                    newBindPoses[newBonesIndices[i]] = translationOffset * bindPoses[rootIndex];
                }

                newMesh.boneWeights = newBoneWeights;
                newMesh.bindposes = newBindPoses;
                Renderer.sharedMesh = newMesh;
                Renderer.bones = newBonesArray;

                Initialized = true;

                Logger.LogDebug($"Bones have been injected!");
            }

            internal void RemoveInjectedBonesAndResetMesh() {
                if (!Initialized)
                    return;

                Renderer.bones = Renderer.bones.Where(b => !NewBones.Contains(b)).ToArray();

                foreach (var bone in NewBones)
                    Destroy(bone.gameObject);

                Destroy(Renderer.sharedMesh);

                TinRoot = null;
                TinTip = null;
                NewBones = null;
                SegmentLength = 0;
                Renderer.sharedMesh = OriginalMesh;

                Initialized = false;
            }

            internal void UpdateIKBonesTransforms() {
                if (!Initialized) {
                    Logger.LogDebug($"Update IK transforms is called, but no bones were injected");
                    return;
                }

                foreach (var bone in NewBones) {
                    var distance = VectorUtils.CalculateSignedDistance(bone.position, Male.ik.tinTarget.position, bone.forward) + Ash.PersistentSettings.ColliderDistanceOffset.Value;
                    bone.localScale = new Vector3(1f, 1f, Mathf.Clamp(distance / SegmentLength, 0f, 1f));
                }

                for (var i = 1; i < NewBones.Length; i++) {
                    NewBones[i].localPosition = new Vector3(
                        0f,
                        0f,
                        NewBones[i - 1].localPosition.z + NewBones[i - 1].localScale.z * SegmentLength
                    );
                }
            }

            internal void UpdateIKIndependentBonesTransforms() {
                if (!Initialized) {
                    Logger.LogDebug($"Update IK-independent transforms is called, but no bones were injected");
                    return;
                }

                for (var i = 1; i < NewBones.Length; i++) {
                    var xyInvSqrtScalingFactor = Mathf.Lerp(1f, Ash.PersistentSettings.BoneXyInvSqrtScaleFactor.Value, 1f - Mathf.Sqrt(1f - (float)i / (NewBones.Length - 1)));
                    NewBones[i].localScale = new Vector3(
                        xyInvSqrtScalingFactor * Ash.PersistentSettings.BoneXyLinearScaleFactor.Value,
                        xyInvSqrtScalingFactor * Ash.PersistentSettings.BoneXyLinearScaleFactor.Value,
                        NewBones[i].localScale.z
                    );
                }

                TinRoot.localScale = new Vector3(1f, 1f, Ash.PersistentSettings.RootBoneLength.Value);
            }

            internal void ResetBoneTransforms() {
                if (!Initialized)
                    return;

                for (var i = 0; i < NewBones.Length; i++) {
                    NewBones[i].localPosition = new Vector3(0f, 0f, i * SegmentLength);
                    NewBones[i].localRotation = Quaternion.identity;
                    NewBones[i].localScale = Vector3.one;
                }

                Logger.LogDebug($"Bone transforms have been reset");
            }
        }
    }
}
