using System;
using System.Collections.Generic;
using KKAPI.Utilities;
using UnityEngine;

namespace Ash.Core.UI.Types
{
    public abstract class AshWindow<T> : ImguiWindow<T> where T : AshWindow<T>
    {
        protected int SelectedTabIndex { get; private set; }
        protected string[] ToolbarTabs { get; private set; } = { };

        public float windowWidth = 800;
        public float windowHeight = 600;

        protected abstract string[] SetTabsModel();

        protected abstract List<Action> SetTabViewsModel();

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
