using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    internal class InterItemRuleSet
    {
        [JsonProperty("MasterItem")]
        internal readonly BaseItem MasterItem;

        [JsonProperty("SlaveItems")]
        internal readonly HashSet<SlaveItem> SlaveItems;

        [JsonConstructor]
        internal InterItemRuleSet(BaseItem masterItem, HashSet<SlaveItem> slaveItems) {
            MasterItem = masterItem;
            SlaveItems = slaveItems;
        }
    }
}
