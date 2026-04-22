using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.Accessories
{
    internal class Alpha8RaycastFilter : MonoBehaviour, ICanvasRaycastFilter
    {
        internal float AlphaHitTestMinimumThreshold { get; set; }
        internal Texture2D SampleTexture { get; set; }
        internal Image Image { get; set; }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
            if (SampleTexture == null)
                return true;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Image.rectTransform,
                screenPoint,
                eventCamera,
                out var localPoint
            );

            var rect = Image.rectTransform.rect;
            var x = (localPoint.x - rect.x) / rect.width;
            var y = (localPoint.y - rect.y) / rect.height;

            try {
                var alpha = SampleTexture.GetPixelBilinear(x, y).a;
                return alpha >= AlphaHitTestMinimumThreshold;
            }
            catch {
                Ash.Logger.LogError($"Unable to read the texture.");
                return true;
            }
        }
    }
}
