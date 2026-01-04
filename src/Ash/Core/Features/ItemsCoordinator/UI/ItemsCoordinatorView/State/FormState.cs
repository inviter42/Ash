using System.Collections.Generic;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components;
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
    public static class FormState
    {
        public static readonly Dictionary<string, InputsValues> FormData = new Dictionary<string, InputsValues>();

        public static bool IsMasterItemSelected() =>
            FormData.ContainsKey(MasterItemSelectionComponent.MasterItemFormDataKey);

        public static void ResetMasterItemSelection() =>
            FormData.Clear();

        public static void SubmitForm() {
            if (!ValidateInputs())
                return;

            // indexing here is safe after passing validation
            var masterItemFormDataRaw = FormData[MasterItemSelectionComponent.MasterItemFormDataKey];
            OneOf<ItemWearFormData, ItemAccessoryFormData> masterItemFormData;
            if (masterItemFormDataRaw.IsT0)
                masterItemFormData = masterItemFormDataRaw.AsT0;
            else
                masterItemFormData = masterItemFormDataRaw.AsT1;

            var masterItemStateFormDataRaw = FormData[RuleDetailsComponent.MasterItemStateFormDataKey];
            OneOf<WEAR_SHOW, bool> masterItemStateFormData;
            if (masterItemStateFormDataRaw.IsT2)
                masterItemStateFormData = masterItemStateFormDataRaw.AsT2;
            else
                masterItemStateFormData = masterItemStateFormDataRaw.AsT3;

            var slaveItemFormDataRaw = FormData[RuleDetailsComponent.SlaveItemFormDataKey];
            OneOf<ItemWearFormData, ItemAccessoryFormData> slaveItemFormData;
            if (slaveItemFormDataRaw.IsT0)
                slaveItemFormData = slaveItemFormDataRaw.AsT0;
            else
                slaveItemFormData = slaveItemFormDataRaw.AsT1;

            var slaveItemStateFormDataRaw = FormData[RuleDetailsComponent.SlaveItemStateFormDataKey];
            OneOf<WEAR_SHOW, bool> slaveItemStateFormData;
            if (slaveItemStateFormDataRaw.IsT2)
                slaveItemStateFormData = slaveItemStateFormDataRaw.AsT2;
            else
                slaveItemStateFormData = slaveItemStateFormDataRaw.AsT3;

            var femaleFormDataRaw = FormData[MasterItemSelectionComponent.FemaleFormDataKey];

            FormData.TryGetValue(
                RuleDetailsComponent.GenerateReverseRulesFormDataKey,
                out var generateReverseRulesFormData
            );

            var shouldGenerateReverseRules = generateReverseRulesFormData.IsT5
                && generateReverseRulesFormData.AsT5 == GenerateReverseRulesEnum.GenerateReverseRules.Generate;

            OneOf<WEAR_SHOW, bool> reverseRulesStateFormData = default;
            if (shouldGenerateReverseRules) {
                var reverseRulesStateFormDataRaw = FormData[RuleDetailsComponent.ReverseRulesStateFormDataKey];
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

            ResetMasterItemSelection();
        }

        private static bool ValidateInputs() {
            if (!FormData.ContainsKey(MasterItemSelectionComponent.MasterItemFormDataKey)) {
                Ash.Logger.LogWarning("MasterItem form data was not found. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            if (!FormData.ContainsKey(RuleDetailsComponent.MasterItemStateFormDataKey)) {
                Ash.Logger.LogWarning("MasterItem state form data was not found. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            if (!FormData.ContainsKey(RuleDetailsComponent.SlaveItemFormDataKey)) {
                Ash.Logger.LogWarning("SlaveItem form data was not found. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            if (!FormData.ContainsKey(RuleDetailsComponent.SlaveItemStateFormDataKey)) {
                Ash.Logger.LogWarning("SlaveItem state form data was not found. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            if (!FormData.ContainsKey(RuleDetailsComponent.GenerateReverseRulesFormDataKey)) {
                Ash.Logger.LogWarning(
                    "Generate reverse rules form data was not found. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            var masterItemFormData = FormData[MasterItemSelectionComponent.MasterItemFormDataKey];
            var masterItemStateFormData = FormData[RuleDetailsComponent.MasterItemStateFormDataKey];
            var slaveItemFormData = FormData[RuleDetailsComponent.SlaveItemFormDataKey];
            var slaveItemStateFormData = FormData[RuleDetailsComponent.SlaveItemStateFormDataKey];
            var femaleFormData = FormData[MasterItemSelectionComponent.FemaleFormDataKey];
            var generateReverseRulesFormData = FormData[RuleDetailsComponent.GenerateReverseRulesFormDataKey];

            if (masterItemFormData.IsT0 && !masterItemStateFormData.IsT2
                || masterItemFormData.IsT1 && !masterItemStateFormData.IsT3
                || slaveItemFormData.IsT0 && !slaveItemStateFormData.IsT2
                || slaveItemFormData.IsT1 && !slaveItemStateFormData.IsT3
               ) {
                Ash.Logger.LogWarning("Item and its state have non-matching types. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            if (ReferenceEquals(masterItemFormData.Value, null)) {
                Ash.Logger.LogWarning("Master item is null. Rule will not be created.");
                return false;
            }

            if (ReferenceEquals(slaveItemFormData.Value, null)) {
                Ash.Logger.LogWarning("Slave item is null. Rule will not be created.");
                return false;
            }

            if (!femaleFormData.IsT4) {
                Ash.Logger.LogWarning("Female form data is of a wrong type. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            if (!generateReverseRulesFormData.IsT5) {
                Ash.Logger.LogWarning(
                    "Generate reverse rules form data is of a wrong type. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            if (generateReverseRulesFormData.AsT5 == GenerateReverseRulesEnum.GenerateReverseRules.Ignore)
                return true;

            var reverseRulesStateFormData = FormData[RuleDetailsComponent.ReverseRulesStateFormDataKey];
            // ReSharper disable once InvertIf
            if (!reverseRulesStateFormData.IsT2 && !reverseRulesStateFormData.IsT3) {
                Ash.Logger.LogWarning(
                    "Reverse rules state form data is of a wrong type. Rule will not be created.");
                ResetMasterItemSelection();
                return false;
            }

            return true;
        }
    }
}
