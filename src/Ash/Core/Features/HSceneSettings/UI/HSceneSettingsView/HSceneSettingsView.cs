using System;
using Ash.Core.SceneManagement;
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
        private const string AnimationControlsTitle = "Animation controls";
        private const string VoiceControlsTitle = "Voice controls";

        private const string MuteBackgroundFemaleSubtitle = "Mute background female:";
        private const string DisableFemaleAutoEjaculationSubtitle = "Disable female automatic ejaculation:";
        private const string SkipSpurtStateSubtitle = "Skip to ejaculation immediately:";
        private const string DisableFemaleHVoiceBarkSubtitle = "Disable female voice lines on H-Pose start:";
        private const string DisableFemaleInactionBarkSubtitle = "Disable female voice lines after inactivity period:";
        private const string DisableFemaleVoiceBarkAtSceneStartSubtitle = "Disable female voice lines at H-Scene start:";
        private const string DisableFemaleVoiceBarkAtSceneEndSubtitle = "Disable female voice lines at H-Scene end:";
        private const string InterruptVoiceClipImmediatelyUponGagChangeSubtitle = "Update female voice clip immediately upon gag change:";
        private const string FixIncorrectShowMouthLiquidStateSubtitle = "Fix ShowMouthLiquid animation:";

        private Vector2 ScrollPosition;

        // ReSharper disable once MemberCanBeMadeStatic.Global
        internal void DrawView() {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
                DrawAnimationControlsSection();
                DrawVoiceControlsSection();
                DrawBugFixesSection();
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

            var drawCalls = new Action[] {
                DrawDisableFemaleAutoEjaculation,
                DrawMuteBackgroundFemale,
                DrawDisableFemaleHVoiceBark,
                DrawDisableFemaleInactionBark,
                DrawDisableFemaleVoiceBarkAtSceneStart,
                DrawDisableFemaleVoiceBarkAtSceneEnd
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
    }
}
