using System;
using System.Linq;
using Ash.Core.Features.Common.Components;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.Common;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State;
using Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Types;
using Ash.Core.UI;
using Ash.Core.UI.Types;
using Ash.GlobalUtils;
using Character;
using UnityEngine;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using static Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.State.HPosRuleForm;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination
{
    internal static class HPosRuleItemSelectionComponent
    {
        internal const string HPosRuleTypeStateKey = "HPosRuleType";
        internal const string HPosRuleItemFormDataKey = "HPosRuleItemItemData";

        private const string NewRuleTitle = "New Rule";
        private const string NewRuleSubtitle1 = "Select item (individual item):";
        private const string NewRuleSubtitle2 = "Select item type (global, all items of the type):";

        internal static bool IsHPosItemSelected(HPosRuleForm form) =>
            form.FormData.ContainsKey(HPosRuleItemFormDataKey);

        internal static void DrawHPosRuleItemSelectionComponent(HPosRuleForm form) {
            var formData = form.FormData;
            Title(NewRuleTitle);

            GUILayout.Space(8);

            RuleTypeSelectionComponent.DrawRuleTypeSelection();

            if (!formData.ContainsKey(FemaleFormDataKey))
                formData[FemaleFormDataKey] = GetActiveFemale();

            var activeFemale = GetActiveFemale();
            if (activeFemale == null) {
                Ash.Logger.LogWarning("Female is null");
                Ash.Logger.LogWarning(Environment.StackTrace);
                formData.Clear();
                return;
            }

            using (new GUILayout.VerticalScope("box")) {
                FemaleSelectionComponent.Component(
                    activeFemale,
                    female => {
                        formData[FemaleFormDataKey] = female;
                        SetActiveFemale(female);
                    });

                // HPosItem selection
                HPosRuleItemSelection(form, activeFemale);
            }
        }

        private static void HPosRuleItemSelection(HPosRuleForm form, Female activeFemale) {
            var formData = form.FormData;
            var model = SceneUtils.GetWearShowTypesOfEquippedItems(activeFemale);
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
                    formData[HPosRuleItemFormDataKey] = new ItemWearFormData(itemPart, wearData);
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
                () => formData[HPosRuleItemFormDataKey] = itemPart)
            );
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private static Female GetActiveFemale() {
            switch (WindowManager.Window) {
                case EditSceneWindow editSceneWindow:
                    return editSceneWindow.GetActiveFemale();
                case HSceneWindow hSceneWindow:
                    return hSceneWindow.GetActiveFemale();
                default:
                    Ash.Logger.LogError($"Component MasterItemSelectionComponent is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return null;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private static void SetActiveFemale(Female female) {
            switch (WindowManager.Window) {
                case EditSceneWindow editSceneWindow:
                    editSceneWindow.SetActiveFemale(female);
                    break;
                case HSceneWindow hSceneWindow:
                    hSceneWindow.SetActiveFemale(female);
                    break;
                default:
                    Ash.Logger.LogError($"Component MasterItemSelectionComponent is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return;
            }
        }
    }
}
