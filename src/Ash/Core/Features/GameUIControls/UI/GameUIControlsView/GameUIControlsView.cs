using System.Collections.Generic;
using Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper;
using Ash.GlobalUtils;
using UnityEngine;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.GameUIControls.UI.GameUIControlsView
{
    internal class GameUIControlsView
    {
        // Tab Labels
        internal const string UIControlsLabel = "UI Settings";

        private const string ImmersiveUiTitle = "Immersive UI";
        private const string GlobalSettingsTitle = "Global Settings";

        private const string SceneUiSubtitle = "Scene UI";
        private const string ConfineCursorSubtitle = "Confine cursor to window (multi-monitor fix)";
        private const string ThumbnailBgRemovalSubtitle = "Remove background from thumbnails (requires restart)";

        private static readonly Dictionary<bool, string> SceneUILabels = new Dictionary<bool, string> {
            [true] = "Immersive",
            [false] = "Default"
        };

        // ReSharper disable once MemberCanBeMadeStatic.Global
        internal void DrawView() {
            using (new GUILayout.VerticalScope("box")) {
                Title(ImmersiveUiTitle);

                Subtitle(SceneUiSubtitle);
                Flow(
                    new[] { true, false },
                    (state, idx) => RadioButton(SceneUILabels.GetValueOrDefaultValue(state, ErrorLabel),
                        Ash.PersistentSettings.IsImmersiveUiEnabled.Value == state,
                        () => {
                            if (Ash.PersistentSettings.IsImmersiveUiEnabled.Value == state)
                                return;

                            Ash.PersistentSettings.IsImmersiveUiEnabled.Value = state;

                            if (state) {
                                Ash.AshUI.CreateImmersiveUI();
                                InGameUIManagementHelper.SwitchToImmersiveUIMode();
                            }
                            else {
                                Ash.AshUI.DestroyImmersiveUI();
                                InGameUIManagementHelper.SwitchToDefaultUIMode();
                            }
                        }
                    )
                );

                Subtitle(ThumbnailBgRemovalSubtitle);
                Flow(
                    new[] { true, false },
                    (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                        Ash.PersistentSettings.ThumbnailBackgroundRemovalEnabled.Value == state,
                        () => Ash.PersistentSettings.ThumbnailBackgroundRemovalEnabled.Value = state)
                );

                GUILayout.Space(20);

                Title(GlobalSettingsTitle);

                Subtitle(ConfineCursorSubtitle);
                Flow(
                    new[] { true, false },
                    (state, idx) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                        Ash.PersistentSettings.ConfineCursorToWindowEnabled.Value == state,
                        () => Ash.PersistentSettings.ConfineCursorToWindowEnabled.Value = state)
                );
            }
        }
    }
}
