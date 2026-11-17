using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ash.Core.SceneManagement
{
    internal static class SceneComponentRegistry
    {
        private static List<Component> ComponentRegistry { get; } = new List<Component>();

        internal static void RegisterComponent(Component component) {
            Ash.Logger.LogDebug($"Register component {component}");
            ComponentRegistry.Add(component);
        }

        internal static void UnregisterComponent(Component component) {
            Ash.Logger.LogDebug($"Unregister component {component}");
            ComponentRegistry.Remove(component);
        }

        internal static IEnumerable<T> GetComponentsOfType<T>() where T : Component {
            return ComponentRegistry.OfType<T>();
        }
    }
}
