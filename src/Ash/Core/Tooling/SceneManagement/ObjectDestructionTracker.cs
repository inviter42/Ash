using System;
using System.Collections.Generic;
using Ash.Logging;
using UnityEngine;

namespace Ash.Core.Tooling.SceneManagement
{
    internal class ObjectDestroyTracker : MonoBehaviour
    {
        internal readonly List<Action> OnBeforeDestroy = new List<Action>();

        internal Component Target { get; private set; }

        internal void Initialize(Component target) {
            Target = target;
        }

        private void OnDestroy() {
            AshLogger.LogDebug(LoggingSettings.LoggingModules.SceneManagement, "Target is being destroyed. Invoking OnBeforeDestroy actions.");
            OnBeforeDestroy.ForEach(action => action.Invoke());
        }
    }
}
