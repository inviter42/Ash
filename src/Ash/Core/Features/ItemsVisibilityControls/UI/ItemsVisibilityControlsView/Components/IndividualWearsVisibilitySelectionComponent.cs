using Ash.GlobalUtils;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    internal static class IndividualWearsVisibilitySelectionComponent
    {
        private const string IndividualClothingSubtitle = "Individual Clothing";

        internal static void Component(Female activeFemale) {
            Subtitle(IndividualClothingSubtitle);
            var wearsModel = SceneUtils.GetWearShowTypesOfEquippedItems(activeFemale);
            Flow(wearsModel, (item, idx) => Button(
                WearShowTypeLabels.GetValueOrDefaultValue(item, ErrorLabel),
                () => SceneUtils.CycleStateOfWearItem(activeFemale, item))
            );
        }
    }
}
