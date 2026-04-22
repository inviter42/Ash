using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures
{
    internal class WearsMenuTextures
    {
        internal readonly Texture2D FrostedGlassAtlasMask;
        internal readonly Texture2D FrostedGlassTintTexture;
        internal readonly Texture2D WearItemIconBackgroundTexture;
        internal readonly Texture2D AccessoryItemIconBackgroundTexture;
        internal readonly Texture2D WearItemButtonHoverStrokeTexture;
        internal readonly Texture2D AccessoryItemButtonHoverStrokeTexture;
        internal readonly Texture2D SwitchFemaleButtonTintTexture;
        internal readonly Texture2D SwitchFemaleButtonStrokeTexture;
        internal readonly Texture2D CloseButtonTintTexture;
        internal readonly Texture2D CloseButtonStrokeTexture;
        internal readonly Texture2D CloseButtonXTexture;
        internal readonly Texture2D ExpandAccessoriesButtonTintTexture;
        internal readonly Texture2D ExpandAccessoriesStrokeTexture;

        internal readonly Sprite AccessoryRotationClickZoneSprite;


        private static readonly WearsMenuConfig Config = IuiMain.WearsMenuConfig;

        internal WearsMenuTextures() {
            FrostedGlassAtlasMask = WearsMenuMaskAtlas.CreateMaskAtlas();

            FrostedGlassTintTexture = IuiTextureGen.CreateEmptyTexture(
                new IuiTextureGen.Size(
                    Config.AccessoryPointerEventZoneOuterDiameter.Width / 2,
                    Config.AccessoryPointerEventZoneOuterDiameter.Height
                ), TextureFormat.RGBA32
            );

            IuiTextureGen.StampRingGradientTexture(
                new IuiTextureGen.GradientRingTextureInfo(
                    new IuiTextureGen.Size(FrostedGlassTintTexture.width, FrostedGlassTintTexture.height),
                    Config.AccessoryPointerEventZoneOuterDiameter,
                    Config.AccessoryPointerEventZoneInnerDiameter.Width,
                    IuiTextureGen.CreateLinearGradient(
                        ColorUtils.Color32Af(48, 40, 44, 0.17f), Color.clear,
                        0.08f
                    ),
                    Color.clear,
                    -90f,
                    new Vector2(-(float)Config.AccessoryPointerEventZoneOuterDiameter.Width / 2, 0)
                ), FrostedGlassTintTexture
            );

            WearItemIconBackgroundTexture = IuiTextureGen.GenerateCircleTexture(
                new IuiTextureGen.CircleTextureInfo(
                    Config.WearItemButtonBackgroundSize,
                    Config.WearItemButtonBackgroundSize,
                    ColorUtils.Color32Af(0, 0, 0, 0.2f),
                    Color.clear
                )
            );

            AccessoryItemIconBackgroundTexture = IuiTextureGen.GenerateCircleTexture(
                new IuiTextureGen.CircleTextureInfo(
                    Config.AccessoryItemButtonBackgroundSize,
                    Config.AccessoryItemButtonBackgroundSize,
                    ColorUtils.Color32Af(0, 0, 0, 0.2f),
                    Color.clear
                )
            );

            WearItemButtonHoverStrokeTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.WearItemActiveStrokeSize,
                    Config.WearItemActiveStrokeSize,
                    Config.WearItemButtonBackgroundSize.Width,
                    ColorUtils.Color32Af(255, 255, 255, 0.5f),
                    Color.clear
                ),
                2f
            );

            AccessoryItemButtonHoverStrokeTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.AccessoryItemActiveStrokeSize,
                    Config.AccessoryItemActiveStrokeSize,
                    Config.AccessoryItemButtonBackgroundSize.Width,
                    ColorUtils.Color32Af(255, 255, 255, 0.5f),
                    Color.clear
                ),
                2f
            );


            SwitchFemaleButtonTintTexture = IuiTextureGen.GenerateRectangleTexture(
                new IuiTextureGen.RectTextureInfo(
                    Config.SwitchFemaleButtonSize,
                    Config.SwitchFemaleButtonSize,
                    Config.SwitchFemaleButtonBorderRadius,
                    ColorUtils.Color32Af(48, 40, 44, 0.15f),
                    Color.clear
                )
            );

            SwitchFemaleButtonStrokeTexture = IuiTextureGen.GenerateRectangleStrokeTexture(
                new IuiTextureGen.RectTextureInfo(
                    Config.SwitchFemaleButtonStrokeSize,
                    Config.SwitchFemaleButtonStrokeSize,
                    Config.SwitchFemaleButtonBorderRadius,
                    ColorUtils.Color32Af(255, 255, 255, 0.25f),
                    Color.clear
                ), WearsMenuConfig.SwitchFemaleButtonTextureStrokeThickness
            );

            CloseButtonTintTexture = IuiTextureGen.GenerateCircleTexture(
                new IuiTextureGen.CircleTextureInfo(
                    Config.XButtonSize,
                    Config.XButtonSize,
                    ColorUtils.Color32Af(48, 40, 44, 0.15f),
                    Color.clear
                )
            );

            CloseButtonStrokeTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.XButtonStrokeSize,
                    Config.XButtonStrokeSize,
                    Config.XButtonSize.Width,
                    ColorUtils.Color32Af(214, 214, 214, 0.3f),
                    Color.clear
                )
            );

            CloseButtonXTexture = IuiTextureGen.GenerateXTexture(
                new IuiTextureGen.XTextureInfo(
                    Config.XButtonIconSize,
                    Config.XButtonIconSize,
                    Config.XButtonTextureLineThickness,
                    ColorUtils.Color32Af(214, 214, 214),
                    Color.clear
                )
            );

            var accessoryRotationClickZoneTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.AccessoryPointerEventZoneOuterDiameter,
                    Config.AccessoryPointerEventZoneOuterDiameter,
                    Config.AccessoryPointerEventZoneInnerDiameter.Width,
                    ColorUtils.Color32Af(48, 40, 44, 0.35f),
                    Color.clear
                )
            );

            AccessoryRotationClickZoneSprite = Sprite.Create(
                accessoryRotationClickZoneTexture,
                new Rect(
                    (float)Config.AccessoryPointerEventZoneOuterDiameter.Width / 2,
                    0,
                    (float)Config.AccessoryPointerEventZoneOuterDiameter.Width / 2,
                    Config.AccessoryPointerEventZoneOuterDiameter.Height
                ),
                new Vector2(0.5f, 0.5f)
            );

            ExpandAccessoriesButtonTintTexture = IuiTextureGen.GenerateCircleTexture(
                new IuiTextureGen.CircleTextureInfo(
                    Config.ExpandAccessoriesButtonSize,
                    Config.ExpandAccessoriesButtonSize,
                    ColorUtils.Color32Af(48, 40, 44, 0.15f),
                    Color.clear
                )
            );

            ExpandAccessoriesStrokeTexture = IuiTextureGen.GenerateRingTexture(
                new IuiTextureGen.ColorRingTextureInfo(
                    Config.ExpandAccessoriesButtonStrokeSize,
                    Config.ExpandAccessoriesButtonStrokeSize,
                    Config.ExpandAccessoriesButtonSize.Width,
                    ColorUtils.Color32Af(214, 214, 214, 0.3f),
                    Color.clear
                )
            );
        }
    }
}
