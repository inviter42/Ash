using UnityEngine;

namespace Ash.Utility.GlobalUtils
{
    internal static class TextureUtils
    {
        internal static Texture2D SaveToTexture2D(RenderTexture renderTexture) {
            if (renderTexture == null)
                return null;

            var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;

            return texture;
        }
    }
}
