using System.Linq;
using Ash.GlobalUtils;
using static Ash.GlobalUtils.GuiPrimitivesLib;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    public static class IndividualAccessoriesVisibilitySelectionComponent
    {
        private const string IndividualAccessoriesSubtitle = "Individual Accessories";

        public static void Component(Female activeFemale) {
            Subtitle(IndividualAccessoriesSubtitle);

            var accessoriesModel = activeFemale.accessories.acceObjs
                .Where(accessoryObj => accessoryObj != null)
                .ToArray();

            Flow(accessoriesModel, (accessoryObj, idx) => {
                var accessoryData =
                    activeFemale.accessories.GetAccessoryData(accessoryObj.acceParam, accessoryObj.slot);
                Button(
                    $"{accessoryObj.slot}: " +
                    $"{AutoTranslatorIntegration.Translate(accessoryData.name)}",
                    () => SceneUtils.CycleStateOfAccessoryItem(activeFemale, accessoryObj.slot)
                );
            }, 3);

            if (Ash.MoreAccessoriesInstance == null)
                return;

            var extendedModel = Ash.MoreAccessoriesInstance.GetAdditionalData(activeFemale.customParam).accessories;

            Flow(extendedModel, (maAccessoryData, idx) => {
                var accessoryObj = (Accessories.AcceObj)maAccessoryData.acceObj;
                var accessoryData =
                    activeFemale.accessories.GetAccessoryData(accessoryObj.acceParam, accessoryObj.slot);
                Button(
                    $"{accessoryObj.slot}: " +
                    $"{AutoTranslatorIntegration.Translate(accessoryData.name)}",
                    () => SceneUtils.CycleStateOfAccessoryItem(activeFemale, accessoryObj.slot)
                );
            }, 3);
        }
    }
}
