using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ash.Core.Features.ImmersiveUI.InputHandler;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.State
{
    internal class IuiMainStateControl
    {
        private readonly IuiMain IuiMain;
        private readonly List<Func<IEnumerator>> PostStateUpdateRoutines = new List<Func<IEnumerator>>();
        private readonly List<Action> StateUpdateRequests = new List<Action>();

        internal IuiMainStateControl(IuiMain uiMain) {
            IuiMain = uiMain;
        }

        internal void UpdateState() {
            foreach (var request in StateUpdateRequests)
                request?.Invoke();

            HideAnyVisibleSubmenuOnEscapePressed();
        }

        internal List<IEnumerator> YieldPostStateUpdateRoutineIterators() {
            return PostStateUpdateRoutines
                .Select(routine => routine())
                .ToList();
        }

        internal void AddPostStateUpdateRoutine(Func<IEnumerator> routine) {
            PostStateUpdateRoutines.Add(routine);
        }

        internal void AddStateUpdateRequest(Action action) {
            StateUpdateRequests.Add(action);
        }

        private void HideAnyVisibleSubmenuOnEscapePressed() {
            if (!IuiMain.IsLegalScene)
                return;

            if (IuiMain.IsAnyCanvasRenderedOnTop)
                return;

            if (!IuiInputHandler.IsEscapeReleased())
                return;

            var activeSubmenus = IuiMain.CanvasGameObj.transform
                .Cast<Transform>()
                .Where(t => t.gameObject.activeSelf && t.gameObject.name.EndsWith("MenuRoot"))
                .Select(t => t.gameObject);

            foreach (var gameObj in activeSubmenus)
                gameObj.SetActive(false);
        }
    }
}
