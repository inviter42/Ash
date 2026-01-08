using System;
using H;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    public struct HStyleDetail : IEquatable<HStyleDetail>
    {
        public H_StyleData.TYPE Type;
        public H_StyleData.DETAIL Detail;

        public bool Equals(HStyleDetail other) {
            return Type == other.Type && Detail == other.Detail;
        }

        public override bool Equals(object obj) {
            return obj is HStyleDetail other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return ((int)Type * 397) ^ (int)Detail;
            }
        }

        public static bool operator ==(HStyleDetail left, HStyleDetail right) {
            return left.Equals(right);
        }

        public static bool operator !=(HStyleDetail left, HStyleDetail right) {
            return !left.Equals(right);
        }
    }
}
