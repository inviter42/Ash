using System;
using System.Collections.Generic;
using Ash.Core.Features.Actions;
using Ash.Core.Features.AshPlugin.Common.Components;
using Ash.Core.Features.AshPlugin.Features.FemaleControls.Hooks._Female;
using Ash.Core.Features.AshPlugin.Main.Types;
#if DEBUG
using Ash.Core.Tooling.SceneManagement;
#endif
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.FemaleControls.UI.FemaleControlsView;

internal class FemaleControlsView
{
    internal const string HSceneControlsViewTabLabel = "Female Controls";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.HSceneControls);

    private const string NeckAndEyesControlsTitle = "Neck and Eyes controls";
    private const string FluidsControlsTitle = "Fluids controls";
    private const string ExpressionControlsTitle = "Expression controls";
    private const string GlobalSettingsTitle = "Global settings";

    private const string FluidsControlsSubtitle = "Fluids Controls";
    private const string RemoveSpermButtonLabel = "Remove sperm";
    private const string ToggleVirginBloodButtonLabel = "Toggle blood";
    private const string FemaleNeckTargetLabel = "Female neck follows:";
    private const string FemaleLookTargetLabel = "Female eyes follow:";
    private const string ApplyQuaternionCalculationForLarLabel = "Use quaternion LookAtRotator calculation:";
    private const string NeckRotatorOverridesSubtitle = "Neck LookAtRotator overrides:";
    private const string EyesRotatorOverridesSubtitle = "Eyes LookAtRotator overrides:";

    private Female ActiveFemale => ParentWindow.ActiveFemale;
    private H_Scene HScene => ParentWindow.HScene;

    private readonly HSceneWindow ParentWindow;


    private Vector2 ScrollPosition;

    internal FemaleControlsView(HSceneWindow parentWindow) {
        ParentWindow = parentWindow;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal void DrawView() {
        if (ActiveFemale == null) {
            Logger.LogWarning("Female is null");
            Logger.LogWarning(Environment.StackTrace);
            return;
        }

        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
            FemaleSelectionComponent.Component(ActiveFemale, female => ParentWindow.ActiveFemale = female);

            DrawFluidsControlsSection(ActiveFemale);
            DrawNeckAndEyesControlsSection(ActiveFemale);
            DrawExpressionControlsSection(ActiveFemale);
            DrawGlobalSettingsSection(ActiveFemale);
        }

        GUILayout.EndScrollView();
    }

    private void DrawFluidsControlsSection(Female female) {
        GUILayout.Space(20);

        Title(FluidsControlsTitle);

        DrawFluidsControl(female);
    }

    private void DrawNeckAndEyesControlsSection(Female female) {
        GUILayout.Space(20);

        Title(NeckAndEyesControlsTitle);

        DrawNeckLookTarget(female);

        GUILayout.Space(12);

        DrawEyeLookTarget(female);

        GUILayout.Space(22);
    }

    private void DrawExpressionControlsSection(Female female) {
        GUILayout.Space(20);

        Title(ExpressionControlsTitle);

        DrawEyesExpressionCycler(female);

        GUILayout.Space(12);

        DrawMouthExpressionCycler(female);

        GUILayout.Space(22);
    }

    private void DrawGlobalSettingsSection(Female female) {
        GUILayout.Space(20);

        Title(GlobalSettingsTitle);

        DrawApplyQuaternionCalculationForLar();

        GUILayout.Space(12);

        DrawNeckRotatorOverrides();

        GUILayout.Space(12);

        DrawEyesRotatorOverrides();
#if DEBUG
        GUILayout.Space(12);

        DrawSetVectorTarget(female);
#endif
    }


    /// <summary>
    /// Fluids control
    /// </summary>
    private void DrawFluidsControl(Female female) {
        Subtitle(FluidsControlsSubtitle);
        using (new GUILayout.HorizontalScope()) {
            Button(RemoveSpermButtonLabel, () => {
                var heroineIds = SceneUtils.GetHeroineIDsInSceneAsStrings();
                if (heroineIds.Length == 0)
                    return;

                var femaleComponent = SceneUtils.GetFemaleComponentByHeroineIDString(female.heroineID.ToString());
                if (femaleComponent != null)
                    femaleComponent.ClearSpermMaterials();
            });

            Button(ToggleVirginBloodButtonLabel,
                () => female.SetVirginBlood(!female.ShowVirginBlood)
            );
        }
    }


    /// <summary>
    /// Neck and Eyes controls
    /// </summary>
    private readonly LookAtRotator.TYPE[] MainFemaleNeckTargets = { LookAtRotator.TYPE.NO, LookAtRotator.TYPE.TARGET,
        LookAtRotator.TYPE.HOLD };

    private readonly LookAtRotator.TYPE[] MainFemaleEyesTargets = { LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.NO,
        LookAtRotator.TYPE.FORWARD, LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD };

    private readonly LookAtRotator.TYPE[] VisitorFemaleNeckTargets = { LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.NO,
        LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD };

    private readonly LookAtRotator.TYPE[] VisitorFemaleEyesTargets = { LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.NO,
        LookAtRotator.TYPE.FORWARD, LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD };

    private static string GetLookAtRotatorNeckLabel(LookAtRotator.TYPE type, int index) {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return type switch {
            LookAtRotator.TYPE.NO or LookAtRotator.TYPE.HOLD or LookAtRotator.TYPE.FORWARD => LookAtRotatorNeckTypeLabels.GetValueOrDefaultValue(type, ErrorLabel),
            LookAtRotator.TYPE.TARGET => index == 0
                ? LookAtRotatorTargetDefaultVisitorLabel
                : LookAtRotatorTargetCameraLabel,
            _ => ErrorLabel
        };
    }

    private static string GetLookAtRotatorEyesLabel(LookAtRotator.TYPE type, int index, bool isVisitor) {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return type switch {
            LookAtRotator.TYPE.NO or LookAtRotator.TYPE.HOLD or LookAtRotator.TYPE.FORWARD => LookAtRotatorEyesTypeLabels.GetValueOrDefaultValue(type, ErrorLabel),
            LookAtRotator.TYPE.TARGET => index == 0
                ? isVisitor ? LookAtRotatorTargetDefaultVisitorLabel : LookAtRotatorTargetDefaultMainLabel
                : LookAtRotatorTargetCameraLabel,
            _ => ErrorLabel
        };
    }

    private void DrawNeckLookTarget(Female female) {
        Subtitle(FemaleNeckTargetLabel);

        // ReSharper disable once ConvertToLocalFunction
        Func<LookAtRotator.TYPE, int, bool> isActiveCheck = (type, btnIndex) => {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            return type switch {
                LookAtRotator.TYPE.NO or LookAtRotator.TYPE.HOLD => female.neckLookType == type,
                LookAtRotator.TYPE.TARGET => btnIndex switch {
                    0 => female.neckLookType == type && female.neckLookTarget == SceneUtils.GetVisitorDefaultLookTarget(),
                    _ => female.neckLookType == type && btnIndex > 0 && female.neckLookTarget == SceneUtils.GetCameraTransform()
                },
                _ => false
            };
        };

        // ReSharper disable once ConvertToLocalFunction
        Action<LookAtRotator.TYPE, int> callback = (type, btnIndex) => {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (type) {
                case LookAtRotator.TYPE.NO:
                case LookAtRotator.TYPE.HOLD:
                {
                    female.ChangeNeckLook(type, null, false);
                    break;
                }

                case LookAtRotator.TYPE.TARGET:
                {
                    var target = btnIndex == 0
                        ? SceneUtils.GetVisitorDefaultLookTarget()
                        : SceneUtils.GetCameraTransform();

                    female.ChangeNeckLook(type, target, false);
                    break;
                }
            }
        };

        var isVisitor = !SceneUtils.IsMainFemale(female);

        var model = isVisitor
            ? VisitorFemaleNeckTargets
            : MainFemaleNeckTargets;

        Flow(
            model,
            (type, idx) => RadioButton(
                GetLookAtRotatorNeckLabel(type, idx),
                isActiveCheck(type, idx),
                () => callback(type, idx)
            )
        );
    }

    private void DrawEyeLookTarget(Female female) {
        Subtitle(FemaleLookTargetLabel);

        // ReSharper disable once ConvertToLocalFunction
        Func<LookAtRotator.TYPE, int, bool> isActiveCheck = (type, btnIndex) => {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            return type switch {
                LookAtRotator.TYPE.NO or LookAtRotator.TYPE.FORWARD or LookAtRotator.TYPE.HOLD => female.eyeLookType == type,
                LookAtRotator.TYPE.TARGET => btnIndex switch {
                    0 => female.eyeLookType == type && (female.eyeLookTarget == null || female.eyeLookTarget == SceneUtils.GetVisitorDefaultLookTarget()),
                    _ => female.eyeLookType == type && female.eyeLookTarget == SceneUtils.GetCameraTransform()
                },
                _ => false
            };
        };

        // ReSharper disable once ConvertToLocalFunction
        Action<LookAtRotator.TYPE, int> callback = (type, idx) => {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (type) {
                case LookAtRotator.TYPE.NO:
                {
                    female.ChangeEyeLook(LookAtRotator.TYPE.NO, null, false);
                    break;
                }
                case LookAtRotator.TYPE.FORWARD:
                case LookAtRotator.TYPE.HOLD:
                {
                    female.ChangeEyeLook(type, null, false);
                    break;
                }

                case LookAtRotator.TYPE.TARGET:
                {
                    var target = idx == 0
                        ? SceneUtils.GetVisitorDefaultLookTarget()
                        : SceneUtils.GetCameraTransform();

                    female.ChangeEyeLook(type, target, false);
                    break;
                }
            }
        };

        var isVisitor = !SceneUtils.IsMainFemale(female);

        var model = isVisitor
            ? VisitorFemaleEyesTargets
            : MainFemaleEyesTargets;

        Flow(
            model,
            (type, idx) => RadioButton(
                GetLookAtRotatorEyesLabel(type, idx, isVisitor),
                isActiveCheck(type, idx),
                () => callback(type, idx)
            )
        );
    }


    /// <summary>
    /// LAR settings
    /// </summary>
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

        var dirty = false;

        var neckRotatorMaxUpValue = Ash.PersistentSettings.NeckRotatorMaxUpValue.Value;
        GUILayout.Label($"Neck max up {Ash.PersistentSettings.NeckRotatorMaxUpValue.Value:F2}");
        Ash.PersistentSettings.NeckRotatorMaxUpValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.NeckRotatorMaxUpValue.Value, 0f, 180f);

        if (!Mathf.Approximately(Ash.PersistentSettings.NeckRotatorMaxUpValue.Value, neckRotatorMaxUpValue))
            dirty = true;

        var neckRotatorMaxDownValue = Ash.PersistentSettings.NeckRotatorMaxDownValue.Value;
        GUILayout.Label($"Neck max down {Ash.PersistentSettings.NeckRotatorMaxDownValue.Value:F2}");
        Ash.PersistentSettings.NeckRotatorMaxDownValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.NeckRotatorMaxDownValue.Value, 0f, 180f);

        if (!Mathf.Approximately(Ash.PersistentSettings.NeckRotatorMaxDownValue.Value, neckRotatorMaxDownValue))
            dirty = true;

        var neckBoneMaxYawMagnitudeValue = Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value;
        GUILayout.Label($"Neck yaw magnitude {Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value:F2}");
        Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value, 0f, 180f);

        if (!Mathf.Approximately(Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value, neckBoneMaxYawMagnitudeValue))
            dirty = true;

        var headBoneMaxYawMagnitudeValue = Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value;
        GUILayout.Label($"Head yaw magnitude {Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value:F2}");
        Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value, 0f, 180f);

        if (!Mathf.Approximately(Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value, headBoneMaxYawMagnitudeValue))
            dirty = true;

        if (!dirty)
            return;

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

        var dirty = false;

        var eyesRotatorMaxUpValue = Ash.PersistentSettings.EyesRotatorMaxUpValue.Value;
        GUILayout.Label($"Eyes max up {Ash.PersistentSettings.EyesRotatorMaxUpValue.Value:F2}");
        Ash.PersistentSettings.EyesRotatorMaxUpValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.EyesRotatorMaxUpValue.Value, 0f, 180f);

        if (!Mathf.Approximately(Ash.PersistentSettings.EyesRotatorMaxUpValue.Value, eyesRotatorMaxUpValue))
            dirty = true;

        var eyesRotatorMaxDownValue = Ash.PersistentSettings.EyesRotatorMaxDownValue.Value;
        GUILayout.Label($"Eyes max down {Ash.PersistentSettings.EyesRotatorMaxDownValue.Value:F2}");
        Ash.PersistentSettings.EyesRotatorMaxDownValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.EyesRotatorMaxDownValue.Value, 0f, 180f);

        if (!Mathf.Approximately(Ash.PersistentSettings.EyesRotatorMaxDownValue.Value, eyesRotatorMaxDownValue))
            dirty = true;

        var eyesBoneMaxPositiveYawValue = Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value;
        GUILayout.Label($"Eyes max positive yaw {Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value:F2}");
        Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value, 0f, 180f);

        if (!Mathf.Approximately(Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value, eyesBoneMaxPositiveYawValue))
            dirty = true;

        var eyesBoneMaxNegativeYawValue = Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value;
        GUILayout.Label($"Eyes max negative yaw {Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value:F2}");
        Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value, 0f, 180f);

        if (!Mathf.Approximately(Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value, eyesBoneMaxNegativeYawValue))
            dirty = true;

        if (!dirty)
            return;

        FemaleHooks.InitEyesLarForAllFemales();
    }

    #region VectorTargeting
        #if DEBUG
            /// <summary>
            /// Vector target debugging section
            /// </summary>
            private static bool IsVectorTargetActive;

            private static TargetTypes TargetType = TargetTypes.Neck;

            private static Vector3 NeckVectorTarget = Vector3.zero;
            private static Vector3 Eye0VectorTarget = Vector3.zero;
            private static Vector3 Eye1VectorTarget = Vector3.zero;

            private static int EyeIndex;

            private enum TargetTypes
            {
                Neck,
                Eyes,
            }


            private void DrawSetVectorTarget(Female female) {
                Subtitle("Activate vector target:");
                Flow(
                    new[] { true, false },
                    (state, idx) => RadioButton(
                        ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                        IsVectorTargetActive == state,
                        () => IsVectorTargetActive = state
                    )
                );

                if (!IsVectorTargetActive)
                    return;

                Subtitle("Choose target type:");
                Flow(
                    new[] { TargetTypes.Neck, TargetTypes.Eyes },
                    (state, idx) => RadioButton(
                        state == TargetTypes.Neck ? "Neck" : "Eyes",
                        TargetType == state,
                        () => TargetType = state)
                );

                if (TargetType == TargetTypes.Eyes) {
                    Subtitle("Choose eye index:");
                    Flow(
                        new[] { 0, 1 },
                        (state, idx) => RadioButton(
                            idx == 0 ? "Left" : "Right",
                            EyeIndex == state,
                            () => EyeIndex = state)
                    );
                }

                ref var activeVector = ref TargetType == TargetTypes.Neck
                    ? ref NeckVectorTarget
                    : ref EyeIndex == 0
                        ? ref Eye0VectorTarget
                        : ref Eye1VectorTarget;

                GUILayout.Label($"X {activeVector.x:F2}");
                activeVector = activeVector with {
                    x = GUILayout.HorizontalSlider(activeVector.x, -180f, 180f)
                };

                GUILayout.Label($"Y {activeVector.y:F2}");
                activeVector = activeVector with {
                    y = GUILayout.HorizontalSlider(activeVector.y, -180f, 180f)
                };

                GUILayout.Label($"Z {activeVector.z:F2}");
                activeVector = activeVector with {
                    z = GUILayout.HorizontalSlider(activeVector.z, -180f, 180f)
                };

                if (TargetType == TargetTypes.Neck) {
                    female.ChangeNeckLook(LookAtRotator.TYPE.TARGET, activeVector, false);
                } else {
                    female.eyeLookType = female.eyeLook.calcType = LookAtRotator.TYPE.NO;
                    female.eyeLookTarget = female.eyeLook.target = null;
                    female.eyeLookForce = false;
                    female.eyeLook.noRate = 0f;
                    female.eyeLook.targetPos = Vector3.zero;

                    female.eyeLook.rotateBones[EyeIndex].bone.localRotation = Quaternion.Euler(activeVector);
                }
            }
        #endif
    #endregion

    /// <summary>
    /// Expression controls
    /// </summary>
    private static readonly Dictionary<Female, int> EyeExpressionIndices = new();
    private static readonly Dictionary<Female, int> MouthExpressionIndices = new();

    private static readonly Dictionary<string, string> EyeExpressionListTranslations = new() {
        ["無表情"] = "Blank",
        ["喜び"] = "Joy",
        ["照れ"] = "Shy",
        ["笑み"] = "Slight smile",
        ["笑顔"] = "Smile",
        ["苦笑い"] = "Awkward",
        ["ニヤケ"] = "Smirk",
        ["ドヤ顔"] = "Smug",
        ["キメ顔"] = "Posing",
        ["自慢顔"] = "Boastful",
        ["ウィンク左"] = "Wink (left)",
        ["ウィンク右"] = "Wink (right)",
        ["眩しい左"] = "Squinting (left)",
        ["眩しい右"] = "Squinting (right)",
        ["細目"] = "Narrowed eyes",
        ["ジト目"] = "Unimpressed",
        ["見下し"] = "Disdain",
        ["呆れ"] = "Dumbfounded",
        ["がっかり"] = "Disappointed",
        ["驚き"] = "Surprised",
        ["真剣"] = "Serious",
        ["考え"] = "Thinking",
        ["企み"] = "Scheming",
        ["ツン"] = "Tsundere",
        ["不機嫌"] = "Grumpy",
        ["怒り"] = "Angry",
        ["激怒"] = "Furious",
        ["困り"] = "Perplexed",
        ["悲しい"] = "Sad",
        ["大泣き"] = "Bawling",
        ["痛がり"] = "In pain",
        ["激痛"] = "Agony",
        ["歪み左"] = "Twisted (left)",
        ["歪み右"] = "Twisted (right)",
        ["恥ずかし"] = "Embarrassed",
        ["感じ"] = "Feeling it",
        ["うっとり"] = "Dreamy",
        ["アヘA"] = "Ahegao A",
        ["アヘB"] = "Ahegao B",
        ["アヘC"] = "Ahegao C",
    };

    private static readonly Dictionary<string, string> MouthExpressionListTranslations = new() {
        ["無表情"] = "Blank",
        ["喜び"] = "Joy",
        ["笑顔"] = "Smiling",
        ["苦笑い"] = "Awkward smile",
        ["ニヤケ"] = "Smirk",
        ["ドヤ顔"] = "Smug",
        ["驚き"] = "Surprised",
        ["真剣"] = "Serious",
        ["企み"] = "Scheming",
        ["ツン"] = "Tsundere",
        ["不機嫌"] = "Grumpy",
        ["怒り"] = "Angry",
        ["激怒"] = "Furious",
        ["困り"] = "Troubled",
        ["悲しい"] = "Sad",
        ["大泣き"] = "Bawling",
        ["痛がり"] = "In pain",
        ["食いしばり"] = "Gritted teeth",
        ["叫び"] = "Screaming",
        ["アヘA"] = "Ahegao A",
        ["アヘB"] = "Ahegao B",
        ["アヘC"] = "Ahegao C",
        ["アヘD"] = "Ahegao D",
        ["アヘE"] = "Ahegao E",
        ["ふくれ"] = "Pouting",
        ["おちょぼ口"] = "Pursed",
        ["口笛"] = "Whistling",
        ["キスA"] = "Kiss A",
        ["キスB"] = "Kiss B",
        ["舌出しA"] = "Tongue A",
        ["舌出しB"] = "Tongue B",
        ["舌出しC"] = "Tongue C",
        ["舌出しD"] = "Tongue D",
        ["舌出しE"] = "Tongue E",
    };

    private void DrawEyesExpressionCycler(Female female) {
        if (!EyeExpressionIndices.TryGetValue(female, out var eyeExpressionIndex))
            EyeExpressionIndices[female] = 0;

        var exprName = EyeExpressionListTranslations
            .GetValueOrDefaultValue(GlobalPluginData.EyeExpressionListFemale[eyeExpressionIndex].name, "??");

        Subtitle($"Set female eyes expression [{eyeExpressionIndex}: {exprName}]:");
        Flow(
            new[] { "Prev", "Set [Blank]", "Next" },
            (state, buttonIndex) => Button(
                state,
                () => {
                    EyeExpressionIndices[female] = buttonIndex switch {
                        0 => (eyeExpressionIndex - 1 + GlobalPluginData.EyeExpressionListFemale.Count) %
                             GlobalPluginData.EyeExpressionListFemale.Count,
                        1 => 0,
                        2 => (eyeExpressionIndex + 1) % GlobalPluginData.EyeExpressionListFemale.Count,
                        _ => EyeExpressionIndices[female]
                    };

                    Apply();
                }
            )
        );

        RadioButton("Auto", !GlobalGameData.IsCustomEyesExpressionPresetActive(female), Reset);

        return;

        void Apply(int forceIndex = -1) {
            if (forceIndex >= 0)
                EyeExpressionIndices[female] = forceIndex;


            var kvp = new KeyValuePair<AhegaoFaceAction.EyesExpressionPreset, int>(
                (AhegaoFaceAction.EyesExpressionPreset)EyeExpressionIndices[female],
                EyeExpressionIndices[female]
            );
            FemaleUtils.ApplyEyesExpression(female, kvp);
        }

        void Reset() {
            EyeExpressionIndices[female] = 0;
            FemaleUtils.ResetEyesExpression(female);
        }
    }

    private void DrawMouthExpressionCycler(Female female) {
        if (!MouthExpressionIndices.TryGetValue(female, out var mouthExpressionIndex))
            MouthExpressionIndices[female] = 0;

        var exprName = MouthExpressionListTranslations
            .GetValueOrDefaultValue(GlobalPluginData.MouthExpressionListFemale[mouthExpressionIndex].name, "??");

        Subtitle($"Set female mouth expression [{mouthExpressionIndex}: {exprName}]:");
        Flow(
            new[] { "Prev", "Set [Blank]", "Next" },
            (state, buttonIndex) => Button(
                state,
                () => {
                    MouthExpressionIndices[female] = buttonIndex switch {
                        0 => (mouthExpressionIndex - 1 + GlobalPluginData.MouthExpressionListFemale.Count) %
                             GlobalPluginData.MouthExpressionListFemale.Count,
                        1 => 0,
                        2 => (mouthExpressionIndex + 1) % GlobalPluginData.MouthExpressionListFemale.Count,
                        _ => MouthExpressionIndices[female]
                    };

                    Apply();
                }
            )
        );

        RadioButton("Auto", !GlobalGameData.IsCustomMouthExpressionPresetActive(female), Reset);

        return;

        void Apply(int forceIndex = -1) {
            if (forceIndex >= 0)
                MouthExpressionIndices[female] = forceIndex;

            var kvp = new KeyValuePair<AhegaoFaceAction.MouthExpressionPreset, int>(
                (AhegaoFaceAction.MouthExpressionPreset)MouthExpressionIndices[female],
                MouthExpressionIndices[female]
            );
            FemaleUtils.ApplyMouthExpression(female, kvp, 0.1f, true);
        }

        void Reset() {
            MouthExpressionIndices[female] = 0;
            FemaleUtils.ResetMouthExpression(female);
        }
    }
}
