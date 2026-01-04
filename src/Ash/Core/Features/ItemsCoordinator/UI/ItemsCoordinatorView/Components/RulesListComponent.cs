using System;
using System.Collections.Generic;
using Ash.Core.Features.ItemsCoordinator.Types;
using Ash.GlobalUtils;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.CommonLabels;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components
{
    public static class RulesListComponent
    {
        private const string ActiveRulesTitle = "Active Rules";
        private const string NoActiveRulesLabel = "No active rules";

        private static readonly List<Action> ListOfRuleChanges = new List<Action>();

        private static Vector2 RulesScrollPosition;

        public static void DrawRulesList() {
            Title(ActiveRulesTitle);

            GUILayout.Space(4);

            RulesScrollPosition = GUILayout.BeginScrollView(RulesScrollPosition, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            using (new GUILayout.VerticalScope("box", GUILayout.ExpandHeight(true))) {
                foreach (var ruleSet in RulesManager.RuleSets) {
                    foreach (var slaveItem in ruleSet.SlaveItems) {
                        CreateRuleGuiElement(
                            new RulesManager.RuleData {
                                MasterItem = ruleSet.MasterItem,
                                SlaveItem = slaveItem
                            }
                        );
                    }
                }

                foreach (var action in ListOfRuleChanges)
                    action.Invoke();

                ListOfRuleChanges.Clear();

                if (RulesManager.RuleSets.Count == 0) {
                    GUILayout.FlexibleSpace();

                    using (new GUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(NoActiveRulesLabel);
                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.FlexibleSpace();
                }
            }

            GUILayout.EndScrollView();
        }

        private static void CreateRuleGuiElement(RulesManager.RuleData data) {
            var translatedMasterName = AutoTranslatorIntegration.Translate(data.MasterItem.Name);
            var translatedSlaveName = AutoTranslatorIntegration.Translate(data.SlaveItem.Name);

            var masterItemQuery = GetMasterItemLabelsData(data.MasterItem, data.SlaveItem);
            var masterItemPartLabel = masterItemQuery.Key;
            var masterItemStateLabel = masterItemQuery.Value;

            var slaveItemQuery = GetSlaveItemLabelsData(data.SlaveItem);
            var slaveItemPartLabel = slaveItemQuery.Key;
            var slaveItemStateLabel = slaveItemQuery.Value;

            var labelText =
                    "[WHEN]\n" +
                    $"    {translatedMasterName} <{masterItemPartLabel}>\n" +
                    $"    [IS]: {masterItemStateLabel}\n" +
                    "[SET]\n" +
                    $"    {translatedSlaveName} <{slaveItemPartLabel}>\n" +
                    $"    [TO]: {slaveItemStateLabel}"
                ;

            GUILayout.BeginVertical();

            using (new GUILayout.HorizontalScope("box")) {
                GUILayout.Label(labelText, GUILayout.ExpandWidth(true));
                Button(
                    "x",
                    () => ListOfRuleChanges.Add(() => RulesManager.RemoveRule(data)
                    ),
                    GUILayout.Width(30),
                    GUILayout.Height(30));
            }

            GUILayout.EndVertical();
        }

        private static KeyValuePair<string, string> GetMasterItemLabelsData(MasterItem masterItem, SlaveItem slaveItem) {
            var masterItemSlotLabel = ErrorLabel;
            var masterItemStateLabel = ErrorLabel;

            switch (masterItem.ItemData.Value) {
                // Set labels for different types of ItemMasterData
                case DataOfWearItem masterCastToWear:
                    masterItemSlotLabel =
                        WearShowTypeLabels.GetValueOrDefaultValue(masterCastToWear.ItemPart, ErrorLabel);
                    masterItemStateLabel =
                        WearShowLabels.GetValueOrDefaultValue(slaveItem.MasterItemState.AsT0, ErrorLabel);
                    break;
                case DataOfAccessoryItem masterCastToAccessory:
                    masterItemSlotLabel = $"In slot: {masterCastToAccessory.SlotNo}";
                    masterItemStateLabel = AccessoryShowLabels.GetValueOrDefaultValue(
                        slaveItem.MasterItemState.AsT1,
                        ErrorLabel);
                    break;
                default:
                    Ash.Logger.LogError("Data for MasterItem is of unknown type!");
                    break;
            }

            return new KeyValuePair<string, string>(masterItemSlotLabel, masterItemStateLabel);
        }

        private static KeyValuePair<string, string> GetSlaveItemLabelsData(SlaveItem slaveItem) {
            var slaveItemPartLabel = ErrorLabel;
            var slaveItemStateLabel = ErrorLabel;

            switch (slaveItem.ItemData.Value) {
                case DataOfWearItem slaveCastToWear:
                    slaveItemPartLabel =
                        WearShowTypeLabels.GetValueOrDefaultValue(slaveCastToWear.ItemPart, ErrorLabel);
                    slaveItemStateLabel =
                        WearShowLabels.GetValueOrDefaultValue(slaveItem.SlaveItemState.AsT0, ErrorLabel);
                    break;
                case DataOfAccessoryItem slaveCastToAccessory:
                    slaveItemPartLabel = $"In slot: {slaveCastToAccessory.SlotNo}";
                    slaveItemStateLabel =
                        AccessoryShowLabels.GetValueOrDefaultValue(slaveItem.SlaveItemState.AsT1,
                            ErrorLabel);
                    break;
            }

            return new KeyValuePair<string, string>(slaveItemPartLabel, slaveItemStateLabel);
        }
    }
}
