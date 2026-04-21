using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Config;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Textures
{
    internal static class RadialMenuAtlasMask
    {
        private static readonly RadialMenuConfig Config = IuiMain.RadialMenuConfig;

        internal static Texture2D CreateMaskAtlas() {
            var texture = CreateAtlasTexture();

            IuiTextureGen.StampRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.FrostedGlassOuterDiameterSize,
                    Config.FrostedGlassOuterDiameterSize,
                    Config.FrostedGlassInnerDiameterSize.Width,
                    Color.white,
                    Color.clear
                ), texture
            );

            return texture;
        }

        private static Texture2D CreateAtlasTexture() {
            var color = new Color[Config.FrostedGlassOuterDiameterSize.Width *
                                  Config.FrostedGlassOuterDiameterSize.Height];

            var texture = new Texture2D(Config.FrostedGlassOuterDiameterSize.Width,
                Config.FrostedGlassOuterDiameterSize.Height, TextureFormat.Alpha8, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            for (var i = 0; i < color.Length; i++)
                color[i] = Color.clear;

            texture.SetPixels(color);
            texture.Apply();

            return texture;
        }
    }
}
