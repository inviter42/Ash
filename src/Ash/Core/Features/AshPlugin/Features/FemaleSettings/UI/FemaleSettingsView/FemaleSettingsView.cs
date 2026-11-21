using System;
using Ash.Core.Features.AshPlugin.Common.Components;
using Ash.Core.Features.AshPlugin.Features.FemaleSettings.Hooks._Female;
using Ash.Core.Features.AshPlugin.Main.Types;
using Ash.Core.Features.AshPlugin.Settings;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.FemaleSettings.UI.FemaleSettingsView;

internal class FemaleSettingsView
{
    internal const string FemaleSettingsViewTabLabel = "Female Settings";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.FemaleSettings);

    private const string LookAtRotatorTitle = "LookAtRotator settings";
    private const string ParticleSystemsControlsTitle = "Particle systems settings";

    private const string ApplyQuaternionCalculationForLarLabel = "Use quaternion LookAtRotator calculation:";
    private const string NeckRotatorOverridesSubtitle = "Neck LookAtRotator overrides:";
    private const string EyesRotatorOverridesSubtitle = "Eyes LookAtRotator overrides:";

    private Female ActiveFemale => ParentWindow.ActiveFemale;

    private readonly HSceneWindow ParentWindow;

    private Vector2 ScrollPosition;

    internal FemaleSettingsView(HSceneWindow parentWindow) {
        ParentWindow = parentWindow;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal void DrawView() {
        if (ActiveFemale == null) {
            GUILayout.Label("No active female.");
            return;
        }

        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
            DrawLookAtRotatorSettingsSection();
            DrawParticleSystemsSection();
        }

        GUILayout.EndScrollView();
    }


    /// <summary>
    /// LAR settings
    /// </summary>
    private void DrawLookAtRotatorSettingsSection() {
            GUILayout.Space(20);

            Title(LookAtRotatorTitle);

            DrawApplyQuaternionCalculationForLar();

            GUILayout.Space(12);

            DrawNeckRotatorOverrides();

            GUILayout.Space(12);

            DrawEyesRotatorOverrides();
        }

    private void DrawApplyQuaternionCalculationForLar() {
        Subtitle(ApplyQuaternionCalculationForLarLabel);
        Info("ⓘ Improves target following for neck and eyes for higher rotator values.");
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.ApplyNewLarCalculationLogic.Value == state,
                () => Ash.PersistentSettings.ApplyNewLarCalculationLogic.Value = state)
        );
    }

    private void DrawNeckRotatorOverrides() {
        Subtitle(NeckRotatorOverridesSubtitle);
        Info("ⓘ Overrides values for how far neck can turn. Recommended to use with Quaternion LookAtRotator calculation.");
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.OverrideNeckRotatorSettings.Value == state,
                () => {
                    if (state) FemaleHooks.InitNeckLarForAllFemales();
                    else FemaleHooks.ResetNeckLarForAllFemales();

                    Ash.PersistentSettings.OverrideNeckRotatorSettings.Value = state;
                })
        );

        if (!Ash.PersistentSettings.OverrideNeckRotatorSettings.Value)
            return;

        var neckRotatorMaxUpValue = Ash.PersistentSettings.NeckRotatorMaxUpValue.Value;
        GUILayout.Label($"Neck max up {neckRotatorMaxUpValue:F2}");
        Ash.PersistentSettings.NeckRotatorMaxUpValue.Value = GUILayout.HorizontalSlider(neckRotatorMaxUpValue, 0f, 180f);

        var neckRotatorMaxDownValue = Ash.PersistentSettings.NeckRotatorMaxDownValue.Value;
        GUILayout.Label($"Neck max down {neckRotatorMaxDownValue:F2}");
        Ash.PersistentSettings.NeckRotatorMaxDownValue.Value = GUILayout.HorizontalSlider(neckRotatorMaxDownValue, 0f, 180f);

        var neckBoneMaxYawMagnitudeValue = Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value;
        GUILayout.Label($"Neck yaw magnitude {neckBoneMaxYawMagnitudeValue:F2}");
        Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value = GUILayout.HorizontalSlider(neckBoneMaxYawMagnitudeValue, 0f, 180f);

        var headBoneMaxYawMagnitudeValue = Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value;
        GUILayout.Label($"Head yaw magnitude {headBoneMaxYawMagnitudeValue:F2}");
        Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value = GUILayout.HorizontalSlider(headBoneMaxYawMagnitudeValue, 0f, 180f);

        var dirty = !Mathf.Approximately(Ash.PersistentSettings.NeckRotatorMaxUpValue.Value, neckRotatorMaxUpValue)
                    || !Mathf.Approximately(Ash.PersistentSettings.NeckRotatorMaxDownValue.Value, neckRotatorMaxDownValue)
                    || !Mathf.Approximately(Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value, neckBoneMaxYawMagnitudeValue)
                    || !Mathf.Approximately(Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value, headBoneMaxYawMagnitudeValue);

        if (dirty)
            FemaleHooks.InitNeckLarForAllFemales();
    }

    private void DrawEyesRotatorOverrides() {
        Subtitle(EyesRotatorOverridesSubtitle);
        Info("ⓘ Overrides values for how far eyes can turn. Recommended to use with Quaternion LookAtRotator calculation.");
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.OverrideEyesRotatorSettings.Value == state,
                () => {
                    if (state) FemaleHooks.InitEyesLarForAllFemales();
                    else FemaleHooks.ResetEyesLarForAllFemales();

                    Ash.PersistentSettings.OverrideEyesRotatorSettings.Value = state;
                })
        );

        if (!Ash.PersistentSettings.OverrideEyesRotatorSettings.Value)
            return;

        var eyesRotatorMaxUpValue = Ash.PersistentSettings.EyesRotatorMaxUpValue.Value;
        GUILayout.Label($"Eyes max up {eyesRotatorMaxUpValue:F2}");
        Ash.PersistentSettings.EyesRotatorMaxUpValue.Value = GUILayout.HorizontalSlider(eyesRotatorMaxUpValue, 0f, 180f);

        var eyesRotatorMaxDownValue = Ash.PersistentSettings.EyesRotatorMaxDownValue.Value;
        GUILayout.Label($"Eyes max down {eyesRotatorMaxDownValue:F2}");
        Ash.PersistentSettings.EyesRotatorMaxDownValue.Value = GUILayout.HorizontalSlider(eyesRotatorMaxDownValue, 0f, 180f);


        var eyesBoneMaxPositiveYawValue = Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value;
        GUILayout.Label($"Eyes max positive yaw {eyesBoneMaxPositiveYawValue:F2}");
        Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value = GUILayout.HorizontalSlider(eyesBoneMaxPositiveYawValue, 0f, 180f);

        var eyesBoneMaxNegativeYawValue = Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value;
        GUILayout.Label($"Eyes max negative yaw {eyesBoneMaxNegativeYawValue:F2}");
        Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value = GUILayout.HorizontalSlider(eyesBoneMaxNegativeYawValue, 0f, 180f);


        var dirty = !Mathf.Approximately(Ash.PersistentSettings.EyesRotatorMaxUpValue.Value, eyesRotatorMaxUpValue)
                    || !Mathf.Approximately(Ash.PersistentSettings.EyesRotatorMaxDownValue.Value, eyesRotatorMaxDownValue)
                    || !Mathf.Approximately(Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value, eyesBoneMaxPositiveYawValue)
                    || !Mathf.Approximately(Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value, eyesBoneMaxNegativeYawValue);

        if (!dirty)
            return;

        FemaleHooks.InitEyesLarForAllFemales();
    }


    /// <summary>
    /// Particle systems settings
    /// </summary>
    private void DrawParticleSystemsSection() {
        GUILayout.Space(20);

        Title(ParticleSystemsControlsTitle);

        DrawFemaleSpermDripPsSettings();
    }

    private static bool DrawMainPs = true;
    private static string PsTarget = "Vagina";

    private void DrawFemaleSpermDripPsSettings() {
        Subtitle("Choose particle system:");
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(state ? "Main Particle System" : "Sub Emitters",
                DrawMainPs == state,
                () => DrawMainPs = state)
        );

        GUILayout.Space(12);

        if (DrawMainPs) {
            DrawParticleSystemSettings(
                Ash.PersistentSettings.PsSimulationSpeedValue,
                Ash.PersistentSettings.PsStartDelayMinValue,
                Ash.PersistentSettings.PsStartDelayMaxValue,
                Ash.PersistentSettings.PsGravityModifierValue,
                Ash.PersistentSettings.PsStartSizeMultiplierValue
            );

            DrawEmissionModuleSettings(
                Ash.PersistentSettings.PsRateOverTimeMultiplierMinValue,
                Ash.PersistentSettings.PsRateOverTimeMultiplierMaxValue
            );

            Flow(
                new[] { "Recommended", "Reset to default" },
                (state, index) => Button(state,
                    () => {
                        switch (index) {
                            case 0: {
                                Ash.PersistentSettings.PsSimulationSpeedValue.Value = Ash.PersistentSettings.PsSimulationSpeedValue.Recommended;
                                Ash.PersistentSettings.PsStartDelayMinValue.Value = Ash.PersistentSettings.PsStartDelayMinValue.Recommended;
                                Ash.PersistentSettings.PsStartDelayMaxValue.Value = Ash.PersistentSettings.PsStartDelayMaxValue.Recommended;
                                Ash.PersistentSettings.PsGravityModifierValue.Value = Ash.PersistentSettings.PsGravityModifierValue.Recommended;
                                Ash.PersistentSettings.PsStartSizeMultiplierValue.Value = Ash.PersistentSettings.PsStartSizeMultiplierValue.Recommended;
                                Ash.PersistentSettings.PsRateOverTimeMultiplierMinValue.Value = Ash.PersistentSettings.PsRateOverTimeMultiplierMinValue.Recommended;
                                Ash.PersistentSettings.PsRateOverTimeMultiplierMaxValue.Value = Ash.PersistentSettings.PsRateOverTimeMultiplierMaxValue.Recommended;

                                break;
                            }
                            case 1: {
                                Ash.PersistentSettings.PsSimulationSpeedValue.Value = Ash.PersistentSettings.PsSimulationSpeedValue.Default;
                                Ash.PersistentSettings.PsStartDelayMinValue.Value = Ash.PersistentSettings.PsStartDelayMinValue.Default;
                                Ash.PersistentSettings.PsStartDelayMaxValue.Value = Ash.PersistentSettings.PsStartDelayMaxValue.Default;
                                Ash.PersistentSettings.PsGravityModifierValue.Value = Ash.PersistentSettings.PsGravityModifierValue.Default;
                                Ash.PersistentSettings.PsStartSizeMultiplierValue.Value = Ash.PersistentSettings.PsStartSizeMultiplierValue.Default;
                                Ash.PersistentSettings.PsRateOverTimeMultiplierMinValue.Value = Ash.PersistentSettings.PsRateOverTimeMultiplierMinValue.Default;
                                Ash.PersistentSettings.PsRateOverTimeMultiplierMaxValue.Value = Ash.PersistentSettings.PsRateOverTimeMultiplierMaxValue.Default;

                                break;
                            }
                        }
                    })
            );
        } else {
            DrawParticleSystemSettings(Ash.PersistentSettings.SeStartSizeMultiplierValue);

            DrawEmissionModuleSettings(Ash.PersistentSettings.SeEmissionBurstCountValue);

            Flow(
                new[] { "Recommended", "Reset to default" },
                (state, index) => Button(state,
                    () => {
                        switch (index) {
                            case 0: {
                                Ash.PersistentSettings.SeStartSizeMultiplierValue.Value = Ash.PersistentSettings.SeStartSizeMultiplierValue.Recommended;
                                Ash.PersistentSettings.SeEmissionBurstCountValue.Value = Ash.PersistentSettings.SeEmissionBurstCountValue.Recommended;

                                break;
                            }
                            case 1: {
                                Ash.PersistentSettings.SeStartSizeMultiplierValue.Value = Ash.PersistentSettings.SeStartSizeMultiplierValue.Default;
                                Ash.PersistentSettings.SeEmissionBurstCountValue.Value = Ash.PersistentSettings.SeEmissionBurstCountValue.Default;

                                break;
                            }
                        }
                    })
            );
        }

        GUILayout.Space(20);

        Title("Settings testing");

        FemaleSelectionComponent.Component(ActiveFemale, female => ParentWindow.ActiveFemale = female);

        GUILayout.Space(12);

        Subtitle("Spawn from:");
        Flow(
            new[] { "Vagina", "Anus", "Mouth" },
            (state, _) => RadioButton(state,
                PsTarget == state,
                () => PsTarget = state)
        );

        GUILayout.Space(12);

        Button(
            "Spawn particles",
            () => {
                if (ActiveFemale == null)
                    return;

                switch (PsTarget) {
                    case "Vagina": {
                        ActiveFemale.dripParticleVagina.Clear();
                        ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(ActiveFemale.dripParticleVagina);
                        ActiveFemale.dripParticleVagina.Play();

                        break;
                    }
                    case "Anus": {
                        ActiveFemale.dripParticleAnus.Clear();
                        ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(ActiveFemale.dripParticleAnus);
                        ActiveFemale.dripParticleAnus.Play();
                        break;
                    }
                    case "Mouth": {
                        ActiveFemale.dripParticleMouth.Clear();
                        ParticleSystemsUtils.AdjustFemaleSpermDripParticleSystemSettings(ActiveFemale.dripParticleMouth);
                        ActiveFemale.dripParticleMouth.Play();
                        break;
                    }
                }
            }
        );
    }

    private void DrawParticleSystemSettings(
        Setting<float> simulationSpeed,
        Setting<float> startDelayMinValue,
        Setting<float> startDelayMaxValue,
        Setting<float> gravityModifierValue,
        Setting<float> startSizeMultiplierValue
    ) {
        DrawSimulationSpeed(simulationSpeed);

        GUILayout.Space(12);
        DrawSpawnMinStartDelay(startDelayMinValue, startDelayMaxValue);

        GUILayout.Space(12);
        DrawSpawnMaxStartDelay(startDelayMinValue, startDelayMaxValue);

        GUILayout.Space(12);
        DrawGravityModifier(gravityModifierValue);

        GUILayout.Space(12);
        DrawStartSizeMultiplier(startSizeMultiplierValue, 0.0001f, 0.025f);

        GUILayout.Space(12);
        DrawCollisionSimulationQuality();
    }

    private void DrawParticleSystemSettings(Setting<float> startSizeMultiplierValue) {
        GUILayout.Space(12);
        DrawStartSizeMultiplier(startSizeMultiplierValue, 0.01f, 0.1f);

        GUILayout.Space(12);
        DrawCollisionSimulationQuality();
    }


    private void DrawSimulationSpeed(Setting<float> simulationSpeed) {
        GUILayout.Label($"Simulation speed {simulationSpeed.Value:F2}");
        simulationSpeed.Value = GUILayout.HorizontalSlider(simulationSpeed.Value, 0, 5);
    }

    private void DrawSpawnMinStartDelay(Setting<float> startDelayMinValue, Setting<float> startDelayMaxValue) {
        GUILayout.Label($"Spawn min start delay {startDelayMinValue.Value:F2} seconds");
        startDelayMinValue.Value = GUILayout.HorizontalSlider(startDelayMinValue.Value, 0, 20);
        startDelayMaxValue.Value = Mathf.Max(startDelayMinValue.Value, startDelayMaxValue.Value);
    }

    private void DrawSpawnMaxStartDelay(Setting<float> startDelayMinValue, Setting<float> startDelayMaxValue) {
        GUILayout.Label($"Spawn max start delay {startDelayMaxValue.Value:F2} seconds");
        startDelayMaxValue.Value = GUILayout.HorizontalSlider(startDelayMaxValue.Value, 0, 20);
        startDelayMinValue.Value = Mathf.Min(startDelayMinValue.Value, startDelayMaxValue.Value);
    }

    private void DrawGravityModifier(Setting<float> gravityModifierValue) {
        GUILayout.Label($"Gravity modifier {gravityModifierValue.Value:F2}");
        gravityModifierValue.Value = GUILayout.HorizontalSlider(gravityModifierValue.Value, 0, 5);
    }

    private void DrawStartSizeMultiplier(Setting<float> startSizeMultiplierValue, float min, float max) {
        GUILayout.Label($"Start size multiplier {startSizeMultiplierValue.Value:F4}");
        startSizeMultiplierValue.Value = GUILayout.HorizontalSlider(startSizeMultiplierValue.Value, min, max);
    }

    private void DrawCollisionSimulationQuality() {
        GUILayout.Label("Collision simulation quality");
        Flow(
            (ParticleSystemCollisionQuality[])Enum.GetValues(typeof(ParticleSystemCollisionQuality)),
            (state, _) => RadioButton(state.ToString(),
                Ash.PersistentSettings.ParticleSystemCollisionQuality.Value == (int)state,
                () => Ash.PersistentSettings.ParticleSystemCollisionQuality.Value = (int)state)
        );
    }

    private void DrawEmissionModuleSettings(Setting<float> rateOverTimeMultiplierMinValue, Setting<float> rateOverTimeMultiplierMaxValue) {
        GUILayout.Space(12);
        GUILayout.Label($"Min spawn rate multiplier {rateOverTimeMultiplierMinValue.Value:F1}");
        rateOverTimeMultiplierMinValue.Value = GUILayout.HorizontalSlider(rateOverTimeMultiplierMinValue.Value, 1f, 75f);
        rateOverTimeMultiplierMaxValue.Value = Mathf.Max(rateOverTimeMultiplierMinValue.Value, rateOverTimeMultiplierMaxValue.Value);

        GUILayout.Space(12);
        GUILayout.Label($"Max spawn rate multiplier {rateOverTimeMultiplierMaxValue.Value:F1}");
        rateOverTimeMultiplierMaxValue.Value = GUILayout.HorizontalSlider(rateOverTimeMultiplierMaxValue.Value, 1f, 75f);
        rateOverTimeMultiplierMinValue.Value = Mathf.Min(rateOverTimeMultiplierMinValue.Value, rateOverTimeMultiplierMaxValue.Value);
    }

    private void DrawEmissionModuleSettings(Setting<int> emissionBurstCount) {
        GUILayout.Space(12);
        GUILayout.Label($"Burst count {emissionBurstCount.Value}");
        emissionBurstCount.Value = Mathf.RoundToInt(GUILayout.HorizontalSlider(emissionBurstCount.Value, 1, 10));
    }
}
