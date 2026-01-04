using System;
using System.Linq;
using Ash.GlobalUtils;
using Character;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.CommonLabels;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    public static class IndividualWearsVisibilitySelectionComponent
    {
        private const string IndividualClothingSubtitle = "Individual Clothing";

        public static void Component(Female activeFemale) {
            Subtitle(IndividualClothingSubtitle);
            var wearsModel = Enum.GetValues(typeof(WEAR_SHOW_TYPE))
                .Cast<WEAR_SHOW_TYPE>()
                .Where(e => e != WEAR_SHOW_TYPE.NUM)
                .ToArray();

            Flow(wearsModel, (item, idx) => Button(
                WearShowTypeLabels.GetValueOrDefaultValue(item, ErrorLabel),
                () => SceneUtils.CycleStateOfClothingItem(activeFemale, item))
            );
        }
    }
}
