using System;
using Ash.Core.Features.Common.Components;
using Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView.Components;
using Ash.Core.UI;
using Ash.Core.UI.Types;
using UnityEngine;
using static Ash.GlobalUtils.ImGuiPrimitivesLib;

namespace Ash.Core.Features.ItemsVisibilityControls.UI.ItemsVisibilityControlsView
{
    internal class ItemsVisibilityControlsView
    {
        // Tab Label
        internal const string ItemsVisibilityControlsTabLabel = "Item Visibility";

        internal void DrawView() {
            var activeFemale = GetActiveFemale();
            if (activeFemale == null) {
                Ash.Logger.LogWarning("Female is null");
                Ash.Logger.LogWarning(Environment.StackTrace);
                return;
            }

            using (new GUILayout.VerticalScope("box")) {
                Title(activeFemale.heroineID.ToString());

                FemaleSelectionComponent.Component(activeFemale, SetActiveFemale);

                GUILayout.Space(12);

                AllClothingVisibilitySelectionComponent.Component(activeFemale);
                IndividualWearsVisibilitySelectionComponent.Component(activeFemale);

                GUILayout.Space(12);

                AllAccessoriesVisibilitySelectionComponent.Component(activeFemale);
                IndividualAccessoriesVisibilitySelectionComponent.Component(activeFemale);
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Female GetActiveFemale() {
            switch (WindowManager.Window) {
                case EditSceneWindow editSceneWindow:
                    return editSceneWindow.GetActiveFemale();
                case HSceneWindow hSceneWindow:
                    return hSceneWindow.GetActiveFemale();
                default:
                    Ash.Logger.LogError($"View ItemsVisibilityControlsView is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return null;
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void SetActiveFemale(Female female) {
            switch (WindowManager.Window) {
                case EditSceneWindow editSceneWindow:
                    editSceneWindow.SetActiveFemale(female);
                    break;
                case HSceneWindow hSceneWindow:
                    hSceneWindow.SetActiveFemale(female);
                    break;
                default:
                    Ash.Logger.LogError($"Component FemaleSelectionComponent is used inside of an unsupported window type {WindowManager.Window.GetType().Name}.");
                    return;
            }
        }
    }
}
