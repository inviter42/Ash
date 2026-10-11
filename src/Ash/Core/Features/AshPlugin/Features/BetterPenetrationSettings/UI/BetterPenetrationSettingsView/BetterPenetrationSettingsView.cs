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

        private static readonly AshLogger Logger = new AshLogger(LoggingSettings.LoggingModules.AnimationControls);

        private const string MainSettingsTitle = "Main Settings";
        private const string BoneSettingsTitle = "Bone Settings";
        private const string PlaybackDataTitle = "Bones dynamic data";

        private const string EnableBetterPenetrationSubtitle = "Enable BetterPenetration:";

        private Vector2 ScrollPosition;

        private H_Scene HScene;

        internal void DrawView() {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
                DrawMainSettingsSection();
                DrawBoneSettingsSection();
#if DEBUG
                DrawDisplaySack();
                DrawBonesDataSection();
#endif
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
            DrawBonesXyScaleFactorSlider();
        }

        private void DrawBonesDataSection() {
            GUILayout.Space(20);
            Title(PlaybackDataTitle);
            DrawBonesDynamicData();
        }


        private void DrawEnableToggles() {
            Subtitle(EnableBetterPenetrationSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.EnableBetterPenetration.Value == state,
                    () => Ash.PersistentSettings.EnableBetterPenetration.Value = state)
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

        private void DrawBonesXyScaleFactorSlider() {
            Subtitle($"Bones XY-scale factor ({Ash.PersistentSettings.BoneXyScaleFactor.Value:F3}):");
            Ash.PersistentSettings.BoneXyScaleFactor.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.BoneXyScaleFactor.Value, 0.1f, 1f);
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

        private void DrawBonesDynamicData() {
            var injectorIndex = 0;
            var activeInjector = Ash.BoneInjector.ActiveInjectors[injectorIndex];
            if (activeInjector.NewBones == null)
                return;

            if (HScene == null && SceneTypeTracker.Scene is H_Scene hScene)
                HScene = hScene;

            if (HScene == null)
                return;

            Subtitle("Injected bones data");
            foreach (var bone in activeInjector.NewBones) {
                Info(
                    $"{bone.name}\n"
                    + $"LocalScale: Vec3({bone.localScale.x:F4}, {bone.localScale.y:F4}, {bone.localScale.z:F4})\n"
                    + $"LocalPosition: Vec3({bone.localPosition.x:F4}, {bone.localPosition.y:F4}, {bone.localPosition.z:F4})\n"
                    + $"DistanceToCollider: float({VectorUtils.CalculateSignedDistance(bone.position, HScene.mainMembers.males[injectorIndex].ik.tinTarget.position, bone.forward):F4})"
                );
            }
        }
    }
}
