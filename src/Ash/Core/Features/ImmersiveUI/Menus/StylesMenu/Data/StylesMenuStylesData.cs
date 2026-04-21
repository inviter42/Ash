using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ash.Core.SceneManagement;
using Character;
using H;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Data
{
    internal class StylesMenuStylesData : MonoBehaviour
    {
        internal readonly List<H_StyleData> ValidStyles = new List<H_StyleData>();

        private readonly Dictionary<string, H_StyleData> StyleDataDictionary = new Dictionary<string, H_StyleData>();

        private readonly List<H_StyleData> InsertStyles = new List<H_StyleData>();
        private readonly List<H_StyleData> ServiceStyles = new List<H_StyleData>();
        private readonly List<H_StyleData> PettingStyles = new List<H_StyleData>();

        internal void OnSelectedStyleTypeChanged(H_StyleData.TYPE type, H_StyleData.STATE state) {
            InitializeValidStylesList(type, state);
        }

        internal void OnSelectedFemaleStateChanged(H_StyleData.TYPE type, H_StyleData.STATE state) {
            InitializeValidStylesList(type, state);
        }

        private void Awake() {
            ClearDictionaries();
            InitializeStylesDictionary();
            InitializeStyleLists();
        }

        private void OnDestroy() {
            ClearDictionaries();
        }

        private void InitializeStylesDictionary() {
            FindStylesAndAddToDict("h/ha_*");
            FindStylesAndAddToDict("h/hh_*");
            FindStylesAndAddToDict("h/hs_*");
            FindStylesAndAddToDict("h/hm_*");
            FindStylesAndAddToDict("h/hw_*");
        }

        private void FindStylesAndAddToDict(string searchPattern) {
            var folder = string.Empty;
            var lastSlashIdx = searchPattern.LastIndexOf("/", StringComparison.Ordinal);

            if (lastSlashIdx != -1) {
                folder = searchPattern.Substring(0, lastSlashIdx);
                searchPattern = searchPattern.Remove(0, lastSlashIdx + 1);
            }

            foreach (var file in Directory.GetFiles($"{GlobalData.assetBundlePath}/{folder}", searchPattern,
                         SearchOption.TopDirectoryOnly)) {
                if (Path.GetExtension(file).Length != 0)
                    continue;

                var assetBundle = Path.GetFileNameWithoutExtension(file);
                if (folder.Length > 0)
                    assetBundle = $"{folder}/{assetBundle}";
                AddStyleToDict(assetBundle);
            }
        }

        private void AddStyleToDict(string assetBundle) {
            var hStyleData = new H_StyleData(assetBundle);
            StyleDataDictionary.Add(hStyleData.id, hStyleData);
        }

        private void InitializeStyleLists() {
            InsertStyles.Clear();
            ServiceStyles.Clear();
            PettingStyles.Clear();

            foreach (var hStyleData in StyleDataDictionary.Values) {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (hStyleData.type) {
                    case H_StyleData.TYPE.INSERT:
                        InsertStyles.Add(hStyleData);
                        break;
                    case H_StyleData.TYPE.SERVICE:
                        ServiceStyles.Add(hStyleData);
                        break;
                    case H_StyleData.TYPE.PETTING:
                        PettingStyles.Add(hStyleData);
                        break;
                    default:
                        Ash.Logger.LogWarning($"Invalid style type {hStyleData.type}");
                        break;
                }
            }
        }

        private void InitializeValidStylesList(H_StyleData.TYPE selectedStyleType, H_StyleData.STATE selectedStyleState) {
            List<H_StyleData> hStyleDataList;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (selectedStyleType) {
                case H_StyleData.TYPE.INSERT:
                    hStyleDataList = InsertStyles;
                    break;
                case H_StyleData.TYPE.SERVICE:
                    hStyleDataList = ServiceStyles;
                    break;
                case H_StyleData.TYPE.PETTING:
                    hStyleDataList = PettingStyles;
                    break;
                default:
                    Ash.Logger.LogError($"Invalid style type {selectedStyleType}");
                    return;
            }

            ValidStyles.Clear();

            foreach (var hStyleData in hStyleDataList.Where(hStyleData => IsStyleValid(hStyleData, selectedStyleState)))
                ValidStyles.Add(hStyleData);
        }

        private bool IsStyleValid(H_StyleData styleData, H_StyleData.STATE selectedStyleState) {
            var scene = SceneTypeTracker.Scene as H_Scene;
            if (scene == null) {
                Ash.Logger.LogError($"Invalid scene type - H_Scene expected");
                return false;
            }

            var female = scene.mainMembers.GetFemale(0);
            if (female.personality.state == Personality.STATE.FIRST ||
                female.personality.state == Personality.STATE.FLIP_FLOP ||
                female.personality.state == Personality.STATE.LAST_EVENT_YUKIKO_1 ||
                female.personality.state == Personality.STATE.LAST_EVENT_SISTERS ||
                female.personality.state == Personality.STATE.LAST_EVENT_YUKIKO_2 ||
                female.personality.state == Personality.STATE.LAST_EVENT_MARIKO)
                return IsStyleValidInEvent(female, styleData);

            return (styleData.member != H_StyleData.MEMBER.M1F2 ||
                    GlobalData.PlayData.Progress >= GamePlayData.PROGRESS.CLEAR_MAIN && scene.visitor != null &&
                    scene.visitor.GetHuman().sex != SEX.MALE &&
                    (scene.visitor.GetFemale().HeroineID != HEROINE.MARIKO ||
                     GlobalData.PlayData.Progress >= GamePlayData.PROGRESS.ALL_FREE)) &&
                   ((styleData.detailFlag & 2048 /*0x0800*/) == 0 ||
                    GlobalData.PlayData.Progress >= GamePlayData.PROGRESS.CLEAR_MAIN &&
                    (female.HeroineID != HEROINE.MARIKO ||
                     GlobalData.PlayData.Progress >= GamePlayData.PROGRESS.ALL_FREE)) && styleData.state == selectedStyleState &&
                   (styleData.map.Length <= 0 ||
                    scene.map.name.IndexOf(styleData.map, StringComparison.Ordinal) == 0) &&
                   scene.CheckEnableStyle(styleData.id);
        }

        private bool IsStyleValidInEvent(Female female, H_StyleData styleData) {
            if (female.personality.weakness && styleData.state != H_StyleData.STATE.WEAKNESS ||
                !female.personality.weakness && styleData.state == H_StyleData.STATE.WEAKNESS)
                return false;
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (female.personality.state) {
                case Personality.STATE.FIRST:
                {
                    string[,] strArray = {
                        {
                            "HS_00_00_08",
                            "HS_01_00_06",
                            "HH_01_00_01",
                            "HA_00_00_00",
                            "HS_00_02_08",
                            "HS_01_02_06",
                            "HH_01_02_01",
                            "HA_00_02_00"
                        }, {
                            "HS_00_00_00",
                            "HS_01_00_00",
                            "HH_01_00_00",
                            "HA_00_00_01",
                            "HS_00_02_00",
                            "HS_01_02_00",
                            "HH_01_02_00",
                            "HA_00_02_01"
                        }, {
                            "HS_00_00_02",
                            "HS_01_00_03",
                            "HH_03_00_02",
                            "HA_01_00_00",
                            "HS_00_02_02",
                            "HS_01_02_03",
                            "HH_03_02_02",
                            "HA_01_02_00"
                        }, {
                            "HM_00_00_01",
                            "HM_01_00_01",
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            "HM_00_02_01",
                            string.Empty,
                            string.Empty
                        }
                    };

                    var heroineId = (int)female.HeroineID;
                    for (var index = 0; index < 8; ++index) {
                        if (styleData.id == strArray[heroineId, index])
                            return true;
                    }

                    return false;
                }

                case Personality.STATE.FLIP_FLOP:
                {
                    string[,] strArray = {
                        {
                            "HS_00_01_00",
                            "HS_01_01_06",
                            "HH_01_00_00",
                            "HA_01_00_03",
                            "HS_00_02_00",
                            "HS_01_02_06",
                            "HH_01_02_00",
                            "HA_01_02_03"
                        }, {
                            "HS_00_00_01",
                            "HS_01_00_06",
                            "HH_01_00_01",
                            "HA_00_00_00",
                            "HS_00_02_01",
                            "HS_01_02_06",
                            "HH_01_02_01",
                            "HA_00_02_00"
                        }, {
                            "HS_00_01_01",
                            "HS_01_01_00",
                            "HH_00_01_00",
                            "HA_00_01_02",
                            "HS_00_02_01",
                            "HS_01_02_00",
                            "HH_00_02_00",
                            "HA_00_02_02"
                        }, {
                            "HS_20_01_08",
                            "HS_01_01_11",
                            "HH_06_01_00",
                            "HA_03_01_00",
                            "HS_20_02_08",
                            string.Empty,
                            string.Empty,
                            string.Empty
                        }
                    };

                    var heroineId = (int)female.HeroineID;
                    for (var index = 0; index < 8; ++index) {
                        if (styleData.id == strArray[heroineId, index])
                            return true;
                    }

                    return false;
                }

                case Personality.STATE.LAST_EVENT_YUKIKO_1:
                {
                    string[] strArray = {
                        "HS_00_01_06",
                        "HS_01_01_03",
                        "HH_02_01_00",
                        "HA_00_01_01",
                        "HS_00_02_06",
                        "HS_01_02_03",
                        "HH_02_02_00",
                        "HA_00_02_01"
                    };

                    return strArray.Any(str => styleData.id == str);
                }

                case Personality.STATE.LAST_EVENT_SISTERS:
                {
                    string[] strArray = {
                        "HW_00_01_00",
                        "HW_00_02_00"
                    };

                    return strArray.Any(str => styleData.id == str);
                }

                case Personality.STATE.LAST_EVENT_YUKIKO_2:
                {
                    string[] strArray = {
                        "HS_10_01_00",
                        "HH_10_01_00",
                        "HA_10_01_00",
                        "HS_10_02_00",
                        "HH_10_02_00",
                        "HA_10_02_00"
                    };

                    return strArray.Any(str => styleData.id == str);
                }
            }

            if (female.personality.state != Personality.STATE.LAST_EVENT_MARIKO)
                return false;

            string[] strArray1 = {
                "HS_20_01_09",
                "HS_01_00_01",
                "HH_01_01_01",
                "HA_00_01_01",
                "HS_20_02_09",
                "HS_01_02_01"
            };

            return strArray1.Any(str => styleData.id == str);
        }

        private void ClearDictionaries() {
            ValidStyles.Clear();
            StyleDataDictionary.Clear();
            InsertStyles.Clear();
            ServiceStyles.Clear();
            PettingStyles.Clear();
        }
    }
}
