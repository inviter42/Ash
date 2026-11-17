using System;

namespace Ash.GlobalUtils
{
    internal static class EnumUtils
    {
        internal static T GetNextEnumValue<T>(T currentValue) where T : Enum {
            var values = Enum.GetValues(typeof(T));
            var currentIndex = Array.IndexOf(values, currentValue);
            var nextIndex = (currentIndex + 1) % values.Length;

            return (T)values.GetValue(nextIndex);
        }

        internal static T GetPreviousEnumValue<T>(T currentValue) where T : Enum {
            var values = Enum.GetValues(typeof(T));
            var currentIndex = Array.IndexOf(values, currentValue);
            var previousIndex = (currentIndex - 1) % values.Length;

            return (T)values.GetValue(previousIndex);
        }
    }
}
