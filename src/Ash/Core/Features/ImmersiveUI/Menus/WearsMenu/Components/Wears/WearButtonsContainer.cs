using System;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures;
using Ash.GlobalUtils;
using Character;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using g_Wears = Wears;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.Wears
{
    internal class WearButtonsContainer
    {
        private readonly WearsMenuTextures Textures;
        private readonly Material IconMaskMaterial;

        private readonly GameObject ButtonsRootGameObject;

        internal WearButtonsContainer(
            WearsMenuConfig config,
            WearsMenuTextures textures,
            Material iconMaskMaterial,
            GameObject parent,
            Female female
        ) {
            var model = SceneUtils.GetWearShowTypesOfEquippedItems(female);

            if (model.Length == 0) {
                Ash.Logger.LogDebug($"Female {female.heroineID} has no equipped wears, skip wear item buttons creation.");
                return;
            }

            Textures = textures;
            IconMaskMaterial = iconMaskMaterial;

            ButtonsRootGameObject = new GameObject("WearItemButtons", typeof(RectTransform));
            ButtonsRootGameObject.transform.SetParent(parent.transform, false);

            var n = 0;

            foreach (var wearShowType in model) {
                var buttonGameObj = CreateWearItemButton(ButtonsRootGameObject, female, wearShowType);
                PositionWearItemButton(
                    buttonGameObj,
                    config.WearItemIconsPositioningCircleRadius,
                    WearsMenuConfig.AngleBetweenWearItemButtonsRad,
                    model.Length,
                    n
                );

                n++;
            }

            IuiPositioningHelpers.AnchorsCenterIn(ButtonsRootGameObject);
        }

        ~WearButtonsContainer() {
              UnityEngine.Object.DestroyImmediate(ButtonsRootGameObject);
        }

        private GameObject CreateWearItemButton(GameObject root, Female female, WEAR_SHOW_TYPE wearShowType) {
            var name = wearShowType.ToString().ToLower().ToTitleCase();
            var wearType = g_Wears.ShowToWearType[(int)wearShowType];
            var wearData = female.wears.GetWearData(wearType);
            var prefab = wearData.prefab;

            // ReSharper disable once ConvertToLocalFunction
            Action<PointerEventData> actionOnPointerClick = eventData => {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (eventData.button) {
                    case PointerEventData.InputButton.Left:
                        SceneUtils.CycleStateOfWearItem(female, wearShowType);
                        break;
                    case PointerEventData.InputButton.Right:
                        SceneUtils.CycleStateOfWearItem(female, wearShowType, false);
                        break;
                }

                SystemSE.Play(SystemSE.SE.CHOICE);
            };


            var buttonGameObj = WearItemButton.CreateWearItemButton(
                name,
                Textures,
                IconMaskMaterial,
                wearShowType,
                wearType,
                prefab,
                actionOnPointerClick
            );

            buttonGameObj.transform.SetParent(root.transform, false);

            IuiPositioningHelpers.AnchorsCenterIn(buttonGameObj, Vector2.zero, new Vector2(1, 0.5f));

            return buttonGameObj;
        }

        private void PositionWearItemButton(GameObject gameObj, int radius, float angleRad, int modelLength, int n) {
            var offset = (modelLength - 1) * angleRad / 2;

            var theta = offset - angleRad * n;
            var x = Mathf.Cos(theta) * radius;
            var y = Mathf.Sin(theta) * radius;

            IuiPositioningHelpers.AnchorsCenterIn(gameObj, new Vector2(x, y));
        }
    }
}
