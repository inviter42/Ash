using Ash.GlobalUtils;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    public static class IndividualWearsVisibilitySelectionComponent
    {
        private const string IndividualClothingSubtitle = "Individual Clothing";

        public static void Component(Female activeFemale) {
            Subtitle(IndividualClothingSubtitle);
            var wearsModel = SceneUtils.GetActiveWearShowTypes(activeFemale);
            Flow(wearsModel, (item, idx) => Button(
                WearShowTypeLabels.GetValueOrDefaultValue(item, ErrorLabel),
                () => SceneUtils.CycleStateOfClothingItem(activeFemale, item))
            );
        }
    }
}
