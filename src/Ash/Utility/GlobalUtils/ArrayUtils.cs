using System.Linq;
using UnityEngine;

namespace Ash.Utility.GlobalUtils;

internal static class ArrayUtils
{
    internal static float[] NormalizeSum(float[] array) {
        if (array == null || array.Length == 0)
            return array;

        var sum = array.Sum();

        if (Mathf.Abs(sum) <= Mathf.Epsilon)
            return array;

        var copy = array.ToArray();
        var scale = 1f / sum;

        for (var i = 0; i < array.Length; i++)
            copy[i] *= scale;

        return copy;
    }
}
