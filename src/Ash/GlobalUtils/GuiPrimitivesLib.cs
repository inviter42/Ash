using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.UI;
using UnityEngine;

namespace Ash.GlobalUtils
{
    internal static class GuiPrimitivesLib
    {
        private static readonly Color MatrixGreen = new Color(0.0f, 0.7f, 0.05f);
        private static readonly GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button) {
            padding = new RectOffset(12, 12, 4, 4)
        };

        public static void Title(string text, GUIStyle style = null) {
            var finalStyle = new GUIStyle(AshUI.TitleStyle);

            if (style != null)
                finalStyle = style;

            GUILayout.BeginHorizontal();
            GUILayout.Label(text, finalStyle);
            GUILayout.EndHorizontal();
        }

        public static void Subtitle(string text, GUIStyle style = null) {
            var finalStyle = new GUIStyle(AshUI.SubtitleStyle);

            if (style != null)
                finalStyle = style;

            GUILayout.BeginHorizontal();
            GUILayout.Label(text, finalStyle);
            GUILayout.EndHorizontal();
        }

        public static void Info(string text, GUIStyle style = null) {
            var finalStyle = new GUIStyle(AshUI.InfoStyle);

            if (style != null)
                finalStyle = style;

            GUILayout.Space(2);
            using (new GUILayout.HorizontalScope("box")) {
                GUILayout.Label(text, finalStyle);
            }

            GUILayout.Space(2);
        }

        public static void Button(string label, Action callback, params GUILayoutOption[] options) {
            if (GUILayout.Button(label, ButtonStyle, options))
                callback.Invoke();
        }

        public static void RadioButton(string label, bool isActive, Action callback, params GUILayoutOption[] options) {
            var oldContentColor = GUI.contentColor;

            if (isActive)
                GUI.contentColor = MatrixGreen;

            Button(label, callback, options);

            GUI.contentColor = oldContentColor;
        }

        public static void Flow<T>(T[] model, Action<T, int> action, int rowSize = 4) {
            for (var i = 0; i < model.Length; i++) {
                if ((i % rowSize) == 0) {
                    if (i > 0) GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                }

                action.Invoke(model[i], i);

                if (i == model.Length - 1)
                    GUILayout.EndHorizontal();
            }
        }

        public static void Flow<T>(List<T> model, Action<T, int> action, int rowSize = 4) {
            for (var i = 0; i < model.Count; i++) {
                if ((i % rowSize) == 0) {
                    if (i > 0) GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                }

                action.Invoke(model.ElementAt(i), i);

                if (i == model.Count - 1)
                    GUILayout.EndHorizontal();
            }
        }
    }
}
