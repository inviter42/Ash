using System;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.AhegaoFaces.UI.AhegaoFacesView;

public class AhegaoFacesView
{
    internal const string AhegaoFacesViewTabLabel = "Ahegao Faces";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.HSceneSettings);

    private const string AhegaoFaceSettingsTitle = "Ahegao Face settings";
    private const string AhegaoManualTriggerSettingsTitle = "Ahegao face manual settings";

    private const string EnableAhegaoFacesSubtitle = "Enable ahegao faces:";


    private Vector2 ScrollPosition;

    internal AhegaoFacesView() { }

    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal void DrawView() {
        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
            DrawAhegaoFaceSettingsSection();
        }

        GUILayout.EndScrollView();
    }


    private void DrawAhegaoFaceSettingsSection() {
        GUILayout.Space(20);

        Title(AhegaoFaceSettingsTitle);

        DrawAhegaoFaceSettings();

        GUILayout.Space(20);

        DrawAhegaoManualTriggerSettings();
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
        if (!Ash.PersistentSettings.AhegaoFacesEnabled.Value)
            return;

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
}
