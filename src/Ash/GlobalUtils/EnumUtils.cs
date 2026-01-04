using System;

namespace Ash.GlobalUtils
{
    internal static class EnumUtils
    {
        public static T GetNextEnumValue<T>(T currentValue) where T : Enum {
            var values = Enum.GetValues(typeof(T));
            var currentIndex = Array.IndexOf(values, currentValue);
            var nextIndex = (currentIndex + 1) % values.Length;

            return (T)values.GetValue(nextIndex);
        }
    }
}
