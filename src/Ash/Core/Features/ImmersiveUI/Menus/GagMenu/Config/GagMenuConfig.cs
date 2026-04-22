using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.GagMenu.Config
{
    internal class GagMenuConfig : IuiMenuConfigTemplate
    {
        private const int BackgroundRectWidth = 384;

        // menu background
        internal readonly IuiTextureGen.Size BackgroundRectSize =
            new IuiTextureGen.Size(
                BackgroundRectWidth,
                0 // height calculated dynamically using a formula (see IuiGagMenu class constructor)
            );

        internal readonly int BackgroundRectBorderRadius = 12;

        // X button
        internal readonly IuiTextureGen.Size XButtonSize = new IuiTextureGen.Size(12, 12);

        internal readonly int XButtonLineThickness = 2;

        // container
        internal readonly RectOffset BackgroundContainerMargins = new RectOffset(30, 30, 20, 32);

        // header text
        internal readonly int HeaderFontSize = 18;

        // subtitle text
        internal readonly int SubtitleFontSize = 16;

        // main vertical layout
        internal readonly int MainVerticalLayoutSpacing = 22;

        // gag group vertical layout
        internal readonly int GroupVerticalLayoutSpacing = 12;

        // gag toggles

        private const int GagToggleWidth = 92;
        private const int GagToggleHeight = 32;

        internal readonly IuiTextureGen.Size GagToggleSize =
            new IuiTextureGen.Size(GagToggleWidth, GagToggleHeight);

        internal readonly int GagToggleTextureBorderRadius = 15;

        internal readonly int GagToggleSpacing = 22;

        internal readonly int GagToggleLabelFontSize = 16;

        internal readonly int GagToggleTextureStrokeThickness = 2;

        internal readonly int GagToggleAaSafetyPadding = 2;

        internal const float ToggleActiveStrokeTransparencyValue = 0.4f;
        internal const float ToggleInactiveStrokeTransparencyValue = 0.2f;
    }
}
