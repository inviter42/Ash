using System.Collections.Generic;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.InterItemCoordination;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Character;
using OneOf;
using InputsValues = OneOf.OneOf<
    Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types.ItemWearFormData,
    Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types.ItemAccessoryFormData,
    Character.WEAR_SHOW,
    bool,
    Female,
    Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types.GenerateReverseRulesEnum.GenerateReverseRules
>;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State
{
    public class InterItemRuleForm
    {
        // Common keys
        public const string FemaleFormDataKey = "FemaleFormData";

        public readonly Dictionary<string, InputsValues> FormData = new Dictionary<string, InputsValues>();

        public void SubmitForm() {
            if (!ValidateInputs())
                return;

            // indexing here is safe after passing validation
            var masterItemFormDataRaw = FormData[MasterItemSelectionComponent.MasterItemFormDataKey];
            var masterItemFormData = masterItemFormDataRaw.IsT0
                ? masterItemFormDataRaw.AsT0
                : (OneOf<ItemWearFormData, ItemAccessoryFormData>)masterItemFormDataRaw.AsT1;

            var masterItemStateFormDataRaw = FormData[InterItemRuleDetailsComponent.MasterItemStateFormDataKey];
            var masterItemStateFormData = masterItemStateFormDataRaw.IsT2
                ? masterItemStateFormDataRaw.AsT2
                : (OneOf<WEAR_SHOW, bool>)masterItemStateFormDataRaw.AsT3;

            var slaveItemFormDataRaw = FormData[InterItemRuleDetailsComponent.SlaveItemFormDataKey];
            var slaveItemFormData = slaveItemFormDataRaw.IsT0
                ? slaveItemFormDataRaw.AsT0
                : (OneOf<ItemWearFormData, ItemAccessoryFormData>)slaveItemFormDataRaw.AsT1;

            var slaveItemStateFormDataRaw = FormData[InterItemRuleDetailsComponent.SlaveItemStateFormDataKey];
            var slaveItemStateFormData = slaveItemStateFormDataRaw.IsT2
                ? slaveItemStateFormDataRaw.AsT2
                : (OneOf<WEAR_SHOW, bool>)slaveItemStateFormDataRaw.AsT3;

            var femaleFormDataRaw = FormData[FemaleFormDataKey];

            FormData.TryGetValue(
                InterItemRuleDetailsComponent.GenerateReverseRulesFormDataKey,
                out var generateReverseRulesFormData
            );

            var shouldGenerateReverseRules = generateReverseRulesFormData.IsT5
                && generateReverseRulesFormData.AsT5 == GenerateReverseRulesEnum.GenerateReverseRules.Generate;

            OneOf<WEAR_SHOW, bool> reverseRulesStateFormData = default;
            if (shouldGenerateReverseRules) {
                var reverseRulesStateFormDataRaw = FormData[InterItemRuleDetailsComponent.ReverseRulesStateFormDataKey];
                if (reverseRulesStateFormDataRaw.IsT2)
                    reverseRulesStateFormData = reverseRulesStateFormDataRaw.AsT2;
                else
                    reverseRulesStateFormData = reverseRulesStateFormDataRaw.AsT3;
            }

            RulesManager.AddRule(
                femaleFormDataRaw.AsT4,
                masterItemFormData,
                masterItemStateFormData,
                slaveItemFormData,
                slaveItemStateFormData,
                shouldGenerateReverseRules,
                reverseRulesStateFormData
            );

            FormData.Clear();
        }

        private bool ValidateInputs() {
            if (!FormData.ContainsKey(MasterItemSelectionComponent.MasterItemFormDataKey)) {
                Ash.Logger.LogWarning("MasterItem form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!FormData.ContainsKey(InterItemRuleDetailsComponent.MasterItemStateFormDataKey)) {
                Ash.Logger.LogWarning("MasterItem state form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!FormData.ContainsKey(InterItemRuleDetailsComponent.SlaveItemFormDataKey)) {
                Ash.Logger.LogWarning("SlaveItem form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!FormData.ContainsKey(InterItemRuleDetailsComponent.SlaveItemStateFormDataKey)) {
                Ash.Logger.LogWarning("SlaveItem state form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!FormData.ContainsKey(FemaleFormDataKey)) {
                Ash.Logger.LogWarning("Female form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!FormData.ContainsKey(InterItemRuleDetailsComponent.GenerateReverseRulesFormDataKey)) {
                Ash.Logger.LogWarning(
                    "Generate reverse rules form data was not found. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            var masterItemFormData = FormData[MasterItemSelectionComponent.MasterItemFormDataKey];
            var masterItemStateFormData = FormData[InterItemRuleDetailsComponent.MasterItemStateFormDataKey];
            var slaveItemFormData = FormData[InterItemRuleDetailsComponent.SlaveItemFormDataKey];
            var slaveItemStateFormData = FormData[InterItemRuleDetailsComponent.SlaveItemStateFormDataKey];
            var femaleFormData = FormData[FemaleFormDataKey];
            var generateReverseRulesFormData = FormData[InterItemRuleDetailsComponent.GenerateReverseRulesFormDataKey];

            if (masterItemFormData.IsT0 && !masterItemStateFormData.IsT2
                || masterItemFormData.IsT1 && !masterItemStateFormData.IsT3
                || slaveItemFormData.IsT0 && !slaveItemStateFormData.IsT2
                || slaveItemFormData.IsT1 && !slaveItemStateFormData.IsT3
               ) {
                Ash.Logger.LogWarning("Item and its state have non-matching types. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!femaleFormData.IsT4) {
                Ash.Logger.LogWarning("Female form data is of a wrong type. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (!generateReverseRulesFormData.IsT5) {
                Ash.Logger.LogWarning(
                    "Generate reverse rules form data is of a wrong type. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (ReferenceEquals(masterItemFormData.Value, null)) {
                Ash.Logger.LogWarning("Master item is null. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (ReferenceEquals(slaveItemFormData.Value, null)) {
                Ash.Logger.LogWarning("Slave item is null. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (ReferenceEquals(femaleFormData.AsT4, null)) {
                Ash.Logger.LogWarning("Female is null. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            if (generateReverseRulesFormData.AsT5 == GenerateReverseRulesEnum.GenerateReverseRules.Ignore)
                return true;

            var reverseRulesStateFormData = FormData[InterItemRuleDetailsComponent.ReverseRulesStateFormDataKey];
            // ReSharper disable once InvertIf
            if (!reverseRulesStateFormData.IsT2 && !reverseRulesStateFormData.IsT3) {
                Ash.Logger.LogWarning(
                    "Reverse rules state form data is of a wrong type. Rule will not be created.");
                FormData.Clear();
                return false;
            }

            return true;
        }
    }
}
