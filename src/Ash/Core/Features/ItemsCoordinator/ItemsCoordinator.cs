using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator.Types;
using Ash.GlobalUtils;
using Character;
using OneOf;

namespace Ash.Core.Features.ItemsCoordinator
{
    internal static class ItemsCoordinator
    {
        public static bool SkipRulesApplication { get; set; }

        public static void ApplyRules(Female female, List<RulesManager.RuleSet> ruleset) {
            if (SkipRulesApplication) {
                Ash.Logger.LogDebug("Not applying rules, because skip flag was set.");
                return;
            }

            if (!female) {
                Ash.Logger.LogWarning("Attempting to apply the wear rule, but the passed Female arg is null.");
                return;
            }

            var dataArrForWears = GetDataForCurrentlyWornWears(female);
            var dataArrForAccessories = GetDataForCurrentWornAccessories(female);

            foreach (var ruleSet in ruleset) {
                // if MasterItem in this rule isn't currently worn by the character - skip
                if (!IsCurrentlyOnTheChara(dataArrForWears, ruleSet.MasterItem) &&
                    !IsCurrentlyOnTheChara(dataArrForAccessories, ruleSet.MasterItem)) {
                    Ash.Logger.LogDebug($"This MasterItem isn't worn by the character <{female.heroineID}> - skip.");
                    continue;
                }

                foreach (var slaveItem in ruleSet.SlaveItems) {
                    // if SlaveItem in this rule isn't currently worn by the character - skip
                    if (!IsCurrentlyOnTheChara(dataArrForWears, slaveItem) &&
                        !IsCurrentlyOnTheChara(dataArrForAccessories, slaveItem)) {
                        Ash.Logger.LogDebug($"This SlaveItem isn't worn by the character <{female.heroineID}> - skip.");
                        continue;
                    }

                    switch (slaveItem.MasterItemState.Value) {
                        case WEAR_SHOW masterItemStateCastToWearShow:
                        {
                            TryToApplyRule(ruleSet, female, masterItemStateCastToWearShow, slaveItem);
                            break;
                        }

                        case bool masterItemStateCastToBool:
                        {
                            TryToApplyRule(ruleSet, female, masterItemStateCastToBool, slaveItem);
                            break;
                        }

                        default:
                            Ash.Logger.LogError("[ItemsCoordinator:ApplyRules] Unknown type of MasterItem!");
                            return;
                    }
                }
            }
        }

        public static void ApplyRuleset(
            Female female,
            WEAR_SHOW_TYPE itemPart
        ) {
            if (SkipRulesApplication) {
                Ash.Logger.LogDebug("Not applying rules, because skip flag was set.");
                return;
            }

            if (female == null) {
                Ash.Logger.LogWarning("Attempting to apply the wear rule, but the passed Female arg is null.");
                return;
            }

            if (!Enum.IsDefined(typeof(WEAR_SHOW_TYPE), itemPart)) {
                Ash.Logger.LogWarning("Attempting to apply the wear rule, but the passed Master item data is illegal.");
                return;
            }

            // Retrieve the ruleset for passed MasterItem
            var rulesets = RulesManager.RuleSets
                .Where(ruleSet =>
                    ruleSet.MasterItem.ItemData.IsT0
                    && ruleSet.MasterItem.ItemData.AsT0.ItemPart == itemPart
                ).ToList();

            ApplyRules(female, rulesets);
        }

        public static void ApplyRuleset(
            Female female,
            int slotNo
        ) {
            if (SkipRulesApplication) {
                Ash.Logger.LogDebug("Not applying rules, because skip flag was set.");
                return;
            }

            if (female == null) {
                Ash.Logger.LogWarning("Attempting to apply the accessory rule, but the passed Female arg is null.");
                return;
            }

            if (slotNo < 0) {
                Ash.Logger.LogWarning(
                    "Attempting to apply the accessory rule, but the passed Master item data is illegal.");
                return;
            }

            // Retrieve the ruleset for passed MasterItem
            var rulesets = RulesManager.RuleSets
                .Where(ruleSet =>
                    ruleSet.MasterItem.ItemData.IsT1
                    && ruleSet.MasterItem.ItemData.AsT1.SlotNo == slotNo
                ).ToList();

            ApplyRules(female, rulesets);
        }

        private static KeyValuePair<WearObj[], WearData[]> GetDataForCurrentlyWornWears(Female female) {
            var validWearObjects = female.wears.wearObjs
                .Where(obj => obj != null)
                .ToArray();

            var currentlyWornTypes = validWearObjects
                .Select(obj => obj.type);

            var wearDataOfCurrentlyWorn = currentlyWornTypes
                .Select(type => female.wears.GetWearData(type))
                .ToArray();

            return new KeyValuePair<WearObj[], WearData[]>(validWearObjects, wearDataOfCurrentlyWorn);
        }

        private static KeyValuePair<Accessories.AcceObj[], AccessoryData[]> GetDataForCurrentWornAccessories(
            Female female) {
            var validAccessoryObjects = female.accessories.acceObjs
                .Where(obj => obj != null)
                .ToArray();

            if (Ash.MoreAccessoriesInstance != null) {
                var extendedAccessoryObjects = Ash.MoreAccessoriesInstance
                    .GetAdditionalData(female.customParam)
                    .accessories
                    .Select(ad => (Accessories.AcceObj)ad.acceObj)
                    .ToArray();

                validAccessoryObjects = validAccessoryObjects
                    .Concat(extendedAccessoryObjects)
                    .ToArray();

            }

            var currentlyWornData = validAccessoryObjects
                .Select(obj => new { obj.acceParam, obj.slot });

            var accessoryDataOfCurrentlyWorn = currentlyWornData
                .Select(data => female.accessories.GetAccessoryData(data.acceParam, data.slot))
                .ToArray();

            return new KeyValuePair<Accessories.AcceObj[], AccessoryData[]>(validAccessoryObjects,
                accessoryDataOfCurrentlyWorn);
        }

        private static bool IsCurrentlyOnTheChara(KeyValuePair<WearObj[], WearData[]> data, BaseItem item) {
            if (!item.ItemData.IsT0)
                return false;

            return data.Value
                .Select((wearData, idx) => new { wearData, idx })
                .Any(obj => obj.wearData.id == item.Id
                            && obj.wearData.prefab.Equals(item.Prefab)
                            && obj.wearData.assetbundleName.Equals(item.AssetbundleName)
                            && data.Key[obj.idx].type == Wears.ShowToWearType[(int)item.ItemData.AsT0.ItemPart]);
        }

        private static bool IsCurrentlyOnTheChara(KeyValuePair<Accessories.AcceObj[], AccessoryData[]> data, BaseItem item) {
            if (!item.ItemData.IsT1)
                return false;

            return data.Value
                .Select((accessoryData, idx) => new { accessoryData, idx })
                .Any(anonObj => {
                    var idx = anonObj.idx;
                    var actualSlotNo = data.Key[anonObj.idx].slot;
                    var accessoryCustom = data.Key[idx].acceParam.slot[actualSlotNo % AccessoryCustom.SLOT_NUM];

                    return anonObj.accessoryData.id == item.Id
                        && anonObj.accessoryData.prefab_F.Equals(item.Prefab)
                        && anonObj.accessoryData.assetbundleName.Equals(item.AssetbundleName)
                        && actualSlotNo == item.ItemData.AsT1.SlotNo
                        && accessoryCustom.type == item.ItemData.AsT1.Type
                        && accessoryCustom.nowAttach == item.ItemData.AsT1.NowAttach;
                });
        }

        private static void TryToApplyRule(RulesManager.RuleSet ruleSet, Female female, OneOf<WEAR_SHOW, bool> masterItemState, SlaveItem slaveItem) {
            if (ruleSet.MasterItem.ItemData.IsT0 && !masterItemState.IsT0 || ruleSet.MasterItem.ItemData.IsT1 && !masterItemState.IsT1){
                Ash.Logger.LogWarning("Inconsistent data types between RuleSet.MasterItem.ItemData " +
                    "and SlaveItem.MasterItemState");
                return;
            }

            // check if MasterItem state matches what's required by the Rule
            if (masterItemState.IsT0) {
                if (female.wears.GetShow(ruleSet.MasterItem.ItemData.AsT0.ItemPart, false) != masterItemState.AsT0)
                    return;
            } else if (masterItemState.IsT1) {
                if (SceneUtils.GetAccessoryShow(female, ruleSet.MasterItem.ItemData.AsT1.SlotNo) != masterItemState.AsT1)
                    return;
            } else {
                Ash.Logger.LogWarning("Unknown type of MasterItemState!");
                return;
            }

            switch (slaveItem.ItemData.Value) {
                case DataOfWearItem slaveItemDataCastToWear:
                {
                    if (!slaveItem.SlaveItemState.IsT0) {
                        Ash.Logger.LogWarning("Inconsistent data types between RuleSet.SlaveItem.ItemData " +
                            "and SlaveItem.SlaveItemState");
                        return;
                    }

                    // skip if slave state is same as required by the rule
                    if (female.wears.GetShow(slaveItemDataCastToWear.ItemPart, false) ==
                        slaveItem.SlaveItemState.AsT0) {
                        return;
                    }

                    if (masterItemState.IsT0) {
                        PrintRuleInfo(female, ruleSet.MasterItem.ItemData.AsT0, masterItemState, slaveItemDataCastToWear, slaveItem.SlaveItemState);
                    } else if (masterItemState.IsT1) {
                        PrintRuleInfo(female, ruleSet.MasterItem.ItemData.AsT1, masterItemState, slaveItemDataCastToWear, slaveItem.SlaveItemState);
                    }

                    SceneUtils.ChangeStateOfClothingItem(female, slaveItemDataCastToWear.ItemPart,
                        slaveItem.SlaveItemState.AsT0);

                    break;
                }

                case DataOfAccessoryItem slaveCastToAccessory:
                    if (!slaveItem.SlaveItemState.IsT1) {
                        Ash.Logger.LogWarning("Inconsistent data types between RuleSet.SlaveItem.ItemData " +
                                              "and SlaveItem.SlaveItemState");
                        return;
                    }

                    // skip if slave state is same as required by the rule
                    if (SceneUtils.GetAccessoryShow(female, slaveCastToAccessory.SlotNo) == slaveItem.SlaveItemState.AsT1) {
                        return;
                    }

                    if (masterItemState.IsT0) {
                        PrintRuleInfo(female, ruleSet.MasterItem.ItemData.AsT0, masterItemState, slaveCastToAccessory, slaveItem.SlaveItemState);
                    } else if (masterItemState.IsT1) {
                        PrintRuleInfo(female, ruleSet.MasterItem.ItemData.AsT1, masterItemState, slaveCastToAccessory, slaveItem.SlaveItemState);
                    }

                    SceneUtils.ChangeStateOfAccessoryItem(
                        female,
                        slaveCastToAccessory.SlotNo,
                        slaveItem.SlaveItemState.AsT1
                    );

                    break;

                default:
                    Ash.Logger.LogError("Unknown type of SlaveItem!");
                    return;
            }
        }

        private static void PrintRuleInfo(
            Female female,
            OneOf<DataOfWearItem, DataOfAccessoryItem> dataOfMasterItem,
            OneOf<WEAR_SHOW, bool> masterItemState,
            OneOf<DataOfWearItem, DataOfAccessoryItem> dataOfSlaveItem,
            OneOf<WEAR_SHOW, bool> slaveItemState
        ) {
            Ash.Logger.LogDebug("------------------------------------------");
            Ash.Logger.LogDebug(female.heroineID.ToString());

            switch (dataOfMasterItem.Value) {
                case DataOfWearItem masterCastToWear:
                    Ash.Logger.LogDebug($"Master {masterCastToWear.ItemPart} is {masterItemState.AsT0}");
                    break;
                case DataOfAccessoryItem masterCastToAccessory:
                    Ash.Logger.LogDebug($"Master {masterCastToAccessory.SlotNo} is {masterItemState.AsT1}");
                    break;
                default:
                    Ash.Logger.LogError("[ItemsCoordinator:PrintRuleInfo] Unknown type of MasterItem!");
                    return;
            }

            switch (dataOfSlaveItem.Value) {
                case DataOfWearItem slaveCastToWear:
                    Ash.Logger.LogDebug($"Setting {slaveCastToWear.ItemPart} to {slaveItemState.AsT0}");
                    break;
                case DataOfAccessoryItem slaveCastToAccessory:
                    Ash.Logger.LogDebug($"Setting {slaveCastToAccessory.SlotNo} to {slaveItemState.AsT1}");
                    break;
                default:
                    Ash.Logger.LogError("Unknown type of SlaveItem!");
                    return;
            }

            Ash.Logger.LogDebug("------------------------------------------");
        }
    }
}
