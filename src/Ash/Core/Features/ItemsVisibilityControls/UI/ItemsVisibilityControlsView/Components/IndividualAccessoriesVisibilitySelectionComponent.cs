using System.Linq;
using Ash.GlobalUtils;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components
{
    internal static class IndividualAccessoriesVisibilitySelectionComponent
    {
        private const string IndividualAccessoriesSubtitle = "Individual Accessories";

        internal static void Component(Female activeFemale) {
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

            var extendedModel = Ash.MoreAccessoriesInstance
                .GetAdditionalData(activeFemale.customParam)
                .accessories
                .Where(accessoryData => accessoryData?.acceObj != null)
                .ToArray();

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
