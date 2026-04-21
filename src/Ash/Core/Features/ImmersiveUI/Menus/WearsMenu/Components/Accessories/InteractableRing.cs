using System;
using System.Collections.Generic;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures;
using Ash.GlobalUtils;
using Character;
using Illusion.Extensions;
using MoreAccessoriesPH;
using UnityEngine;
using UnityEngine.UI;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using Object = UnityEngine.Object;
using g_Accessories = Accessories;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.Accessories
{
    internal class InteractableRing
    {
        internal GameObject AccessoryContainer { get; private set; }

        private float CurrentRotation;

        private readonly WearsMenuConfig Config;
        private readonly WearsMenuTextures Textures;

        private readonly Material IconMaskMaterial;

        internal InteractableRing(
            WearsMenuConfig config,
            WearsMenuTextures textures,
            Material iconMaskMaterial,
            GameObject parent,
            Female female
        ) {
            Config = config;
            Textures = textures;
            IconMaskMaterial = iconMaskMaterial;

            AccessoryContainer = CreateAccessoryContainer(parent, female);
        }

        ~InteractableRing() {
            Object.DestroyImmediate(AccessoryContainer);
        }

        private GameObject CreateAccessoryContainer(GameObject parent, Female female) {
            var containerGameObj = new GameObject("AccessoryContainer", typeof(RectTransform));
            containerGameObj.transform.SetParent(parent.transform, false);

            var accessoryItemButtons = CreateAccessoryItemButtons(containerGameObj, female);
            if (accessoryItemButtons == null) {
                Object.Destroy(containerGameObj);
                return null;
            }

            var clickArea = CreateAccessoryPointerEventZone(containerGameObj, accessoryItemButtons, female);
            clickArea.transform.SetAsFirstSibling();

            IuiPositioningHelpers.AnchorsCenterIn(containerGameObj, WearsMenuConfig.AccessoryContainerOffset);

            containerGameObj.SetActive(false);

            return containerGameObj;
        }

        private GameObject CreateAccessoryItemButtons(GameObject parent, Female female) {
            var mainModel = SceneUtils.GetEquippedAccessoryObjects(female);
            var extModel = SceneUtils.GetEquippedExtendedAccessoryData(female);

            if (mainModel.Count == 0 && extModel.Count == 0) {
                Ash.Logger.LogDebug($"Female {female.heroineID} has no equipped accessories, skip accessory item buttons creation.");
                return null;
            }

            var buttonsRootGameObj = new GameObject("AccessoryItemButtons", typeof(RectTransform));
            buttonsRootGameObj.transform.SetParent(parent.transform, false);

            var n = 0;
            var currentSectionIndex = 0;
            var lastAttach = ACCESSORY_ATTACH.NONE;
            var combinedModelCount = mainModel.Count + extModel.Count;
            var halfSectorOffsetRad = GetSectorAngleRad(mainModel, extModel) / 2;

            var halfButtonArcAngleRad = Mathf.Asin(
                (float)Config.AccessoryItemActiveStrokeSize.Width / 2
                / Config.AccessoryIconsPositioningCircleRadius
            );
            var halfButtonAndPaddingOffsetRad = halfButtonArcAngleRad + WearsMenuConfig.AccessorySectionTextPaddingRad;

            for (var i = 0; i < combinedModelCount; i++) {
                var currentAttach = SceneUtils.GetAccessoryAttachByIndex(mainModel, extModel, i);
                if (currentAttach != lastAttach) {
                    if (n > 0) {
                        lastAttach = currentAttach;
                        currentSectionIndex++;
                    }

                    var textGameObj = CreateAccessorySectionText(buttonsRootGameObj, currentAttach);

                    PositionAccessorySectionText(
                        textGameObj,
                        Config.AccessoryIconsPositioningCircleRadius,
                        WearsMenuConfig.AngleBetweenAccessoryItemButtonsRad,
                        n,
                        WearsMenuConfig.AccessorySectionAngleRad + WearsMenuConfig.AccessorySectionTextPaddingRad,
                        currentSectionIndex,
                        halfSectorOffsetRad,
                        halfButtonAndPaddingOffsetRad
                    );
                }

                var acceObj = SceneUtils.GetAcceObjByIndex(mainModel, extModel, i);
                var accessoryData = female.accessories.GetAccessoryData(acceObj.acceParam, acceObj.slot);
                var buttonGameObj = CreateAccessoryItemButton(buttonsRootGameObj, female, acceObj, accessoryData, i);

                PositionAccessoryItemButton(
                    buttonGameObj,
                    Config.AccessoryIconsPositioningCircleRadius,
                    WearsMenuConfig.AngleBetweenAccessoryItemButtonsRad,
                    n,
                    WearsMenuConfig.AccessorySectionAngleRad + WearsMenuConfig.AccessorySectionTextPaddingRad,
                    currentSectionIndex,
                    halfSectorOffsetRad
                );

                n++;
            }

            return buttonsRootGameObj;
        }

        private GameObject CreateAccessoryPointerEventZone(GameObject parent, GameObject target, Female female) {
            if (target == null)
                return null;

            var clickZoneGameObj = new GameObject("ClickZone", typeof(RectTransform), typeof(Image), typeof(IuiPointerEventsHandler));
            clickZoneGameObj.transform.SetParent(parent.transform, false);

            var clickZoneImage = clickZoneGameObj.GetComponent<Image>();
            clickZoneImage.sprite = Textures.AccessoryRotationClickZoneSprite;
            clickZoneImage.type = Image.Type.Sliced;
            clickZoneImage.raycastTarget = true;
            clickZoneImage.alphaHitTestMinimumThreshold = WearsMenuConfig.AccessoryPointerEventZoneAlphaHitTestMinimumThreshold;
            clickZoneImage.SetNativeSize();

            ImageUtils.SetImageTransparency(clickZoneImage, 0.01f);

            var pointerEventsHandler = clickZoneGameObj.GetComponent<IuiPointerEventsHandler>();
            var sectorAngleDeg = GetSectorAngleRad(
                SceneUtils.GetEquippedAccessoryObjects(female),
                SceneUtils.GetEquippedExtendedAccessoryData(female)
            ) * Mathf.Rad2Deg;

            pointerEventsHandler.ActionOnDrag.Add(eventData => {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    clickZoneGameObj.GetComponent<RectTransform>(),
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localMousePos
                );

                var tangent = new Vector2(-localMousePos.y, localMousePos.x).normalized;
                var dot = Vector2.Dot(eventData.delta, tangent);
                var rotationAmount = dot * WearsMenuConfig.AccessoryPointerEventZoneRotationSensitivity;

                CurrentRotation = Mathf.Clamp(
                    CurrentRotation + rotationAmount,
                    -sectorAngleDeg / 2,
                    sectorAngleDeg / 2
                );

                target.transform.rotation = Quaternion.Euler(0, 0, CurrentRotation);
            });

            IuiPositioningHelpers.AnchorsCenterIn(clickZoneGameObj, Vector2.zero, new Vector2(0, 0.5f));

            return clickZoneGameObj;
        }

        private GameObject CreateAccessoryItemButton(GameObject root, Female female, g_Accessories.AcceObj acceObj,
            AccessoryData accessoryData, int index) {
            var name = AutoTranslatorIntegration.Translate(accessoryData.name).ToLower().ToTitleCase();
            var prefab = accessoryData.prefab_F;

            // ReSharper disable once ConvertToLocalFunction
            Action actionOnPointerClick = () => {
                SceneUtils.CycleStateOfAccessoryItem(female, acceObj.slot);
                SystemSE.Play(SystemSE.SE.CHOICE);
            };



            var type = SceneUtils.GetAccessoryTypeByIndex(female, index);
            var buttonGameObj = AccessoryItemButton.CreateAccessoryItemButton(
                name,
                Textures,
                IconMaskMaterial,
                type,
                prefab,
                actionOnPointerClick
            );

            buttonGameObj.transform.SetParent(root.transform, false);

            return buttonGameObj;
        }

        private GameObject CreateAccessorySectionText(GameObject buttonsRootGameObj, ACCESSORY_ATTACH currentAttach) {
            var name = currentAttach.ToString().ToLower().ToTitleCase();
            var textGameObj = new GameObject($"Section__{name}", typeof(RectTransform), typeof(Text));
            textGameObj.transform.SetParent(buttonsRootGameObj.transform, false);

            var textComp = textGameObj.GetComponent<Text>();
            textComp.text = AccessoryAttachLabels.GetValueOrDefaultValue(currentAttach, ErrorLabel);
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/corbel-italic.ttf");
            textComp.fontSize = Config.AccessorySectionFontSize;
            textComp.fontStyle = FontStyle.Italic;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = ColorUtils.Color32Af(227, 227, 227, 0.6f);
            textComp.raycastTarget = false;

            return textGameObj;
        }

        private void PositionAccessorySectionText(GameObject gameObj, int radius, float angleRad, int n,
            float sectionAngleRad, int sectionNum, float halfSectorOffsetRad, float halfButtonAndPaddingOffsetRad)
        {
            var theta = halfSectorOffsetRad + halfButtonAndPaddingOffsetRad - angleRad * n - sectionAngleRad * sectionNum;
            var x = Mathf.Cos(theta) * radius;
            var y = Mathf.Sin(theta) * radius;

            IuiPositioningHelpers.AnchorsCenterIn(gameObj, new Vector2(x, y));

            gameObj.transform.localEulerAngles = new Vector3(0, 0, theta * Mathf.Rad2Deg);
        }

        private void PositionAccessoryItemButton(GameObject gameObj, int radius, float angleRad, int n, float sectionAngleRad,
            int sectionNum, float halfSectorOffsetRad)
        {
            var theta = halfSectorOffsetRad - angleRad * n - sectionAngleRad * sectionNum;
            var x = Mathf.Cos(theta) * radius;
            var y = Mathf.Sin(theta) * radius;

            IuiPositioningHelpers.AnchorsCenterIn(gameObj, new Vector2(x, y));
        }

        private int CountUniqueNonConsecutiveAttachTypes(
            List<g_Accessories.AcceObj> mainModel,
            List<MoreAccessoriesPH.MoreAccessories.AdditionalData.AccessoryData> extModel
        ) {
            var count = 1;

            for (var i = 0; i < mainModel.Count + extModel.Count; i++) {
                var current = SceneUtils.GetAccessoryAttachByIndex(mainModel, extModel, i);
                var prev = i > 0
                    ? SceneUtils.GetAccessoryAttachByIndex(mainModel, extModel, i - 1)
                    : ACCESSORY_ATTACH.NONE;

                if (i > 0 && !current.Equals(prev))
                    count++;
            }

            return count;
        }

        private float GetSectorAngleRad(
            List<g_Accessories.AcceObj> mainModel,
            List<MoreAccessories.AdditionalData.AccessoryData> extModel
        ) {
            var uniqueSectionsCount = CountUniqueNonConsecutiveAttachTypes(mainModel, extModel);
            var combinedModelCount = mainModel.Count + extModel.Count;

            var sectionOffsetRad = (uniqueSectionsCount - 1) * (WearsMenuConfig.AccessorySectionAngleRad +
                                                                WearsMenuConfig.AccessorySectionTextPaddingRad);
            var spacingOffsetRad = (combinedModelCount - 1) * WearsMenuConfig.AngleBetweenAccessoryItemButtonsRad;

            return spacingOffsetRad + sectionOffsetRad;
        }
    }
}
