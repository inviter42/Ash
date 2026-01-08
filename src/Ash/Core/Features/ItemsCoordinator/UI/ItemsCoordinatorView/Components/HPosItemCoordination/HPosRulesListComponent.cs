using System;
using System.Collections.Generic;
using Ash.Core.Features.ItemsCoordinator.Types;
using Ash.GlobalUtils;
using UnityEngine;
using static Ash.Core.Features.Common.Misc.CommonLabels;
using static Ash.GlobalUtils.GuiPrimitivesLib;

namespace Ash.Core.Features.ItemsCoordinator.UI.ItemsCoordinatorView.Components.HPosItemCoordination
{
    public static class HPosRulesListComponent
    {
        private const string ActiveRulesTitle = "Active Rules";
        private const string NoActiveRulesLabel = "No active H-Pos rules";

        private static readonly List<Action> ListOfRuleChanges = new List<Action>();

        private static Vector2 RulesScrollPosition;

        public static void DrawHPosRulesList() {
            Title(ActiveRulesTitle);

            GUILayout.Space(4);

            RulesScrollPosition = GUILayout.BeginScrollView(RulesScrollPosition, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            using (new GUILayout.VerticalScope("box", GUILayout.ExpandHeight(true))) {
                foreach (var ruleSet in RulesManager.HPosRuleSets) {
                    CreateRuleGuiElement(
                        new HPosRuleData {
                            HPosItem = ruleSet.HPosItem,
                            HPosStyle = ruleSet.HPosStyle
                        }
                    );
                }

                foreach (var action in ListOfRuleChanges)
                    action.Invoke();

                ListOfRuleChanges.Clear();

                if (RulesManager.HPosRuleSets.Count == 0) {
                    GUILayout.FlexibleSpace();

                    using (new GUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(NoActiveRulesLabel);
                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.FlexibleSpace();
                }
            }

            GUILayout.EndScrollView();
        }

        private static void CreateRuleGuiElement(HPosRuleData data) {
            var hPosItemName = data.HPosItem.IsT0
                ? AutoTranslatorIntegration.Translate(data.HPosItem.AsT0.Name)
                : WearShowTypeLabels.GetValueOrDefaultValue(data.HPosItem.AsT1, ErrorLabel);

            var hPosItemType = data.HPosItem.IsT0
                ? $" ({WearShowTypeLabels.GetValueOrDefaultValue(data.HPosItem.AsT0.ItemData.AsT0.ItemPart, ErrorLabel)})"
                : "";

            var hPosStyle = data.HPosStyle.IsT0
                ? HStylesLabels.GetValueOrDefaultValue(data.HPosStyle.AsT0, ErrorLabel)
                : HStylesExtendedLabels.GetValueOrDefaultValue(data.HPosStyle.AsT1, ErrorLabel);

            var labelText =
                "[DONT STRIP]\n" +
                $"    {hPosItemName}{hPosItemType}\n" +
                "[IN]\n" +
                $"    {hPosStyle}";

            GUILayout.BeginVertical();

            using (new GUILayout.HorizontalScope("box")) {
                GUILayout.Label(labelText, GUILayout.ExpandWidth(true));
                Button(
                    "x",
                    () => ListOfRuleChanges.Add(() => RulesManager.RemoveRule(data)
                    ),
                    GUILayout.Width(30),
                    GUILayout.Height(30));
            }

            GUILayout.EndVertical();
        }
    }
}
