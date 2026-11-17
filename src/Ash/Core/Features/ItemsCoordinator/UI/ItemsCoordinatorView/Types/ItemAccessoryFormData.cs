using Character;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types
{
    internal class ItemAccessoryFormData
    {
        internal readonly int SlotNo;
        internal readonly AccessoryParameter AccessoryParameter;
        internal readonly AccessoryData AccessoryData;

        internal ItemAccessoryFormData(int slotNo, AccessoryParameter accessoryParameter, AccessoryData accessoryData) {
            SlotNo = slotNo;
            AccessoryParameter = accessoryParameter;
            AccessoryData = accessoryData;
        }
    }
}
