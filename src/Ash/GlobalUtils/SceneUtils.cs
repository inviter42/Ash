using System;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator;
using Ash.Core.SceneManagement;
using Character;
using UObject = UnityEngine.Object;

namespace Ash.GlobalUtils
{
    public static class SceneUtils
    {
        /// <summary>
        /// Searches for all the objects of type Female and combines all found objects' HeroineID properties
        /// into a string Array.
        /// </summary>
        /// <returns>string[](could be empty)</returns>
        public static string[] GetHeroineIDsInSceneAsStrings() {
            return SceneComponentRegistry.GetComponentsOfType<Female>().ToArray()
                .Select(obj => obj.HeroineID.ToString())
                .ToArray();
        }

        /// <summary>
        /// Takes an ID from HeroineID enum as tries to find an instance with this name.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>Female|Null</returns>
        public static Female GetFemaleComponentByHeroineIDString(string identifier) {
            return Array.Find(
                UObject.FindObjectsOfType<Female>(),
                female => female.HeroineID.ToString() == identifier
            );
        }

        public static void ChangeStateOfClothingItem(Female female, WEAR_SHOW_TYPE item, WEAR_SHOW state) {
            if (female == null) {
                Ash.Logger.LogWarning("Unabled to change the state of clothing item - female is null");
                return;
            }

            if (item == WEAR_SHOW_TYPE.NUM) {
                // illegal wear type
                return;
            }

            female.wears.ChangeShow(item, state);
            female.wears.CheckShow();
        }

        public static void ChangeStateOfAllClothingItems(Female female, WEAR_SHOW state) {
            switch (state) {
                case WEAR_SHOW.HIDE:
                    ItemsCoordinator.SkipRulesApplication = true;

                    foreach (WEAR_SHOW_TYPE item in Enum.GetValues(typeof(WEAR_SHOW_TYPE))) {
                        ChangeStateOfClothingItem(female, item, state);
                    }
                    ItemsCoordinator.SkipRulesApplication = false;
                    break;
                case WEAR_SHOW.ALL:
                case WEAR_SHOW.HALF:
                default:
                    foreach (WEAR_SHOW_TYPE item in Enum.GetValues(typeof(WEAR_SHOW_TYPE))) {
                        ChangeStateOfClothingItem(female, item, state);
                    }
                    ItemsCoordinator.ApplyRules(female, RulesManager.InterItemRuleSets);
                    break;
            }
        }

        public static void CycleStateOfClothingItem(Female female, WEAR_SHOW_TYPE item) {
            if (female == null) {
                Ash.Logger.LogWarning("Unabled to change the state of clothing item - female is null.");
                return;
            }

            if (item == WEAR_SHOW_TYPE.NUM) {
                Ash.Logger.LogWarning("Unabled to change the state of clothing item - illegal wear type.");
                return;
            }

            var visualState = female.wears.GetShow(item, true);
            var logicalState = female.wears.GetShow(item, false);

            if (visualState == logicalState) {
                // cycle to next state
                var nextState = EnumUtils.GetNextEnumValue(logicalState);
                ChangeStateOfClothingItem(female, item, nextState);
            }
            else {
                var pairItem = Wears.GetWearShowTypePair(item);

                ChangeStateOfClothingItem(female, item, WEAR_SHOW.ALL);
                ChangeStateOfClothingItem(female, pairItem, WEAR_SHOW.ALL);
            }
        }

        public static void ChangeStateOfAccessoryItem(Female female, int slotNo, bool itemState) {
            female.accessories.SetShow(slotNo, itemState);
        }

        public static void CycleStateOfAccessoryItem(Female female, int slotNo) {
            if (female == null) {
                Ash.Logger.LogWarning("Unabled to change the state of accessory item - female is null.");
                return;
            }

            if (slotNo < 0) {
                Ash.Logger.LogWarning("Unabled to change the state of accessory item - illegal slotNo.");
            }

            female.accessories.SetShow(slotNo, !GetAccessoryShow(female, slotNo));
        }

        public static void ChangeStateOfAllAccessoryItems(Female female, bool state) {
            if (female == null) {
                Ash.Logger.LogWarning("Unabled to change the state of accessory item - female is null.");
                return;
            }

            switch (state) {
                case true:
                    female.accessories.ChangeAllShow(true);
                    ItemsCoordinator.ApplyRules(female, RulesManager.InterItemRuleSets);
                    break;
                case false:
                    ItemsCoordinator.SkipRulesApplication = true;
                    female.accessories.ChangeAllShow(false);
                    ItemsCoordinator.SkipRulesApplication = false;
                    break;
            }
        }

        public static bool GetAccessoryShow(Female female, int slotNo) {
            if (slotNo < AccessoryCustom.SLOT_NUM) {
                return female.accessories.acceObjs[slotNo]?.obj.activeSelf ?? false;
            }

            if (Ash.MoreAccessoriesInstance == null)
                return false;

            return ((Accessories.AcceObj)Ash.MoreAccessoriesInstance
                .GetAdditionalData(female.customParam)
                .accessories[slotNo - AccessoryCustom.SLOT_NUM]
                .acceObj)
                .obj
                .activeSelf;
        }

        public static WEAR_SHOW_TYPE[] GetActiveWearShowTypes(Female female) {
            if (female == null)
                return null;

            return Enum.GetValues(typeof(WEAR_SHOW_TYPE))
                .Cast<WEAR_SHOW_TYPE>()
                .Where(wearShowType => wearShowType != WEAR_SHOW_TYPE.NUM)
                .Where(e => female.wears.wearObjs[(int)Wears.ShowToWearType[(int)e]] != null)
                .Where(e => {
                    var wearObj = female.wears.wearObjs[(int)Wears.ShowToWearType[(int)e]];
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (e) {
                        case WEAR_SHOW_TYPE.TOPUPPER:
                        case WEAR_SHOW_TYPE.SWIMUPPER:
                        case WEAR_SHOW_TYPE.SWIM_TOPUPPER:
                            return wearObj.ShowUpperNum > 0;
                        case WEAR_SHOW_TYPE.TOPLOWER:
                        case WEAR_SHOW_TYPE.SWIMLOWER:
                        case  WEAR_SHOW_TYPE.SWIM_TOPLOWER:
                            return wearObj.ShowLowerNum > 0;
                        default:
                            return true;

                    }
                })
                .ToArray();

        }
    }
}
