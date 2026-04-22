using UnityEngine;

namespace Ash.Core.Features.Common.Misc
{
    internal static class CommonImGuiStyles
    {
        // Custom font
        // ReSharper disable once MemberCanBePrivate.Global
        internal static Font DynamicFont;

        // Font styles
        internal static GUIStyle TitleStyle;
        internal static GUIStyle SubtitleStyle;
        internal static GUIStyle InfoStyle;

        // UI Styles
        internal static GUIStyle ToolbarStyle;

        private static bool AreStylesInitialized;

        internal static void InitializeStyles() {
            if (AreStylesInitialized)
                return;

            if (!DynamicFont) {
                DynamicFont = Font.CreateDynamicFontFromOSFont(
                    new[] { "Segeo UI" }, 20
                );
                // DontDestroyOnLoad(DynamicFont);
            }

            if (TitleStyle == null) {
                TitleStyle = new GUIStyle(GUI.skin.label) {
                    font = DynamicFont,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(0, 0, 2, 2),
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (SubtitleStyle == null) {
                SubtitleStyle = new GUIStyle(GUI.skin.label) {
                    font = DynamicFont,
                    fontSize = 14,
                    fontStyle = FontStyle.Normal,
                    padding = new RectOffset(4, 0, 10, 2),
                    alignment = TextAnchor.MiddleLeft
                };
            }

            if (InfoStyle == null) {
                InfoStyle = new GUIStyle(GUI.skin.label) {
                    font = DynamicFont,
                    fontSize = 13,
                    fontStyle = FontStyle.Normal,
                    padding = new RectOffset(6, 6, 2, 4),
                    alignment = TextAnchor.UpperLeft,
                    wordWrap = true
                };
            }

            if (ToolbarStyle == null) {
                ToolbarStyle = new GUIStyle(GUI.skin.button) {
                    margin = new RectOffset(0, 0, 0, 10)
                };
            }

            AreStylesInitialized = true;
        }
    }
}
