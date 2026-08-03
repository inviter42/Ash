using System;
using Ash.Core.Features.Common.Components;
using Ash.Core.SceneManagement;
using Ash.Core.UI;
using Ash.Core.UI.Types;
using Ash.GlobalUtils;
using UnityEngine;
using Valve.VR.InteractionSystem;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.HSceneSettings.UI.HSceneSettingsView
{
    internal class HSceneSettingsView
    {
        internal const string HSceneSettingsViewTabLabel = "H-Scene Settings";

        private const string BugFixesTitle = "Bug Fixes";
        private const string AnimationControlsTitle = "Animation settings";
        private const string ParticleSystemsControlsTitle = "Particle systems settings";
        private const string VoiceControlsTitle = "Voice settings";
        private const string DirtyTalkTriggerSettingsTitle = "Dirty Talk settings";

        private const string MuteBackgroundFemaleSubtitle = "Mute background female:";
        private const string DisableFemaleAutoEjaculationSubtitle = "Disable female automatic ejaculation:";
        private const string SkipSpurtStateSubtitle = "Skip to ejaculation immediately:";
        private const string DisableFemaleHVoiceBarkSubtitle = "Disable female voice lines on H-Pose start:";
        private const string DisableFemaleInactionBarkSubtitle = "Disable female voice lines after inactivity period:";
        private const string DisableFemaleVoiceBarkAtSceneStartSubtitle = "Disable female voice lines at H-Scene start:";
        private const string DisableFemaleVoiceBarkAtSceneEndSubtitle = "Disable female voice lines at H-Scene end:";
        private const string DisableFemaleVoiceBarkAfterEjaInSubtitle = "Disable female voice lines after ejaculation inside:";
        private const string DisableFemaleVoiceBarkAfterExtractSubtitle = "Disable female voice lines after extraction:";
        private const string DisableFemaleSpermDripAfterExtractSubtitle = "Disable female sperm drip after extraction:";
        private const string InterruptVoiceClipImmediatelyUponGagChangeSubtitle = "Update female voice clip immediately upon gag change:";
        private const string FixIncorrectShowMouthLiquidStateSubtitle = "Fix ShowMouthLiquid animation:";

        private Vector2 ScrollPosition;

        // ReSharper disable once MemberCanBeMadeStatic.Global
        internal void DrawView() {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
                DrawAnimationControlsSection();
                DrawParticleSystemsSection();
                DrawVoiceControlsSection();
                DrawBugFixesSection();
                DrawDirtyTalkSettingsSection();
            }

            GUILayout.EndScrollView();
        }

        private void DrawAnimationControlsSection() {
            GUILayout.Space(20);

            Title(AnimationControlsTitle);

            DrawSkipSpurtState();
        }

        private void DrawParticleSystemsSection() {
            GUILayout.Space(20);

            Title(ParticleSystemsControlsTitle);

            DrawDisableFemaleSpermDripAfterExtract();

            GUILayout.Space(12);

            DrawFemaleSpermDripPsSettings();
        }

        private void DrawVoiceControlsSection() {
            GUILayout.Space(20);

            Title(VoiceControlsTitle);

            var drawCalls = new Action[] {
                DrawDisableFemaleAutoEjaculation,
                DrawMuteBackgroundFemale,
                DrawDisableFemaleHVoiceBark,
                DrawDisableFemaleInactionBark,
                DrawDisableFemaleVoiceBarkAtSceneStart,
                DrawDisableFemaleVoiceBarkAtSceneEnd,
                DrawDisableFemaleVoiceBarkAfterEjaIn,
                DrawDisableFemaleVoiceBarkAfterExtract
            };

            drawCalls.ForEach(dc => {
                dc.Invoke();
                GUILayout.Space(12);
            });
        }

        private void DrawBugFixesSection() {
            GUILayout.Space(20);

            Title(BugFixesTitle);

            DrawInterruptVoiceClipImmediatelyUponGagChange();

            GUILayout.Space(12);

            DrawFixIncorrectShowMouthLiquidState();
        }

        private void DrawDirtyTalkSettingsSection() {
            GUILayout.Space(20);

            Title(DirtyTalkTriggerSettingsTitle);

            DrawDirtyTalkSettings();
        }


        private void DrawMuteBackgroundFemale() {
            Subtitle(MuteBackgroundFemaleSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.ShouldMuteBackgroundFemale.Value == state,
                    () => {
                        Ash.PersistentSettings.ShouldMuteBackgroundFemale.Value = state;
                        foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>()) {
                            female.UpdateVoiceVolume();
                        }
                    })
            );
        }

        private void DrawDisableFemaleAutoEjaculation() {
            Subtitle(DisableFemaleAutoEjaculationSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.DisableFemaleAutoEjaculation.Value == state,
                    () => Ash.PersistentSettings.DisableFemaleAutoEjaculation.Value = state)
            );
        }

        private void DrawSkipSpurtState() {
            Subtitle(SkipSpurtStateSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.SkippingSpurtStateEnabled.Value == state,
                    () => Ash.PersistentSettings.SkippingSpurtStateEnabled.Value = state)
            );
        }

        private void DrawDisableFemaleHVoiceBark() {
            Subtitle(DisableFemaleHVoiceBarkSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.DisableFemaleHVoiceBark.Value == state,
                    () => Ash.PersistentSettings.DisableFemaleHVoiceBark.Value = state)
            );
        }

        private void DrawDisableFemaleInactionBark() {
            Subtitle(DisableFemaleInactionBarkSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.DisableFemaleInactionBark.Value == state,
                    () => Ash.PersistentSettings.DisableFemaleInactionBark.Value = state)
            );
        }

        private void DrawDisableFemaleVoiceBarkAtSceneStart() {
            Subtitle(DisableFemaleVoiceBarkAtSceneStartSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneStart.Value == state,
                    () => Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneStart.Value = state)
            );
        }

        private void DrawDisableFemaleVoiceBarkAtSceneEnd() {
            Subtitle(DisableFemaleVoiceBarkAtSceneEndSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneEnd.Value == state,
                    () => Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneEnd.Value = state)
            );
        }

        private void DrawDisableFemaleVoiceBarkAfterEjaIn() {
            Subtitle(DisableFemaleVoiceBarkAfterEjaInSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.DisableFemaleVoiceBarkAfterEjaIn.Value == state,
                    () => Ash.PersistentSettings.DisableFemaleVoiceBarkAfterEjaIn.Value = state)
            );
        }

        private void DrawDisableFemaleVoiceBarkAfterExtract() {
            Subtitle(DisableFemaleVoiceBarkAfterExtractSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.DisableFemaleVoiceBarkAfterExtract.Value == state,
                    () => Ash.PersistentSettings.DisableFemaleVoiceBarkAfterExtract.Value = state)
            );
        }

        private void DrawInterruptVoiceClipImmediatelyUponGagChange() {
            Subtitle(InterruptVoiceClipImmediatelyUponGagChangeSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.InterruptVoiceClipImmediatelyUponGagChange.Value == state,
                    () => Ash.PersistentSettings.InterruptVoiceClipImmediatelyUponGagChange.Value = state)
            );
        }

        private void DrawFixIncorrectShowMouthLiquidState() {
            Subtitle(FixIncorrectShowMouthLiquidStateSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.FixIncorrectShowMouthLiquidState.Value == state,
                    () => Ash.PersistentSettings.FixIncorrectShowMouthLiquidState.Value = state)
            );
        }

        private void DrawDirtyTalkSettings() {
            var minDelayInt = Ash.PersistentSettings.DirtyTalkMinValue.Value;
            var maxDelayInt = Ash.PersistentSettings.DirtyTalkMaxValue.Value;

            var subtitle = minDelayInt == maxDelayInt
                ? minDelayInt == 0
                    ? "Dirty Talk auto-trigger is disabled"
                    : $"Dirty Talk will auto-trigger every {minDelayInt} seconds"
                : $"Dirty Talk will trigger randomly every {minDelayInt} - {maxDelayInt} seconds";
            Subtitle(subtitle);

            GUILayout.Space(12);

            GUILayout.Label($"Min delay {minDelayInt} seconds");

            var minVal = GUILayout.HorizontalSlider(minDelayInt, 0, 360);
            minDelayInt = Mathf.RoundToInt(minVal);

            maxDelayInt = Math.Max(minDelayInt, maxDelayInt);

            GUILayout.Space(12);

            GUILayout.Label($"Max delay {maxDelayInt} seconds");

            var maxVal = GUILayout.HorizontalSlider(maxDelayInt, 0, 360);
            maxDelayInt = Mathf.RoundToInt(maxVal);

            minDelayInt = Math.Min(minDelayInt, maxDelayInt);

            Ash.PersistentSettings.DirtyTalkMinValue.Value = minDelayInt;
            Ash.PersistentSettings.DirtyTalkMaxValue.Value = maxDelayInt;
        }

        private void DrawDisableFemaleSpermDripAfterExtract() {
            Subtitle(DisableFemaleSpermDripAfterExtractSubtitle);
            Flow(
                new[] { true, false },
                (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    Ash.PersistentSettings.DisableFemaleSpermDripAfterExtract.Value == state,
                    () => Ash.PersistentSettings.DisableFemaleSpermDripAfterExtract.Value = state)
            );
        }

        private void DrawFemaleSpermDripPsSettings() {
            var particleSystemStartDelayMinValue = Ash.PersistentSettings.ParticleSystemStartDelayMinValue.Value;
            var particleSystemStartDelayMaxValue = Ash.PersistentSettings.ParticleSystemStartDelayMaxValue.Value;
            var particleSystemGravityModifierValue = Ash.PersistentSettings.ParticleSystemGravityModifierValue.Value;
            var particleSystemStartSizeMultiplierValue = Ash.PersistentSettings.ParticleSystemStartSizeMultiplierValue.Value;
            var particleSystemRateOverTimeMultiplierMinValue = Ash.PersistentSettings.ParticleSystemRateOverTimeMultiplierMinValue.Value;
            var particleSystemRateOverTimeMultiplierMaxValue = Ash.PersistentSettings.ParticleSystemRateOverTimeMultiplierMaxValue.Value;

            GUILayout.Label($"Particle spawn min start delay {particleSystemStartDelayMinValue:F2} seconds");
            particleSystemStartDelayMinValue = GUILayout.HorizontalSlider(particleSystemStartDelayMinValue, 0, 20);
            particleSystemStartDelayMaxValue = Mathf.Max(particleSystemStartDelayMinValue, particleSystemStartDelayMaxValue);

            GUILayout.Space(12);
            GUILayout.Label($"Particle spawn max start delay {particleSystemStartDelayMaxValue:F2} seconds");
            particleSystemStartDelayMaxValue = GUILayout.HorizontalSlider(particleSystemStartDelayMaxValue, 0, 20);
            particleSystemStartDelayMinValue = Mathf.Min(particleSystemStartDelayMinValue, particleSystemStartDelayMaxValue);

            GUILayout.Space(12);
            GUILayout.Label($"Particle gravity modifier {particleSystemGravityModifierValue:F2}");
            particleSystemGravityModifierValue = GUILayout.HorizontalSlider(particleSystemGravityModifierValue, 0, 5);

            GUILayout.Space(12);
            GUILayout.Label($"Particle start size multiplier {particleSystemStartSizeMultiplierValue:F4}");
            particleSystemStartSizeMultiplierValue = GUILayout.HorizontalSlider(particleSystemStartSizeMultiplierValue, 0, 1);

            GUILayout.Space(12);
            GUILayout.Label($"Particle min spawn rate multiplier {particleSystemRateOverTimeMultiplierMinValue:F1}");
            particleSystemRateOverTimeMultiplierMinValue = GUILayout.HorizontalSlider(particleSystemRateOverTimeMultiplierMinValue, 1, 50);
            particleSystemRateOverTimeMultiplierMaxValue = Mathf.Max(particleSystemRateOverTimeMultiplierMinValue, particleSystemRateOverTimeMultiplierMaxValue);

            GUILayout.Space(12);
            GUILayout.Label($"Particle min spawn rate multiplier {particleSystemRateOverTimeMultiplierMaxValue:F1}");
            particleSystemRateOverTimeMultiplierMaxValue = GUILayout.HorizontalSlider(particleSystemRateOverTimeMultiplierMaxValue, 1, 50);
            particleSystemRateOverTimeMultiplierMinValue = Mathf.Min(particleSystemRateOverTimeMultiplierMinValue, particleSystemRateOverTimeMultiplierMaxValue);

            GUILayout.Space(12);
            GUILayout.Label("Collision simulation quality");
            Flow(
                (ParticleSystemCollisionQuality[])Enum.GetValues(typeof(ParticleSystemCollisionQuality)),
                (state, idx) => RadioButton(state.ToString(),
                    Ash.PersistentSettings.ParticleSystemCollisionQuality.Value == (int)state,
                    () => Ash.PersistentSettings.ParticleSystemCollisionQuality.Value = (int)state)
            );

            Ash.PersistentSettings.ParticleSystemStartDelayMinValue.Value = particleSystemStartDelayMinValue;
            Ash.PersistentSettings.ParticleSystemStartDelayMaxValue.Value = particleSystemStartDelayMaxValue;
            Ash.PersistentSettings.ParticleSystemGravityModifierValue.Value = particleSystemGravityModifierValue;
            Ash.PersistentSettings.ParticleSystemStartSizeMultiplierValue.Value = particleSystemStartSizeMultiplierValue;
            Ash.PersistentSettings.ParticleSystemRateOverTimeMultiplierMinValue.Value = particleSystemRateOverTimeMultiplierMinValue;
            Ash.PersistentSettings.ParticleSystemRateOverTimeMultiplierMaxValue.Value = particleSystemRateOverTimeMultiplierMaxValue;

            var female = GetActiveFemale();
            if (female == null) {
                Ash.Logger.LogWarning("Female is null");
                Ash.Logger.LogWarning(Environment.StackTrace);
                return;
            }

            FemaleSelectionComponent.Component(female, SetActiveFemale);

            GUILayout.Space(12);
            Button(
                "Spawn particles",
                () => {
                    female.dripParticleVagina.Clear();
                    ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(female.dripParticleVagina);
                    female.dripParticleVagina.Play();
                }
            );
        }

        private Female GetActiveFemale() {
            switch (WindowManager.Window) {
                case HSceneWindow hSceneWindow:
                    return hSceneWindow.GetActiveFemale();
                default:
                    Ash.Logger.LogError($"View HSceneSettingsView is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return null;
            }
        }

        private void SetActiveFemale(Female female) {
            switch (WindowManager.Window) {
                case HSceneWindow hSceneWindow:
                    hSceneWindow.SetActiveFemale(female);
                    break;
                default:
                    Ash.Logger.LogError($"View HSceneSettingsView is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return;
            }
        }
    }
}
