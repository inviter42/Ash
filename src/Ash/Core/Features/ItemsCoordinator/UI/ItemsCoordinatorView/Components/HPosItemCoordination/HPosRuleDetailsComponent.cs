using System;
using Ash.Core.Features.ItemsCoordinator.Types;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.GlobalUtils;
using Character;
using H;
using OneOf;
using UnityEngine;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.HPosRuleForm;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination
{
    internal static class HPosRuleDetailsComponent
    {
        internal const string HPosStyleFormDataKey = "HPosStyle";

        internal static void DrawHPosRuleDetailsView(HPosRuleForm form) {
            var formData = form.FormData;

            Button("Back", formData.Clear);

            if (!HPosRuleItemSelectionComponent.IsHPosItemSelected(form))
                return;

            if (!formData.TryGetValue(FemaleFormDataKey, out var femaleFormData)) {
                Ash.Logger.LogWarning("Female form data doesn't contain a value.");
                Ash.Logger.LogWarning(Environment.StackTrace);
                formData.Clear();
                return;
            }

            var activeFemale = femaleFormData.AsT1;

            if (!formData.TryGetValue(HPosRuleItemSelectionComponent.HPosRuleItemFormDataKey, out var hPosRuleItemFormDataRaw)) {
                Ash.Logger.LogWarning("MasterItem form data contain a value.");
                formData.Clear();
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

            SelectPoseType(form, hPosItemFormData);

            // Submit form
            GUILayout.Space(10);
            Button(CreateButtonLabel, form.SubmitForm, GUILayout.Height(30));
        }

        private static void SelectPoseType(HPosRuleForm form, OneOf<ItemWearFormData, WEAR_SHOW_TYPE> hPosItemFormData) {
            var formData = form.FormData;

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
                        new HStyleDetail(H_StyleData.TYPE.SERVICE, H_StyleData.DETAIL.TITTY_FUCK)
                    };
                    break;
                case WEAR_SHOW_TYPE.TOPLOWER:
                case WEAR_SHOW_TYPE.BOTTOM:
                case WEAR_SHOW_TYPE.SWIM_TOPLOWER:
                case WEAR_SHOW_TYPE.SWIM_BOTTOM:
                    // show2
                    hStylesModel = new[] { H_StyleData.TYPE.SERVICE };
                    hStyleDetailModel = new [] {
                        new HStyleDetail(H_StyleData.TYPE.PETTING, H_StyleData.DETAIL.VAGINA),
                        new HStyleDetail(H_StyleData.TYPE.PETTING, H_StyleData.DETAIL.ANAL),
                        new HStyleDetail((H_StyleData.TYPE)(-2), (H_StyleData.DETAIL)(-2))
                    };
                    break;
                case WEAR_SHOW_TYPE.SHORTS:
                case WEAR_SHOW_TYPE.SWIMLOWER:
                    // show4
                    hStylesModel = new[] { H_StyleData.TYPE.INSERT };
                    hStyleDetailModel = new [] {
                        new HStyleDetail(H_StyleData.TYPE.INSERT, H_StyleData.DETAIL.ANAL),
                        new HStyleDetail(H_StyleData.TYPE.PETTING, H_StyleData.DETAIL.VAGINA),
                        new HStyleDetail(H_StyleData.TYPE.PETTING, H_StyleData.DETAIL.ANAL),
                        new HStyleDetail((H_StyleData.TYPE)(-2), (H_StyleData.DETAIL)(-2))
                    };
                    break;
                case WEAR_SHOW_TYPE.PANST:
                    // show5
                    hStylesModel = new[] { H_StyleData.TYPE.INSERT };
                    hStyleDetailModel = new [] {
                        new HStyleDetail(H_StyleData.TYPE.PETTING, H_StyleData.DETAIL.VAGINA),
                        new HStyleDetail(H_StyleData.TYPE.PETTING, H_StyleData.DETAIL.ANAL),
                        new HStyleDetail((H_StyleData.TYPE)(-2), (H_StyleData.DETAIL)(-2))
                    };
                    break;
                default:
                    return;
            }

            Flow(hStylesModel, (type, idx) => RadioButton(
                HStylesLabels.GetValueOrDefaultValue(type, ErrorLabel),
                formData.ContainsKey(HPosStyleFormDataKey)
                && formData[HPosStyleFormDataKey].IsT2
                && formData[HPosStyleFormDataKey].AsT2 == type,
                () => formData[HPosStyleFormDataKey] = type
            ));

            Flow(hStyleDetailModel, (hStyleDetail, idx) => RadioButton(
                HStylesExtendedLabels.GetValueOrDefaultValue(hStyleDetail, ErrorLabel),
                formData.ContainsKey(HPosStyleFormDataKey)
                && formData[HPosStyleFormDataKey].IsT4
                && formData[HPosStyleFormDataKey].AsT4 == hStyleDetail,
                () => formData[HPosStyleFormDataKey] = hStyleDetail
            ));
        }
    }
}
