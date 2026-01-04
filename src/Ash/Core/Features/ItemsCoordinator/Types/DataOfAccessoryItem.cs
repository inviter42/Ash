using System;
using Character;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    public class DataOfAccessoryItem: IEquatable<DataOfAccessoryItem>
    {
        public int SlotNo { get; }

        public ACCESSORY_TYPE Type { get; }

        public ACCESSORY_ATTACH NowAttach { get; }

        public DataOfAccessoryItem(int slotNo, ACCESSORY_TYPE type, ACCESSORY_ATTACH nowAttach) {
            if (slotNo < 0) {
                throw new ArgumentOutOfRangeException(nameof(slotNo),
                    $"Invalid slot value: {slotNo}");
            }

            if (!Enum.IsDefined(typeof(ACCESSORY_TYPE), type)) {
                throw new ArgumentOutOfRangeException(nameof(type),
                    $"Invalid {nameof(ACCESSORY_TYPE)} value: {type}");
            }

            if (!Enum.IsDefined(typeof(ACCESSORY_ATTACH), nowAttach)) {
                throw new ArgumentOutOfRangeException(nameof(nowAttach),
                    $"Invalid {nameof(ACCESSORY_ATTACH)} value: {nowAttach}");
            }

            SlotNo = slotNo;
            Type = type;
            NowAttach = nowAttach;
        }

        public override bool Equals(object obj) {
            return obj is DataOfAccessoryItem item && Equals(item);
        }

        public bool Equals(DataOfAccessoryItem other) {
            return Equals(SlotNo, other?.SlotNo)
                   && Equals(Type, other?.Type)
                   && Equals(NowAttach, other?.NowAttach);
        }

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + SlotNo.GetHashCode();
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + NowAttach.GetHashCode();

                return hash;
            }
        }

        public static bool operator ==(DataOfAccessoryItem left, DataOfAccessoryItem right) {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(DataOfAccessoryItem left, DataOfAccessoryItem right) {
            return !(left == right);
        }
    }
}
