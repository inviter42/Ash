using System.Globalization;
using UnityEngine;

namespace Ash.Utility.GlobalUtils;

internal static class VectorUtils
{
    internal static float CalculateSignedDistance(Vector3 vectorA, Vector3 vectorB, Vector3 forward) {
        return (vectorB - vectorA).magnitude * Mathf.Sign(Vector3.Dot((vectorB - vectorA).normalized, forward));
    }

    internal static string Vec(Vector3 vector, string format="F6") {
        return $"({vector.x.ToString(format, CultureInfo.InvariantCulture)}, {vector.y.ToString(format, CultureInfo.InvariantCulture)}, {vector.z.ToString(format, CultureInfo.InvariantCulture)})";
    }

    internal static string Rot(Quaternion quaternion, string format="F6") {
        return $"({quaternion.x.ToString(format, CultureInfo.InvariantCulture)}, {quaternion.y.ToString(format, CultureInfo.InvariantCulture)}, {quaternion.z.ToString(format, CultureInfo.InvariantCulture)}, {quaternion.w.ToString(format, CultureInfo.InvariantCulture)})";
    }
}
