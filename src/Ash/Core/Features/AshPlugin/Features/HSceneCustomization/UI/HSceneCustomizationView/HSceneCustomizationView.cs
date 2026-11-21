using System;
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
    internal const string HSceneCustomizationViewTabLabel = "H-Scene Customization";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.HSceneSettings);

    private const string AnimationControlsTitle = "Animation settings";
    private const string VoiceControlsTitle = "Voice settings";
    private const string DirtyTalkTriggerSettingsTitle = "Dirty Talk settings";
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
    private const string EnableFemaleLarTargetMouseControlSubtitle = "Enable changing LookAtRotator targets using mouse:";


    private Vector2 ScrollPosition;

    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal void DrawView() {
        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
            DrawAnimationControlsSection();
            DrawVoiceControlsSection();
            DrawDirtyTalkSettingsSection();
            DrawSceneInteractivitySection();
        }

        GUILayout.EndScrollView();
    }


    private void DrawAnimationControlsSection() {
        GUILayout.Space(20);

        Title(AnimationControlsTitle);

        DrawSkipSpurtState();
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

    private void DrawSceneInteractivitySection() {
        GUILayout.Space(20);

        Title(SceneInteractivityTitle);

        DrawSceneInteractivitySettings();
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
