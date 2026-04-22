using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config
{
    internal class WearsMenuConfig : IuiMenuConfigTemplate
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// GENERAL
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal readonly Color TextNormalColor = ColorUtils.Color32Af(251, 244, 248, 0.55f);
        internal readonly Color TextHoverColor = ColorUtils.Color32Af(251, 244, 248, 0.85f);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// BUTTONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal readonly Vector2 ButtonsContainerOffset = new Vector2(-80, 0);

        // General Settings

        internal const float ButtonDownAnimationScale = 0.98f;
        internal const float IconTransparencyNormalValue = 1f;

        internal const float IconMaskRadius = 0.5f;
        internal const float IconMaskFeather = 0.02f;

        internal readonly int ButtonsContainerMinWidth = 990;
        internal readonly int ButtonsContainerMaxWidth = 1400;
        internal readonly int ButtonsContainerMinHeight = 336;
        internal readonly int ButtonsContainerMaxHeight = 565;


        // Icon Positioning

        private const int AccessoryIconsPositioningCircleRadiusConst = 612;

        internal readonly int WearItemIconsPositioningCircleRadius = 390;
        internal readonly int AccessoryIconsPositioningCircleRadius = AccessoryIconsPositioningCircleRadiusConst;


        // Icon Distribution

        private const float AngleBetweenWearItemButtonsDegree = 13;
        private const float AngleBetweenAccessoryItemButtonsDegree = 6f;
        private const float AccessorySectionAngleDegree = 5f;
        private const float AccessorySectionTextPaddingDegree = 1.2f;

        internal const float AngleBetweenWearItemButtonsRad = AngleBetweenWearItemButtonsDegree * Mathf.Deg2Rad;
        internal const float AngleBetweenAccessoryItemButtonsRad = AngleBetweenAccessoryItemButtonsDegree * Mathf.Deg2Rad;
        internal const float AccessorySectionAngleRad = AccessorySectionAngleDegree * Mathf.Deg2Rad;
        internal const float AccessorySectionTextPaddingRad = AccessorySectionTextPaddingDegree * Mathf.Deg2Rad;

        internal static readonly Vector2 AccessoryContainerOffset = new Vector2(-25, 0);



        ///////////////////////////////////////////////////////
        /// WEAR ITEM BUTTONS
        ///////////////////////////////////////////////////////

        // Background

        private const int WearItemIconBackgroundWidth = 68;
        private const int WearItemIconBackgroundHeight = 68;

        internal readonly IuiTextureGen.Size WearItemButtonBackgroundSize =
            new IuiTextureGen.Size(WearItemIconBackgroundWidth, WearItemIconBackgroundHeight);


        // Icons

        internal readonly int WearItemIconSize = 62;


        // Active Strokes

        private const int WearItemButtonActiveStrokeThickness = 8;
        private const int WearItemButtonActiveStrokeAaCompensationThickness = 1;

        internal readonly IuiTextureGen.Size WearItemActiveStrokeSize =
            new IuiTextureGen.Size(
                WearItemIconBackgroundWidth
                + WearItemButtonActiveStrokeThickness
                - WearItemButtonActiveStrokeAaCompensationThickness,
                WearItemIconBackgroundHeight
                + WearItemButtonActiveStrokeThickness
                - WearItemButtonActiveStrokeAaCompensationThickness
            );


        ///////////////////////////////////////////////////////
        /// ACCESSORY ITEM BUTTONS
        ///////////////////////////////////////////////////////

        // Background

        private const int AccessoryItemIconBackgroundWidth = 54;
        private const int AccessoryItemIconBackgroundHeight = 54;

        internal readonly IuiTextureGen.Size AccessoryItemButtonBackgroundSize =
            new IuiTextureGen.Size(AccessoryItemIconBackgroundWidth, AccessoryItemIconBackgroundHeight);


        // Icons

        internal readonly int AccessoryItemIconSize = 48;

        // Icons Fallback Text

        internal readonly int AccessoryItemIconFallbackTextMaxWidth = 50;
        internal readonly int AccessoryItemIconFallbackTextMaxHeight = 30;
        internal readonly int AccessoryItemIconFallbackTextResizeMinFontSize = 13;
        internal readonly int AccessoryItemIconFallbackTextResizeMaxFontSize = 14;


        // Active Strokes

        private const int AccessoryItemButtonActiveStrokeThickness = 8;
        private const int AccessoryItemButtonActiveStrokeAaCompensationThickness = 1;

        internal readonly IuiTextureGen.Size AccessoryItemActiveStrokeSize =
            new IuiTextureGen.Size(
                AccessoryItemIconBackgroundWidth
                + AccessoryItemButtonActiveStrokeThickness
                - AccessoryItemButtonActiveStrokeAaCompensationThickness,
                AccessoryItemIconBackgroundHeight
                + AccessoryItemButtonActiveStrokeThickness
                - AccessoryItemButtonActiveStrokeAaCompensationThickness
            );


        // Sections

        internal readonly int AccessorySectionFontSize = 14;


        // Accessory Interactive Click Area

        internal const float AccessoryPointerEventZoneRotationSensitivity = 0.1f;
        internal const float AccessoryPointerEventZoneAlphaHitTestMinimumThreshold = 0.2f;

        private const int AccessoryPointerEventZoneOuterDiameterPadding = 54;

        internal readonly IuiTextureGen.Size AccessoryPointerEventZoneOuterDiameter = new IuiTextureGen.Size(
            AccessoryIconsPositioningCircleRadiusConst * 2
            + AccessoryItemIconBackgroundWidth
            + AccessoryPointerEventZoneOuterDiameterPadding,
            AccessoryIconsPositioningCircleRadiusConst * 2
            + AccessoryItemIconBackgroundWidth
            + AccessoryPointerEventZoneOuterDiameterPadding
        );

        internal readonly IuiTextureGen.Size AccessoryPointerEventZoneInnerDiameter = new IuiTextureGen.Size(
            AccessoryIconsPositioningCircleRadiusConst * 2
            - AccessoryItemIconBackgroundWidth
            - AccessoryPointerEventZoneOuterDiameterPadding,
            AccessoryIconsPositioningCircleRadiusConst * 2
            - AccessoryItemIconBackgroundWidth
            - AccessoryPointerEventZoneOuterDiameterPadding
        );

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///  SWITCH FEMALE CONTAINER
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal readonly int SwitchFemaleContainerSpacing = 8;
        internal readonly Vector2 SwitchFemaleContainerOffset = new Vector2(130, 0);


        // current female text

        internal readonly int FemaleLabelFontSize = 22;


        // switch female button

        private const int SwitchFemaleButtonBackgroundWidth = 70;
        private const int SwitchFemaleButtonBackgroundHeight = 22;

        internal readonly IuiTextureGen.Size SwitchFemaleButtonSize = new IuiTextureGen.Size(
            SwitchFemaleButtonBackgroundWidth,
            SwitchFemaleButtonBackgroundHeight
        );
        internal readonly int SwitchFemaleButtonBorderRadius = 6;

        internal const int SwitchFemaleButtonTextureStrokeThickness = 2;
        internal const int SwitchFemaleButtonStrokeAaCompensationThickness = 2;

        internal readonly IuiTextureGen.Size SwitchFemaleButtonStrokeSize =
            new IuiTextureGen.Size(
                SwitchFemaleButtonBackgroundWidth
                + SwitchFemaleButtonTextureStrokeThickness
                - SwitchFemaleButtonStrokeAaCompensationThickness,
                SwitchFemaleButtonBackgroundHeight
                + SwitchFemaleButtonTextureStrokeThickness
                - SwitchFemaleButtonStrokeAaCompensationThickness
            );

        internal readonly int SwitchFemaleButtonLabelFontSize = 12;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// X BUTTON
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private const int XButtonBackgroundWidth = 20;
        private const int XButtonBackgroundHeight = 20;

        private const int XButtonTextureStrokeThickness = 8;
        private const int XButtonStrokeAaCompensationThickness = 2;

        internal readonly IuiTextureGen.Size XButtonSize = new IuiTextureGen.Size(
            XButtonBackgroundWidth,
            XButtonBackgroundHeight
        );

        internal readonly IuiTextureGen.Size XButtonStrokeSize =
            new IuiTextureGen.Size(
                XButtonBackgroundWidth
                + XButtonTextureStrokeThickness
                - XButtonStrokeAaCompensationThickness,
                XButtonBackgroundHeight
                + XButtonTextureStrokeThickness
                - XButtonStrokeAaCompensationThickness
            );


        internal readonly IuiTextureGen.Size XButtonIconSize = new IuiTextureGen.Size(8, 8);
        internal readonly int XButtonTextureLineThickness = 2;

        internal readonly Vector2 XButtonOffset = new Vector2(80, 40);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// EXPAND ACCESSORIES BUTTON
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private const int ExpandAccessoriesButtonBackgroundWidth = 24;
        private const int ExpandAccessoriesButtonBackgroundHeight = 24;

        private const int ExpandAccessoriesButtonTextureStrokeThickness = 8;
        private const int ExpandAccessoriesButtonStrokeAaCompensationThickness = 2;

        internal readonly IuiTextureGen.Size ExpandAccessoriesButtonSize =
            new IuiTextureGen.Size(ExpandAccessoriesButtonBackgroundWidth, ExpandAccessoriesButtonBackgroundHeight);


        internal readonly IuiTextureGen.Size ExpandAccessoriesButtonStrokeSize =
            new IuiTextureGen.Size(
                ExpandAccessoriesButtonBackgroundWidth
                + ExpandAccessoriesButtonTextureStrokeThickness
                - ExpandAccessoriesButtonStrokeAaCompensationThickness,
                ExpandAccessoriesButtonBackgroundHeight
                + ExpandAccessoriesButtonTextureStrokeThickness
                - ExpandAccessoriesButtonStrokeAaCompensationThickness
            );

        internal readonly IuiTextureGen.Size ExpandAccessoriesButtonIconSize = new IuiTextureGen.Size(16, 16);

        internal readonly Vector2 ExpandAccessoriesButtonOffset = new Vector2(480, 0);
    }
}
