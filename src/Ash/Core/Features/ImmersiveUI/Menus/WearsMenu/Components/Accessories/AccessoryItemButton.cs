using System;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Utils;
using Ash.GlobalUtils;
using Character;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.Accessories
{
    internal static class AccessoryItemButton
    {
        private static readonly WearsMenuConfig Config = IuiMain.WearsMenuConfig;

        private static RawImage AddButtonBackground(GameObject buttonGameObj, Texture texture) =>
            ItemButtonBase.AddButtonBackground(buttonGameObj, texture);

        private static void AddButtonComponent(GameObject buttonGameObj, RawImage buttonImage) =>
            ItemButtonBase.AddButtonComponent(buttonGameObj, buttonImage);

        private static RawImage AddHoverStrokeElement(GameObject root, Texture texture) =>
            ItemButtonBase.AddHoverStrokeElement(root, texture);

        internal static GameObject CreateAccessoryItemButton(string name,
            WearsMenuTextures textures,
            Material iconMaskMaterial,
            ACCESSORY_TYPE type,
            string prefab,
            Action actionOnPointerClick) {
            var buttonGameObj = new GameObject(name, typeof(RectTransform), typeof(Button), typeof(RawImage),
                typeof(IuiPointerEventsHandler));

            var iconGameObj = AddButtonIcon(buttonGameObj, name, type, prefab, iconMaskMaterial);

            var textGameObj = AddButtonFallbackText(buttonGameObj, name);
            textGameObj.SetActive(!iconGameObj.GetComponent<RawImage>().enabled);

            var buttonBgImage = AddButtonBackground(buttonGameObj, textures.AccessoryItemIconBackgroundTexture);
            AddButtonComponent(buttonGameObj, buttonBgImage);

            var stroke = AddHoverStrokeElement(buttonGameObj, textures.AccessoryItemButtonHoverStrokeTexture);

            var stateHandler = buttonGameObj.GetComponent<IuiPointerEventsHandler>();

            stateHandler.ActionOnPointerEnter.Add(eventData => {
                ImageUtils.SetRawImageTransparency(stroke, 1f);
            });

            stateHandler.ActionOnPointerExit.Add(eventData => {
                ImageUtils.SetRawImageTransparency(stroke, 0f);
            });

            stateHandler.ActionOnPointerDown.Add(eventData => {
                buttonGameObj.transform.localScale = Vector3.one * WearsMenuConfig.ButtonDownAnimationScale;
            });

            stateHandler.ActionOnPointerUp.Add(eventData => {
                buttonGameObj.transform.localScale = Vector3.one;
            });

            stateHandler.ActionOnPointerClick.Add(eventData => {
                actionOnPointerClick?.Invoke();
            });

            buttonGameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(
                Config.AccessoryItemActiveStrokeSize.Width,
                Config.AccessoryItemActiveStrokeSize.Height
            );

            return buttonGameObj;
        }

        private static GameObject AddButtonIcon(GameObject buttonGameObj, string name, ACCESSORY_TYPE type, string prefab,
            Material iconMaskMaterial
        ) {
            var iconGameObj = new GameObject($"Icon__{name}__{prefab}", typeof(RectTransform), typeof(RawImage));
            iconGameObj.transform.SetParent(buttonGameObj.transform, false);

            var icon = iconGameObj.GetComponent<RawImage>();
            var thumbnail = ThumbnailUtils.LoadThumbnail(type, prefab);
            icon.texture = Ash.PersistentSettings.ThumbnailBackgroundRemovalEnabled.Value
                ? ImageUtils.RemoveBackgroundFuzzy(thumbnail, 0.01f, 0.1f) // todo: cache/store on disk?
                : thumbnail;
            icon.material = iconMaskMaterial;
            icon.enabled = thumbnail != null;

            var rt = IuiPositioningHelpers.AnchorsCenterIn(iconGameObj);
            rt.sizeDelta = new Vector2(Config.AccessoryItemIconSize, Config.AccessoryItemIconSize);

            ImageUtils.SetRawImageTransparency(icon, WearsMenuConfig.IconTransparencyNormalValue);

            return iconGameObj;
        }

        private static GameObject AddButtonFallbackText(GameObject buttonGameObj, string name) {
            var textGameObj = new GameObject("FallbackText", typeof(RectTransform), typeof(Text));
            textGameObj.transform.SetParent(buttonGameObj.transform, false);

            var textComp = textGameObj.GetComponent<Text>();
            textComp.text = name;
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/corbel-regular.ttf");
            textComp.fontSize = 14;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = ColorUtils.Color32Af(255, 255, 255, 0.4f);
            textComp.horizontalOverflow = HorizontalWrapMode.Wrap;
            textComp.verticalOverflow = VerticalWrapMode.Truncate;
            textComp.resizeTextForBestFit = true;
            textComp.resizeTextMinSize = Config.AccessoryItemIconFallbackTextResizeMinFontSize;
            textComp.resizeTextMaxSize = Config.AccessoryItemIconFallbackTextResizeMaxFontSize;
            textComp.raycastTarget = false;

            textGameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(
                Config.AccessoryItemIconFallbackTextMaxWidth,
                Config.AccessoryItemIconFallbackTextMaxHeight
            );

            return textGameObj;
        }
    }
}
