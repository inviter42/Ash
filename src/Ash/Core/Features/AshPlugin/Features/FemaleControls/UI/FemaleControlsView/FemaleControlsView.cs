using System;
using System.Collections.Generic;
using Ash.Core.Features.Actions;
using Ash.Core.Features.AshPlugin.Common.Components;
using Ash.Core.Features.AshPlugin.Main.Types;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.FemaleControls.UI.FemaleControlsView;

internal class FemaleControlsView
{
    internal const string FemaleControlsViewTabLabel = "Female Controls";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.FemaleControls);

    private const string NeckAndEyesControlsTitle = "Neck and Eyes controls";
    private const string FluidsControlsTitle = "Fluids controls";
    private const string ExpressionControlsTitle = "Expression controls";

    private const string FluidsControlsSubtitle = "Fluids Controls";
    private const string RemoveSpermButtonLabel = "Remove sperm";
    private const string ToggleVirginBloodButtonLabel = "Toggle blood";
    private const string FemaleNeckTargetLabel = "Female neck follows:";
    private const string FemaleLookTargetLabel = "Female eyes follow:";

    private Female ActiveFemale => ParentWindow.ActiveFemale;

    private readonly HSceneWindow ParentWindow;

    private Vector2 ScrollPosition;

    internal FemaleControlsView(HSceneWindow parentWindow) {
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
            FemaleSelectionComponent.Component(ActiveFemale, female => ParentWindow.ActiveFemale = female);

            DrawFluidsControlsSection(ActiveFemale);
            DrawNeckAndEyesControlsSection(ActiveFemale);
            DrawExpressionControlsSection(ActiveFemale);
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
    private readonly LookAtRotator.TYPE[] MainFemaleNeckTargets = {
        LookAtRotator.TYPE.NO,
        LookAtRotator.TYPE.TARGET,
        LookAtRotator.TYPE.HOLD
    };

    private readonly LookAtRotator.TYPE[] MainFemaleEyesTargets = {
        LookAtRotator.TYPE.TARGET,
        LookAtRotator.TYPE.NO,
        LookAtRotator.TYPE.FORWARD,
        LookAtRotator.TYPE.TARGET,
        LookAtRotator.TYPE.HOLD
    };

    private readonly LookAtRotator.TYPE[] VisitorFemaleNeckTargets = {
        LookAtRotator.TYPE.TARGET,
        LookAtRotator.TYPE.NO,
        LookAtRotator.TYPE.TARGET,
        LookAtRotator.TYPE.HOLD
    };

    private readonly LookAtRotator.TYPE[] VisitorFemaleEyesTargets = {
        LookAtRotator.TYPE.TARGET,
        LookAtRotator.TYPE.NO,
        LookAtRotator.TYPE.FORWARD,
        LookAtRotator.TYPE.TARGET,
        LookAtRotator.TYPE.HOLD
    };

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

        var isVisitor = !SceneUtils.IsMainFemale(female);

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
                        ? isVisitor ? SceneUtils.GetVisitorDefaultLookTarget() : null
                        : SceneUtils.GetCameraTransform();

                    female.ChangeEyeLook(type, target, false);
                    break;
                }
            }
        };

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
