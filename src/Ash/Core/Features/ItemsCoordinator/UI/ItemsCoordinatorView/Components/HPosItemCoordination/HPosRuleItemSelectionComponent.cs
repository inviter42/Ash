using System.Linq;
using Ash.Core.Features.Common.Components;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.Common;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.Core.UI.Types;
using Ash.GlobalUtils;
using Character;
using UnityEngine;
using static Ash.GlobalUtils.GuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.HPosRuleForm;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination
{
    public static class HPosRuleItemSelectionComponent
    {
        public const string HPosRuleTypeStateKey = "HPosRuleType";
        public const string HPosRuleItemFormDataKey = "HPosRuleItemItemData";

        private const string NewRuleTitle = "New Rule";
        private const string NewRuleSubtitle1 = "Select item (individual item):";
        private const string NewRuleSubtitle2 = "Select item type (global, all items of the type):";

        public static bool IsHPosItemSelected() =>
            FormData.ContainsKey(HPosRuleItemFormDataKey);

        public static void DrawHPosRuleItemSelectionComponent() {
            Title(NewRuleTitle);

            GUILayout.Space(8);

            RuleTypeSelectionComponent.DrawRuleTypeSelection();

            if (!FormData.ContainsKey(FemaleFormDataKey))
                FormData[FemaleFormDataKey] = GetActiveFemale();

            var activeFemale = GetActiveFemale();

            using (new GUILayout.VerticalScope("box")) {
                FemaleSelectionComponent.Component(
                    activeFemale,
                    female => {
                        FormData[FemaleFormDataKey] = female;
                        SetActiveFemale(female);
                    });

                // HPosItem selection
                HPosRuleItemSelection(activeFemale);
            }
        }

        private static void HPosRuleItemSelection(Female activeFemale) {
            var model = SceneUtils.GetActiveWearShowTypes(activeFemale);
            var strippedTypes = new[] {
                WEAR_SHOW_TYPE.TOPUPPER,
                WEAR_SHOW_TYPE.TOPLOWER,
                WEAR_SHOW_TYPE.BOTTOM,
                WEAR_SHOW_TYPE.BRA,
                WEAR_SHOW_TYPE.SHORTS,
                WEAR_SHOW_TYPE.SWIMUPPER,
                WEAR_SHOW_TYPE.SWIMLOWER,
                WEAR_SHOW_TYPE.SWIM_TOPUPPER,
                WEAR_SHOW_TYPE.SWIM_TOPLOWER,
                WEAR_SHOW_TYPE.SWIM_BOTTOM,
                WEAR_SHOW_TYPE.PANST
            };

            Subtitle(NewRuleSubtitle1);
            model = model.Intersect(strippedTypes).ToArray();
            Flow(model, (itemPart, idx) => Button(
                WearShowTypeLabels.GetValueOrDefaultValue(itemPart, ErrorLabel),
                () => {
                    var wearData = activeFemale.wears.GetWearData(Wears.ShowToWearType[(int)itemPart]);
                    FormData[HPosRuleItemFormDataKey] = new ItemWearFormData { Type = itemPart, WearData = wearData };
                })
            );

            GUILayout.Space(4);
            using (new GUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                Subtitle("OR");
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(4);

            Subtitle(NewRuleSubtitle2);
            Flow(model, (itemPart, idx) => Button(
                WearShowTypeLabels.GetValueOrDefaultValue(itemPart, ErrorLabel),
                () => FormData[HPosRuleItemFormDataKey] = itemPart)
            );
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private static Female GetActiveFemale() {
            switch (Ash.AshUI.Window) {
                case EditSceneWindow editSceneWindow:
                    return editSceneWindow.GetActiveFemale();
                case HSceneWindow hSceneWindow:
                    return hSceneWindow.GetActiveFemale();
                default:
                    Ash.Logger.LogError($"Component MasterItemSelectionComponent is used inside of an unsupported window type {Ash.AshUI.Window.GetType().Name}.");
                    return null;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private static void SetActiveFemale(Female female) {
            switch (Ash.AshUI.Window) {
                case EditSceneWindow editSceneWindow:
                    editSceneWindow.SetActiveFemale(female);
                    break;
                case HSceneWindow hSceneWindow:
                    hSceneWindow.SetActiveFemale(female);
                    break;
                default:
                    Ash.Logger.LogError($"Component MasterItemSelectionComponent is used inside of an unsupported window type {Ash.AshUI.Window.GetType().Name}.");
                    return;
            }
        }
    }
}
