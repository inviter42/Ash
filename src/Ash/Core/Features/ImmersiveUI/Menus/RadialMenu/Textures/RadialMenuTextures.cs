using System.Collections.Generic;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Config;
using Ash.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Textures
{
    internal class RadialMenuTextures
    {
        internal readonly Texture2D FrostGlassTintTexture;
        internal readonly Texture2D FrostedGlassMask;
        internal readonly Texture2D IconBackgroundTexture;
        internal readonly Texture2D ActiveElementStrokeTexture;
        internal readonly Texture2D MiddleBackgroundTexture;
        internal readonly Texture2D MiddleHighlightTexture;
        internal readonly Texture2D SectorHighlightTexture;

        private static readonly RadialMenuConfig Config = IuiMain.RadialMenuConfig;

        internal RadialMenuTextures(List<string> iconPathsFromBundle) {
            FrostGlassTintTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.FrostedGlassOuterDiameterSize,
                    Config.FrostedGlassOuterDiameterSize,
                    Config.FrostedGlassInnerDiameterSize.Width,
                    ColorUtils.Color32Af(48, 40, 44, 0.35f),
                    Color.clear
                )
            );

            FrostedGlassMask = RadialMenuAtlasMask.CreateMaskAtlas();

            IconBackgroundTexture = IuiTextureGen.GenerateCircleTexture(
                new IuiTextureGen.CircleTextureInfo(
                    Config.IconBackgroundSize,
                    Config.IconBackgroundSize,
                    ColorUtils.Color32Af(31, 31, 31, 0.35f),
                    Color.clear
                )
            );

            ActiveElementStrokeTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.ActiveStrokeSize,
                    Config.ActiveStrokeSize,
                    Config.IconBackgroundSize.Width,
                    ColorUtils.Color32Af(217, 217, 217),
                    Color.clear
                ),
                2f
            );

            MiddleBackgroundTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.FrostedGlassInnerDiameterSize,
                    Config.FrostedGlassInnerDiameterSize,
                    0,
                    ColorUtils.Color32Af(0, 0, 0, 0.15f),
                    Color.clear
                )
            );

            MiddleHighlightTexture = IuiTextureGen.GenerateRingGradientTexture(
                new IuiTextureGen.GradientRingTextureInfo(
                    Config.FrostedGlassInnerDiameterSize + Config.MiddleHighlightAaCompensationThickness,
                    Config.FrostedGlassInnerDiameterSize + Config.MiddleHighlightAaCompensationThickness,
                    Config.FrostedGlassInnerDiameterSize.Width - Config.MiddleHighlightThickness,
                    IuiTextureGen.CreateLinearGradient(
                        ColorUtils.Color32Af(159, 159, 159, 0.125f),
                        ColorUtils.Color32Af(255, 255, 255, 0f),
                        0.05f
                    ),
                    Color.clear
                )
            );

            SectorHighlightTexture = IuiTextureGen.GenerateSectorTexture(
                new IuiTextureGen.SectorTextureInfo(
                    Config.FrostedGlassOuterDiameterSize,
                    Config.FrostedGlassOuterDiameterSize,
                    Config.FrostedGlassInnerDiameterSize.Width,
                    ColorUtils.Color32Af(0, 0, 0, 0.45f),
                    Color.clear,
                    -(360f / iconPathsFromBundle.Count) / 2,
                    360f / iconPathsFromBundle.Count / 2
                )
            );
        }
    }
}
