using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config
{
    internal class StylesMenuConfig : IuiMenuConfigTemplate
    {
        private const int BackgroundRectWidth = 750;
        private const int BackgroundRectHeight = 520;

        // menu background
        internal readonly IuiTextureGen.Size BackgroundRectSize =
            new IuiTextureGen.Size(BackgroundRectWidth, BackgroundRectHeight);

        internal readonly int BackgroundRectBorderRadius = 12;

        // X button
        internal readonly IuiTextureGen.Size XButtonSize = new IuiTextureGen.Size(12, 12);

        internal const int XButtonLineThickness = 2;

        // container
        internal readonly RectOffset BackgroundContainerMargins = new RectOffset(20, 0, 20, 30);

        // header text
        internal readonly int HeaderFontSize = 40;

        // main vertical layout
        internal readonly int MainVerticalLayoutSpacing = 21;

        // scroll view + thumbnail horizontal layout
        internal readonly int MainHorizontalLayoutSpacing = 0;

        internal readonly int StylesListWidth = 297;
        internal readonly int StylesListPadding = 6;
        internal readonly int StylesListItemSpacing = 0;

        // scroll bar
        // a separate width value of the logical scroll component for better UX
        internal readonly int ScrollbarLogicalComponentWidth = 20;

        internal readonly int ScrollbarTextureWidth = 4;
        internal readonly int ScrollbarTextureBorderRadius = 2;

        // scale vert resolution for better result
        // ScrollbarTextureWidth, ScrollbarTextureBorderRadius
        internal readonly IuiTextureGen.Size ScrollbarTextureSize = new IuiTextureGen.Size(4, 2 * 4);

        // scroll list buttons
        internal readonly IuiTextureGen.Size ScrollListButtonSize = new IuiTextureGen.Size(246, 42);

        internal readonly int ScrollListButtonTextureBorderRadius = 8;

        internal readonly int ScrollListButtonLabelFontSize = 18;

        internal readonly Color ScrollListButtonNormalColor = Color.clear;
        internal readonly Color ScrollListButtonHighlightedColor = ColorUtils.Color32Af(255, 255, 255, 0.025f);
        internal readonly Color ScrollListButtonPressedColor = ColorUtils.Color32Af(255, 255, 255, 0.04f);
        internal readonly float ScrollListButtonFadeDuration = 0f;

        // thumbnail

        internal readonly Vector2 ThumbnailPadding = new Vector2(-50, -50);


        // genre toggles

        internal const int GenreNumberOfToggles = 3; // H_StyleData.TYPE

        internal readonly int GenreTogglesContainerBottomMargin = 18;
        internal readonly int GenreTogglesContainerLeftMargin = 26;

        private const int GenreToggleWidth = 92;
        private const int GenreToggleHeight = 38;

        internal readonly IuiTextureGen.Size GenreToggleSize =
            new IuiTextureGen.Size(GenreToggleWidth, GenreToggleHeight);

        internal readonly int GenreToggleTextureBorderRadius = 22;

        internal readonly int GenreToggleSpacing = 22;

        internal readonly int GenreToggleLabelFontSize = 16;

        internal readonly int GenreToggleTextureStrokeThickness = 2;

        internal readonly int GenreToggleAaSafetyPadding = 2;

        // female state toggles

        internal const int FemaleStateNumberOfToggles = 3; // H_StyleData.STATE

        private const int FemaleStateButtonWidth = 42;
        private const int FemaleStateButtonHeight = 42;

        internal const int FemaleStateToggleSpacing = 22;

        private const int FemaleStateButtonsContainerHeight = FemaleStateButtonHeight * FemaleStateNumberOfToggles +
            FemaleStateToggleSpacing * FemaleStateNumberOfToggles - 1;

        internal readonly int FemaleStateButtonsContainerBottomMargin =
            BackgroundRectHeight - FemaleStateButtonsContainerHeight - 47;

        internal readonly int FemaleStateButtonsContainerRightMargin = 19;

        internal readonly IuiTextureGen.Size FemaleStateToggleSize =
            new IuiTextureGen.Size(FemaleStateButtonWidth, FemaleStateButtonHeight);

        internal readonly int FemaleStateToggleLabelFontSize = 14;

        internal readonly int FemaleStateButtonTextureStrokeThickness = 4;

        // side buttons state

        internal const float SideToggleActiveStrokeTransparencyValue = 0.4f;
        internal const float SideToggleInactiveStrokeTransparencyValue = 0.2f;
    }
}
