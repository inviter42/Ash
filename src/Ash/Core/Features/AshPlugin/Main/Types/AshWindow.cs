using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.AshPlugin.Common.Misc;
using Ash.Utility.GlobalUtils;
using KKAPI.Utilities;
using UnityEngine;

namespace Ash.Core.Features.AshPlugin.Main.Types;

internal abstract class AshWindow<T> : ImguiWindow<T> where T : AshWindow<T>
{
    protected Dictionary<string, Dictionary<string, Action>> WindowData { get; set; }

    private int SelectedTabIndex { get; set; }
    private string[] FlatTabs { get; set; }
    private Action[] FlatTabViews { get; set; }

    public float windowWidth = 980;
    public float windowHeight = 600;


    protected new virtual void OnGUI() {
        CommonImGuiStyles.InitializeStyles();

        var skin = GUI.skin;
        GUI.skin = MakeTransparent(IMGUIUtils.SolidBackgroundGuiSkin);
        WindowRect = GUILayout.Window(WindowId, WindowRect, DrawContentsInt, Title);
        if (WindowRect.width < MinimumSize.x) {
            var rect = WindowRect;
            rect.width = MinimumSize.x;
            WindowRect = rect;
        }

        if (WindowRect.height < MinimumSize.y) {
            var rect = WindowRect;
            rect.height = MinimumSize.y;
            WindowRect = rect;
        }

        if (WindowRect.x < 0f || WindowRect.x + WindowRect.width > Screen.width) {
            var rect = WindowRect;
            rect.x = Mathf.Clamp(
                WindowRect.x,
                0f,
                Mathf.Max(0f, Screen.width - WindowRect.width)
            );

            WindowRect = rect;
        }

        if (WindowRect.y < 0f || WindowRect.y + WindowRect.height > Screen.height) {
            var rect = WindowRect;
            rect.y = Mathf.Clamp(
                WindowRect.y,
                0f,
                Mathf.Max(0f, Screen.height - WindowRect.height)
            );

            WindowRect = rect;
        }

        GUI.skin = skin;
    }

    protected override Rect GetDefaultWindowRect(Rect screenRect) {
        return new Rect(
            (float)Screen.width / 2 - windowWidth / 2,
            (float)Screen.height / 2 - windowHeight / 2,
            windowWidth, windowHeight
        );
    }

    protected override void DrawContents() {
        if (WindowData == null || WindowData.Count == 0) {
            GUILayout.Label("No window data found.");
            return;
        }

        GUILayout.BeginHorizontal();

        // separator
        GUILayout.Space(6);

        // tabs
        GUILayout.BeginVertical(GUILayout.Width(windowWidth * 0.25f));

        FlatTabs ??= WindowData.SelectMany(data => data.Value.Keys).ToArray();
        FlatTabViews ??= WindowData.SelectMany(data => data.Value.Values).ToArray();

        if (WindowData.Count == 1) {
            DrawSelectionGrid();
        } else {
            DrawSectionedTabs();
        }

        GUILayout.EndVertical();

        // separator
        GUILayout.Space(4);

        // current view
        GUILayout.BeginVertical();
        FlatTabViews[SelectedTabIndex].Invoke();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    // Verbatim from KKAPI.Utilities
    private void DrawContentsInt(int id) {
        var num = GUI.skin.window.border.top - 4;
        if (GUI.Button(new Rect((float)(WindowRect.width - (double)num - 2.0), 2f, num, num), "x")) {
            enabled = false;
        }
        else {
            try {
                DrawContents();
                IMGUIUtils.DrawTooltip(WindowRect);
            }
            catch (Exception ex) {
                if (!ex.Message.Contains("GUILayout"))
                    Ash.Logger.LogError($"[{Title ?? GetType().FullName}] GUI crash: {ex}");
            }

            WindowRect = IMGUIUtils.DragResizeEatWindow(id, WindowRect);
        }
    }

    private static GUISkin MakeTransparent(GUISkin skin) {
        skin.window.normal.background = CreateColorTexture(ColorUtils.Color32Af(38, 38, 38, 0.85f));
        return skin;
    }

    private static Texture2D CreateColorTexture(Color col) {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, col);
        tex.Apply();
        DontDestroyOnLoad(tex);
        return tex;
    }

    private void DrawSelectionGrid() {
        SelectedTabIndex = GUILayout.SelectionGrid(SelectedTabIndex, FlatTabs, 1, ImGuiPrimitivesLib.VerticalTabsStyle);
    }

    // ReSharper disable Unity.PerformanceAnalysis false positive perf trigger
    private void DrawSectionedTabs() {
        foreach (var data in WindowData) {
            GUILayout.Label(data.Key, ImGuiPrimitivesLib.VerticalTabSectionStyle);

            foreach (var tab in data.Value) {
                if (GUILayout.Toggle(SelectedTabIndex == Array.IndexOf(FlatTabs, tab.Key), tab.Key, ImGuiPrimitivesLib.VerticalTabsStyle))
                    SelectedTabIndex = Array.IndexOf(FlatTabs, tab.Key);
            }

            GUILayout.Space(16);
        }
    }
}
