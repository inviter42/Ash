using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using Ash.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures
{
    internal class StylesMenuTextures
    {
        internal readonly Texture2D FrostedGlassAtlasMask;
        internal readonly Texture2D FrostGlassTintTexture;
        internal readonly Texture2D XButtonTexture;
        internal readonly Texture2D ScrollbarBackgroundTexture;
        internal readonly Texture2D ScrollbarHandleTexture;
        internal readonly Sprite ScrollListButtonBackgroundSprite;
        internal readonly Texture2D GenreToggleTintTexture;
        internal readonly Texture2D GenreToggleStrokeTexture;
        internal readonly Texture2D FemaleStateToggleTintTexture;
        internal readonly Texture2D FemaleStateToggleStrokeTexture;

        private static readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;

        internal StylesMenuTextures() {
            FrostedGlassAtlasMask = StylesMenuMaskAtlas.CreateMaskAtlas();

            FrostGlassTintTexture = IuiTextureGen.GenerateRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    Config.BackgroundRectSize,
                    Config.BackgroundRectSize,
                    Config.BackgroundRectBorderRadius,
                    ColorUtils.Color32Af(48, 40, 44, 0.35f),
                    Color.clear
                )
            );

            XButtonTexture = IuiTextureGen.GenerateXTexture(
                new IuiTextureGen.XTextureInfo(
                    Config.XButtonSize,
                    Config.XButtonSize,
                    StylesMenuConfig.XButtonLineThickness,
                    ColorUtils.Color32Af(255, 255, 255, 0.3f),
                    Color.clear
                )
            );

            ScrollbarBackgroundTexture = IuiTextureGen.GenerateRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    Config.ScrollbarTextureSize,
                    Config.ScrollbarTextureSize,
                    Config.ScrollbarTextureBorderRadius,
                    ColorUtils.Color32Af(126, 126, 126, 0.2f),
                    Color.clear
                )
            );

            ScrollbarHandleTexture = IuiTextureGen.GenerateRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    Config.ScrollbarTextureSize,
                    Config.ScrollbarTextureSize,
                    Config.ScrollbarTextureBorderRadius,
                    ColorUtils.Color32Af(255, 255, 255, 0.2f),
                    Color.clear
                )
            );

            var scrollListButtonBackgroundTexture = IuiTextureGen.GenerateRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    Config.ScrollListButtonSize,
                    Config.ScrollListButtonSize,
                    Config.ScrollListButtonTextureBorderRadius,
                    Color.white,
                    Color.clear
                )
            );

            ScrollListButtonBackgroundSprite = Sprite.Create(
                scrollListButtonBackgroundTexture,
                new Rect(0, 0, scrollListButtonBackgroundTexture.width, scrollListButtonBackgroundTexture.height),
                new Vector2(0.5f, 0.5f)
            );

            GenreToggleTintTexture = IuiTextureGen.GenerateRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    Config.GenreToggleSize,
                    Config.GenreToggleSize,
                    Config.GenreToggleTextureBorderRadius,
                    ColorUtils.Color32Af(48, 40, 44, 0.15f),
                    Color.clear
                )
            );

            GenreToggleStrokeTexture = IuiTextureGen.GenerateRectangleStrokeTexture(
                new IuiTextureGen.RectTextureInfo(
                    Config.GenreToggleSize
                    + Config.GenreToggleTextureStrokeThickness
                    + Config.GenreToggleAaSafetyPadding,
                    Config.GenreToggleSize + Config.GenreToggleTextureStrokeThickness,
                    Config.GenreToggleTextureBorderRadius,
                    Color.white,
                    Color.clear
                ), Config.GenreToggleTextureStrokeThickness
            );

            FemaleStateToggleTintTexture = IuiTextureGen.GenerateCircleTexture(
                new IuiTextureGen.CircleTextureInfo(
                    Config.FemaleStateToggleSize,
                    Config.FemaleStateToggleSize,
                ColorUtils.Color32Af(48, 40, 44, 0.15f),
                Color.clear
                )
            );

            FemaleStateToggleStrokeTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.FemaleStateToggleSize + Config.FemaleStateButtonTextureStrokeThickness,
                    Config.FemaleStateToggleSize + Config.FemaleStateButtonTextureStrokeThickness,
                    Config.FemaleStateToggleSize.Width - Config.FemaleStateButtonTextureStrokeThickness,
                    Color.white,
                    Color.clear
                ), 1.5f
            );
        }
    }
}
