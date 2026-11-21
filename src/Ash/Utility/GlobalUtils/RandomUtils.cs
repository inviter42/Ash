using System;
using System.Collections.Generic;
using System.Linq;

namespace Ash.Utility.GlobalUtils;

internal static class RandomUtils
{
    private static readonly Random Random = new();

    internal static int RandomWithExclude(int min, int max, int exclude) {
        if (min > max)
            throw new ArgumentOutOfRangeException(nameof(min), "Min must not exceed Max.");

        var excludedInRange = exclude >= min && exclude <= max;

        // Number of allowed values, computed in long to avoid int overflow.
        var count = (long)max - min + 1;
        if (excludedInRange)
            count--;

        if (count == 0)
            throw new ArgumentException("The range contains only the excluded value.");

        double sample;
        lock (Random)
            sample = Random.NextDouble();

        // Offset is in 0..count-1, so the result starts out in min..max (minus one slot).
        var value = min + (long)(sample * count);

        // Skip over the excluded value; everything at or above it moves up by one.
        if (excludedInRange && value >= exclude)
            value++;

        return (int)value;
    }

    internal static T RandomFromList<T>(List<T> list) {
        lock (Random)
            return list[Random.Next(0, list.Count)];
    }

    public static KeyValuePair<int, float[]> WeightedRandomWithChoiceDecay(float[] weights, float decay) {
        if (weights == null)
            throw new ArgumentNullException(nameof(weights));

        if (weights.Length == 0)
            throw new ArgumentException("Weights must not be empty", nameof(weights));

        if (weights.Any(w => w <= 0f))
            throw new ArgumentException("Weights must not contain negative values", nameof(weights));

        var total = weights
            .Where(weight => weight > 0f)
            .Aggregate(0.0, (current, weight) => current + weight);

        if (Math.Abs(total - 1.0f) > double.Epsilon)
            weights = ArrayUtils.NormalizeSum(weights);

        double roll;
        lock (Random)
            roll = Random.NextDouble();

        roll *= total;

        var cumulative = 0.0;
        var selected = -1;

        for (var i = 0; i < weights.Length; i++) {
            if (weights[i] <= 0f) {
                continue;
            }

            selected = i;
            cumulative += weights[i];

            if (roll < cumulative) {
                break;
            }
        }

        var updated = weights.ToArray();
        updated[selected] *= 1 - decay;

        return new KeyValuePair<int, float[]>(selected, updated);
    }
}
