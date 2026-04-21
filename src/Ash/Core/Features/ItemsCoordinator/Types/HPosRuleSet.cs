using System;
using Character;
using H;
using Newtonsoft.Json;
using OneOf;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    internal class HPosRuleSet : IEquatable<HPosRuleSet>
    {
        [JsonProperty("HPosItem")]
        internal readonly OneOf<BaseItem, WEAR_SHOW_TYPE> HPosItem;

        [JsonProperty("HPosStyle")]
        internal readonly OneOf<H_StyleData.TYPE, HStyleDetail> HPosStyle;

        [JsonConstructor]
        internal HPosRuleSet(OneOf<BaseItem, WEAR_SHOW_TYPE> hPosItem,
            OneOf<H_StyleData.TYPE, HStyleDetail> hPosStyle) {
            HPosItem = hPosItem;
            HPosStyle = hPosStyle;
        }

        public bool Equals(HPosRuleSet other) {
            if (other is null)
                return false;

            return HPosItem.Equals(other.HPosItem) && HPosStyle.Equals(other.HPosStyle);
        }

        public override bool Equals(object obj) {
            return obj is HPosRuleSet other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (HPosItem.GetHashCode() * 397) ^ HPosStyle.GetHashCode();
            }
        }

        public static bool operator ==(HPosRuleSet left, HPosRuleSet right) {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(HPosRuleSet left, HPosRuleSet right) {
            return !(left == right);
        }
    }
}
