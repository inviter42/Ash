using System;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.ItemsCoordinator;
using Ash.Core.SceneManagement;
using Character;
using MoreAccessoriesPH;
using UObject = UnityEngine.Object;

namespace Ash.GlobalUtils
{
    internal static class SceneUtils
    {
        /// <summary>
        /// Searches for all the objects of type Female and combines all found objects' HeroineID properties
        /// into a string Array.
        /// </summary>
        /// <returns>string[](could be empty)</returns>
        internal static string[] GetHeroineIDsInSceneAsStrings() {
            return SceneComponentRegistry.GetComponentsOfType<Female>().ToArray()
                .Select(obj => obj.HeroineID.ToString())
                .ToArray();
        }

        /// <summary>
        /// Takes an ID from HeroineID enum as tries to find an instance with this name.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>Female|Null</returns>
        internal static Female GetFemaleComponentByHeroineIDString(string identifier) {
            return Array.Find(
                UObject.FindObjectsOfType<Female>(),
                female => female.HeroineID.ToString() == identifier
            );
        }

        internal static void ChangeStateOfClothingItem(Female female, WEAR_SHOW_TYPE item, WEAR_SHOW state) {
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

        internal static void ChangeStateOfAllClothingItems(Female female, WEAR_SHOW state) {
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

        internal static WEAR_SHOW CycleStateOfWearItem(Female female, WEAR_SHOW_TYPE item, bool forward = true) {
            if (female == null) {
                Ash.Logger.LogWarning("Unabled to change the state of clothing item - female is null.");
                return (WEAR_SHOW)(-1);
            }

            if (item == WEAR_SHOW_TYPE.NUM) {
                Ash.Logger.LogWarning("Unabled to change the state of clothing item - illegal wear type.");
                return (WEAR_SHOW)(-1);
            }

            var visualState = female.wears.GetShow(item, true);
            var logicalState = female.wears.GetShow(item, false);

            if (visualState == logicalState) {
                // cycle to next state
                var wearShowNum =
                    female.wears.GetWearShowNum(item); // this number is a last possible show state
                var newState = wearShowNum == 1
                    ? logicalState == WEAR_SHOW.ALL ? WEAR_SHOW.HIDE : WEAR_SHOW.ALL
                    : forward
                        ? EnumUtils.GetNextEnumValue(logicalState)
                        : EnumUtils.GetPreviousEnumValue(logicalState);

                ChangeStateOfClothingItem(female, item, newState);
            }
            else {
                var pairItem = Wears.GetWearShowTypePair(item);

                ChangeStateOfClothingItem(female, item, WEAR_SHOW.ALL);
                ChangeStateOfClothingItem(female, pairItem, WEAR_SHOW.ALL);
            }

            return female.wears.GetShow(item, false);
        }

        internal static void ChangeStateOfAccessoryItem(Female female, int slotNo, bool itemState) {
            female.accessories.SetShow(slotNo, itemState);
        }

        internal static void CycleStateOfAccessoryItem(Female female, int slotNo) {
            if (female == null) {
                Ash.Logger.LogWarning("Unabled to change the state of accessory item - female is null.");
                return;
            }

            if (slotNo < 0) {
                Ash.Logger.LogWarning("Unabled to change the state of accessory item - illegal slotNo.");
            }

            female.accessories.SetShow(slotNo, !GetAccessoryShow(female, slotNo));
        }

        internal static void ChangeStateOfAllAccessoryItems(Female female, bool state) {
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

        internal static bool GetAccessoryShow(Female female, int slotNo) {
            if (slotNo < AccessoryCustom.SLOT_NUM) {
                return female.accessories.acceObjs[slotNo]?.obj.activeSelf ?? false;
            }

            if (Ash.MoreAccessoriesInstance == null)
                return false;

            var accessoryData = Ash.MoreAccessoriesInstance
                .GetAdditionalData(female.customParam)
                .accessories[slotNo - AccessoryCustom.SLOT_NUM];

            if (accessoryData?.acceObj == null)
                return false;

            return ((Accessories.AcceObj)accessoryData.acceObj)
                .obj
                .activeSelf;
        }

        internal static WEAR_SHOW_TYPE[] GetWearShowTypesOfEquippedItems(Female female) {
            if (female == null)
                return null;

            return Enum.GetValues(typeof(WEAR_SHOW_TYPE))
                .Cast<WEAR_SHOW_TYPE>()
                .Where(wearShowType => wearShowType != WEAR_SHOW_TYPE.NUM)
                .Where(e => female.wears.IsEquiped(female.customParam, e))
                .ToArray();
        }

        internal static List<Accessories.AcceObj> GetEquippedAccessoryObjects(Female female) {
            return female.accessories.acceObjs
                .Where(accessoryObj => accessoryObj != null)
                .ToList();
        }

        internal static List<MoreAccessoriesPH.MoreAccessories.AdditionalData.AccessoryData> GetEquippedExtendedAccessoryData(Female female) {
            return Ash.MoreAccessoriesInstance
                .GetAdditionalData(female.customParam)
                .accessories
                .Where(accessoryData => accessoryData?.acceObj != null)
                .ToList();
        }

        internal static ACCESSORY_ATTACH GetAccessoryAttachByIndex(Female female, int index) {
            var mainModel = GetEquippedAccessoryObjects(female);
            var extModel = GetEquippedExtendedAccessoryData(female);
            var getAttachFromMainModel = new Func<int, ACCESSORY_ATTACH>(i => mainModel[i].acceParam.slot[mainModel[i].slot].nowAttach);
            var getAttachFromExtModel = new Func<int, ACCESSORY_ATTACH>(i => extModel[i % mainModel.Count].accessoryCustom.nowAttach);
            var getter = index < mainModel.Count
                ? getAttachFromMainModel
                : getAttachFromExtModel;

            return getter(index);
        }

        internal static ACCESSORY_ATTACH GetAccessoryAttachByIndex(
            List<Accessories.AcceObj> mainModel,
            List<MoreAccessoriesPH.MoreAccessories.AdditionalData.AccessoryData> extModel,
            int index
        ) {
            var getAttachFromMainModel = new Func<int, ACCESSORY_ATTACH>(i => mainModel[i].acceParam.slot[mainModel[i].slot].nowAttach);
            var getAttachFromExtModel = new Func<int, ACCESSORY_ATTACH>(i => extModel[i % mainModel.Count].accessoryCustom.nowAttach);
            var getter = index < mainModel.Count
                ? getAttachFromMainModel
                : getAttachFromExtModel;

            return getter(index);
        }

        internal static ACCESSORY_TYPE GetAccessoryTypeByIndex(Female female, int index) {
            var mainModel = GetEquippedAccessoryObjects(female);
            var extModel = GetEquippedExtendedAccessoryData(female);
            var getTypeFromMainModel = new Func<int, ACCESSORY_TYPE>(i => mainModel[i].acceParam.slot[mainModel[i].slot].type);
            var getTypeFromExtModel = new Func<int, ACCESSORY_TYPE>(i => extModel[i % mainModel.Count].accessoryCustom.type);
            var getter = index < mainModel.Count
                ? getTypeFromMainModel
                : getTypeFromExtModel;

            return getter(index);
        }

        internal static ACCESSORY_TYPE GetAccessoryTypeByIndex(
            List<Accessories.AcceObj> mainModel,
            List<MoreAccessoriesPH.MoreAccessories.AdditionalData.AccessoryData> extModel,
            int index
        ) {
            var getTypeFromMainModel = new Func<int, ACCESSORY_TYPE>(i => mainModel[i].acceParam.slot[mainModel[i].slot].type);
            var getTypeFromExtModel = new Func<int, ACCESSORY_TYPE>(i => extModel[i % mainModel.Count].accessoryCustom.type);
            var getter = index < mainModel.Count
                ? getTypeFromMainModel
                : getTypeFromExtModel;

            return getter(index);
        }

        internal static Accessories.AcceObj GetAcceObjByIndex(Female female, int index) {
            var mainModel = GetEquippedAccessoryObjects(female);
            var extModel = GetEquippedExtendedAccessoryData(female);
            return index < mainModel.Count
                ? mainModel[index]
                : (Accessories.AcceObj)extModel[index % mainModel.Count].acceObj;
        }

        internal static Accessories.AcceObj GetAcceObjByIndex(
            List<Accessories.AcceObj> mainModel,
            List<MoreAccessories.AdditionalData.AccessoryData> extModel,
            int index
        ) {
            return index < mainModel.Count
                ? mainModel[index]
                : (Accessories.AcceObj)extModel[index % mainModel.Count].acceObj;
        }
    }
}
