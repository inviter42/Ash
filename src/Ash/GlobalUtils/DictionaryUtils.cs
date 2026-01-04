using System.Collections.Generic;

namespace Ash.GlobalUtils
{
    internal static class DictionaryUtils
    {
        public static T GetValueOrDefaultValue<TS, T>(this Dictionary<TS, T> dictionary, TS key, T defaultValue) {
            return dictionary.TryGetValue(key, out var value)
                ? value
                : defaultValue;
        }
    }
}
