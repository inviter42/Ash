using Ash.Core.Features.AshPlugin.Common.Components;
using Ash.Core.Features.AshPlugin.Main.Hooks;
using Ash.Core.Features.AshPlugin.Main.Types;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using Character;
using H;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.DebuggingTesting.UI.DebugView;

internal class DebugView
{
    internal const string DebugViewTabLabel = "Debug";

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.DebuggingTesting);

    private const string VectorTargetTitle = "Debug";
    private const string SettingsTitle = "Settings";
    private const string PlaybackDataTitle = "Playback data";

    private const string RemoveAnimationMinSpeedCapSubtitle = "Remove animation min speed cap:";

    private const string ActivateVectorTargetSubtitle = "Activate vector target:";
    private const string ChooseTargetTypeSubtitle = "Choose target type:";
    private const string ChooseEyeIndexSubtitle = "Choose eye index:";

    private Female ActiveFemale => ParentWindow.ActiveFemale;

    private readonly HSceneWindow ParentWindow;

    private Vector2 ScrollPosition;

    internal DebugView(HSceneWindow parentWindow) {
        ParentWindow = parentWindow;
    }


    internal void DrawView() {
        if (ActiveFemale == null) {
            GUILayout.Label("No active female.");
            return;
        }

        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true));

        using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
            FemaleSelectionComponent.Component(ActiveFemale, female => ParentWindow.ActiveFemale = female);

            DrawVectorTargetSection(ActiveFemale);
            DrawAnimationSettingsSection();
            DrawAnimationControlsSection();
            DrawAnimationBakingSection();
        }

        GUILayout.EndScrollView();
    }


    private void DrawVectorTargetSection(Female female) {
        GUILayout.Space(20);

        Title(VectorTargetTitle);

        DrawSetVectorTarget(female);
    }

    private void DrawAnimationSettingsSection() {
        GUILayout.Space(20);

        Title(SettingsTitle);

        DrawRemoveAnimationMinSpeedCap();
        DrawSetWheelSpeed();
    }

    private void DrawAnimationControlsSection() {
        GUILayout.Space(20);

        Title(PlaybackDataTitle);

        DrawAnimationData();
    }

    private void DrawAnimationBakingSection() {
        GUILayout.Space(20);

        Title("Animation Baking");

        DrawAnimationBaking();
    }


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
        Subtitle(ActivateVectorTargetSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(
                ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                IsVectorTargetActive == state,
                () => IsVectorTargetActive = state
            )
        );

        if (!IsVectorTargetActive)
            return;

        Subtitle(ChooseTargetTypeSubtitle);
        Flow(
            new[] { TargetTypes.Neck, TargetTypes.Eyes },
            (state, _) => RadioButton(
                state == TargetTypes.Neck ? "Neck" : "Eyes",
                TargetType == state,
                () => TargetType = state)
        );

        if (TargetType == TargetTypes.Eyes) {
            Subtitle(ChooseEyeIndexSubtitle);
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


    /// <summary>
    /// Animation baking
    /// </summary>
    ///
    private void DrawRemoveAnimationMinSpeedCap() {
        Subtitle(RemoveAnimationMinSpeedCapSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.RemoveAnimationMinSpeedCapValue.Value == state,
                () => {
                    Ash.PersistentSettings.RemoveAnimationMinSpeedCapValue.Value = state;
                    if (SceneTypeTracker.Scene is H_Scene hScene) {
                        foreach (var member in hScene.members) {
                            if (member.stateMgr.nowStateClass is H_State_Loop stateClass)
                                stateClass.minSpeed = Ash.PersistentSettings.RemoveAnimationMinSpeedCapValue.Value
                                    ? 0f
                                    : 0.5f;
                        }
                    }
                })
        );
    }

    private void DrawSetWheelSpeed() {
        Subtitle($"Wheel speed ({Ash.PersistentSettings.AnimationWheelSpeedValue.Value:F4}):");
        Ash.PersistentSettings.AnimationWheelSpeedValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.AnimationWheelSpeedValue.Value, 0.0001f, 0.1f);
        if (SceneTypeTracker.Scene is H_Scene hScene)
            hScene.MixCtrl.wheelSpeed = Ash.PersistentSettings.AnimationWheelSpeedValue.Value;
    }

    private void DrawAnimationData() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        Subtitle("Males");
        foreach (var male in hScene.mainMembers.males)
            DrawHumanInfo(male);

        Subtitle("Females");
        foreach (var female in hScene.mainMembers.females)
            DrawHumanInfo(female);

        Subtitle("Visitor");
        DrawHumanInfo(hScene.visitor?.human);

        return;

        void DrawHumanInfo(Human human) {
            if (!human)
                return;

            var animator = human.body.anime;
            var t = animator.GetCurrentAnimatorStateInfo(0)
                .normalizedTime % 1;

            var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length == 0)
                return;

            var identifier = human.sex == SEX.FEMALE
                ? ((Female)human).heroineID.ToString()
                : ((Male)human).maleID.ToString();

            Info(identifier);
            Info($"Animation name: {clipInfo[0].clip.name}");
            Info($"Animation progress: {t:F2}");
            Info($"Playback speed: {animator.speed:F4}");
        }
    }

    private static AnimationClip BakedClip;
    private static bool DelegateAttached;

    private void DrawAnimationBaking() {
        Subtitle("Reconstruct animation data:");
        Button("Reconstruct", () => {
            if (SceneTypeTracker.Scene is H_Scene hScene) {
                var clipName = string.Empty;
                var human = hScene.mainMembers.females[0];
                if (human) {
                    Ash.Logger.LogDebug("Attempting to bake animation");
                    var animator = human.GetComponentInChildren<Animator>();
                    if (animator) {
                        var info = animator.GetCurrentAnimatorClipInfo(0);
                        if (info.Length > 0) {
                            clipName = info[0].clip.name;
                        }
                    }

                    var baker = Ash.AshGameObj.GetComponent<DevHooks.AnimationBaker>();
                    Ash.Logger.LogDebug($"Baker {baker}");
                    if (baker) {
                        if (!DelegateAttached) {
                            DevHooks.AnimationBaker.BakeCompleteCallback += clip => {
                                Ash.Logger.LogDebug($"Baking complete ({clip.name})");
                                BakedClip = clip;
                            };
                            DelegateAttached = true;
                        }
                        DevHooks.AnimationBaker.ReconstructAnimation(human, clipName, 30f);
                    }
                }
            }
        });

        Button("Validate", () => {
            if (BakedClip == null) {
                Ash.Logger.LogDebug("Baked clip is null");
                return;
            }

            Ash.Logger.LogDebug($"Baked clip: {BakedClip.name}");
            Ash.Logger.LogDebug($"Length {BakedClip.length}, frameRate {BakedClip.frameRate}, legacy {BakedClip.legacy}");
        });

        Button("Replace animator and Play", () => {
            if (BakedClip == null) {
                Ash.Logger.LogDebug("No baked clip is found");
                return;
            }

            if (SceneTypeTracker.Scene is H_Scene hScene) {
                var visitorHuman = hScene.visitor.human;
                var legacyAnim = visitorHuman.gameObject.GetOrAddComponent<Animation>();
                var animator = visitorHuman.GetComponentInChildren<Animator>();
                if (animator != null) {
                    animator.runtimeAnimatorController = null;
                    animator.enabled = false;
                }

                visitorHuman.ik.ClearIK();

                BakedClip.legacy = true;
                BakedClip.wrapMode = WrapMode.Loop;

                legacyAnim.AddClip(BakedClip, BakedClip.name);
                legacyAnim.clip = BakedClip;
                legacyAnim[BakedClip.name].enabled = true;
                legacyAnim[BakedClip.name].weight = 1.0f;
                legacyAnim.enabled = true;
                legacyAnim.Play(BakedClip.name);
            }
        });
    }
}
