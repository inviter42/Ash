using System.Collections.Generic;
using System.Linq;
using Ash.Logging;
using UnityEngine;

namespace Ash.Core.Tooling.SceneManagement
{
    internal static class SceneComponentRegistry
    {
        private static List<Component> ComponentRegistry { get; } = new List<Component>();

        internal static void RegisterComponent(Component component) {
            AshLogger.LogDebug(LoggingSettings.LoggingModules.SceneManagement, $"Register component {component}");
            ComponentRegistry.Add(component);
        }

        internal static void UnregisterComponent(Component component) {
            AshLogger.LogDebug(LoggingSettings.LoggingModules.SceneManagement, $"Unregister component {component}");
            ComponentRegistry.Remove(component);
        }

        internal static IEnumerable<T> GetComponentsOfType<T>() where T : Component {
            return ComponentRegistry.OfType<T>();
        }
    }
}
