using Ash.GlobalUtils;
using Character;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    public static class AllClothingVisibilitySelectionComponent
    {
        private const string AllClothingSubtitle = "All Clothes";

        public static void Component(Female activeFemale) {
            Subtitle(AllClothingSubtitle);
            using (new GUILayout.HorizontalScope("box")) {
                Button(
                    WearShowLabels.GetValueOrDefaultValue(WEAR_SHOW.HIDE, ErrorLabel),
                    () => SceneUtils.ChangeStateOfAllClothingItems(activeFemale, WEAR_SHOW.HIDE)
                );

                GUILayout.Space(10);

                Button(
                    WearShowLabels.GetValueOrDefaultValue(WEAR_SHOW.HALF, ErrorLabel),
                    () => SceneUtils.ChangeStateOfAllClothingItems(activeFemale, WEAR_SHOW.HALF)
                );

                GUILayout.Space(10);

                Button(
                    WearShowLabels.GetValueOrDefaultValue(WEAR_SHOW.ALL, ErrorLabel),
                    () => SceneUtils.ChangeStateOfAllClothingItems(activeFemale, WEAR_SHOW.ALL)
                );
            }
        }
    }
}
