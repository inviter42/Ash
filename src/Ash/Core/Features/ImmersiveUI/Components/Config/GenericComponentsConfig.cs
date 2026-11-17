using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;

namespace Ash.Core.Features.ImmersiveUI.Components.Config;

internal class GenericComponentsConfig : IuiMenuConfigTemplate
{
    private const int RoundButtonBackgroundWidth = 24;
    private const int RoundButtonBackgroundHeight = 24;

    private const int RoundButtonTextureStrokeThickness = 8;
    private const int RoundButtonStrokeAaCompensationThickness = 2;

    internal readonly IuiTextureGen.Size RoundButtonSize =
        new(RoundButtonBackgroundWidth, RoundButtonBackgroundHeight);


    internal readonly IuiTextureGen.Size RoundButtonStrokeSize =
        new(
            RoundButtonBackgroundWidth
            + RoundButtonTextureStrokeThickness
            - RoundButtonStrokeAaCompensationThickness,
            RoundButtonBackgroundHeight
            + RoundButtonTextureStrokeThickness
            - RoundButtonStrokeAaCompensationThickness
        );
}
