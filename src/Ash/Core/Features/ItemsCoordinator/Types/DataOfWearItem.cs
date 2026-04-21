using System;
using Character;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    // must be public, part of JSON serialization
    public class DataOfWearItem: IEquatable<DataOfWearItem>
    {
        public WEAR_SHOW_TYPE ItemPart { get; }

        public DataOfWearItem(WEAR_SHOW_TYPE itemPart) {
            if (!Enum.IsDefined(typeof(WEAR_SHOW_TYPE), itemPart)) {
                throw new ArgumentOutOfRangeException(nameof(itemPart),
                    $"Invalid {nameof(WEAR_SHOW_TYPE)} value: {itemPart}");
            }

            ItemPart = itemPart;
        }

        public bool Equals(DataOfWearItem other) {
            if (other is null)
                return false;

            return Equals(ItemPart, other.ItemPart);
        }

        public override bool Equals(object obj) {
            return obj is DataOfWearItem item && Equals(item);
        }

        public override int GetHashCode() {
            unchecked {
                // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
                var hash = 17;
                hash = hash * 23 + ItemPart.GetHashCode();

                return hash;
            }
        }

        public static bool operator ==(DataOfWearItem left, DataOfWearItem right) {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(DataOfWearItem left, DataOfWearItem right) {
            return !(left == right);
        }
    }
}
