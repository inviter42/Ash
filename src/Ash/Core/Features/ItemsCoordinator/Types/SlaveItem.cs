using Character;
using Newtonsoft.Json;
using OneOf;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    /// <summary>
    /// This class describes the Slave wear item. Its state
    /// is affected by the Master item it is owned by.
    /// </summary>
    public class SlaveItem : BaseItem
    {
        [JsonProperty("MasterItemState")]
        public readonly OneOf<WEAR_SHOW, bool> MasterItemState;

        [JsonProperty("SlaveItemState")]
        public readonly OneOf<WEAR_SHOW, bool> SlaveItemState;

        [JsonConstructor]
        public SlaveItem(int id,
            string assetbundleName,
            string prefab,
            string name,
            OneOf<WEAR_SHOW, bool> masterItemState,
            OneOf<WEAR_SHOW, bool> slaveItemState,
            OneOf<DataOfWearItem, DataOfAccessoryItem> data
        ) : base(id, assetbundleName, prefab, name, data) {
            MasterItemState = masterItemState;
            SlaveItemState = slaveItemState;
        }

        public override bool Equals(object obj) {
            return Equals(obj as SlaveItem);
        }

        public bool Equals(SlaveItem other) {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!base.Equals(other))
                return false;

            return MasterItemState.Equals(other.MasterItemState)
                   && SlaveItemState.Equals(other.SlaveItemState);
        }

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + base.GetHashCode();
                hash = hash * 23 + MasterItemState.GetHashCode();
                hash = hash * 23 + SlaveItemState.GetHashCode();

                return hash;
            }
        }
    }
}
