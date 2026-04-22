using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Config;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Textures
{
    internal static class GagMenuMaskAtlas
    {
        private static readonly GagMenuConfig Config = IuiMain.GagMenuConfig;

        internal static Texture2D CreateMaskAtlas(IuiTextureGen.Size textureSize) {
            var texture = CreateAtlasTexture(textureSize);

            IuiTextureGen.StampRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    textureSize,
                    textureSize,
                    Config.BackgroundRectBorderRadius,
                    Color.white, Color.clear,
                    Vector2.zero
                ), texture
            );

            return texture;
        }

        private static Texture2D CreateAtlasTexture(IuiTextureGen.Size textureSize) {
            var color = new Color[textureSize.Width * textureSize.Height];
            var texture = new Texture2D(textureSize.Width, textureSize.Height, TextureFormat.Alpha8, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            for(var i = 0; i < color.Length; i++)
                color[i] = Color.clear;

            texture.SetPixels(color);
            texture.Apply();

            return texture;
        }
    }
}
