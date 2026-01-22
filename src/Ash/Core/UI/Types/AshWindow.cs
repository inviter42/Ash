using System;
using System.Collections.Generic;
using KKAPI.Utilities;
using UnityEngine;

namespace Ash.Core.UI.Types
{
    public abstract class AshWindow<T> : ImguiWindow<T> where T : AshWindow<T>
    {
        private int SelectedTabIndex { get; set; }
        private string[] ToolbarTabs { get; set; } = { };

        public float windowWidth = 800;
        public float windowHeight = 600;

        protected abstract string[] SetTabsModel();

        protected abstract List<Action> SetTabViewsModel();

        protected new virtual void OnGUI()
        {
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

            GUI.skin = skin;
        }

        // Verbatim from KKAPI.Utilities
        private void DrawContentsInt(int id)
        {
            var num = GUI.skin.window.border.top - 4;
            if (GUI.Button(new Rect((float) (WindowRect.width - (double) num - 2.0), 2f, num, num), "x"))
            {
                enabled = false;
            }
            else
            {
                try
                {
                    DrawContents();
                    IMGUIUtils.DrawTooltip(WindowRect);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("GUILayout"))
                        Ash.Logger.LogError($"[{Title ??GetType().FullName}] GUI crash: {ex}");
                }
                WindowRect = IMGUIUtils.DragResizeEatWindow(id,WindowRect);
            }
        }

        private static GUISkin MakeTransparent(GUISkin skin)
        {
            skin.window.normal.background = CreateColorTexture(new Color(0.15f, 0.15f, 0.15f, 0.85f));
            return skin;
        }

        private static Texture2D CreateColorTexture(Color col)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, col);
            tex.Apply();
            DontDestroyOnLoad(tex);
            return tex;
        }

        protected override Rect GetDefaultWindowRect(Rect screenRect) {
            return new Rect(
                (float)Screen.width / 2 - windowWidth / 2,
                (float)Screen.height / 2 - windowHeight / 2,
                windowWidth, windowHeight
            );
        }

        protected override void DrawContents() {
            ToolbarTabs = SetTabsModel();
            SelectedTabIndex = GUILayout.Toolbar(SelectedTabIndex, ToolbarTabs, AshUI.ToolbarStyle);

            var tabViewsModel = SetTabViewsModel();
            if (tabViewsModel.Count == 0) {
                GUILayout.Label("No views found.");
                return;
            }

            tabViewsModel[SelectedTabIndex].Invoke();
        }
    }
}
