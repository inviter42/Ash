using Newtonsoft.Json;
using OneOf;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    /// <summary>
    /// This class describe the Master wear item. Its state
    /// will determine the state of all the Slave items it owns.
    /// </summary>
    public class MasterItem : BaseItem
    {
        [JsonConstructor]
        public MasterItem(
            int id,
            string assetbundleName,
            string prefab,
            string name,
            OneOf<DataOfWearItem, DataOfAccessoryItem> data
        ) : base(id, assetbundleName, prefab, name, data) { }
    }
}
