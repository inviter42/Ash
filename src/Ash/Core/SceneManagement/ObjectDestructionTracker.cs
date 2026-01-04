using UnityEngine;

namespace Ash.Core.SceneManagement
{
    internal class ObjectDestroyTracker : MonoBehaviour
    {
        private Component Target { get; set; }

        public void Initialize(Component target) {
            Target = target;
        }

        public void OnDestroy() {
            Ash.Logger.LogWarning("Target is being destroyed. Unregistering component.");
            SceneComponentRegistry.UnregisterComponent(Target);
        }
    }
}
