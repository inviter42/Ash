using System.Collections.Generic;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    public struct InterItemRuleSet
    {
        public BaseItem MasterItem;
        public HashSet<SlaveItem> SlaveItems;
    }
}
