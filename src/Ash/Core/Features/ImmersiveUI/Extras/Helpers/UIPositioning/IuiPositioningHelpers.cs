using System.Collections.Generic;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning
{
    internal static class IuiPositioningHelpers
    {

        internal static RectTransform AnchorsFillParent(GameObject gameObject, Vector2 margins = default) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.sizeDelta = margins;

            return rectTransform;
        }

        internal static RectTransform AnchorsLeftTop(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static RectTransform AnchorsCenterTop(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static RectTransform AnchorsRightTop(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static RectTransform AnchorsLeftCenter(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0, 0.5f);
            rectTransform.anchorMax = new Vector2(0, 0.5f);
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static RectTransform AnchorsCenterIn(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static RectTransform AnchorsRightCenter(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(1, 0.5f);
            rectTransform.anchorMax = new Vector2(1, 0.5f);
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static RectTransform AnchorsLeftBottom(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static RectTransform AnchorsCenterBottom(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static RectTransform AnchorsRightBottom(GameObject gameObject, Vector2 offsets = default,
            Vector2? pivot = null) {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (!rectTransform)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.anchoredPosition = offsets;
            rectTransform.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            return rectTransform;
        }

        internal static void RotateToIconAtIndex(List<string> iconPathsFromBundle, int index, RectTransform rt) {
            if (index < 0 || index >= iconPathsFromBundle.Count) {
                Ash.Logger.LogDebug("Icon index is out of range.");
                return;
            }

            var sectorSweepAngle = 360f / iconPathsFromBundle.Count;
            var iconAngle = index * sectorSweepAngle;

            // no animation version for debug
            // rt.rotation = Quaternion.Euler(0f, 0f, -iconAngle);

            rt.rotation = Quaternion.Slerp(rt.rotation, Quaternion.Euler(0, 0, -iconAngle),
                Time.deltaTime * IuiSettings.RadialMenuHighlightAnimationSpeed);
        }

        internal static void RotateToAngle(RectTransform rt, float angle) {
            if (angle < 0)
                angle += 360;

            if (angle >= 360f)
                angle -= 360;

            rt.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
