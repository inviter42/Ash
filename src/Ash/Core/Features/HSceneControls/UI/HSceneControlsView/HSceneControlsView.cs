using Ash.GlobalUtils;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;

namespace Ash.Core.Features.HSceneControls.UI.HSceneControlsView
{
    internal class HSceneControlsView
    {
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void DrawFemaleTabContents(string heroineIDString) {
            using (new GUILayout.HorizontalScope("box")) {
                Button("Remove sperm", () => {
                    var heroineIds = SceneUtils.GetHeroineIDsInSceneAsStrings();
                    if (heroineIds.Length == 0)
                        return;

                    var femaleComponent = SceneUtils.GetFemaleComponentByHeroineIDString(heroineIDString);
                    if (femaleComponent != null)
                        femaleComponent.ClearSpermMaterials();
                });
            }
        }
    }
}
