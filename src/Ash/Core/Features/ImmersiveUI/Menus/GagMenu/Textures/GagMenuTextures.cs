using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Config;
using Ash.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Textures
{
    internal class GagMenuTextures
    {
        internal readonly Texture2D FrostedGlassAtlasMask;
        internal readonly Texture2D FrostGlassTintTexture;
        internal readonly Texture2D XButtonTexture;
        internal readonly Texture2D GagToggleStrokeTexture;

        private readonly GagMenuConfig Config = IuiMain.GagMenuConfig;

        internal GagMenuTextures(IuiTextureGen.Size bgTextureSize) {
            FrostedGlassAtlasMask = GagMenuMaskAtlas.CreateMaskAtlas(bgTextureSize);

            FrostGlassTintTexture = IuiTextureGen.GenerateRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    bgTextureSize,
                    bgTextureSize,
                    Config.BackgroundRectBorderRadius,
                    ColorUtils.Color32Af(48, 40, 44, 0.35f),
                    Color.clear
                )
            );

            XButtonTexture = IuiTextureGen.GenerateXTexture(
                new IuiTextureGen.XTextureInfo(
                    Config.XButtonSize,
                    Config.XButtonSize,
                    Config.XButtonLineThickness,
                    ColorUtils.Color32Af(255, 255, 255, 0.3f),
                    Color.clear
                )
            );

            GagToggleStrokeTexture = IuiTextureGen.GenerateRectangleStrokeTexture(
                new IuiTextureGen.RectTextureInfo(
                    new IuiTextureGen.Size(
                        Config.GagToggleSize.Width
                        + Config.GagToggleTextureStrokeThickness
                        + Config.GagToggleAaSafetyPadding,
                        Config.GagToggleSize.Height
                        + Config.GagToggleTextureStrokeThickness
                        + Config.GagToggleAaSafetyPadding
                    ),
                    new IuiTextureGen.Size(
                        Config.GagToggleSize.Width + Config.GagToggleTextureStrokeThickness,
                        Config.GagToggleSize.Height + Config.GagToggleTextureStrokeThickness
                    ),
                    Config.GagToggleTextureBorderRadius,
                    Color.white,
                    Color.clear
                ), Config.GagToggleTextureStrokeThickness
            );
        }
    }
}
