using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures
{
    internal static class IuiSdfs
    {
        internal static float CalculateCircleSdf(Vector2 point, Vector2 center, float radius) {
            return Vector2.Distance(point, center) - radius;
        }

        internal static float CalculateRoundedRectSdf(float dx, float dy, float halfW, float halfH, float r) {
            r = Mathf.Min(r, Mathf.Min(halfW, halfH));

            var cX = Mathf.Max(dx - (halfW - r), 0.0f);
            var cY = Mathf.Max(dy - (halfH - r), 0.0f);

            var distance = Mathf.Sqrt(cX * cX + cY * cY);

            return distance - r;
        }
    }
}
