using System.Linq;
using Ash.Core.SceneManagement;
using Ash.Core.UI.Types;
using Ash.GlobalUtils;
using UnityEngine;

namespace Ash.Core.UI
{
    internal static class WindowManager
    {
        internal static MonoBehaviour Window { get; private set; }

        private static bool IsLegalScene() => LegalScenes.Contains(SceneTypeTracker.TypeOfCurrentScene);

        private static readonly SceneTypeTracker.SceneTypes[] LegalScenes =
            { SceneTypeTracker.SceneTypes.H, SceneTypeTracker.SceneTypes.EditScene, SceneTypeTracker.SceneTypes.SelectScene };

        internal static void UpdateWindowVisibility() {
            if (!HotkeyUtils.HotkeyIsDown(Ash.ConfigEntryToggleWindowHotkey.Value.MainKey))
                return;

            if (!IsLegalScene()) {
                Ash.Logger.LogWarning($"Illegal scene {SceneTypeTracker.TypeOfCurrentScene}");
                return;
            }

            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            ToggleWindowVisibility();
        }

        internal static void UnloadWindow() {
            if (Window is MonoBehaviour behaviour)
                behaviour.enabled = false;

            Window = null;
        }

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        private static void ToggleWindowVisibility() {
            if (!Window) CreateWindowComponent();

            if (Window is MonoBehaviour behaviour)
                behaviour.enabled = !behaviour.enabled;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static void CreateWindowComponent() {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (SceneTypeTracker.TypeOfCurrentScene) {
                case SceneTypeTracker.SceneTypes.H:
                    Window = Ash.AshGameObj.AddComponent<HSceneWindow>();
                    break;
                case SceneTypeTracker.SceneTypes.EditScene:
                case SceneTypeTracker.SceneTypes.SelectScene:
                    Window = Ash.AshGameObj.AddComponent<EditSceneWindow>();
                    break;
                default:
                    Ash.Logger.LogError(
                        $"Unable to create Window component for unregistered scene type {SceneTypeTracker.TypeOfCurrentScene}");
                    break;
            }
        }
    }
}
