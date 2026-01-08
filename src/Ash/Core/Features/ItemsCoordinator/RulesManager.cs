using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator.Types;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.Core.SceneManagement;
using Ash.GlobalUtils;
using Character;
using H;
using OneOf;

namespace Ash.Core.Features.ItemsCoordinator
{
    internal static class RulesManager
    {
        // RuleSets are stored here
        public static List<InterItemRuleSet> InterItemRuleSets { get; } = IO.Load<List<InterItemRuleSet>>(IO.InterItemRulesFileName);
        public static List<HPosRuleSet> HPosRuleSets { get; } =  IO.Load<List<HPosRuleSet>>(IO.HPosRulesFileName);

        public static void AddRule(
            Female female,
            OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData,
            OneOf<WEAR_SHOW, bool> masterItemStateFormData,
            OneOf<ItemWearFormData, ItemAccessoryFormData> slaveItemFormData,
            OneOf<WEAR_SHOW, bool> slaveItemStateFormData,
            bool generateReverseRules = false,
            OneOf<WEAR_SHOW, bool> reverseRulesState = default
        ) {
            if (!female) {
                Ash.Logger.LogWarning("Attempting to add the Rule, but Female is null");
                return;
            }

            var masterItem = MakeMasterItem(masterItemFormData);
            var slaveItem = MakeSlaveItem(slaveItemFormData, masterItemStateFormData, slaveItemStateFormData);

            var idx = InterItemRuleSets.FindIndex(rule => rule.MasterItem == masterItem);
            if (idx != -1) {
                var slavesHashSet = InterItemRuleSets[idx].SlaveItems;
                if (slavesHashSet == null) {
                    Ash.Logger.LogWarning(
                        "Rule for item was found, but Slaves array is null. This empty rule will be erased.");
                    InterItemRuleSets.RemoveAt(idx);
                    return;
                }

                if (!slavesHashSet.Add(slaveItem))
                    Ash.Logger.LogWarning("Failed to add new SlaveItem to HashSet - already exists?");
            }
            else {
                InterItemRuleSets.Add(new InterItemRuleSet {
                    MasterItem = masterItem,
                    SlaveItems = new HashSet<SlaveItem> { slaveItem }
                });
            }

            // Write DB to disk after new rule is added
            IO.Save(InterItemRuleSets, IO.InterItemRulesFileName);

            // Should only apply newly added rule and nothing else
            var females = SceneComponentRegistry.GetComponentsOfType<Female>().ToArray();
            foreach (var f in females) {
                switch (masterItemFormData.Value) {
                    case ItemWearFormData masterCastToWear:
                        ItemsCoordinator.ApplyRuleset(f, masterCastToWear.Type);
                        break;

                    case ItemAccessoryFormData masterCastToAccessory:
                        ItemsCoordinator.ApplyRuleset(f, masterCastToAccessory.SlotNo);
                        break;

                    default:
                        Ash.Logger.LogError("[RulesManager] Unknown type of MasterItem!");
                        return;
                }
            }

            if (generateReverseRules)
                GenerateReverseRules(
                    female,
                    masterItemFormData,
                    masterItemStateFormData,
                    slaveItemFormData,
                    slaveItemStateFormData,
                    reverseRulesState
                );
        }

        public static void AddRule(
            Female female,
            OneOf<ItemWearFormData, WEAR_SHOW_TYPE> hPosItemFormData,
            OneOf<H_StyleData.TYPE, HStyleDetail> hPosStyleFormData
        ) {
            if (!female) {
                Ash.Logger.LogWarning("Attempting to add the Rule, but Female is null");
                return;
            }

            var hPosItem = hPosItemFormData.IsT0
                ? MakeMasterItem(hPosItemFormData.AsT0)
                : (OneOf<BaseItem, WEAR_SHOW_TYPE>)hPosItemFormData.AsT1;

            HPosRuleSets.Add(new HPosRuleSet(hPosItem, hPosStyleFormData));

            IO.Save(HPosRuleSets, IO.HPosRulesFileName);
        }

        public static void RemoveRule(InterItemRuleData data) {
            var idx = InterItemRuleSets.FindIndex(rule => rule.MasterItem == data.MasterItem);
            if (idx == -1) {
                Ash.Logger.LogWarning("Unable to remove the rule for this MasterItem.");
                return;
            }

            if (!InterItemRuleSets[idx].SlaveItems.Remove(data.SlaveItem)) {
                Ash.Logger.LogWarning("Unable to remove the rule - SlaveItem was not found in the HashSet<SlaveItems>");
                return;
            }

            // nuke the ruleset if there are no rules left in it
            if (InterItemRuleSets[idx].SlaveItems.Count == 0)
                InterItemRuleSets.RemoveAt(idx);

            // Write DB to disk after rule is removed
            IO.Save(InterItemRuleSets, IO.InterItemRulesFileName);
        }

        public static void RemoveRule(HPosRuleData data) {
            var idx = HPosRuleSets.FindIndex(rule => rule.HPosItem.Equals(data.HPosItem)
            );
            if (idx == -1) {
                Ash.Logger.LogWarning("Unable to remove the rule for this HPosItem.");
                return;
            }

            HPosRuleSets.RemoveAt(idx);

            // Write DB to disk after rule is removed
            IO.Save(HPosRuleSets, IO.HPosRulesFileName);
        }

        private static BaseItem MakeMasterItem(OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemData) {
            switch (masterItemData.Value) {
                case ItemWearFormData masterCastToWear:
                {
                    var dataOfWearItem = new DataOfWearItem(masterCastToWear.Type);
                    return new BaseItem(
                        masterCastToWear.WearData.id,
                        masterCastToWear.WearData.assetbundleName,
                        masterCastToWear.WearData.prefab,
                        masterCastToWear.WearData.name,
                        dataOfWearItem
                    );
                }
                case ItemAccessoryFormData masterCastToAccessory:
                {
                    var accessoryCustom = masterCastToAccessory.AccessoryParameter.slot[masterCastToAccessory.SlotNo % AccessoryCustom.SLOT_NUM];
                    var dataOfAccessoryItem = new DataOfAccessoryItem(
                        masterCastToAccessory.SlotNo,
                        accessoryCustom.type,
                        accessoryCustom.nowAttach
                    );

                    return new BaseItem(
                        masterCastToAccessory.AccessoryData.id,
                        masterCastToAccessory.AccessoryData.assetbundleName,
                        masterCastToAccessory.AccessoryData.prefab_F,
                        masterCastToAccessory.AccessoryData.name,
                        dataOfAccessoryItem
                    );
                }
                default:
                    Ash.Logger.LogError("Unknown type of ItemMasterData!");
                    return null;
            }
        }

        private static SlaveItem MakeSlaveItem(
            OneOf<ItemWearFormData, ItemAccessoryFormData> slaveItemData,
            OneOf<WEAR_SHOW, bool> masterItemStateFormData,
            OneOf<WEAR_SHOW, bool> slaveItemStateFormData
        ) {
            switch (slaveItemData.Value) {
                case ItemWearFormData slaveCastToWear:
                {
                    var dataOfWearItem = new DataOfWearItem(slaveCastToWear.Type);
                    return new SlaveItem(
                        slaveCastToWear.WearData.id,
                        slaveCastToWear.WearData.assetbundleName,
                        slaveCastToWear.WearData.prefab,
                        slaveCastToWear.WearData.name,
                        masterItemStateFormData,
                        slaveItemStateFormData,
                        dataOfWearItem
                    );
                }
                case ItemAccessoryFormData slaveCastToAccessory:
                {
                    var accessoryCustom = slaveCastToAccessory.AccessoryParameter.slot[slaveCastToAccessory.SlotNo % AccessoryCustom.SLOT_NUM];
                    var dataOfAccessoryItem = new DataOfAccessoryItem(
                        slaveCastToAccessory.SlotNo,
                        accessoryCustom.type,
                        accessoryCustom.nowAttach
                    );

                    return new SlaveItem(
                        slaveCastToAccessory.AccessoryData.id,
                        slaveCastToAccessory.AccessoryData.assetbundleName,
                        slaveCastToAccessory.AccessoryData.prefab_F,
                        slaveCastToAccessory.AccessoryData.name,
                        masterItemStateFormData,
                        slaveItemStateFormData,
                        dataOfAccessoryItem
                    );
                }
                default:
                    Ash.Logger.LogError("Unknown type of ItemMasterData!");
                    return null;
            }
        }

        private static void GenerateReverseRules(
            Female female,
            OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData,
            OneOf<WEAR_SHOW, bool> masterItemStateFormData,
            OneOf<ItemWearFormData, ItemAccessoryFormData> slaveItemFormData,
            OneOf<WEAR_SHOW, bool> slaveItemStateFormData, // <- reserved for future use
            OneOf<WEAR_SHOW, bool> requiredState
        ) {
            switch (masterItemFormData.Value) {
                case ItemWearFormData masterCastToWear:
                {
                    // grab all unused WEAR_SHOW states
                    foreach (WEAR_SHOW otherShow in Enum.GetValues(typeof(WEAR_SHOW))) {
                        if (otherShow == masterItemStateFormData.AsT0)
                            continue;

                        switch (slaveItemFormData.Value) {
                            case ItemWearFormData slaveCastToWear:
                                AddRule(female, masterCastToWear, otherShow, slaveCastToWear, requiredState.AsT0);
                                break;

                            case ItemAccessoryFormData slaveCastToAccessory:
                                AddRule(female, masterCastToWear, otherShow, slaveCastToAccessory, requiredState.AsT1);
                                break;

                            default:
                                Ash.Logger.LogError("Unknown item type of SlaveItem");
                                return;
                        }
                    }

                    break;
                }

                case ItemAccessoryFormData masterCastToAccessory:
                {
                    switch (slaveItemFormData.Value) {
                        case ItemWearFormData slaveCastToWear:
                            AddRule(female, masterCastToAccessory, !masterItemStateFormData.AsT1, slaveCastToWear,
                                requiredState.AsT0);
                            break;

                        case ItemAccessoryFormData slaveCastToAccessory:
                            AddRule(female, masterCastToAccessory, !masterItemStateFormData.AsT1, slaveCastToAccessory,
                                requiredState.AsT1);
                            break;

                        default:
                            Ash.Logger.LogError("Unknown item type of SlaveItem");
                            return;
                    }

                    break;
                }

                default:
                    Ash.Logger.LogError("Unknown item type of MasterItem");
                    return;
            }
        }
    }
}
