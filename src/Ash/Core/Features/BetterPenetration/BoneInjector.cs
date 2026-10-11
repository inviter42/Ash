using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Tooling.SceneManagement;
using Ash.Utility.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.BetterPenetration
{
    internal class BoneInjector : MonoBehaviour
    {
        internal List<Injector> ActiveInjectors { get; } = new List<Injector>();

        private void Awake() {
            SceneTypeTracker.SceneUnloaded += DestroyAllInjectors;
        }

        private void OnDestroy() {
            SceneTypeTracker.SceneUnloaded -= DestroyAllInjectors;
        }


        internal void ResetBoneTransforms(Male male) {
            ActiveInjectors.Find(inj => inj.Male == male)?.ResetBoneTransforms();
        }

        internal void RemoveInjectedBonesAndResetMesh(Male male) {
            ActiveInjectors.Find(inj => inj.Male == male)?.RemoveInjectedBonesAndResetMesh();
        }

        internal void TryInjectBones(Male male) {
            var existingInjector = ActiveInjectors.Find(inj => inj.Male == male);
            if (existingInjector != null && existingInjector.Male != null)
                return;

            ActiveInjectors.Remove(existingInjector);

            var injector = new Injector(male);
            injector.TryInjectBones();

            ActiveInjectors.Add(injector);
        }

        internal void UpdateBonesTransforms(Male male) {
            ActiveInjectors.Find(inj => inj.Male == male)?.UpdateBonesTransforms();
        }

        private void DestroyAllInjectors() {
            ActiveInjectors.Clear();
        }


        internal class Injector
        {
            internal Transform[] NewBones { get; private set; }
            internal Male Male { get; }

            private Transform TinRoot;
            private Transform TinTip;
            private Mesh OriginalMesh;
            private SkinnedMeshRenderer Renderer;
            private float SegmentLength;

            private bool Initialized;


            internal Injector(Male male) {
                Male = male;
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

            internal void TryInjectBones() {
                if (Initialized)
                    return;

                TinRoot = Male.ik.tinRoot;
                TinTip = Male.ik.tinTip;
                Renderer = Male.tinRoot.GetComponentInChildren<SkinnedMeshRenderer>();

                if (TinRoot == null || TinTip == null || Renderer == null) {
                    Ash.Logger.LogDebug($"TinRoot {TinRoot}{(TinRoot == null ? "(null)" : "")}, TinTip {TinTip}{(TinTip == null ? "(null)" : "")}, Renderer {Renderer}{(Renderer == null ? "(null)" : "")}");
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

                // todo: we need to keep the original mesh saved to have an ability to toggle back to it in the ash plugin menu
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

                Ash.Logger.LogDebug($"Bones have been injected!");

                ReconstructValidState();
            }

            internal void UpdateBonesTransforms() {
                if (!Initialized) {
                    Ash.Logger.LogDebug($"Update is called, but no bones were injected");
                    return;
                }

                var colliderPosition = Male.ik.tinTarget.position;
                var activeBone = NewBones.LastOrDefault(bone => bone.localScale.z > 0 && bone.localScale.z < 1);
                if (activeBone == null) {
                    if (NewBones.All(b => b.localScale.z == 0)) {
                        activeBone = NewBones[0];
                    } else if (NewBones.All(b => b.localScale.z >= 1)) {
                        activeBone = NewBones[NewBones.Length - 1];
                    } else {
                        var firstZeroScaleBone = NewBones.FirstOrDefault(b => b.localScale.z == 0);
                        var lastOneScaleBone = NewBones.LastOrDefault(b => b.localScale.z >= 1);
                        // ReSharper disable once PossibleNullReferenceException
                        var firstZeroScaleBoneDistToCollider = VectorUtils.CalculateSignedDistance(firstZeroScaleBone.position, colliderPosition, firstZeroScaleBone.forward) + Ash.PersistentSettings.ColliderDistanceOffset.Value;
                        // ReSharper disable once PossibleNullReferenceException
                        var lastOneScaleBoneDistToCollider = VectorUtils.CalculateSignedDistance(lastOneScaleBone.position, colliderPosition, lastOneScaleBone.forward) + Ash.PersistentSettings.ColliderDistanceOffset.Value;
                        if (firstZeroScaleBoneDistToCollider > 0 && lastOneScaleBoneDistToCollider > 0) {
                            activeBone = firstZeroScaleBone;
                        } else if (firstZeroScaleBoneDistToCollider < 0) {
                            activeBone = lastOneScaleBone;
                        } else {
                            return; // intermediate state is ambiguous, wait for the next frame to resolve
                        }
                    }
                }

                var distance = VectorUtils.CalculateSignedDistance(activeBone.position, colliderPosition, activeBone.forward) + Ash.PersistentSettings.ColliderDistanceOffset.Value;
                var clampedDistance = Mathf.Clamp(distance, 0f, SegmentLength);
                var scaleFactor = clampedDistance / SegmentLength;
                activeBone.localScale = new Vector3(1f, 1f, Mathf.Clamp(scaleFactor, 0f, scaleFactor));

                for (var i = 1; i < NewBones.Length; i++) {
                    NewBones[i].localPosition = new Vector3(
                        0f,
                        0f,
                        NewBones[i - 1].localPosition.z + NewBones[i - 1].localScale.z * SegmentLength
                    );

                    var xyDecayingScalingFactor = Mathf.Lerp(1f, Ash.PersistentSettings.BoneXyScaleFactor.Value, 1f - Mathf.Sqrt(1f - (float)i / (NewBones.Length - 1)));
                    NewBones[i].localScale = new Vector3(xyDecayingScalingFactor, xyDecayingScalingFactor, NewBones[i].localScale.z);
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

                Ash.Logger.LogDebug($"Bone transforms have been reset");
            }

            private void ReconstructValidState() {
                if (!Initialized) {
                    Ash.Logger.LogDebug($"ReconstructValidState is called, but no bones were injected");
                    return;
                }

                ResetBoneTransforms();

                var colliderPosition = Male.ik.tinTarget.position;

                foreach (var bone in NewBones) {
                    var distance = VectorUtils.CalculateSignedDistance(bone.position, colliderPosition, bone.forward) + Ash.PersistentSettings.ColliderDistanceOffset.Value;
                    var clampedDistance = Mathf.Clamp(distance, 0f, SegmentLength);
                    var scaleFactor = clampedDistance / SegmentLength;
                    bone.localScale = new Vector3(1f, 1f, Mathf.Clamp(scaleFactor, 0f, scaleFactor));
                }

                for (var i = 1; i < NewBones.Length; i++) {
                    NewBones[i].localPosition = new Vector3(
                        0f,
                        0f,
                        NewBones[i - 1].localPosition.z + NewBones[i - 1].localScale.z * SegmentLength
                    );
                }
            }
        }
    }
}
