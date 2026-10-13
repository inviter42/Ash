using System;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.BetterPenetrationSettings.UI.BetterPenetrationSettingsView
{
    internal class BetterPenetrationSettingsView
    {
        internal const string AnimationControlsViewTabLabel = "BetterPenetration settings";

        private static readonly AshLogger Logger = new AshLogger(LoggingSettings.LoggingModules.BetterPenetration);

        private const string MainSettingsTitle = "Main Settings";
        private const string BoneSettingsTitle = "Bone Settings";
        private const string BonesTelemetryTitle = "Bones Telemetry";

        private const string EnableBetterPenetrationSubtitle = "Enable BetterPenetration:";
        private const string EnableBonesTelemetrySubtitle = "Enable bones telemetry:";

        private Vector2 ScrollPosition;

        private H_Scene HScene;

        internal void DrawView() {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
                DrawMainSettingsSection();
                DrawBoneSettingsSection();
                DrawDisplaySack();
                DrawBonesTelemetrySection();
            }

            GUILayout.EndScrollView();
        }

        private void DrawMainSettingsSection() {
            GUILayout.Space(20);
            Title(MainSettingsTitle);
            DrawEnableToggles();
            DrawBoneDensitySlider();
        }

        private void DrawBoneSettingsSection() {
            GUILayout.Space(20);
            Title(BoneSettingsTitle);
            DrawColliderDistanceSlider();
            DrawRootBoneLengthSlider();
            DrawBonesXyInvSqrtScaleFactorSlider();
            DrawBonesXyLinearScaleFactorSlider();
        }

        private void DrawBonesTelemetrySection() {
            GUILayout.Space(20);
            Title(BonesTelemetryTitle);

            DrawBonesTelemetryToggles();
            DrawBonesTelemetry();
        }


        private void DrawEnableToggles() {
            Subtitle(EnableBetterPenetrationSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.EnableBetterPenetration.Value == state,
                    () => {
                        if (!state) {
                            Ash.BoneInjector.RemoveInjectedBonesAndResetMesh();
                            Ash.BoneInjector.DestroyAllInjectors();
                        }
                        Ash.PersistentSettings.EnableBetterPenetration.Value = state;
                    })
            );
        }

        private void DrawBoneDensitySlider() {
            Subtitle($"[Requires restart] Bone density ({Ash.PersistentSettings.BoneDensity.Value}):");
            Ash.PersistentSettings.BoneDensity.Value = Convert.ToInt32(GUILayout.HorizontalSlider(Ash.PersistentSettings.BoneDensity.Value, 1f, 50f));
        }

        private void DrawColliderDistanceSlider() {
            Subtitle($"Collider distance offset ({Ash.PersistentSettings.ColliderDistanceOffset.Value:F3}):");
            Ash.PersistentSettings.ColliderDistanceOffset.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.ColliderDistanceOffset.Value, 0f, 0.1f);
        }

        private void DrawRootBoneLengthSlider() {
            Subtitle($"Root bone length factor ({Ash.PersistentSettings.RootBoneLength.Value:F3}):");
            Ash.PersistentSettings.RootBoneLength.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.RootBoneLength.Value, 0.1f, 2f);
        }

        private void DrawBonesXyInvSqrtScaleFactorSlider() {
            Subtitle($"Bones XY-scale factor (inverse sqrt) ({Ash.PersistentSettings.BoneXyInvSqrtScaleFactor.Value:F3}):");
            Ash.PersistentSettings.BoneXyInvSqrtScaleFactor.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.BoneXyInvSqrtScaleFactor.Value, 0.1f, 1f);
        }

        private void DrawBonesXyLinearScaleFactorSlider() {
            Subtitle($"Bones XY-scale factor (linear) ({Ash.PersistentSettings.BoneXyLinearScaleFactor.Value:F3}):");
            Ash.PersistentSettings.BoneXyLinearScaleFactor.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.BoneXyLinearScaleFactor.Value, 0.1f, 1f);
        }

        private void DrawDisplaySack() {
            GUILayout.Space(20);
            Subtitle("Display male balls:");
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.ShowMaleSackValue.Value == state,
                    () => {
                        Ash.PersistentSettings.ShowMaleSackValue.Value = state;
                        if (SceneTypeTracker.Scene is H_Scene hScene) {
                            foreach (var male in hScene.mainMembers.males) {
                                male.tinRoot.transform.FindChild("cm_O_dan_f").gameObject.SetActive(state);
                            }
                        }
                    })
            );
        }

        private void DrawBonesTelemetryToggles() {
            Subtitle(EnableBonesTelemetrySubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.EnableBonesTelemetry.Value == state,
                    () => Ash.PersistentSettings.EnableBonesTelemetry.Value = state)
            );
        }

        private void DrawBonesTelemetry() {
            if (!Ash.PersistentSettings.EnableBonesTelemetry.Value)
                return;

            if (Ash.BoneInjector.ActiveInjectors.Count == 0) {
                Info("No active injectors found.");
                return;
            }

            const int injectorIndex = 0;
            var activeInjector = Ash.BoneInjector.ActiveInjectors[injectorIndex];
            if (activeInjector?.NewBones == null)
                return;

            if (HScene == null && SceneTypeTracker.Scene is H_Scene hScene)
                HScene = hScene;

            if (HScene == null)
                return;

            foreach (var bone in activeInjector.NewBones) {
                var colliderPosition = activeInjector.Male.ik.tinTarget.position;
                var dist = VectorUtils.CalculateSignedDistance(bone.position, colliderPosition, bone.forward);
                Info(
                    $"{bone.name}\n"
                    + $"LocalScale: ({VectorUtils.Vec(bone.localScale)})\n"
                    + $"LocalPosition: ({VectorUtils.Vec(bone.localPosition)})\n"
                    + $"DistanceToCollider: ({dist:F6}), with offset ({dist + Ash.PersistentSettings.ColliderDistanceOffset.Value:F6})\n"
                    + $"Segment length: ({activeInjector.SegmentLength:F6})"
                );
            }
        }
    }
}
