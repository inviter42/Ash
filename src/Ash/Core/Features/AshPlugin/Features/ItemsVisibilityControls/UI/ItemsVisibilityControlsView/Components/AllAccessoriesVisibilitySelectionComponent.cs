using Ash.Utility.GlobalUtils;
using UnityEngine;
using static Ash.Utility.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.AshPlugin.Common.Misc.CommonLabels;

namespace Ash.Core.Features.AshPlugin.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    internal static class AllAccessoriesVisibilitySelectionComponent
    {
        private const string AllAccessoriesSubtitle = "All Accessories";

        internal static void Component(Female activeFemale) {
            Subtitle(AllAccessoriesSubtitle);
            using (new GUILayout.HorizontalScope("box")) {
                Button(
                    AccessoryShowLabels.GetValueOrDefaultValue(false, ErrorLabel),
                    () => SceneUtils.ChangeStateOfAllAccessoryItems(activeFemale, false)
                );

                GUILayout.Space(10);

                Button(
                    AccessoryShowLabels.GetValueOrDefaultValue(true, ErrorLabel),
                    () => SceneUtils.ChangeStateOfAllAccessoryItems(activeFemale, true)
                );
            }
        }
    }
}
