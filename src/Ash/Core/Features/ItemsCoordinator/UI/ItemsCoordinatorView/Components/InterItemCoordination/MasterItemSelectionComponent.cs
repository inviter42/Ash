using System.Linq;
using Ash.Core.Features.Common.Components;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.Common;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.Core.UI.Types;
using Ash.GlobalUtils;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.InterItemRuleForm;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.InterItemCoordination
{
    public static class MasterItemSelectionComponent
    {
        public const string InterItemRuleTypeStateKey = "InterItemRuleType";
        public const string MasterItemFormDataKey = "MasterItemData";

        private const string NewRuleTitle = "New Rule";

        private const string NewRuleSubtitle = "Select Master item:";

        public static bool IsMasterItemSelected() =>
            FormData.ContainsKey(MasterItemFormDataKey);

        public static void DrawMasterItemSelectionComponent() {
            Title(NewRuleTitle);

            GUILayout.Space(8);

            RuleTypeSelectionComponent.DrawRuleTypeSelection();

            if (!FormData.ContainsKey(FemaleFormDataKey))
                FormData[FemaleFormDataKey] = GetActiveFemale();

            var activeFemale = GetActiveFemale();

            using (new GUILayout.VerticalScope("box")) {
                FemaleSelectionComponent.Component(
                    activeFemale,
                    female => {
                        FormData[FemaleFormDataKey] = female;
                        SetActiveFemale(female);
                });

                // MasterItem selection
                Subtitle(NewRuleSubtitle);

                MasterWearSelection(activeFemale);
                GUILayout.Space(12);
                MasterAccessorySelection(activeFemale);
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private static Female GetActiveFemale() {
            switch (Ash.AshUI.Window) {
                case EditSceneWindow editSceneWindow:
                    return editSceneWindow.GetActiveFemale();
                case HSceneWindow hSceneWindow:
                    return hSceneWindow.GetActiveFemale();
                default:
                    Ash.Logger.LogError($"Component MasterItemSelectionComponent is used inside of an unsupported window type {Ash.AshUI.Window.GetType().Name}.");
                    return null;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private static void SetActiveFemale(Female female) {
            switch (Ash.AshUI.Window) {
                case EditSceneWindow editSceneWindow:
                    editSceneWindow.SetActiveFemale(female);
                    break;
                case HSceneWindow hSceneWindow:
                    hSceneWindow.SetActiveFemale(female);
                    break;
                default:
                    Ash.Logger.LogError($"Component MasterItemSelectionComponent is used inside of an unsupported window type {Ash.AshUI.Window.GetType().Name}.");
                    return;
            }
        }

        private static void MasterWearSelection(Female activeFemale) {
            var masterWearsModel = SceneUtils.GetActiveWearShowTypes(activeFemale);
            Flow(masterWearsModel, (itemPart, idx) => Button(
                WearShowTypeLabels.GetValueOrDefaultValue(itemPart, ErrorLabel),
                () => {
                    var wearData = activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)itemPart]);
                    FormData[MasterItemFormDataKey] = new ItemWearFormData { Type = itemPart, WearData = wearData };
                })
            );
        }

        private static void MasterAccessorySelection(Female activeFemale) {
            var masterAccessoriesModel = activeFemale.accessories.acceObjs
                .Where(accessoryObj => accessoryObj != null)
                .ToArray();

            Flow(masterAccessoriesModel, (accessoryObj, idx) => {
                var accessoryData = activeFemale.accessories.GetAccessoryData(accessoryObj.acceParam, accessoryObj.slot);
                Button(
                    $"{accessoryObj.slot}: " +
                    $"{AutoTranslatorIntegration.Translate(accessoryData.name)}",
                    () => FormData[MasterItemFormDataKey] =
                        new ItemAccessoryFormData {
                            SlotNo = accessoryObj.slot, AccessoryParameter = accessoryObj.acceParam,
                            AccessoryData = accessoryData
                        }
                );
            }, 3);

            if (Ash.MoreAccessoriesInstance == null)
                return;

            var extendedModel = Ash.MoreAccessoriesInstance
                .GetAdditionalData(activeFemale.customParam)
                .accessories
                .Where(accessoryData => accessoryData?.acceObj != null)
                .ToArray();

            Flow(extendedModel, (maAccessoryData, idx) => {
                var accessoryObj = (Accessories.AcceObj)maAccessoryData.acceObj;
                var accessoryData =
                    activeFemale.accessories.GetAccessoryData(accessoryObj.acceParam, accessoryObj.slot);
                Button(
                    $"{accessoryObj.slot}: " +
                    $"{AutoTranslatorIntegration.Translate(accessoryData.name)}",
                    () => FormData[MasterItemFormDataKey] =
                        new ItemAccessoryFormData {
                            SlotNo = accessoryObj.slot, AccessoryParameter = accessoryObj.acceParam,
                            AccessoryData = accessoryData
                        }
                );
            }, 3);
        }
    }
}
