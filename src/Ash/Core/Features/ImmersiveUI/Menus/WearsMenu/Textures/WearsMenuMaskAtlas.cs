using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures
{
    internal static class WearsMenuMaskAtlas
    {
        private static readonly WearsMenuConfig Config = IuiMain.WearsMenuConfig;

        internal static Texture2D CreateMaskAtlas() {
            var texture = CreateAtlasTexture();

            IuiTextureGen.StampRingGradientTexture(
                new IuiTextureGen.GradientRingTextureInfo(
                    new IuiTextureGen.Size(texture.width, texture.height),
                    Config.AccessoryPointerEventZoneOuterDiameter,
                    Config.AccessoryPointerEventZoneInnerDiameter.Width,
                    IuiTextureGen.CreateLinearGradient(
                        Color.white, Color.clear,
                        0.08f
                    ),
                    Color.clear,
                    -90f,
                    new Vector2(-(float)Config.AccessoryPointerEventZoneOuterDiameter.Width / 2, 0)
                ), texture
            );

            return texture;
        }

        private static Texture2D CreateAtlasTexture() {
            var textureSize = new IuiTextureGen.Size(
                Config.AccessoryPointerEventZoneOuterDiameter.Width / 2,
                Config.AccessoryPointerEventZoneOuterDiameter.Height
            );

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
