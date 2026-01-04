using Ash.GlobalUtils;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.CommonLabels;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    public static class AllAccessoriesVisibilitySelectionComponent
    {
        private const string AllAccessoriesSubtitle = "All Accessories";

        public static void Component(Female activeFemale) {
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
