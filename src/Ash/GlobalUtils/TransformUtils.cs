using UnityEngine;

namespace Ash.GlobalUtils
{
    internal static class TransformUtils
    {
        internal static Bounds CalculateBoundingBoxSize(GameObject gameObject, bool ignoreInactive = true) {
            var min = new Vector3(float.MaxValue, float.MaxValue, 0);
            var max = new Vector3(float.MinValue, float.MinValue, 0);

            foreach (RectTransform child in gameObject.transform) {
                if (ignoreInactive && !child.gameObject.activeSelf)
                    continue;

                var corners = new Vector3[4];
                child.GetLocalCorners(corners);

                foreach (var corner in corners) {
                    var worldCorner = child.localPosition + corner;
                    min = Vector3.Min(min, worldCorner);
                    max = Vector3.Max(max, worldCorner);
                }
            }

            return new Bounds(
                (max - min) / 2,
                max - min
            );
        }

        internal static Bounds CalculateBoundingBoxSizeFromCenter(GameObject gameObject) {
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(gameObject.transform);

            var maxAbsX = Mathf.Max(
                Mathf.Abs(bounds.min.x),
                Mathf.Abs(bounds.max.x)
            );

            var maxAbsY = Mathf.Max(
                Mathf.Abs(bounds.min.y),
                Mathf.Abs(bounds.max.y)
            );

            return new Bounds(
                new Vector3(maxAbsX, maxAbsY, 0) / 2,
                new Vector3(maxAbsX * 2, maxAbsY * 2, 0)
            );
        }
    }
}
