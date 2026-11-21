using System.Collections.Generic;
using Ash.Core.Features.AshPlugin.Features.UISettings.UI.Helpers.InGameUIManagementHelper;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.UISettings.UI.UISettingsView;

internal class UISettingsView
{
    // Tab Labels
    internal const string UISettingsLabel = "UI Settings";

    private const string ImmersiveUiTitle = "Immersive UI";
    private const string GlobalSettingsTitle = "Global Settings";
    private const string PluginUISettingsTitle = "Plugin UI Settings";

    private const string GameUiTypeSubtitle = "Scene UI";
    private const string ConfineCursorSubtitle = "Confine cursor to window (multi-monitor fix)";
    private const string ThumbnailBgRemovalSubtitle = "[Requires restart] Remove background from thumbnails";
    private const string AccentColorSubtitle = "Accent Color";

    private static readonly Dictionary<bool, string> SceneUILabels = new() {
        [true] = "Immersive",
        [false] = "Default"
    };

    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal void DrawView() {
        using (new GUILayout.VerticalScope("box")) {
            Title(ImmersiveUiTitle);
            DrawGameUITypeSelection();
            DrawThumbnailBgRemoval();

            GUILayout.Space(20);

            Title(GlobalSettingsTitle);
            DrawConfineCursor();

            GUILayout.Space(20);

            Title(PluginUISettingsTitle);
            DrawPluginUISettings();
        }
    }

    private void DrawGameUITypeSelection() {
        Subtitle(GameUiTypeSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(SceneUILabels.GetValueOrDefaultValue(state, ErrorLabel),
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
    }

    private void DrawThumbnailBgRemoval() {
        Subtitle(ThumbnailBgRemovalSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.ThumbnailBackgroundRemovalEnabled.Value == state,
                () => Ash.PersistentSettings.ThumbnailBackgroundRemovalEnabled.Value = state)
        );
    }

    private void DrawConfineCursor() {
        Subtitle(ConfineCursorSubtitle);
        Flow(
            new[] { true, false },
            (state, _) => RadioButton(ToggleStateLabels.GetValueOrDefaultValue(state, ErrorLabel),
                Ash.PersistentSettings.ConfineCursorToWindowEnabled.Value == state,
                () => Ash.PersistentSettings.ConfineCursorToWindowEnabled.Value = state)
        );
    }

    private void DrawPluginUISettings() {
        Subtitle(AccentColorSubtitle);

        var red = Ash.PersistentSettings.PluginUIColorRedValue.Value;
        GUILayout.Label($"Red {Ash.PersistentSettings.PluginUIColorRedValue.Value:F2}");
        Ash.PersistentSettings.PluginUIColorRedValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.PluginUIColorRedValue.Value, 0, 1);

        var green = Ash.PersistentSettings.PluginUIColorGreenValue.Value;
        GUILayout.Label($"Green {Ash.PersistentSettings.PluginUIColorGreenValue.Value:F2}");
        Ash.PersistentSettings.PluginUIColorGreenValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.PluginUIColorGreenValue.Value, 0, 1);

        var blue = Ash.PersistentSettings.PluginUIColorBlueValue.Value;
        GUILayout.Label($"Blue {Ash.PersistentSettings.PluginUIColorBlueValue.Value:F2}");
        Ash.PersistentSettings.PluginUIColorBlueValue.Value = GUILayout.HorizontalSlider(Ash.PersistentSettings.PluginUIColorBlueValue.Value, 0, 1);

        if (!Mathf.Approximately(red, Ash.PersistentSettings.PluginUIColorRedValue.Value)
            || !Mathf.Approximately(green, Ash.PersistentSettings.PluginUIColorGreenValue.Value)
            || !Mathf.Approximately(blue, Ash.PersistentSettings.PluginUIColorBlueValue.Value)
        ) {
            AccentColor = new Color(
                Ash.PersistentSettings.PluginUIColorRedValue.Value,
                Ash.PersistentSettings.PluginUIColorGreenValue.Value,
                Ash.PersistentSettings.PluginUIColorBlueValue.Value
            );
        }

        GUILayout.Space(4);

        if (GUILayout.Button("Reset")) {
            Ash.PersistentSettings.PluginUIColorRedValue.Value = Ash.PersistentSettings.PluginUIColorRedValue.Default;
            Ash.PersistentSettings.PluginUIColorGreenValue.Value = Ash.PersistentSettings.PluginUIColorGreenValue.Default;
            Ash.PersistentSettings.PluginUIColorBlueValue.Value = Ash.PersistentSettings.PluginUIColorBlueValue.Default;

            AccentColor = new Color(
                Ash.PersistentSettings.PluginUIColorRedValue.Value,
                Ash.PersistentSettings.PluginUIColorGreenValue.Value,
                Ash.PersistentSettings.PluginUIColorBlueValue.Value
            );
        }
    }
}
