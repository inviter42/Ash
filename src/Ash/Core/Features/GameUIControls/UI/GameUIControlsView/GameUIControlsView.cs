using Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;

namespace Ash.Core.Features.GameUIControls.UI.GameUIControlsView
{
    internal class GameUIControlsView
    {
        // Tab Labels
        public const string UIControlsLabel = "UI Controls";

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void DrawView() {
            using (new GUILayout.HorizontalScope("box")) {
                Button("Immersive UI", InGameUIManagementHelper.ImmersiveUIMode);

                GUILayout.Space(10);

                Button("Full UI", InGameUIManagementHelper.DefaultUIMode);
            }
        }
    }
}
