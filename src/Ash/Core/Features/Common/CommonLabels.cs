using System.Collections.Generic;
using Character;

namespace Ash.Core.Features.Common
{
    internal static class CommonLabels
    {
        public static readonly Dictionary<WEAR_SHOW_TYPE, string> WearShowTypeLabels =
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

        public static readonly Dictionary<WEAR_SHOW, string> WearShowLabels = new Dictionary<WEAR_SHOW, string> {
            [WEAR_SHOW.ALL] = "Dressed",
            [WEAR_SHOW.HALF] = "Half",
            [WEAR_SHOW.HIDE] = "Hidden",
        };

        public static readonly Dictionary<bool, string> AccessoryShowLabels = new Dictionary<bool, string> {
            [true] = "Equipped",
            [false] = "Hidden"
        };

        public static readonly Dictionary<ACCESSORY_TYPE, string> AccessoryShowTypeLabels =
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

        public const string ErrorLabel = "???";
    }
}
