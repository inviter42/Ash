using System;
using OneOf;
using Character;
using H;

namespace Ash.Core.Features.ItemsCoordinator.Types
{
    public struct HPosRuleData : IEquatable<HPosRuleData>
    {
        public OneOf<BaseItem, WEAR_SHOW_TYPE> HPosItem;
        public OneOf<H_StyleData.TYPE, HStyleDetail> HPosStyle;

        public bool Equals(HPosRuleData other) {
            return HPosItem.Equals(other.HPosItem) && HPosStyle.Equals(other.HPosStyle);
        }

        public override bool Equals(object obj) {
            return obj is HPosRuleData other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (HPosItem.GetHashCode() * 397) ^ HPosStyle.GetHashCode();
            }
        }

        public static bool operator ==(HPosRuleData left, HPosRuleData right) {
            return left.Equals(right);
        }

        public static bool operator !=(HPosRuleData left, HPosRuleData right) {
            return !left.Equals(right);
        }
    }
}
