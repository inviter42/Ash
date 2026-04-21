using Character;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types
{
    internal class ItemWearFormData
    {
        internal readonly WEAR_SHOW_TYPE Type;
        internal readonly WearData WearData;

        internal ItemWearFormData(WEAR_SHOW_TYPE type, WearData wearData) {
            Type = type;
            WearData = wearData;
        }
    }
}
