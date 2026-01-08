using Ash.Core.Features.ItemsCoordinator.Types;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.GlobalUtils;
using Character;
using H;
using OneOf;
using UnityEngine;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.HPosRuleForm;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination
{
    public static class HPosRuleDetailsComponent
    {
        public const string HPosStyleFormDataKey = "HPosStyle";

        public static void DrawHPosRuleDetailsView() {
            Button("Back", FormData.Clear);

            if (!HPosRuleItemSelectionComponent.IsHPosItemSelected())
                return;

            FormData.TryGetValue(FemaleFormDataKey, out var femaleFormData);
            var activeFemale = femaleFormData.AsT1;

            if (activeFemale == null) {
                Ash.Logger.LogWarning("Female is null.");
                return;
            }

            if (!FormData.TryGetValue(HPosRuleItemSelectionComponent.HPosRuleItemFormDataKey, out var hPosRuleItemFormDataRaw)) {
                Ash.Logger.LogWarning("MasterItem form data doesn't exist.");
                return;
            }

            var hPosItemFormData = hPosRuleItemFormDataRaw.IsT0
                ? hPosRuleItemFormDataRaw.AsT0
                : (OneOf<ItemWearFormData, WEAR_SHOW_TYPE>)hPosRuleItemFormDataRaw.AsT3;

            string hPosDataTitle;
            if (hPosItemFormData.IsT0) {
                var wearDataForHPosItem =
                    activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)hPosItemFormData.AsT0.Type]);
                hPosDataTitle = AutoTranslatorIntegration.Translate(wearDataForHPosItem.name);
            }
            else {
                hPosDataTitle = WearShowTypeLabels.GetValueOrDefaultValue(hPosItemFormData.AsT1, ErrorLabel);
                hPosDataTitle += " (Global)";
            }

            Title(hPosDataTitle);

            GUILayout.Space(8);

            SelectPoseType(hPosItemFormData);

            // Submit form
            GUILayout.Space(10);
            Button(CreateButtonLabel, SubmitForm, GUILayout.Height(30));
        }

        private static void SelectPoseType(OneOf<ItemWearFormData, WEAR_SHOW_TYPE> hPosItemFormData) {
            Subtitle("Select pose type:");

            var itemTypeData = hPosItemFormData.IsT0 ? hPosItemFormData.AsT0.Type : hPosItemFormData.AsT1;
            H_StyleData.TYPE[] hStylesModel = {};
            HStyleDetail[] hStyleDetailModel;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (itemTypeData) {
                case WEAR_SHOW_TYPE.TOPUPPER:
                case WEAR_SHOW_TYPE.SWIM_TOPUPPER:
                    // show1
                case WEAR_SHOW_TYPE.BRA:
                case WEAR_SHOW_TYPE.SWIMUPPER:
                    // show3
                    hStyleDetailModel = new [] {
                        new HStyleDetail { Type = H_StyleData.TYPE.SERVICE, Detail = H_StyleData.DETAIL.TITTY_FUCK }
                    };
                    break;
                case WEAR_SHOW_TYPE.TOPLOWER:
                case WEAR_SHOW_TYPE.BOTTOM:
                case WEAR_SHOW_TYPE.SWIM_TOPLOWER:
                case WEAR_SHOW_TYPE.SWIM_BOTTOM:
                    // show2
                    hStylesModel = new[] { H_StyleData.TYPE.SERVICE };
                    hStyleDetailModel = new [] {
                        new HStyleDetail { Type = H_StyleData.TYPE.PETTING, Detail = H_StyleData.DETAIL.VAGINA },
                        new HStyleDetail { Type = H_StyleData.TYPE.PETTING, Detail = H_StyleData.DETAIL.ANAL },
                        new HStyleDetail { Type = (H_StyleData.TYPE)(-2), Detail = (H_StyleData.DETAIL)(-2) }
                    };
                    break;
                case WEAR_SHOW_TYPE.SHORTS:
                case WEAR_SHOW_TYPE.SWIMLOWER:
                    // show4
                    hStylesModel = new[] { H_StyleData.TYPE.INSERT };
                    hStyleDetailModel = new [] {
                        new HStyleDetail { Type = H_StyleData.TYPE.INSERT, Detail = H_StyleData.DETAIL.ANAL },
                        new HStyleDetail { Type = H_StyleData.TYPE.PETTING, Detail = H_StyleData.DETAIL.VAGINA },
                        new HStyleDetail { Type = H_StyleData.TYPE.PETTING, Detail = H_StyleData.DETAIL.ANAL },
                        new HStyleDetail { Type = (H_StyleData.TYPE)(-2), Detail = (H_StyleData.DETAIL)(-2) }
                    };
                    break;
                case WEAR_SHOW_TYPE.PANST:
                    // show5
                    hStylesModel = new[] { H_StyleData.TYPE.INSERT };
                    hStyleDetailModel = new [] {
                        new HStyleDetail { Type = H_StyleData.TYPE.PETTING, Detail = H_StyleData.DETAIL.VAGINA },
                        new HStyleDetail { Type = H_StyleData.TYPE.PETTING, Detail = H_StyleData.DETAIL.ANAL },
                        new HStyleDetail { Type = (H_StyleData.TYPE)(-2), Detail = (H_StyleData.DETAIL)(-2) }
                    };
                    break;
                default:
                    return;
            }

            Flow(hStylesModel, (type, idx) => RadioButton(
                HStylesLabels.GetValueOrDefaultValue(type, ErrorLabel),
                FormData.ContainsKey(HPosStyleFormDataKey)
                && FormData[HPosStyleFormDataKey].IsT2
                && FormData[HPosStyleFormDataKey].AsT2 == type,
                () => FormData[HPosStyleFormDataKey] = type
            ));

            Flow(hStyleDetailModel, (hStyleDetail, idx) => RadioButton(
                HStylesExtendedLabels.GetValueOrDefaultValue(hStyleDetail, ErrorLabel),
                FormData.ContainsKey(HPosStyleFormDataKey)
                && FormData[HPosStyleFormDataKey].IsT4
                && FormData[HPosStyleFormDataKey].AsT4 == hStyleDetail,
                () => FormData[HPosStyleFormDataKey] = hStyleDetail
            ));
        }
    }
}
