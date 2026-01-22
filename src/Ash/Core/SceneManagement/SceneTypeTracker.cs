using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ash.Core.SceneManagement
{
    internal class SceneTypeTracker : MonoBehaviour
    {
        public enum SceneTypes
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

        public static SceneTypes TypeOfCurrentScene = SceneTypes.Unknown;
        public static Scene CurrentScene;

        public static bool IsLegalScene() => LegalScenes.Contains(TypeOfCurrentScene);

        private static readonly SceneTypes[] LegalScenes =
            { SceneTypes.H, SceneTypes.EditScene, SceneTypes.SelectScene };

        public void Awake() {
            SceneManager.sceneLoaded += UpdateSceneData;
        }

        private static void UpdateSceneData(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {
            foreach (SceneTypes type in Enum.GetValues(typeof(SceneTypes))) {
                if (scene.name != type.ToString())
                    continue;

                TypeOfCurrentScene = type;
                break;
            }

            var gc = (GameControl)FindObjectOfType(typeof(GameControl));
            if (gc == null) {

                Ash.Logger.LogWarning("GameControl not found");
                return;
            }

            CurrentScene = gc.sceneCtrl.nowScene;
        }
    }
}
