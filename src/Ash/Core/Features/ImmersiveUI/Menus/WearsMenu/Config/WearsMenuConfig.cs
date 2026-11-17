using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Utility.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;

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

    internal readonly Vector2 ButtonsContainerOffset = new(-80, 0);

    // General Settings

    internal const float ButtonDownAnimationScale = 0.98f;
    internal const float IconTransparencyNormalValue = 1f;

    internal const float IconMaskRadius = 0.5f;
    internal const float IconMaskFeather = 0.02f;

    internal const int ButtonsContainerMinWidth = 990;
    internal const int ButtonsContainerMaxWidth = 1400;
    internal const int ButtonsContainerMinHeight = 336;
    internal const int ButtonsContainerMaxHeight = 565;


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

    internal static readonly Vector2 AccessoryContainerOffset = new(-25, 0);



    ///////////////////////////////////////////////////////
    /// WEAR ITEM BUTTONS
    ///////////////////////////////////////////////////////

    // Background

    private const int WearItemIconBackgroundWidth = 68;
    private const int WearItemIconBackgroundHeight = 68;

    internal readonly IuiTextureGen.Size WearItemButtonBackgroundSize =
        new(WearItemIconBackgroundWidth, WearItemIconBackgroundHeight);


    // Icons

    internal readonly int WearItemIconSize = 62;


    // Active Strokes

    private const int WearItemButtonActiveStrokeThickness = 8;
    private const int WearItemButtonActiveStrokeAaCompensationThickness = 1;

    internal readonly IuiTextureGen.Size WearItemActiveStrokeSize =
        new(
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
        new(AccessoryItemIconBackgroundWidth, AccessoryItemIconBackgroundHeight);


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
        new(
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

    internal readonly IuiTextureGen.Size AccessoryPointerEventZoneOuterDiameter = new(
        AccessoryIconsPositioningCircleRadiusConst * 2
        + AccessoryItemIconBackgroundWidth
        + AccessoryPointerEventZoneOuterDiameterPadding,
        AccessoryIconsPositioningCircleRadiusConst * 2
        + AccessoryItemIconBackgroundWidth
        + AccessoryPointerEventZoneOuterDiameterPadding
    );

    internal readonly IuiTextureGen.Size AccessoryPointerEventZoneInnerDiameter = new(
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
    internal readonly Vector2 SwitchFemaleContainerOffset = new(130, 0);


    // current female text

    internal readonly int FemaleLabelFontSize = 22;


    // switch female button

    private const int SwitchFemaleButtonBackgroundWidth = 70;
    private const int SwitchFemaleButtonBackgroundHeight = 22;

    internal readonly IuiTextureGen.Size SwitchFemaleButtonSize = new(
        SwitchFemaleButtonBackgroundWidth,
        SwitchFemaleButtonBackgroundHeight
    );
    internal readonly int SwitchFemaleButtonBorderRadius = 6;

    internal const int SwitchFemaleButtonTextureStrokeThickness = 2;
    internal const int SwitchFemaleButtonStrokeAaCompensationThickness = 2;

    internal readonly IuiTextureGen.Size SwitchFemaleButtonStrokeSize =
        new(
            SwitchFemaleButtonBackgroundWidth
            + SwitchFemaleButtonTextureStrokeThickness
            - SwitchFemaleButtonStrokeAaCompensationThickness,
            SwitchFemaleButtonBackgroundHeight
            + SwitchFemaleButtonTextureStrokeThickness
            - SwitchFemaleButtonStrokeAaCompensationThickness
        );

    internal const int SwitchFemaleButtonLabelFontSize = 12;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////



    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// X BUTTON
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private const int XButtonBackgroundWidth = 20;
    private const int XButtonBackgroundHeight = 20;

    private const int XButtonTextureStrokeThickness = 8;
    private const int XButtonStrokeAaCompensationThickness = 2;

    internal readonly IuiTextureGen.Size XButtonSize = new(
        XButtonBackgroundWidth,
        XButtonBackgroundHeight
    );

    internal readonly IuiTextureGen.Size XButtonStrokeSize =
        new(
            XButtonBackgroundWidth
            + XButtonTextureStrokeThickness
            - XButtonStrokeAaCompensationThickness,
            XButtonBackgroundHeight
            + XButtonTextureStrokeThickness
            - XButtonStrokeAaCompensationThickness
        );


    internal readonly IuiTextureGen.Size XButtonIconSize = new(8, 8);
    internal const int XButtonTextureLineThickness = 2;

    internal readonly Vector2 XButtonOffset = new(80, 40);

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// EXPAND ACCESSORIES BUTTON
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private const int ExpandAccessoriesButtonBackgroundWidth = 24;
    private const int ExpandAccessoriesButtonBackgroundHeight = 24;

    private const int ExpandAccessoriesButtonTextureStrokeThickness = 8;
    private const int ExpandAccessoriesButtonStrokeAaCompensationThickness = 2;

    internal readonly IuiTextureGen.Size ExpandAccessoriesButtonSize =
        new(ExpandAccessoriesButtonBackgroundWidth, ExpandAccessoriesButtonBackgroundHeight);


    internal readonly IuiTextureGen.Size ExpandAccessoriesButtonStrokeSize =
        new(
            ExpandAccessoriesButtonBackgroundWidth
            + ExpandAccessoriesButtonTextureStrokeThickness
            - ExpandAccessoriesButtonStrokeAaCompensationThickness,
            ExpandAccessoriesButtonBackgroundHeight
            + ExpandAccessoriesButtonTextureStrokeThickness
            - ExpandAccessoriesButtonStrokeAaCompensationThickness
        );

    internal readonly IuiTextureGen.Size ExpandAccessoriesButtonIconSize = new(16, 16);

    internal readonly Vector2 ExpandAccessoriesButtonOffset = new(480, 0);
}
