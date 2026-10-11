using UnityEngine;

namespace Ash.Utility.GlobalUtils
{
    internal static class VectorUtils
    {
        internal static float CalculateSignedDistance(Vector3 vectorA, Vector3 vectorB, Vector3 forward) {
            return (vectorB - vectorA).magnitude * Mathf.Sign(Vector3.Dot((vectorB - vectorA).normalized, forward));
        }
    }
}
