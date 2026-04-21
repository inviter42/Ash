using Ash.Core.Features.ImmersiveUI.Components.Config;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Components.Textures
{
    internal class SharedTextures
    {
        internal readonly Texture2D RoundButtonTintTexture;
        internal readonly Texture2D RoundButtonStrokeTexture;

        private static readonly GenericComponentsConfig Config = IuiMain.GenericComponentsConfig;

        internal SharedTextures() {
            RoundButtonTintTexture = IuiTextureGen.GenerateCircleTexture(
                new IuiTextureGen.CircleTextureInfo(
                    Config.RoundButtonSize,
                    Config.RoundButtonSize,
                    ColorUtils.Color32Af(48, 40, 44, 0.15f),
                    Color.clear
                )
            );

            RoundButtonStrokeTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.RoundButtonStrokeSize,
                    Config.RoundButtonStrokeSize,
                    Config.RoundButtonSize.Width,
                    ColorUtils.Color32Af(214, 214, 214, 0.3f),
                    Color.clear
                )
            );
        }
    }
}
