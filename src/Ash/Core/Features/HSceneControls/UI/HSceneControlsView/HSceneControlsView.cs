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

        private static readonly Dictionary<bool, string> MuteBackgroundFemaleLabels = new Dictionary<bool, string> {
            [true] = "On",
            [false] = "Off"
        };

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void DrawView() {
            var activeFemale = GetActiveFemale();
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
    }
}
