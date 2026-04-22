using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures
{
    internal static class StylesMenuMaskAtlas
    {
        private static readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;

        internal static Texture2D CreateMaskAtlas() {
            var genreTogglesContainerOffset = new Vector2(
                Config.GenreTogglesContainerLeftMargin,
                Config.BackgroundRectSize.Height + Config.GenreTogglesContainerBottomMargin
            );

            var femaleStateTogglesContainerOffset = new Vector2(
                Config.BackgroundRectSize.Width + Config.FemaleStateButtonsContainerRightMargin,
                Config.FemaleStateButtonsContainerBottomMargin
            );

            var texture = CreateAtlasTexture(
                genreTogglesContainerOffset,
                femaleStateTogglesContainerOffset
            );

            for (var i = 0; i < StylesMenuConfig.GenreNumberOfToggles; i++) {
                var offset = new Vector2(
                    genreTogglesContainerOffset.x +
                    Config.GenreToggleSize.Width * i +
                    Config.GenreToggleSpacing * i,
                    genreTogglesContainerOffset.y
                );

                IuiTextureGen.StampRectangleTexture(
                    new IuiTextureGen.RectTextureInfo(
                        new IuiTextureGen.Size(texture.width, texture.height),
                        Config.GenreToggleSize,
                        Config.GenreToggleTextureBorderRadius,
                        Color.white, Color.clear,
                        offset
                    ), texture
                );
            }

            for (var i = 0; i < StylesMenuConfig.FemaleStateNumberOfToggles; i++) {
                var offset = new Vector2(
                    femaleStateTogglesContainerOffset.x,
                    femaleStateTogglesContainerOffset.y +
                    Config.FemaleStateToggleSize.Height * i +
                    StylesMenuConfig.FemaleStateToggleSpacing * i
                );

                IuiTextureGen.StampCircleTexture(
                    new IuiTextureGen.CircleTextureInfo(
                        new IuiTextureGen.Size(texture.width, texture.height),
                        Config.FemaleStateToggleSize,
                        Color.white,
                        Color.clear,
                        offset
                    ), texture
                );
            }

            IuiTextureGen.StampRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    new IuiTextureGen.Size(texture.width, texture.height),
                    Config.BackgroundRectSize,
                    Config.BackgroundRectBorderRadius,
                    Color.white, Color.clear,
                    Vector2.zero
                ), texture
            );

            return texture;
        }

        private static Texture2D CreateAtlasTexture(Vector2 genreTogglesContainerOffset, Vector2 femaleStateTogglesContainerOffset) {
            var maxX = femaleStateTogglesContainerOffset.x + Config.FemaleStateToggleSize.Width;
            var maxY = genreTogglesContainerOffset.y + Config.GenreToggleSize.Height;

            var textureSize = new IuiTextureGen.Size((int)maxX, (int)maxY);

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
