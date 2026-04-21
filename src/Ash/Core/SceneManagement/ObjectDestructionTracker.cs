using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ash.Core.SceneManagement
{
    internal class ObjectDestroyTracker : MonoBehaviour
    {
        internal readonly List<Action> OnBeforeDestroy = new List<Action>();

        internal Component Target { get; private set; }

        internal void Initialize(Component target) {
            Target = target;
        }

        private void OnDestroy() {
            Ash.Logger.LogDebug("Target is being destroyed. Invoking OnBeforeDestroy actions.");
            OnBeforeDestroy.ForEach(action => action.Invoke());
        }
    }
}
