using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.Core.UI;
using Ash.GlobalUtils;
using Character;
using OneOf;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.InterItemRuleForm;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.InterItemCoordination
{
    public static class InterItemRuleDetailsComponent
    {
        public const string MasterItemStateFormDataKey = "MasterItemState";
        public const string SlaveItemFormDataKey = "SlaveItemData";
        public const string SlaveItemStateFormDataKey = "SlaveItemState";
        public const string GenerateReverseRulesFormDataKey = "GenerateReverseRules";
        public const string ReverseRulesStateFormDataKey = "GenerateReverseRulesState";

        private static readonly Dictionary<GenerateReverseRulesEnum.GenerateReverseRules, string> GenerateReverseRulesLabels =
            new Dictionary<GenerateReverseRulesEnum.GenerateReverseRules, string> {
                [GenerateReverseRulesEnum.GenerateReverseRules.Generate] = "Yes",
                [GenerateReverseRulesEnum.GenerateReverseRules.Ignore] = "No",
            };

        private const string RuleParametersTitle = "Rule Parameters";
        private const string SlaveItemSelectionSubtitle = "Set:";
        private const string SlaveItemStateSelectionSubtitle = "To:";
        private const string GenerateReverseRulesSubtitle = "Automatically create rules for other Master states:";
        private const string ReverseRulesStateSubtitle = "In other Master states set Slave to:";

        public static void DrawInterItemRuleDetailsView(InterItemRuleForm form) {
            var formData = form.FormData;

            Button("Back", formData.Clear);

            if (!MasterItemSelectionComponent.IsMasterItemSelected(form))
                return;

            formData.TryGetValue(FemaleFormDataKey, out var femaleFormData);
            var activeFemale = femaleFormData.AsT4;
            if (activeFemale == null) {
                Ash.Logger.LogWarning("Female is null.");
                Ash.Logger.LogWarning(Environment.StackTrace);
                formData.Clear();
                return;
            }

            // OneOf is never nullish
            if (!formData.TryGetValue(MasterItemSelectionComponent.MasterItemFormDataKey, out var masterItemFormDataRaw)) {
                Ash.Logger.LogWarning("MasterItem form data doesn't exist.");
                return;
            }

            var masterItemFromData = masterItemFormDataRaw.IsT0
                ? masterItemFormDataRaw.AsT0
                : (OneOf<ItemWearFormData, ItemAccessoryFormData>)masterItemFormDataRaw.AsT1;

            // MasterItem state selection
            MasterItemStateSelection(form, activeFemale, masterItemFromData);

            GUILayout.Space(8);

            // SlaveItem selection
            SlaveItemSelection(form, activeFemale, masterItemFromData);
            // SlaveItem state selection
            SlaveItemStateSelection(form);

            // Generate reverse rules
            GenerateReverseRulesRadioSet(form);

            GUILayout.Space(10);

            // Reverse rules state
            ReverseRulesItemState(form);

            // Submit form
            GUILayout.Space(10);
            Button(CreateButtonLabel, form.SubmitForm, GUILayout.Height(30));
        }

        private static void MasterItemStateSelection(InterItemRuleForm form, Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            switch (masterItemFormData.Value) {
                case ItemWearFormData masterItemFormDataWear:
                {
                    MasterItemWearStateSelectionTitle(activeFemale, masterItemFormDataWear);

                    GUILayout.Space(14);

                    Title(RuleParametersTitle);
                    Subtitle(
                        $"When {WearShowTypeLabels.GetValueOrDefaultValue(masterItemFormDataWear.Type, ErrorLabel)} is:");
                    ItemWearStateSelection(form, MasterItemStateFormDataKey);
                    break;
                }

                case ItemAccessoryFormData masterItemFormDataAccessory:
                {
                    MasterItemAccessoryStateSelectionTitle(activeFemale, masterItemFormDataAccessory);

                    GUILayout.Space(14);

                    Title(RuleParametersTitle);
                    var accessoryTypeLabel = AccessoryShowTypeLabels.GetValueOrDefaultValue(
                        masterItemFormDataAccessory.AccessoryParameter.slot[masterItemFormDataAccessory.SlotNo % AccessoryCustom.SLOT_NUM].type,
                        ErrorLabel);
                    Subtitle($"When {accessoryTypeLabel} is:");
                    ItemAccessoryStateSelection(form, MasterItemStateFormDataKey);
                    break;
                }

                default:
                    Ash.Logger.LogError("Unknown type of MasterItem form data.");
                    return;
            }
        }

        private static void MasterItemWearStateSelectionTitle(Female activeFemale, ItemWearFormData masterItemFormDataWear) {
            var wearDataForMasterItem =
                activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)masterItemFormDataWear.Type]);

            Title(AutoTranslatorIntegration.Translate(wearDataForMasterItem.name));
            Title(
                WearShowTypeLabels.GetValueOrDefaultValue(masterItemFormDataWear.Type, ErrorLabel),
                new GUIStyle(AshUI.TitleStyle) { fontSize = 13 }
            );
        }

        private static void MasterItemAccessoryStateSelectionTitle(Female activeFemale, ItemAccessoryFormData masterItemFormDataAccessory) {
            if (masterItemFormDataAccessory.AccessoryParameter == null) {
                Ash.Logger.LogWarning("AccessoryParameter for MasterItem is null.");
                return;
            }

            var accessoryDataForMasterItem = activeFemale.accessories.GetAccessoryData(
                masterItemFormDataAccessory.AccessoryParameter,
                masterItemFormDataAccessory.SlotNo);

            if (accessoryDataForMasterItem == null) {
                Ash.Logger.LogWarning("AccessoryData for MasterItem is null.");
                return;
            }

            Title(AutoTranslatorIntegration.Translate(accessoryDataForMasterItem.name));
            Title(
                AccessoryShowTypeLabels.GetValueOrDefaultValue(
                    masterItemFormDataAccessory.AccessoryParameter
                        .slot[masterItemFormDataAccessory.SlotNo % AccessoryCustom.SLOT_NUM].type,
                    ErrorLabel
                ),
                new GUIStyle(AshUI.TitleStyle) { fontSize = 13 }
            );
        }

        private static void ItemWearStateSelection(InterItemRuleForm form, string formDataKey) {
            var formData = form.FormData;
            Flow(
                Enum.GetValues(typeof(WEAR_SHOW)).Cast<WEAR_SHOW>().Reverse().ToArray(),
                (state, idx) => RadioButton(
                    WearShowLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    formData.ContainsKey(formDataKey)
                    && formData[formDataKey].IsT2
                    && formData[formDataKey].AsT2 == state,
                    () => formData[formDataKey] = state)
            );
        }

        private static void ItemAccessoryStateSelection(InterItemRuleForm form, string formDataKey) {
            var formData = form.FormData;
            Flow(
                new[] { false, true },
                (state, idx) => RadioButton(
                    AccessoryShowLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    formData.ContainsKey(formDataKey)
                    && formData[formDataKey].IsT3
                    && formData[formDataKey].AsT3 == state,
                    () => formData[formDataKey] = state)
            );
        }

        private static void SlaveItemSelection(InterItemRuleForm form, Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            Subtitle(SlaveItemSelectionSubtitle);
            SlaveItemWearSelection(form, activeFemale, masterItemFormData);

            GUILayout.Space(12);

            // slave item accessories model
            SlaveItemAccessorySelection(form, activeFemale, masterItemFormData);
            SlaveItemExtendedAccessorySelection(form, activeFemale, masterItemFormData);
        }

        private static void SlaveItemWearSelection(InterItemRuleForm form, Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            var formData = form.FormData;
            var slaveItemWearsModelFiltered = SceneUtils.GetActiveWearShowTypes(activeFemale)
                .Where(type => {
                    if (masterItemFormData.IsT1)
                        return true;

                    var thisWearData = activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)type]);

                    return type != masterItemFormData.AsT0.Type
                           && !thisWearData.Equals(masterItemFormData.AsT0.WearData);
                })
                .ToArray();

            Flow(slaveItemWearsModelFiltered, (itemPart, idx) => {
                var slaveWearData = activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)itemPart]);
                RadioButton(
                    WearShowTypeLabels.GetValueOrDefaultValue(itemPart, ErrorLabel),
                    formData.ContainsKey(SlaveItemFormDataKey)
                    && formData[SlaveItemFormDataKey].IsT0
                    && formData[SlaveItemFormDataKey].AsT0.Type == itemPart,
                    () => {
                        if (formData.ContainsKey(SlaveItemFormDataKey) && formData[SlaveItemFormDataKey].IsT3)
                            formData.Remove(SlaveItemStateFormDataKey);

                        formData[SlaveItemFormDataKey] = new ItemWearFormData
                            { Type = itemPart, WearData = slaveWearData };
                    }
                );
            });
        }

        private static void SlaveItemAccessorySelection(InterItemRuleForm form, Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            var formData = form.FormData;
            var slaveItemAccessoriesModel = activeFemale.accessories.acceObjs
                .Where(accessoryObj => accessoryObj != null)
                .Where(accessoryObj => {
                    if (masterItemFormData.IsT0)
                        return true;

                    return accessoryObj.slot != masterItemFormData.AsT1.SlotNo;
                })
                .ToArray();

            Flow(slaveItemAccessoriesModel, (accessoryObj, idx) => {
                var accessoryData = activeFemale.accessories.GetAccessoryData(accessoryObj.acceParam, accessoryObj.slot);
                RadioButton(
                    $"{accessoryObj.slot}: " +
                    $"{AutoTranslatorIntegration.Translate(accessoryData.name)}",
                    formData.ContainsKey(SlaveItemFormDataKey)
                    && formData[SlaveItemFormDataKey].IsT1
                    && formData[SlaveItemFormDataKey].AsT1.SlotNo == accessoryObj.slot,
                    () => {
                        if (formData.ContainsKey(SlaveItemStateFormDataKey) && formData[SlaveItemStateFormDataKey].IsT2)
                            formData.Remove(SlaveItemStateFormDataKey);

                        formData[SlaveItemFormDataKey] =
                            new ItemAccessoryFormData {
                                SlotNo = accessoryObj.slot, AccessoryParameter = accessoryObj.acceParam,
                                AccessoryData = accessoryData
                            };
                    }
                );
            }, 3);
        }

        private static void SlaveItemExtendedAccessorySelection(InterItemRuleForm form, Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            var formData = form.FormData;

            if (Ash.MoreAccessoriesInstance == null)
                return;

            var extendedModel = Ash.MoreAccessoriesInstance
                .GetAdditionalData(activeFemale.customParam)
                .accessories
                .Where(accessoryData => accessoryData?.acceObj != null)
                .Where(maAccessoryData => {
                    var accessoryObj = (Accessories.AcceObj)maAccessoryData.acceObj;
                    if (masterItemFormData.IsT0)
                        return true;

                    return accessoryObj.slot != masterItemFormData.AsT1.SlotNo;
                })
                .ToArray();

            Flow(extendedModel, (maAccessoryData, idx) => {
                var accessoryObj = (Accessories.AcceObj)maAccessoryData.acceObj;
                var accessoryData =
                    activeFemale.accessories.GetAccessoryData(accessoryObj.acceParam, accessoryObj.slot);
                RadioButton(
                    $"{accessoryObj.slot}: " +
                    $"{AutoTranslatorIntegration.Translate(accessoryData.name)}",
                    formData.ContainsKey(SlaveItemFormDataKey)
                    && formData[SlaveItemFormDataKey].IsT1
                    && formData[SlaveItemFormDataKey].AsT1.SlotNo == accessoryObj.slot,
                    () => {
                        if (formData.ContainsKey(SlaveItemStateFormDataKey) && formData[SlaveItemStateFormDataKey].IsT2)
                            formData.Remove(SlaveItemStateFormDataKey);

                        formData[SlaveItemFormDataKey] =
                            new ItemAccessoryFormData {
                                SlotNo = accessoryObj.slot, AccessoryParameter = accessoryObj.acceParam,
                                AccessoryData = accessoryData
                            };
                    }
                );
            }, 3);
        }

        private static void SlaveItemStateSelection(InterItemRuleForm form) {
            var formData = form.FormData;
            Subtitle(SlaveItemStateSelectionSubtitle);
            formData.TryGetValue(SlaveItemFormDataKey, out var slaveItemFromData);
            switch (slaveItemFromData.Value) {
                case ItemWearFormData _:
                    ItemWearStateSelection(form, SlaveItemStateFormDataKey);
                    break;

                case ItemAccessoryFormData _:
                    ItemAccessoryStateSelection(form, SlaveItemStateFormDataKey);
                    break;

                default:
                    Ash.Logger.LogError("Unknown type of SlaveItem form data.");
                    return;
            }
        }

        private static void GenerateReverseRulesRadioSet(InterItemRuleForm form) {
            var formData = form.FormData;

            Subtitle(GenerateReverseRulesSubtitle);

            if (!formData.ContainsKey(GenerateReverseRulesFormDataKey))
                formData[GenerateReverseRulesFormDataKey] = GenerateReverseRulesEnum.GenerateReverseRules.Ignore;

            Flow(
                Enum.GetValues(typeof(GenerateReverseRulesEnum.GenerateReverseRules)).Cast<GenerateReverseRulesEnum.GenerateReverseRules>().ToArray(),
                (state, idx) => RadioButton(
                    GenerateReverseRulesLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    formData.ContainsKey(GenerateReverseRulesFormDataKey)
                    && formData[GenerateReverseRulesFormDataKey].IsT5
                    && formData[GenerateReverseRulesFormDataKey].AsT5 == state,
                    () => formData[GenerateReverseRulesFormDataKey] = state)
            );
        }

        private static void ReverseRulesItemState(InterItemRuleForm form) {
            var formData = form.FormData;
            if (!formData.ContainsKey(SlaveItemFormDataKey)
                || !formData.ContainsKey(SlaveItemStateFormDataKey)
                || !formData.ContainsKey(GenerateReverseRulesFormDataKey)
                || (GenerateReverseRulesEnum.GenerateReverseRules)formData[GenerateReverseRulesFormDataKey].Value
                   != GenerateReverseRulesEnum.GenerateReverseRules.Generate)
                return;

            Subtitle(ReverseRulesStateSubtitle);
            formData.TryGetValue(SlaveItemFormDataKey, out var slaveItemFormData);
            switch (slaveItemFormData.Value) {
                case ItemWearFormData _:
                {
                    Flow(
                        Enum.GetValues(typeof(WEAR_SHOW))
                            .Cast<WEAR_SHOW>()
                            .Reverse()
                            .ToArray(),
                        (state, idx) => RadioButton(
                            WearShowLabels.GetValueOrDefaultValue(state, ErrorLabel),
                            formData.ContainsKey(ReverseRulesStateFormDataKey)
                            && formData[ReverseRulesStateFormDataKey].IsT2
                            && formData[ReverseRulesStateFormDataKey].AsT2 == state,
                            () => formData[ReverseRulesStateFormDataKey] = state)
                    );

                    break;
                }

                case ItemAccessoryFormData _:
                {
                    Flow(
                        new[] {false, true},
                        (state, idx) => RadioButton(
                            AccessoryShowLabels.GetValueOrDefaultValue(state, ErrorLabel),
                            formData.ContainsKey(ReverseRulesStateFormDataKey)
                            && formData[ReverseRulesStateFormDataKey].IsT3
                            && formData[ReverseRulesStateFormDataKey].AsT3 == state,
                            () => formData[ReverseRulesStateFormDataKey] = state)
                    );

                    break;
                }
            }
        }
    }
}
