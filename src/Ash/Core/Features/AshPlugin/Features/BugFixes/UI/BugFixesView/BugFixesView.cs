using Ash.Core.Features.AshPlugin.Features.BugFixes.Hooks._Female;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using Valve.VR.InteractionSystem;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.BugFixes.UI.BugFixesView;

internal class BugFixesView
{
    internal const string BugFixesViewTabLabel = "Bug Fixes";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.BugFixes);

    private const string BugFixesTitle = "Bug Fixes";


    private const string InterruptVoiceClipImmediatelyUponGagChangeSubtitle = "Update female voice clip immediately upon gag insert/remove:";
    private const string FixIncorrectShowMouthLiquidStateSubtitle = "Fix ShowMouthLiquid animation:";
    private const string FixTearsShaderColorBindingSubtitle = "Fix tears shader color property binding:";
    private const string FixVirginBloodZFightingSubtitle = "Fix virgin blood z-fighting:";
    private const string FixSpermZFightingSubtitle = "Fix sperm z-fighting:";
    private const string FixFlushZFightingSubtitle = "Fix flush z-fighting:";

    private Vector2 ScrollPosition;

    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal void DrawView() {
        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
            DrawBugFixesSection();
        }

        GUILayout.EndScrollView();
    }

    private void DrawBugFixesSection() {
        GUILayout.Space(20);

        Title(BugFixesTitle);

        var drawCalls = new[] {
            DrawInterruptVoiceClipImmediatelyUponGagChange,
            DrawFixIncorrectShowMouthLiquidState,
            DrawFixTearsShaderColorBinding,
            DrawFixVirginBloodZFighting,
            DrawFixSpermZFighting,
            DrawFixFlushZFighting,
        };

        drawCalls.ForEach(dc => {
            dc?.Invoke();
            GUILayout.Space(12);
        });
    }


    private void DrawInterruptVoiceClipImmediatelyUponGagChange() {
        Subtitle(InterruptVoiceClipImmediatelyUponGagChangeSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.InterruptVoiceClipImmediatelyUponGagChange.Value == state,
                () => Ash.PersistentSettings.InterruptVoiceClipImmediatelyUponGagChange.Value = state)
        );
    }

    private void DrawFixIncorrectShowMouthLiquidState() {
        Subtitle(FixIncorrectShowMouthLiquidStateSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.FixIncorrectShowMouthLiquidState.Value == state,
                () => Ash.PersistentSettings.FixIncorrectShowMouthLiquidState.Value = state)
        );
    }

    private void DrawFixTearsShaderColorBinding() {
        Subtitle(FixTearsShaderColorBindingSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.FixTearsShaderColorBinding.Value == state,
                () => Ash.PersistentSettings.FixTearsShaderColorBinding.Value = state)
        );

        if (!Ash.PersistentSettings.FixTearsShaderColorBinding.Value)
            return;

        GUILayout.Label($"Tears max alpha {Ash.PersistentSettings.TearsAlphaInterpolationMax.Value:F6}");
        Ash.PersistentSettings.TearsAlphaInterpolationMax.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.TearsAlphaInterpolationMax.Value, 0, 1);
    }

    private void DrawFixVirginBloodZFighting() {
        Subtitle(FixVirginBloodZFightingSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.FixVirginBloodZFighting.Value == state,
                () => {
                    Ash.PersistentSettings.FixVirginBloodZFighting.Value = state;
                    if (state) FemaleHooks.ReplaceVirginBloodMaterialForAllFemales();
                    else FemaleHooks.ResetVirginBloodMaterialForAllFemales();
                })
        );
    }

    private void DrawFixSpermZFighting() {
        Subtitle(FixSpermZFightingSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.FixSpermZFighting.Value == state,
                () => {
                    Ash.PersistentSettings.FixSpermZFighting.Value = state;
                    if (state) FemaleHooks.ReplaceSpermMaterialsForAllFemales();
                    else FemaleHooks.ResetSpermMaterialsForAllFemales();
                })
        );
    }

    private void DrawFixFlushZFighting() {
        Subtitle(FixFlushZFightingSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.FixFlushZFighting.Value == state,
                () => {
                    Ash.PersistentSettings.FixFlushZFighting.Value = state;
                    if (state) FemaleHooks.ReplaceFlushMaterialForAllFemales();
                    else FemaleHooks.ResetFlushMaterialForAllFemales();
                })
        );
    }
}
