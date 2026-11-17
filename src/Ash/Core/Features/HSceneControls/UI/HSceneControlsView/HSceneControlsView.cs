using System;
using Ash.Core.Features.Common.Components;
using Ash.Core.SceneManagement;
using Ash.Core.UI;
using Ash.Core.UI.Types;
using Ash.GlobalUtils;
using UnityEngine;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.HSceneControls.UI.HSceneControlsView
{
    internal class HSceneControlsView
    {
        internal const string HSceneControlsViewTabLabel = "H-Scene Controls";

        private const string NeckAndEyesControlsTitle = "Neck and Eyes controls";
        private const string FluidsControlsTitle = "Fluids controls";

        private const string FluidsControlsSubtitle = "Fluids Controls";
        private const string RemoveSpermButtonLabel = "Remove sperm";
        private const string ToggleVirginBloodButtonLabel = "Toggle blood";
        private const string FemaleNeckTargetLabel = "Female neck follows:";
        private const string FemaleLookTargetLabel = "Female eyes follow:";

        private Vector2 ScrollPosition;

        // ReSharper disable once MemberCanBeMadeStatic.Global
        internal void DrawView() {
            var female = GetActiveFemale();
            if (female == null) {
                Ash.Logger.LogWarning("Female is null");
                Ash.Logger.LogWarning(Environment.StackTrace);
                return;
            }

            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true))) {
                FemaleSelectionComponent.Component(female, SetActiveFemale);

                DrawFluidsControlsSection(female);
                DrawNeckAndEyesControlsSection(female);
            }

            GUILayout.EndScrollView();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Female GetActiveFemale() {
            switch (WindowManager.Window) {
                case HSceneWindow hSceneWindow:
                    return hSceneWindow.GetActiveFemale();
                default:
                    Ash.Logger.LogError($"View HSceneControlsView is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return null;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void SetActiveFemale(Female female) {
            switch (WindowManager.Window) {
                case HSceneWindow hSceneWindow:
                    hSceneWindow.SetActiveFemale(female);
                    break;
                default:
                    Ash.Logger.LogError($"View HSceneControlsView is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Transform GetCameraTransform() {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (SceneTypeTracker.TypeOfCurrentScene) {
                case SceneTypeTracker.SceneTypes.SelectScene:
                case SceneTypeTracker.SceneTypes.H:
                    var illusionCamera = UnityEngine.Object.FindObjectOfType<IllusionCamera>();
                    // ReSharper disable once InvertIf
                    if (illusionCamera == null) {
                        Ash.Logger.LogWarning(
                            $"Unable to find IllusionCamera in the scene {SceneTypeTracker.TypeOfCurrentScene}");

                        return null;
                    }

                    return illusionCamera.transform;

                default:
                    return null;
            }
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

        private void DrawNeckLookTarget(Female female) {
            Subtitle(FemaleNeckTargetLabel);

            // ReSharper disable once ConvertToLocalFunction
            Func<LookAtRotator.TYPE, int, bool> isActiveCheck = (type, idx) => {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (type) {
                    case LookAtRotator.TYPE.NO:
                    case LookAtRotator.TYPE.HOLD:
                        return female.neckLookType == type;
                    case LookAtRotator.TYPE.TARGET:
                        return female.neckLookType == type
                               && (idx == 0 && female.neckLookTarget == GetVisitorDefaultLookTarget(female)
                                   || idx > 0 && female.neckLookTarget == GetCameraTransform());
                    default:
                        return false;
                }
            };

            // ReSharper disable once ConvertToLocalFunction
            Action<LookAtRotator.TYPE, int> callback = (type, idx) => {
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
                        var target = idx == 0
                            ? GetVisitorDefaultLookTarget(female)
                            : GetCameraTransform();

                        female.ChangeNeckLook(type, target, false);
                        break;
                    }
                }
            };

            var model = IsMainFemale(female)
                ? new[] { LookAtRotator.TYPE.NO, LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD }
                : new[] {
                    LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.NO, LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD
                };

            Flow(
                model,
                (type, idx) => RadioButton(
                    GetLookAtRotatorLabel(type, idx),
                    isActiveCheck(type, idx),
                    () => callback(type, idx)
                )
            );
        }

        private void DrawEyeLookTarget(Female female) {
            Subtitle(FemaleLookTargetLabel);

            // ReSharper disable once ConvertToLocalFunction
            Func<LookAtRotator.TYPE, int, bool> isActiveCheck = (type, idx) => {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (type) {
                    case LookAtRotator.TYPE.NO:
                        // default look for main is eyeLookType = TARGET, and eyeLookTarget = null
                        return female.eyeLookType == LookAtRotator.TYPE.TARGET
                               && female.eyeLookTarget == null;
                    case LookAtRotator.TYPE.FORWARD:
                    case LookAtRotator.TYPE.HOLD:
                        return female.eyeLookType == type;
                    case LookAtRotator.TYPE.TARGET:
                        return female.eyeLookType == type
                               && (idx == 0 && female.eyeLookTarget == GetVisitorDefaultLookTarget(female)
                                   || idx > 0 && female.eyeLookTarget == GetCameraTransform());
                    default:
                        return false;
                }
            };

            // ReSharper disable once ConvertToLocalFunction
            Action<LookAtRotator.TYPE, int> callback = (type, idx) => {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (type) {
                    case LookAtRotator.TYPE.NO:
                    {
                        // default look for main is eyeLookType = TARGET, and eyeLookTarget = null
                        female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, null, false);
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
                            ? GetVisitorDefaultLookTarget(female)
                            : GetCameraTransform();

                        female.ChangeEyeLook(type, target, false);
                        break;
                    }
                }
            };

            var model = IsMainFemale(female)
                ? new[] { LookAtRotator.TYPE.NO, LookAtRotator.TYPE.FORWARD, LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD }
                : new[] {
                    LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.FORWARD, LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD
                };

            Flow(
                model,
                (type, idx) => RadioButton(
                    GetLookAtRotatorLabel(type, idx),
                    isActiveCheck(type, idx),
                    () => callback(type, idx)
                )
            );
        }

        private Transform GetVisitorDefaultLookTarget(Female female) {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null) {
                Ash.Logger.LogError($"Expected H_Scene is null.");
                return null;
            }

            if (hScene.visitor?.GetFemale() == female)
                return hScene.mainMembers.CaclVisitorLookPos();

            return null;
        }

        private bool IsMainFemale(Female female) {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null) {
                Ash.Logger.LogError($"Expected H_Scene is null.");
                return false;
            }

            return hScene.mainMembers.GetFemale(0) == female;
        }

        private string GetLookAtRotatorLabel(LookAtRotator.TYPE type, int index) {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (type) {
                case LookAtRotator.TYPE.NO:
                case LookAtRotator.TYPE.HOLD:
                case LookAtRotator.TYPE.FORWARD:
                    return LookAtRotatorTypeLabels.GetValueOrDefaultValue(type, ErrorLabel);
                case LookAtRotator.TYPE.TARGET:
                    return string.Join(
                        " ",
                        new [] {
                            LookAtRotatorTypeLabels.GetValueOrDefaultValue(type, ErrorLabel),
                            index == 0
                                ? LookAtRotatorSpecifierDefault
                                : LookAtRotatorSpecifierCamera
                        }
                    );
                default:
                    return ErrorLabel;
            }
        }
    }
}
