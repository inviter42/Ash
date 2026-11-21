using System;
using Ash.Core.Features.AshPlugin.Features.MaleSettings.Hooks._RigidEmitter;
using Ash.Core.Features.AshPlugin.Main.Types;
using Ash.Logging;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;

namespace Ash.Core.Features.AshPlugin.Features.MaleSettings.UI.MaleSettingsView;

internal class MaleSettingsView
{
    internal const string MaleSettingsViewTabLabel = "Male Settings";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.MaleSettings);

    private const string RigidEmitterControlsTitle = "Rigit Emitter settings";

    private const string SelectEmittersSubtitle = "Select emitters (multiple choice):";
    private const string Pack0SettingsSubtitle = "Pack 0 settings:";
    private const string Pack1SettingsSubtitle = "Pack 1 settings:";
    private const string Pack2SettingsSubtitle = "Pack 2 settings:";

    private Male ActiveMale => ParentWindow.ActiveMale;
    private H_Scene HScene => ParentWindow.HScene;

    private readonly HSceneWindow ParentWindow;

    private Vector2 ScrollPosition;

    internal MaleSettingsView(HSceneWindow parentWindow) {
        ParentWindow = parentWindow;
    }

    internal void DrawView() {
        if (ActiveMale == null) {
            Logger.LogWarning("Male is null");
            Logger.LogWarning(Environment.StackTrace);
            return;
        }

        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
            DrawRigidEmitterSection();
        }

        GUILayout.EndScrollView();
    }

    private void DrawRigidEmitterSection() {
        Title(RigidEmitterControlsTitle);

        Subtitle(SelectEmittersSubtitle);
        Flow(
            new[] { "Pack 1", "Pack 2", "Pack 3" },
            (state, index) => RadioButton(state.ToString(),
                index switch {
                    0 => Ash.PersistentSettings.EjaculatorPack0AddToPoolValue.Value,
                    1 => Ash.PersistentSettings.EjaculatorPack1AddToPoolValue.Value,
                    2 => Ash.PersistentSettings.EjaculatorPack2AddToPoolValue.Value,
                    _ => false
                },
                () => {
                    RigidEmitterHooks.ActivePackPool.Clear();

                    switch (index) {
                        case 0:
                            Ash.PersistentSettings.EjaculatorPack0AddToPoolValue.Value = !Ash.PersistentSettings.EjaculatorPack0AddToPoolValue.Value;
                            RigidEmitterHooks.ActivePackPool.Add(0);
                            break;
                        case 1:
                            Ash.PersistentSettings.EjaculatorPack1AddToPoolValue.Value = !Ash.PersistentSettings.EjaculatorPack1AddToPoolValue.Value;
                            RigidEmitterHooks.ActivePackPool.Add(1);
                            break;
                        case 2:
                            Ash.PersistentSettings.EjaculatorPack2AddToPoolValue.Value = !Ash.PersistentSettings.EjaculatorPack2AddToPoolValue.Value;
                            RigidEmitterHooks.ActivePackPool.Add(2);
                            break;
                    }
                })
        );

        Subtitle(Pack0SettingsSubtitle);

        var localScale0 = Ash.PersistentSettings.EjaculatorPack0LocalScaleValue.Value;
        var relaxDist0 = Ash.PersistentSettings.EjaculatorPack0TsRelaxDistanceValue.Value;
        var minDist0 = Ash.PersistentSettings.EjaculatorPack0TsMinDistanceValue.Value;
        var maxDist0 = Ash.PersistentSettings.EjaculatorPack0TsMaxDistanceValue.Value;

        GUILayout.Label($"Local scale {localScale0:F2}");
        Ash.PersistentSettings.EjaculatorPack0LocalScaleValue.Value = GUILayout.HorizontalSlider(localScale0, 0, 1);

        GUILayout.Label($"Relax dist {relaxDist0:F3}");
        Ash.PersistentSettings.EjaculatorPack0TsRelaxDistanceValue.Value = GUILayout.HorizontalSlider(relaxDist0, 0.001f, 1f);

        GUILayout.Label($"Min dist {minDist0:F3}");
        minDist0 = GUILayout.HorizontalSlider(minDist0, 0.001f, 1f);
        maxDist0 = Mathf.Max(minDist0, maxDist0);

        GUILayout.Label($"Max dist {maxDist0:F3}");
        maxDist0 = GUILayout.HorizontalSlider(maxDist0, 0.001f, 1f);
        minDist0 = Mathf.Min(minDist0, minDist0);

        var dirty = !Mathf.Approximately(localScale0, Ash.PersistentSettings.EjaculatorPack0LocalScaleValue.Value)
                    || !Mathf.Approximately(relaxDist0, Ash.PersistentSettings.EjaculatorPack0TsRelaxDistanceValue.Value)
                    || !Mathf.Approximately(minDist0, Ash.PersistentSettings.EjaculatorPack0TsMinDistanceValue.Value)
                    || !Mathf.Approximately(maxDist0, Ash.PersistentSettings.EjaculatorPack0TsMaxDistanceValue.Value);

        Ash.PersistentSettings.EjaculatorPack0TsMinDistanceValue.Value = minDist0;
        Ash.PersistentSettings.EjaculatorPack0TsMaxDistanceValue.Value = maxDist0;

        GUILayout.Space(10);

        Flow(
            new[] { "Recommended", "Reset to default" },
            (state, index) => Button(state,
                () => {
                    switch (index) {
                        case 0: {
                            Ash.PersistentSettings.EjaculatorPack0LocalScaleValue.Value = Ash.PersistentSettings.EjaculatorPack0LocalScaleValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack0TsRelaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack0TsRelaxDistanceValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack0TsMinDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack0TsMinDistanceValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack0TsMaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack0TsMaxDistanceValue.Recommended;

                            break;
                        }
                        case 1: {
                            Ash.PersistentSettings.EjaculatorPack0LocalScaleValue.Value = Ash.PersistentSettings.EjaculatorPack0LocalScaleValue.Default;
                            Ash.PersistentSettings.EjaculatorPack0TsRelaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack0TsRelaxDistanceValue.Default;
                            Ash.PersistentSettings.EjaculatorPack0TsMinDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack0TsMinDistanceValue.Default;
                            Ash.PersistentSettings.EjaculatorPack0TsMaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack0TsMaxDistanceValue.Default;

                            break;
                        }
                    }
                })
        );

        GUILayout.Space(10);

        Subtitle(Pack1SettingsSubtitle);

        var localScale1 = Ash.PersistentSettings.EjaculatorPack1LocalScaleValue.Value;
        var relaxDist1 = Ash.PersistentSettings.EjaculatorPack1TsRelaxDistanceValue.Value;
        var minDist1 = Ash.PersistentSettings.EjaculatorPack1TsMinDistanceValue.Value;
        var maxDist1 = Ash.PersistentSettings.EjaculatorPack1TsMaxDistanceValue.Value;

        GUILayout.Label($"Local scale {localScale1:F2}");
        Ash.PersistentSettings.EjaculatorPack1LocalScaleValue.Value = GUILayout.HorizontalSlider(localScale1, 0, 1);

        GUILayout.Label($"Relax dist {relaxDist1:F3}");
        Ash.PersistentSettings.EjaculatorPack1TsRelaxDistanceValue.Value = GUILayout.HorizontalSlider(relaxDist1, 0.001f, 1f);

        GUILayout.Label($"Min dist {minDist1:F3}");
        minDist1 = GUILayout.HorizontalSlider(minDist1, 0.001f, 1f);
        maxDist1 = Mathf.Max(minDist1, maxDist1);

        GUILayout.Label($"Max dist {maxDist1:F3}");
        maxDist1 = GUILayout.HorizontalSlider(maxDist1, 0.001f, 1f);
        minDist1 = Mathf.Min(minDist1, minDist1);

        dirty = dirty
                || !Mathf.Approximately(localScale1, Ash.PersistentSettings.EjaculatorPack1LocalScaleValue.Value)
                || !Mathf.Approximately(relaxDist1, Ash.PersistentSettings.EjaculatorPack1TsRelaxDistanceValue.Value)
                || !Mathf.Approximately(minDist1, Ash.PersistentSettings.EjaculatorPack1TsMinDistanceValue.Value)
                || !Mathf.Approximately(maxDist1, Ash.PersistentSettings.EjaculatorPack1TsMaxDistanceValue.Value);

        Ash.PersistentSettings.EjaculatorPack1TsMinDistanceValue.Value = minDist1;
        Ash.PersistentSettings.EjaculatorPack1TsMaxDistanceValue.Value = maxDist1;

        GUILayout.Space(10);

        Flow(
            new[] { "Recommended", "Reset to default" },
            (state, index) => Button(state,
                () => {
                    switch (index) {
                        case 0: {
                            Ash.PersistentSettings.EjaculatorPack1LocalScaleValue.Value = Ash.PersistentSettings.EjaculatorPack1LocalScaleValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack1TsRelaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack1TsRelaxDistanceValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack1TsMinDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack1TsMinDistanceValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack1TsMaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack1TsMaxDistanceValue.Recommended;

                            break;
                        }
                        case 1: {
                            Ash.PersistentSettings.EjaculatorPack1LocalScaleValue.Value = Ash.PersistentSettings.EjaculatorPack1LocalScaleValue.Default;
                            Ash.PersistentSettings.EjaculatorPack1TsRelaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack1TsRelaxDistanceValue.Default;
                            Ash.PersistentSettings.EjaculatorPack1TsMinDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack1TsMinDistanceValue.Default;
                            Ash.PersistentSettings.EjaculatorPack1TsMaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack1TsMaxDistanceValue.Default;

                            break;
                        }
                    }
                })
        );

        GUILayout.Space(10);

        Subtitle(Pack2SettingsSubtitle);

        var localScale2 = Ash.PersistentSettings.EjaculatorPack2LocalScaleValue.Value;
        var relaxDist2 = Ash.PersistentSettings.EjaculatorPack2TsRelaxDistanceValue.Value;
        var minDist2 = Ash.PersistentSettings.EjaculatorPack2TsMinDistanceValue.Value;
        var maxDist2 = Ash.PersistentSettings.EjaculatorPack2TsMaxDistanceValue.Value;

        GUILayout.Label($"Local scale {localScale2:F2}");
        Ash.PersistentSettings.EjaculatorPack2LocalScaleValue.Value = GUILayout.HorizontalSlider(localScale2, 0, 1);

        GUILayout.Label($"Relax dist {relaxDist2:F3}");
        Ash.PersistentSettings.EjaculatorPack2TsRelaxDistanceValue.Value = GUILayout.HorizontalSlider(relaxDist2, 0.001f, 1f);

        GUILayout.Label($"Min dist {minDist2:F3}");
        minDist2 = GUILayout.HorizontalSlider(minDist2, 0.001f, 1f);
        maxDist2 = Mathf.Max(minDist2, maxDist2);

        GUILayout.Label($"Max dist {maxDist2:F3}");
        maxDist2 = GUILayout.HorizontalSlider(maxDist2, 0.001f, 1f);
        minDist2 = Mathf.Min(minDist2, minDist2);

        dirty = dirty
                || !Mathf.Approximately(localScale2, Ash.PersistentSettings.EjaculatorPack2LocalScaleValue.Value)
                || !Mathf.Approximately(relaxDist2, Ash.PersistentSettings.EjaculatorPack2TsRelaxDistanceValue.Value)
                || !Mathf.Approximately(minDist2, Ash.PersistentSettings.EjaculatorPack2TsMinDistanceValue.Value)
                || !Mathf.Approximately(maxDist2, Ash.PersistentSettings.EjaculatorPack2TsMaxDistanceValue.Value);

        Ash.PersistentSettings.EjaculatorPack2TsMinDistanceValue.Value = minDist2;
        Ash.PersistentSettings.EjaculatorPack2TsMaxDistanceValue.Value = maxDist2;

        GUILayout.Space(10);

        Flow(
            new[] { "Recommended", "Reset to default" },
            (state, index) => Button(state,
                () => {
                    switch (index) {
                        case 0: {
                            Ash.PersistentSettings.EjaculatorPack2LocalScaleValue.Value = Ash.PersistentSettings.EjaculatorPack2LocalScaleValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack2TsRelaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack2TsRelaxDistanceValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack2TsMinDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack2TsMinDistanceValue.Recommended;
                            Ash.PersistentSettings.EjaculatorPack2TsMaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack2TsMaxDistanceValue.Recommended;

                            break;
                        }
                        case 1: {
                            Ash.PersistentSettings.EjaculatorPack2LocalScaleValue.Value = Ash.PersistentSettings.EjaculatorPack2LocalScaleValue.Default;
                            Ash.PersistentSettings.EjaculatorPack2TsRelaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack2TsRelaxDistanceValue.Default;
                            Ash.PersistentSettings.EjaculatorPack2TsMinDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack2TsMinDistanceValue.Default;
                            Ash.PersistentSettings.EjaculatorPack2TsMaxDistanceValue.Value = Ash.PersistentSettings.EjaculatorPack2TsMaxDistanceValue.Default;

                            break;
                        }
                    }
                })
        );

        GUILayout.Space(10);

        var reEmitNum = Ash.PersistentSettings.EjaculatorRigidEmitterEmitNumValue.Value;

        GUILayout.Label($"Rigid emitter emit number {reEmitNum}");
        Ash.PersistentSettings.EjaculatorRigidEmitterEmitNumValue.Value = Mathf.RoundToInt(GUILayout.HorizontalSlider(reEmitNum, 1, 5));

        dirty = dirty || reEmitNum != Ash.PersistentSettings.EjaculatorRigidEmitterEmitNumValue.Value;

        if (dirty)
            RigidEmitterHooks.UpdateSettingsForAllMales();

        GUILayout.Space(10);

        Button(
            "Spawn particles",
            () => {
                if (ActiveMale == null)
                    return;

                ActiveMale.ejaculator.Emit();
            }
        );
    }
}
