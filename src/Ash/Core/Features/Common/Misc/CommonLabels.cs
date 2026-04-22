using System.Collections.Generic;
using Ash.Core.Features.ItemsCoordinator.Types;
using Character;
using H;

namespace Ash.Core.Features.Common.Misc
{
    internal static class CommonLabels
    {
        internal static readonly Dictionary<WEAR_SHOW_TYPE, string> WearShowTypeLabels =
            new Dictionary<WEAR_SHOW_TYPE, string> {
                [WEAR_SHOW_TYPE.TOPUPPER] = "Upper Top",
                [WEAR_SHOW_TYPE.TOPLOWER] = "Lower Top",
                [WEAR_SHOW_TYPE.BOTTOM] = "Bottom",
                [WEAR_SHOW_TYPE.BRA] = "Bra",
                [WEAR_SHOW_TYPE.SHORTS] = "Panties",
                [WEAR_SHOW_TYPE.SWIMUPPER] = "Upper Swimsuit",
                [WEAR_SHOW_TYPE.SWIMLOWER] = "Lower Swimsuit",
                [WEAR_SHOW_TYPE.SWIM_TOPUPPER] = "Upper Swimwear",
                [WEAR_SHOW_TYPE.SWIM_TOPLOWER] = "Lower Swimwear",
                [WEAR_SHOW_TYPE.SWIM_BOTTOM] = "Bottom Swimwear",
                [WEAR_SHOW_TYPE.GLOVE] = "Gloves",
                [WEAR_SHOW_TYPE.PANST] = "Pantyhose",
                [WEAR_SHOW_TYPE.SOCKS] = "Socks",
                [WEAR_SHOW_TYPE.SHOES] = "Shoes",
            };

        internal static readonly Dictionary<WEAR_SHOW, string> WearShowLabels = new Dictionary<WEAR_SHOW, string> {
            [WEAR_SHOW.ALL] = "Dressed",
            [WEAR_SHOW.HALF] = "Half",
            [WEAR_SHOW.HIDE] = "Hidden",
        };

        internal static readonly Dictionary<bool, string> AccessoryShowLabels = new Dictionary<bool, string> {
            [true] = "Equipped",
            [false] = "Hidden"
        };

        internal static readonly Dictionary<ACCESSORY_TYPE, string> AccessoryShowTypeLabels =
            new Dictionary<ACCESSORY_TYPE, string> {
                [ACCESSORY_TYPE.NONE] = "None",
                [ACCESSORY_TYPE.EAR] = "Ear",
                [ACCESSORY_TYPE.GLASSES] = "Glasses",
                [ACCESSORY_TYPE.FACE] = "Face",
                [ACCESSORY_TYPE.NECK] = "Neck",
                [ACCESSORY_TYPE.SHOULDER] = "Shoulder",
                [ACCESSORY_TYPE.CHEST] = "Chest",
                [ACCESSORY_TYPE.WAIST] = "Waist",
                [ACCESSORY_TYPE.BACK] = "Back",
                [ACCESSORY_TYPE.ARM] = "Arm",
                [ACCESSORY_TYPE.HAND] = "Hand",
                [ACCESSORY_TYPE.LEG] = "Leg",
                [ACCESSORY_TYPE.NUM] = "??"
            };

        internal static readonly Dictionary<ACCESSORY_ATTACH, string> AccessoryAttachLabels =
            new Dictionary<ACCESSORY_ATTACH, string> {
                [ACCESSORY_ATTACH.NONE] = "None",
                [ACCESSORY_ATTACH.AP_Head] = "Head",
                [ACCESSORY_ATTACH.AP_Megane] = "Eyewear",
                [ACCESSORY_ATTACH.AP_Earring_L] = "Left ear",
                [ACCESSORY_ATTACH.AP_Earring_R] = "Right ear",
                [ACCESSORY_ATTACH.AP_Mouth] = "Mouth",
                [ACCESSORY_ATTACH.AP_Nose] = "Nose",
                [ACCESSORY_ATTACH.AP_Neck] = "Neck",
                [ACCESSORY_ATTACH.AP_Chest] = "Breast",
                [ACCESSORY_ATTACH.AP_Wrist_L] = "Left wrist",
                [ACCESSORY_ATTACH.AP_Wrist_R] = "Right wrist",
                [ACCESSORY_ATTACH.AP_Arm_L] = "Left forearm",
                [ACCESSORY_ATTACH.AP_Arm_R] = "Right forearm",
                [ACCESSORY_ATTACH.AP_Index_L] = "Index finger (l)",
                [ACCESSORY_ATTACH.AP_Index_R] = "Index finger (r)",
                [ACCESSORY_ATTACH.AP_Middle_L] = "Middle finger (l)",
                [ACCESSORY_ATTACH.AP_Middle_R] = "Middle finger (r)",
                [ACCESSORY_ATTACH.AP_Ring_L] = "Ring finger (l)",
                [ACCESSORY_ATTACH.AP_Ring_R] = "Ring finger (r)",
                [ACCESSORY_ATTACH.AP_Leg_L] = "Left leg",
                [ACCESSORY_ATTACH.AP_Leg_R] = "Right leg",
                [ACCESSORY_ATTACH.AP_Ankle_L] = "Left ankle",
                [ACCESSORY_ATTACH.AP_Ankle_R] = "Right ankle",
                [ACCESSORY_ATTACH.AP_Tikubi_L] = "Left nipple",
                [ACCESSORY_ATTACH.AP_Tikubi_R] = "Right nipple",
                [ACCESSORY_ATTACH.AP_Waist] = "Waist",
                [ACCESSORY_ATTACH.AP_Shoulder_L] = "Left shoulder",
                [ACCESSORY_ATTACH.AP_Shoulder_R] = "Right shoulder",
                [ACCESSORY_ATTACH.AP_Hand_L] = "Left wrist/hand",
                [ACCESSORY_ATTACH.AP_Hand_R] = "Right wrist/hand"
            };

        internal static readonly Dictionary<H_StyleData.TYPE, string> HStylesLabels =
            new Dictionary<H_StyleData.TYPE, string> {
                [H_StyleData.TYPE.INSERT] = "Insert",
                [H_StyleData.TYPE.SERVICE] = "Service",
                [H_StyleData.TYPE.PETTING] = "Caress"
            };

        internal static readonly Dictionary<H_StyleData.STATE, string> HFemaleStateLabels =
            new Dictionary<H_StyleData.STATE, string> {
                [H_StyleData.STATE.RESIST] = "Res",
                [H_StyleData.STATE.FLOP] = "Acc",
                [H_StyleData.STATE.WEAKNESS] = "Exh"
            };

        internal static readonly Dictionary<HStyleDetail, string> HStylesExtendedLabels =
            new Dictionary<HStyleDetail, string> {
                [new HStyleDetail(H_StyleData.TYPE.SERVICE, H_StyleData.DETAIL.TITTY_FUCK)] = "Boob Job",
                [new HStyleDetail(H_StyleData.TYPE.PETTING, H_StyleData.DETAIL.VAGINA)] = "Caress → Any Vaginal",
                [new HStyleDetail(H_StyleData.TYPE.PETTING, H_StyleData.DETAIL.ANAL)] = "Caress → Any Anal",
                [new HStyleDetail(H_StyleData.TYPE.INSERT, H_StyleData.DETAIL.ANAL)] = "Insert → Any Anal",
                [new HStyleDetail((H_StyleData.TYPE)(-2), (H_StyleData.DETAIL)(-2))] = "All poses"
            };

        internal static readonly Dictionary<GAG_ITEM, string> GagItemLabels =
            new Dictionary<GAG_ITEM, string> {
                [GAG_ITEM.NONE] = "None",
                [GAG_ITEM.BALLGAG] = "Ball Gag",
                [GAG_ITEM.GUMTAPE] = "Tape"
            };

        internal static readonly Dictionary<LookAtRotator.TYPE, string> LookAtRotatorTypeLabels =
            new Dictionary<LookAtRotator.TYPE, string> {
                [LookAtRotator.TYPE.NO] = "Pose",
                [LookAtRotator.TYPE.FORWARD] = "Forward",
                [LookAtRotator.TYPE.TARGET] = "Target",
                [LookAtRotator.TYPE.HOLD] = "Fixed"
            };

        internal const string LookAtRotatorSpecifierDefault = "default";
        internal const string LookAtRotatorSpecifierCamera = "camera";

        internal static readonly Dictionary<string, string> IconLabels = new Dictionary<string, string> {
            ["piemenu-cog"] = "Settings",
            ["piemenu-heart"] = "Positions",
            ["piemenu-house"] = "Map",
            ["piemenu-log-out"] = "Leave",
            ["piemenu-mars"] = "Male",
            ["piemenu-message-circle-x"] = "Gag",
            ["piemenu-move-3d"] = "Move",
            ["piemenu-palette"] = "Editor",
            ["piemenu-repeat-2"] = "Replace",
            ["piemenu-shirt"] = "Clothing",
            ["piemenu-spotlight"] = "Light"
        };

        internal static readonly Dictionary<bool, string> ToggleStateLabels = new Dictionary<bool, string> {
            [true] = "On",
            [false] = "Off"
        };

        internal const string ErrorLabel = "???";

        internal const string CreateButtonLabel = "Create";
        internal const string CloseButtonLabel = "Close";
    }
}
