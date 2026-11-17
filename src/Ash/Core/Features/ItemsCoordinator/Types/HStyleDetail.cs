using System;
using H;
using Newtonsoft.Json;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    internal class HStyleDetail : IEquatable<HStyleDetail>
    {
        [JsonProperty("Type")]
        internal readonly H_StyleData.TYPE Type;

        [JsonProperty("Detail")]
        internal readonly H_StyleData.DETAIL Detail;

        [JsonConstructor]
        internal HStyleDetail(H_StyleData.TYPE type, H_StyleData.DETAIL detail) {
            Type = type;
            Detail = detail;
        }

        public bool Equals(HStyleDetail other) {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

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
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(HStyleDetail left, HStyleDetail right) {
            return !(left == right);
        }
    }
}
