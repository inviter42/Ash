using System;
using Newtonsoft.Json;
using OneOf;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    public class BaseItem : IEquatable<BaseItem>
    {
        public int Id { get; }

        public string AssetbundleName { get; }

        public string Prefab { get; }

        public string Name { get; }

        [JsonProperty("ItemData")]
        public readonly OneOf<DataOfWearItem, DataOfAccessoryItem> ItemData;

        [JsonConstructor]
        public BaseItem(int id, string assetbundleName, string prefab, string name,
            OneOf<DataOfWearItem, DataOfAccessoryItem> data) {
            Id = id;
            AssetbundleName = assetbundleName;
            Prefab = prefab;
            Name = name;
            ItemData = data;
        }

        public override bool Equals(object obj) {
            return obj is BaseItem item && Equals(item);
        }

        public bool Equals(BaseItem other) {
            return other != null
                   && Id == other.Id
                   && Equals(AssetbundleName, other.AssetbundleName)
                   && Equals(Prefab, other.Prefab)
                   && Equals(ItemData, other.ItemData);
        }

        public override int GetHashCode() {
            var hash = 17;
            hash = hash * 23 + Id.GetHashCode();

            if (AssetbundleName != null)
                hash = hash * 23 + AssetbundleName.GetHashCode();

            if (Prefab != null)
                hash = hash * 23 + Prefab.GetHashCode();

            if (!ReferenceEquals(ItemData.Value, null))
                hash = hash * 23 + ItemData.GetHashCode();

            return hash;
        }

        public static bool operator ==(BaseItem left, BaseItem right) {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(BaseItem left, BaseItem right) {
            return !(left == right);
        }
    }
}
