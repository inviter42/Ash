using System;
using System.Linq;
using Ash.Core.Features.Common.Components;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.Common;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.Core.UI;
using Ash.Core.UI.Types;
using Ash.GlobalUtils;
using UnityEngine;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.InterItemRuleForm;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.InterItemCoordination
{
    internal static class MasterItemSelectionComponent
    {
        internal const string InterItemRuleTypeStateKey = "InterItemRuleType";
        internal const string MasterItemFormDataKey = "MasterItemData";

        private const string NewRuleTitle = "New Rule";

        private const string NewRuleSubtitle = "Select Master item:";

        internal static bool IsMasterItemSelected(InterItemRuleForm form) =>
            form.FormData.ContainsKey(MasterItemFormDataKey);

        internal static void DrawMasterItemSelectionComponent(InterItemRuleForm form) {
            var formData = form.FormData;

            Title(NewRuleTitle);

            GUILayout.Space(8);

            RuleTypeSelectionComponent.DrawRuleTypeSelection();

            if (!formData.ContainsKey(FemaleFormDataKey))
                formData[FemaleFormDataKey] = GetActiveFemale();

            var activeFemale = GetActiveFemale();
            if (activeFemale == null) {
                Ash.Logger.LogWarning("Female is null");
                Ash.Logger.LogWarning(Environment.StackTrace);
                formData.Clear();
                return;
            }

            using (new GUILayout.VerticalScope("box")) {
                FemaleSelectionComponent.Component(
                    activeFemale,
                    female => {
                        formData[FemaleFormDataKey] = female;
                        SetActiveFemale(female);
                });

                // MasterItem selection
                Subtitle(NewRuleSubtitle);

                MasterWearSelection(form, activeFemale);
                GUILayout.Space(12);
                MasterAccessorySelection(form, activeFemale);
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private static Female GetActiveFemale() {
            switch (WindowManager.Window) {
                case EditSceneWindow editSceneWindow:
                    return editSceneWindow.GetActiveFemale();
                case HSceneWindow hSceneWindow:
                    return hSceneWindow.GetActiveFemale();
                default:
                    Ash.Logger.LogError($"Component MasterItemSelectionComponent is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return null;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private static void SetActiveFemale(Female female) {
            switch (WindowManager.Window) {
                case EditSceneWindow editSceneWindow:
                    editSceneWindow.SetActiveFemale(female);
                    break;
                case HSceneWindow hSceneWindow:
                    hSceneWindow.SetActiveFemale(female);
                    break;
                default:
                    Ash.Logger.LogError($"Component MasterItemSelectionComponent is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return;
            }
        }

        private static void MasterWearSelection(InterItemRuleForm form, Female activeFemale) {
            var formData = form.FormData;
            var masterWearsModel = SceneUtils.GetWearShowTypesOfEquippedItems(activeFemale);
            Flow(masterWearsModel, (itemPart, idx) => Button(
                WearShowTypeLabels.GetValueOrDefaultValue(itemPart, ErrorLabel),
                () => {
                    var wearData = activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)itemPart]);
                    formData[MasterItemFormDataKey] = new ItemWearFormData(itemPart, wearData);
                })
            );
        }

        private static void MasterAccessorySelection(InterItemRuleForm form, Female activeFemale) {
            var formData = form.FormData;
            var masterAccessoriesModel = activeFemale.accessories.acceObjs
                .Where(accessoryObj => accessoryObj != null)
                .ToArray();

            Flow(masterAccessoriesModel, (accessoryObj, idx) => {
                var accessoryData = activeFemale.accessories.GetAccessoryData(accessoryObj.acceParam, accessoryObj.slot);
                Button(
                    $"{accessoryObj.slot}: " +
                    $"{AutoTranslatorIntegration.Translate(accessoryData.name)}",
                    () => formData[MasterItemFormDataKey] =
                        new ItemAccessoryFormData(accessoryObj.slot, accessoryObj.acceParam, accessoryData)
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
                    () => formData[MasterItemFormDataKey] =
                        new ItemAccessoryFormData(accessoryObj.slot, accessoryObj.acceParam, accessoryData)
                );
            }, 3);
        }
    }
}
