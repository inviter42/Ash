using System.Collections.Generic;
using Ash.Core.Features.ItemsCoordinator.Types;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Character;
using H;
using OneOf;
using InputsValues = OneOf.OneOf<
    Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types.ItemWearFormData,
    Female,
    H.H_StyleData.TYPE,
    Character.WEAR_SHOW_TYPE,
    Ash.Core.Features.ItemsCoordinator.Types.HStyleDetail
>;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State
{
    public class HPosRuleForm
    {

        public const string FemaleFormDataKey = "FemaleFormData";

        public readonly Dictionary<string, InputsValues> FormData = new Dictionary<string, InputsValues>();

        public void SubmitForm() {
            if (!ValidateInputs())
                return;

            var hPosItemFormDataRaw = FormData[HPosRuleItemSelectionComponent.HPosRuleItemFormDataKey];
            var hPosItemFormData = hPosItemFormDataRaw.IsT0
                ? hPosItemFormDataRaw.AsT0
                : (OneOf<ItemWearFormData, WEAR_SHOW_TYPE>) hPosItemFormDataRaw.AsT3;

            var hPosStyleFormDataRaw = FormData[HPosRuleDetailsComponent.HPosStyleFormDataKey];
            var hPosStyleFormData = hPosStyleFormDataRaw.IsT2
                ? hPosStyleFormDataRaw.AsT2
                : (OneOf<H_StyleData.TYPE, HStyleDetail>)hPosStyleFormDataRaw.AsT4;

            var femaleFormDataRaw = FormData[FemaleFormDataKey];

            // call add rule here

            RulesManager.AddRule(
                femaleFormDataRaw.AsT1,
                hPosItemFormData,
                hPosStyleFormData
            );

            FormData.Clear();
        }

        private bool ValidateInputs() {
            if (!FormData.ContainsKey(HPosRuleItemSelectionComponent.HPosRuleItemFormDataKey)) {
                Ash.Logger.LogWarning("HPosItem form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!FormData.ContainsKey(HPosRuleDetailsComponent.HPosStyleFormDataKey)) {
                Ash.Logger.LogWarning("HPosStyle form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!FormData.ContainsKey(FemaleFormDataKey)) {
                Ash.Logger.LogWarning("Female form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            var hPosItemFormData = FormData[HPosRuleItemSelectionComponent.HPosRuleItemFormDataKey];
            var hPosStyleFormData = FormData[HPosRuleDetailsComponent.HPosStyleFormDataKey];
            var femaleFormData = FormData[FemaleFormDataKey];

            if (!hPosItemFormData.IsT0 && !hPosItemFormData.IsT3) {
                Ash.Logger.LogWarning("HPosItem form data is of a wrong type. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!hPosStyleFormData.IsT2 && !hPosStyleFormData.IsT4) {
                Ash.Logger.LogWarning("HPosStyle form data is of a wrong type. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!femaleFormData.IsT1) {
                Ash.Logger.LogWarning("Female form data is of a wrong type. Rule will not be created.");
                FormData.Clear();
                return false;
            }


            if (hPosItemFormData.IsT0 && ReferenceEquals(hPosItemFormData.AsT0.WearData, null)) {
                Ash.Logger.LogWarning("HPosItem is null. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            // ReSharper disable once InvertIf
            if (ReferenceEquals(femaleFormData.AsT1, null)) {
                Ash.Logger.LogWarning("Female is null. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            return true;
        }
    }
}
