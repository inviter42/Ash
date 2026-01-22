using System;
using System.Collections.Generic;
using Ash.Core.Features.Common.Components;
using Ash.Core.SceneManagement;
using Ash.Core.UI.Types;
using Ash.GlobalUtils;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.HSceneControls.UI.HSceneControlsView
{
    internal class HSceneControlsView
    {
        public const string HSceneControlsViewTabLabel = "H-Scene Controls";

        private const string FluidsControlsSubtitle = "Fluids Controls";
        private const string RemoveSpermButtonLabel = "Remove sperm";
        private const string ToggleVirginBloodButtonLabel = "Toggle blood";
        private const string MuteBackgroundFemaleSubtitle = "Mute background female:";
        private const string FemaleNeckTargetLabel = "Female neck follows:";
        private const string FemaleLookTargetLabel = "Female eyes follow:";

        private static readonly Dictionary<bool, string> MuteBackgroundFemaleLabels = new Dictionary<bool, string> {
            [true] = "On",
            [false] = "Off"
        };

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void DrawView() {
            var activeFemale = GetActiveFemale();
            if (activeFemale == null) {
                Ash.Logger.LogWarning("Female is null");
                Ash.Logger.LogWarning(Environment.StackTrace);
                return;
            }

            using (new GUILayout.VerticalScope("box")) {
                FemaleSelectionComponent.Component(activeFemale, SetActiveFemale);

                GUILayout.Space(12);

                Subtitle(FluidsControlsSubtitle);
                using (new GUILayout.HorizontalScope()) {
                    Button(RemoveSpermButtonLabel, () => {
                        var heroineIds = SceneUtils.GetHeroineIDsInSceneAsStrings();
                        if (heroineIds.Length == 0)
                            return;

                        var femaleComponent = SceneUtils.GetFemaleComponentByHeroineIDString(activeFemale.heroineID.ToString());
                        if (femaleComponent != null)
                            femaleComponent.ClearSpermMaterials();
                    });

                    Button(ToggleVirginBloodButtonLabel,
                        () => activeFemale.SetVirginBlood(!activeFemale.ShowVirginBlood)
                    );
                }

                GUILayout.Space(12);

                Subtitle(MuteBackgroundFemaleSubtitle);
                Flow(
                    new[] { true, false },
                    (state, idx) => RadioButton(MuteBackgroundFemaleLabels.GetValueOrDefaultValue(state, ErrorLabel),
                        Ash.Settings.ShouldMuteBackgroundFemale.Value == state,
                        () => {
                            Ash.Settings.ShouldMuteBackgroundFemale.Value = state;
                            foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>()) {
                                female.UpdateVoiceVolume();
                            }
                        })
                );

                GUILayout.Space(12);

                Subtitle(FemaleNeckTargetLabel);
                Flow(
                    new[] { LookAtRotator.TYPE.NO, LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD },
                    (type, idx) => RadioButton(LookRotatorTypeLabels.GetValueOrDefaultValue(type, ErrorLabel),
                        activeFemale.neckLookType == type,
                        () => {
                            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                            switch (type) {
                                case LookAtRotator.TYPE.NO:
                                case LookAtRotator.TYPE.HOLD:
                                {
                                    activeFemale.ChangeNeckLook(type, null, false);
                                    break;
                                }

                                case LookAtRotator.TYPE.TARGET:
                                {
                                    activeFemale.ChangeNeckLook(type, GetCameraTransform(), false);
                                    break;
                                }
                            }
                        })
                );

                GUILayout.Space(12);

                Subtitle(FemaleLookTargetLabel);
                Flow(
                    new[] { LookAtRotator.TYPE.FORWARD, LookAtRotator.TYPE.TARGET, LookAtRotator.TYPE.HOLD },
                    (type, idx) => RadioButton(LookRotatorTypeLabels.GetValueOrDefaultValue(type, ErrorLabel),
                        activeFemale.eyeLookType == type,
                        () => {
                            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                            switch (type) {
                                case LookAtRotator.TYPE.FORWARD:
                                case LookAtRotator.TYPE.HOLD:
                                {
                                    activeFemale.ChangeEyeLook(type, null, false);
                                    break;
                                }

                                case LookAtRotator.TYPE.TARGET:
                                {
                                    activeFemale.ChangeEyeLook(type, GetCameraTransform(), false);
                                    break;
                                }
                            }
                        })
                );
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Female GetActiveFemale() {
            switch (Ash.AshUI.Window) {
                case HSceneWindow hSceneWindow:
                    return hSceneWindow.GetActiveFemale();
                default:
                    Ash.Logger.LogError($"View HSceneControlsView is used inside of an unsupported window type {Ash.AshUI.Window.GetType().Name}.");
                    return null;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void SetActiveFemale(Female female) {
            switch (Ash.AshUI.Window) {
                case HSceneWindow hSceneWindow:
                    hSceneWindow.SetActiveFemale(female);
                    break;
                default:
                    Ash.Logger.LogError($"View HSceneControlsView is used inside of an unsupported window type {Ash.AshUI.Window.GetType().Name}.");
                    return;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Transform GetCameraTransform() {
            switch (SceneTypeTracker.CurrentScene) {
                case SelectScene selectScene:
                    return selectScene.cam.transform;
                case H_Scene hScene:
                    return hScene.camera.transform;
                default:
                    return null;
            }
        }
    }
}
