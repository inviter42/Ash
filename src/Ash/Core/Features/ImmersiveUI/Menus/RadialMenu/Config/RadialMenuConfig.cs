using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Config
{
    internal class RadialMenuConfig : IuiMenuConfigTemplate
    {
        // frosted glass
        internal readonly IuiTextureGen.Size FrostedGlassOuterDiameterSize = new IuiTextureGen.Size(540, 540);

        internal readonly IuiTextureGen.Size FrostedGlassInnerDiameterSize = new IuiTextureGen.Size(324, 324);

        // icon background
        private const int IconBackgroundWidth = 60;
        private const int IconBackgroundHeight = 60;

        internal readonly IuiTextureGen.Size IconBackgroundSize =
            new IuiTextureGen.Size(IconBackgroundWidth, IconBackgroundHeight);

        // icons
        internal readonly int IconSize = 28;

        internal const float InactiveIconTransparencyValue = 0.2f;
        internal const float ActiveIconTransparencyValue = 0.7f;

        // active strokes
        private const int ActiveStrokeThickness = 8;
        private const int ActiveStrokeAaCompensationThickness = 1;

        internal readonly IuiTextureGen.Size ActiveStrokeSize = new IuiTextureGen.Size(
            IconBackgroundWidth
            + ActiveStrokeThickness
            - ActiveStrokeAaCompensationThickness,
            IconBackgroundHeight
            + ActiveStrokeThickness
            - ActiveStrokeAaCompensationThickness
        );

        internal const float ActiveStrokeTransparencyValue = 0.45f;

        // middle text
        internal readonly int MiddleTextFontSize = 48;

        // cancel group
        internal readonly IuiTextureGen.Size CancelGroupIconDimensions = new IuiTextureGen.Size(30, 30);

        internal readonly Vector2 CancelGroupPositionOffset = new Vector2(0, -86);
        internal readonly int CancelTextFontSize = 13;

        // middle ring highlight
        internal readonly int MiddleHighlightThickness = 6;
        internal readonly int MiddleHighlightAaCompensationThickness = 3;
    }
}
