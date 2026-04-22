using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ash.Core.SceneManagement
{
    internal class SceneTypeTracker : MonoBehaviour
    {
        internal enum SceneTypes
        {
            Unknown,
            StartUpScene,
            CautionScene,
            LogoScene,
            TitleScene,
            SelectScene,
            H,
            EditScene
        }

        internal static SceneTypes TypeOfCurrentScene = SceneTypes.Unknown;

        internal static Scene Scene;

        internal static event Action SceneTypeUpdated;
        internal static event Action SceneLoaded;
        internal static event Action SceneUnloaded;

        private void Awake() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {
            UpdateSceneData(scene);
            SceneLoaded?.Invoke();
        }

        private static void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene) {
            UpdateSceneData(scene);
            SceneUnloaded?.Invoke();
        }

        private static void UpdateSceneData(UnityEngine.SceneManagement.Scene scene) {
            foreach (SceneTypes type in Enum.GetValues(typeof(SceneTypes))) {
                if (scene.name != type.ToString())
                    continue;

                TypeOfCurrentScene = type;

                SceneTypeUpdated?.Invoke();

                break;
            }
        }
    }
}
