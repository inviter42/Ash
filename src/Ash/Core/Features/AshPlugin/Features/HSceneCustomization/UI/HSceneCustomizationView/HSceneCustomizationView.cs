using System;
using Ash.Core.Features.AshPlugin.Common.Components;
using Ash.Core.Features.AshPlugin.Main.Types;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using Valve.VR.InteractionSystem;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.HSceneCustomization.UI.HSceneCustomizationView;

internal class HSceneCustomizationView
{
    internal const string HSceneSettingsViewTabLabel = "H-Scene Customization";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.HSceneSettings);

    private const string AnimationControlsTitle = "Animation settings";
    private const string ParticleSystemsControlsTitle = "Particle systems settings";
    private const string VoiceControlsTitle = "Voice settings";
    private const string DirtyTalkTriggerSettingsTitle = "Dirty Talk settings";
    private const string AhegaoFaceSettingsTitle = "Ahegao Face settings";
    private const string AhegaoManualTriggerSettingsTitle = "Ahegao face manual settings";
    private const string SceneInteractivityTitle = "Scene Interactivity settings";

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
    private const string EnableAhegaoFacesSubtitle = "Enable ahegao faces:";
    private const string EnableFemaleLarTargetMouseControlSubtitle = "Enable changing LookAtRotator targets using mouse:";


    private readonly HSceneWindow ParentWindow;
    private Female ActiveFemale => ParentWindow.ActiveFemale;

    private Vector2 ScrollPosition;

    internal HSceneCustomizationView(HSceneWindow parentWindow) {
        ParentWindow = parentWindow;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal void DrawView() {
        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
            DrawAnimationControlsSection();
            DrawParticleSystemsSection();
            DrawVoiceControlsSection();
            DrawDirtyTalkSettingsSection();
            DrawAhegaoFaceSettingsSection();
            DrawLarTargetMouseControlSection();
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

        var drawCalls = new[] {
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

    private void DrawDirtyTalkSettingsSection() {
        GUILayout.Space(20);

        Title(DirtyTalkTriggerSettingsTitle);

        DrawDirtyTalkSettings();
    }

    private void DrawAhegaoFaceSettingsSection() {
        GUILayout.Space(20);

        Title(AhegaoFaceSettingsTitle);

        DrawAhegaoFaceSettings();

        GUILayout.Space(20);

        DrawAhegaoManualTriggerSettings();
    }

    private void DrawLarTargetMouseControlSection() {
        GUILayout.Space(20);

        Title(SceneInteractivityTitle);

        DrawSceneInteractivitySettings();
    }


    private void DrawDisableFemaleSpermDripAfterExtract() {
        Subtitle(DisableFemaleSpermDripAfterExtractSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
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
            (state, _) => RadioButton(state.ToString(),
                Ash.PersistentSettings.ParticleSystemCollisionQuality.Value == (int)state,
                () => Ash.PersistentSettings.ParticleSystemCollisionQuality.Value = (int)state)
        );

        Ash.PersistentSettings.ParticleSystemStartDelayMinValue.Value = particleSystemStartDelayMinValue;
        Ash.PersistentSettings.ParticleSystemStartDelayMaxValue.Value = particleSystemStartDelayMaxValue;
        Ash.PersistentSettings.ParticleSystemGravityModifierValue.Value = particleSystemGravityModifierValue;
        Ash.PersistentSettings.ParticleSystemStartSizeMultiplierValue.Value = particleSystemStartSizeMultiplierValue;
        Ash.PersistentSettings.ParticleSystemRateOverTimeMultiplierMinValue.Value = particleSystemRateOverTimeMultiplierMinValue;
        Ash.PersistentSettings.ParticleSystemRateOverTimeMultiplierMaxValue.Value = particleSystemRateOverTimeMultiplierMaxValue;

        if (ActiveFemale == null) {
            Logger.LogWarning("Female is null");
            Logger.LogWarning(Environment.StackTrace);
            return;
        }

        FemaleSelectionComponent.Component(ActiveFemale, female => ParentWindow.ActiveFemale = female);

        GUILayout.Space(12);
        Button(
            "Spawn particles",
            () => {
                ActiveFemale.dripParticleVagina.Clear();
                ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(ActiveFemale.dripParticleVagina);
                ActiveFemale.dripParticleVagina.Play();
            }
        );
    }

    private void DrawMuteBackgroundFemale() {
        Subtitle(MuteBackgroundFemaleSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
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
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.DisableFemaleAutoEjaculation.Value == state,
                () => Ash.PersistentSettings.DisableFemaleAutoEjaculation.Value = state)
        );
    }

    private void DrawSkipSpurtState() {
        Subtitle(SkipSpurtStateSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.SkippingSpurtStateEnabled.Value == state,
                () => Ash.PersistentSettings.SkippingSpurtStateEnabled.Value = state)
        );
    }

    private void DrawDisableFemaleHVoiceBark() {
        Subtitle(DisableFemaleHVoiceBarkSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.DisableFemaleHVoiceBark.Value == state,
                () => Ash.PersistentSettings.DisableFemaleHVoiceBark.Value = state)
        );
    }

    private void DrawDisableFemaleInactionBark() {
        Subtitle(DisableFemaleInactionBarkSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.DisableFemaleInactionBark.Value == state,
                () => Ash.PersistentSettings.DisableFemaleInactionBark.Value = state)
        );
    }

    private void DrawDisableFemaleVoiceBarkAtSceneStart() {
        Subtitle(DisableFemaleVoiceBarkAtSceneStartSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneStart.Value == state,
                () => Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneStart.Value = state)
        );
    }

    private void DrawDisableFemaleVoiceBarkAtSceneEnd() {
        Subtitle(DisableFemaleVoiceBarkAtSceneEndSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneEnd.Value == state,
                () => Ash.PersistentSettings.DisableFemaleVoiceBarkAtSceneEnd.Value = state)
        );
    }

    private void DrawDisableFemaleVoiceBarkAfterEjaIn() {
        Subtitle(DisableFemaleVoiceBarkAfterEjaInSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.DisableFemaleVoiceBarkAfterEjaIn.Value == state,
                () => Ash.PersistentSettings.DisableFemaleVoiceBarkAfterEjaIn.Value = state)
        );
    }

    private void DrawDisableFemaleVoiceBarkAfterExtract() {
        Subtitle(DisableFemaleVoiceBarkAfterExtractSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.DisableFemaleVoiceBarkAfterExtract.Value == state,
                () => Ash.PersistentSettings.DisableFemaleVoiceBarkAfterExtract.Value = state)
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

    private void DrawAhegaoFaceSettings() {
        Subtitle(EnableAhegaoFacesSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.AhegaoFacesEnabled.Value == state,
                () => Ash.PersistentSettings.AhegaoFacesEnabled.Value = state)
        );

        if (!Ash.PersistentSettings.AhegaoFacesEnabled.Value)
            return;

        var weightsDecay = Ash.PersistentSettings.AhegaoDecayValue.Value;
        var weightsDecayPStr = $"{weightsDecay * 100:F0}";
        Subtitle($"Weights decay: {weightsDecayPStr}%");
        Info($"ⓘ Lowers the probability of the randomly selected ahegao expression to appear again by {weightsDecayPStr}%. The probability is gradually restored with each new trigger event.");
        Ash.PersistentSettings.AhegaoDecayValue.Value = GUILayout.HorizontalSlider(weightsDecay, 0, 0.999f);

        var flushAppearance = Ash.PersistentSettings.AhegaoFlushAppearanceRateValue.Value;
        Subtitle($"Flush appearance rate: {flushAppearance:F4}");
        Info("ⓘ Rate at which cheek blush is added over time.");
        Ash.PersistentSettings.AhegaoFlushAppearanceRateValue.Value = GUILayout.HorizontalSlider(flushAppearance, 0.0001f, 0.1f);

        var flushDecay = Ash.PersistentSettings.AhegaoFlushDecayRateValue.Value;
        Subtitle($"Flush decay rate: {flushDecay:F4}");
        Info("ⓘ Rate at which cheek blush is dissipating over time.");
        Ash.PersistentSettings.AhegaoFlushDecayRateValue.Value = GUILayout.HorizontalSlider(flushDecay, 0.0001f, 0.01f);

        var flushMaxCap = Ash.PersistentSettings.AhegaoFlushValueCapValue.Value;
        Subtitle($"Flush top cap: {flushMaxCap:F2}");
        Info("ⓘ Maximum possible value during ahegao, above which flush won't be added further. Doesn't affect flush outside ahegao.");
        Ash.PersistentSettings.AhegaoFlushValueCapValue.Value = GUILayout.HorizontalSlider(flushMaxCap, 0f, 1f);

        var triggerBaseProbability = Ash.PersistentSettings.AhegaoClimaxTriggerBaseProbabilityValue.Value;
        Subtitle($"Trigger base probability: {triggerBaseProbability * 100:F0}%");
        Info("ⓘ Starting probability to trigger ahegao at climax.");
        Ash.PersistentSettings.AhegaoClimaxTriggerBaseProbabilityValue.Value = GUILayout.HorizontalSlider(triggerBaseProbability, 0f, 1f);

        var triggerProbabilityIncrement = Ash.PersistentSettings.AhegaoClimaxTriggerProbabilityIncrementValue.Value;
        Subtitle($"Trigger probability increment: {triggerProbabilityIncrement * 100:F0}%");
        Info("ⓘ Value by which ahegao trigger probability will be increased in it failed to trigger.");
        Ash.PersistentSettings.AhegaoClimaxTriggerProbabilityIncrementValue.Value = GUILayout.HorizontalSlider(triggerProbabilityIncrement, 0f, 1f);
    }

    private void DrawAhegaoManualTriggerSettings() {
        Title(AhegaoManualTriggerSettingsTitle);

        var minDurationFloat = Ash.PersistentSettings.AhegaoMinDurationValue.Value;
        var maxDurationFloat = Ash.PersistentSettings.AhegaoMaxDurationValue.Value;

        var ahegaoDurationSubtitle = (Mathf.Approximately(minDurationFloat, maxDurationFloat)
                                         ? $"Ahegao face will last for {minDurationFloat:F2} seconds"
                                         : $"Ahegao face will last for {minDurationFloat:F2} - {maxDurationFloat:F2} seconds")
                                     + ", when triggered manually.";

        Subtitle(ahegaoDurationSubtitle);

        GUILayout.Space(12);

        GUILayout.Label($"Min duration {minDurationFloat:F2} seconds");
        Info("ⓘ Applies only to manually triggered action.");

        minDurationFloat = GUILayout.HorizontalSlider(minDurationFloat, 1, 20);

        maxDurationFloat = Math.Max(minDurationFloat, maxDurationFloat);

        GUILayout.Space(12);

        GUILayout.Label($"Max duration {maxDurationFloat:F2} seconds");
        Info("ⓘ Applies only to manually triggered action.");

        maxDurationFloat = GUILayout.HorizontalSlider(maxDurationFloat, 1, 20);

        minDurationFloat = Math.Min(minDurationFloat, maxDurationFloat);

        Ash.PersistentSettings.AhegaoMinDurationValue.Value = minDurationFloat;
        Ash.PersistentSettings.AhegaoMaxDurationValue.Value = maxDurationFloat;
    }

    private void DrawSceneInteractivitySettings() {
        Subtitle(EnableFemaleLarTargetMouseControlSubtitle);
        Info("ⓘ Double click on Female to bring/dismiss her attention. Click and hold to freeze/reset neck and eyes position.");
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.EnableFemaleLarTargetMouseControl.Value == state,
                () => Ash.PersistentSettings.EnableFemaleLarTargetMouseControl.Value = state)
        );
    }
}
