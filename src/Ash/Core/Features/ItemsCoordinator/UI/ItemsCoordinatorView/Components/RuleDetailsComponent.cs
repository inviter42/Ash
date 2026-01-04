using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.Core.UI;
using Ash.GlobalUtils;
using Character;
using UnityEngine;
using OneOf;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.CommonLabels;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.FormState;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components
{
    public static class RuleDetailsComponent
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
        private const string CreateButtonLabel = "Create";

        public static void DrawRuleDetailsView() {
            Button("Back", ResetMasterItemSelection);

            if (!IsMasterItemSelected())
                return;

            FormData.TryGetValue(MasterItemSelectionComponent.FemaleFormDataKey, out var femaleFormData);
            var activeFemale = femaleFormData.AsT4;

            if (activeFemale == null) {
                Ash.Logger.LogWarning("Female is null.");
                return;
            }

            // OneOf is never nullish
            if (!FormData.TryGetValue(MasterItemSelectionComponent.MasterItemFormDataKey, out var masterItemFormDataRaw)) {
                Ash.Logger.LogWarning("MasterItem form data doesn't exist.");
                return;
            }

            var masterItemFromData = masterItemFormDataRaw.IsT0
                ? masterItemFormDataRaw.AsT0
                : (OneOf<ItemWearFormData, ItemAccessoryFormData>)masterItemFormDataRaw.AsT1;

            // MasterItem state selection
            MasterItemStateSelection(activeFemale, masterItemFromData);

            GUILayout.Space(8);

            // SlaveItem selection
            SlaveItemSelection(activeFemale, masterItemFromData);
            // SlaveItem state selection
            SlaveItemStateSelection();

            // Generate reverse rules
            GenerateReverseRulesRadioSet();

            GUILayout.Space(10);

            // Reverse rules state
            ReverseRulesItemState();

            // Submit form
            GUILayout.Space(10);
            Button(CreateButtonLabel, SubmitForm, GUILayout.Height(30));
        }

        private static void MasterItemStateSelection(Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            switch (masterItemFormData.Value) {
                case ItemWearFormData masterItemFormDataWear:
                {
                    MasterItemWearStateSelectionTitle(activeFemale, masterItemFormDataWear);

                    GUILayout.Space(14);

                    Title(RuleParametersTitle);
                    Subtitle(
                        $"When {WearShowTypeLabels.GetValueOrDefaultValue(masterItemFormDataWear.Type, ErrorLabel)} is:");
                    ItemWearStateSelection(MasterItemStateFormDataKey);
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
                    ItemAccessoryStateSelection(MasterItemStateFormDataKey);
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

        private static void ItemWearStateSelection(string formDataKey) {
            Flow(
                Enum.GetValues(typeof(WEAR_SHOW)).Cast<WEAR_SHOW>().Reverse().ToArray(),
                (state, idx) => RadioButton(
                    WearShowLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    FormData.ContainsKey(formDataKey)
                    && FormData[formDataKey].IsT2
                    && FormData[formDataKey].AsT2 == state,
                    () => FormData[formDataKey] = state)
            );
        }

        private static void ItemAccessoryStateSelection(string formDataKey) {
            Flow(
                new[] { false, true },
                (state, idx) => RadioButton(
                    AccessoryShowLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    FormData.ContainsKey(formDataKey)
                    && FormData[formDataKey].IsT3
                    && FormData[formDataKey].AsT3 == state,
                    () => FormData[formDataKey] = state)
            );
        }

        private static void SlaveItemSelection(Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            Subtitle(SlaveItemSelectionSubtitle);

            SlaveItemWearSelection(activeFemale, masterItemFormData);

            GUILayout.Space(12);

            // slave item accessories model
            SlaveItemAccessorySelection(activeFemale, masterItemFormData);
            SlaveItemExtendedAccessorySelection(activeFemale, masterItemFormData);
        }

        private static void SlaveItemWearSelection(Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            var slaveItemWearsModelFiltered = Enum.GetValues(typeof(WEAR_SHOW_TYPE))
                .Cast<WEAR_SHOW_TYPE>()
                .Where(type => type != WEAR_SHOW_TYPE.NUM)
                .Where(type => {
                    if (masterItemFormData.IsT1)
                        return true;

                    var thisWearData = activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)type]);

                    return type != masterItemFormData.AsT0.Type
                           && !thisWearData.Equals(masterItemFormData.AsT0.WearData);
                })
                .Where(type => Array.Exists(activeFemale.wears.wearObjs,
                    wo => wo != null && wo.type == Wears.ShowToWearType[(int)type]))
                .ToArray();


            Flow(slaveItemWearsModelFiltered, (itemPart, idx) => {
                var slaveWearData = activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)itemPart]);
                RadioButton(
                    WearShowTypeLabels.GetValueOrDefaultValue(itemPart, ErrorLabel),
                    FormData.ContainsKey(SlaveItemFormDataKey)
                    && FormData[SlaveItemFormDataKey].IsT0
                    && FormData[SlaveItemFormDataKey].AsT0.Type == itemPart,
                    () => {
                        if (FormData.ContainsKey(SlaveItemFormDataKey) && FormData[SlaveItemFormDataKey].IsT3)
                            FormData.Remove(SlaveItemStateFormDataKey);

                        FormData[SlaveItemFormDataKey] = new ItemWearFormData
                            { Type = itemPart, WearData = slaveWearData };
                    }
                );
            });
        }

        private static void SlaveItemAccessorySelection(Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
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
                    FormData.ContainsKey(SlaveItemFormDataKey)
                    && FormData[SlaveItemFormDataKey].IsT1
                    && FormData[SlaveItemFormDataKey].AsT1.SlotNo == accessoryObj.slot,
                    () => {
                        if (FormData.ContainsKey(SlaveItemStateFormDataKey) && FormData[SlaveItemStateFormDataKey].IsT2)
                            FormData.Remove(SlaveItemStateFormDataKey);

                        FormData[SlaveItemFormDataKey] =
                            new ItemAccessoryFormData {
                                SlotNo = accessoryObj.slot, AccessoryParameter = accessoryObj.acceParam,
                                AccessoryData = accessoryData
                            };
                    }
                );
            }, 3);
        }

        private static void SlaveItemExtendedAccessorySelection(Female activeFemale, OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData) {
            if (Ash.MoreAccessoriesInstance == null)
                return;

            var extendedModel = Ash.MoreAccessoriesInstance
                .GetAdditionalData(activeFemale.customParam)
                .accessories
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
                    FormData.ContainsKey(SlaveItemFormDataKey)
                    && FormData[SlaveItemFormDataKey].IsT1
                    && FormData[SlaveItemFormDataKey].AsT1.SlotNo == accessoryObj.slot,
                    () => {
                        if (FormData.ContainsKey(SlaveItemStateFormDataKey) && FormData[SlaveItemStateFormDataKey].IsT2)
                            FormData.Remove(SlaveItemStateFormDataKey);

                        FormData[SlaveItemFormDataKey] =
                            new ItemAccessoryFormData {
                                SlotNo = accessoryObj.slot, AccessoryParameter = accessoryObj.acceParam,
                                AccessoryData = accessoryData
                            };
                    }
                );
            }, 3);
        }

        private static void SlaveItemStateSelection() {
            Subtitle(SlaveItemStateSelectionSubtitle);
            FormData.TryGetValue(SlaveItemFormDataKey, out var slaveItemFromData);
            switch (slaveItemFromData.Value) {
                case ItemWearFormData _:
                    ItemWearStateSelection(SlaveItemStateFormDataKey);
                    break;

                case ItemAccessoryFormData _:
                    ItemAccessoryStateSelection(SlaveItemStateFormDataKey);
                    break;

                default:
                    Ash.Logger.LogError("Unknown type of SlaveItem form data.");
                    return;
            }
        }

        private static void GenerateReverseRulesRadioSet() {
            Subtitle(GenerateReverseRulesSubtitle);

            if (!FormData.ContainsKey(GenerateReverseRulesFormDataKey))
                FormData[GenerateReverseRulesFormDataKey] = GenerateReverseRulesEnum.GenerateReverseRules.Ignore;

            Flow(
                Enum.GetValues(typeof(GenerateReverseRulesEnum.GenerateReverseRules)).Cast<GenerateReverseRulesEnum.GenerateReverseRules>().ToArray(),
                (state, idx) => RadioButton(
                    GenerateReverseRulesLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    FormData.ContainsKey(GenerateReverseRulesFormDataKey)
                    && FormData[GenerateReverseRulesFormDataKey].IsT5
                    && FormData[GenerateReverseRulesFormDataKey].AsT5 == state,
                    () => FormData[GenerateReverseRulesFormDataKey] = state)
            );
        }

        private static void ReverseRulesItemState() {
            if (!FormData.ContainsKey(SlaveItemFormDataKey)
                || !FormData.ContainsKey(SlaveItemStateFormDataKey)
                || !FormData.ContainsKey(GenerateReverseRulesFormDataKey)
                || (GenerateReverseRulesEnum.GenerateReverseRules)FormData[GenerateReverseRulesFormDataKey].Value
                   != GenerateReverseRulesEnum.GenerateReverseRules.Generate)
                return;

            Subtitle(ReverseRulesStateSubtitle);
            FormData.TryGetValue(SlaveItemFormDataKey, out var slaveItemFormData);
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
                            FormData.ContainsKey(ReverseRulesStateFormDataKey)
                            && FormData[ReverseRulesStateFormDataKey].IsT2
                            && FormData[ReverseRulesStateFormDataKey].AsT2 == state,
                            () => FormData[ReverseRulesStateFormDataKey] = state)
                    );

                    break;
                }

                case ItemAccessoryFormData _:
                {
                    Flow(
                        new[] {false, true},
                        (state, idx) => RadioButton(
                            AccessoryShowLabels.GetValueOrDefaultValue(state, ErrorLabel),
                            FormData.ContainsKey(ReverseRulesStateFormDataKey)
                            && FormData[ReverseRulesStateFormDataKey].IsT3
                            && FormData[ReverseRulesStateFormDataKey].AsT3 == state,
                            () => FormData[ReverseRulesStateFormDataKey] = state)
                    );

                    break;
                }
            }
        }
    }
}
