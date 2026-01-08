
using System.Collections.Generic;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.InterItemCoordination;
using Ash.GlobalUtils;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.Common
{
    public static class RuleTypeSelectionComponent
    {
        private const string ChooseRuleTypeSubtitle = "Choose rule type:";

        private static readonly Dictionary<string, string> RuleTypeLabels = new Dictionary<string, string> {
            { HPosRuleItemSelectionComponent.HPosRuleTypeStateKey, "H-Pose Rule" },
            { MasterItemSelectionComponent.InterItemRuleTypeStateKey, "Inter Item Rule" },
        };

        public static void DrawRuleTypeSelection() {
            Subtitle(ChooseRuleTypeSubtitle);
            Flow(
                new[] { HPosRuleItemSelectionComponent.HPosRuleTypeStateKey, MasterItemSelectionComponent.InterItemRuleTypeStateKey },
                (state, idx) => RadioButton(
                    RuleTypeLabels.GetValueOrDefaultValue(state, ErrorLabel),
                    ItemsCoordinatorView.SelectedRuleType == state,
                    () => ItemsCoordinatorView.SelectedRuleType = state)
            );
        }
    }
}
