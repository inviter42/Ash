using Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper;
using Ash.Core.SceneManagement;
using Ash.Core.UI.Types;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ash.Core.UI
{
    public class AshUI : MonoBehaviour
    {
        // Custom font
        // ReSharper disable once MemberCanBePrivate.Global
        public static Font DynamicFont;

        // Font styles
        public static GUIStyle TitleStyle;
        public static GUIStyle SubtitleStyle;
        public static GUIStyle InfoStyle;

        // UI Styles
        public static GUIStyle ToolbarStyle;

        public MonoBehaviour Window { get; private set; }

        private bool AreStylesInitialized;

        public void Awake() {
            SceneManager.sceneLoaded += InGameUIObjectManagement.UpdateData;
            SceneManager.sceneUnloaded += UnloadWindow;
        }

        public void Update() {
            if (!HotkeyIsDown())
                return;

            if (!SceneDataTracker.IsLegalScene()) {
                Ash.Logger.LogWarning($"Illegal scene {SceneDataTracker.TypeOfCurrentScene}");
                return;
            }

            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            ToggleWindowVisibility();
        }

        public void OnGUI() {
            if (AreStylesInitialized)
                return;

            InitializeStyles();
        }

        private void UnloadWindow(UnityEngine.SceneManagement.Scene scene) {
            if (Window is MonoBehaviour behaviour)
                behaviour.enabled = false;

            Window = null;
        }

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        public void ToggleWindowVisibility() {
            if (!Window) CreateWindowComponent();

            if (Window is MonoBehaviour behaviour)
                behaviour.enabled = !behaviour.enabled;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void CreateWindowComponent() {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (SceneDataTracker.TypeOfCurrentScene) {
                case SceneDataTracker.SceneTypes.H:
                    Window = Ash.AshGameObj.AddComponent<HSceneWindow>();
                    break;
                case SceneDataTracker.SceneTypes.EditScene:
                case SceneDataTracker.SceneTypes.SelectScene:
                    Window = Ash.AshGameObj.AddComponent<EditSceneWindow>();
                    break;
                default:
                    Ash.Logger.LogError(
                        $"Unable to create Window component for unregistered scene type {SceneDataTracker.TypeOfCurrentScene}");
                    break;
            }
        }

        private void InitializeStyles() {
            if (!DynamicFont) {
                DynamicFont = Font.CreateDynamicFontFromOSFont(
                    new[] { "Segeo UI" }, 20
                );
                DontDestroyOnLoad(DynamicFont);
            }

            if (TitleStyle == null) {
                TitleStyle = new GUIStyle(GUI.skin.label) {
                    font = DynamicFont,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(0, 0, 2, 2),
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (SubtitleStyle == null) {
                SubtitleStyle = new GUIStyle(GUI.skin.label) {
                    font = DynamicFont,
                    fontSize = 14,
                    fontStyle = FontStyle.Normal,
                    padding = new RectOffset(4, 0, 10, 2),
                    alignment = TextAnchor.MiddleLeft
                };
            }

            if (InfoStyle == null) {
                InfoStyle = new GUIStyle(GUI.skin.label) {
                    font = DynamicFont,
                    fontSize = 13,
                    fontStyle = FontStyle.Normal,
                    padding = new RectOffset(6, 6, 2, 4),
                    alignment = TextAnchor.UpperLeft,
                    wordWrap = true
                };
            }

            if (ToolbarStyle == null) {
                ToolbarStyle = new GUIStyle(GUI.skin.button) {
                    margin = new RectOffset(0, 0, 0, 10)
                };
            }

            AreStylesInitialized =  true;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private bool HotkeyIsDown() {
            var ev = Event.current;
            var mainKey = Ash.ConfigEntryToggleWindowHotkey.Value.MainKey;

            if (ev == null || !ev.isKey || ev.type != EventType.KeyDown)
                return false;

            if (ev.keyCode != mainKey && (ev.keyCode != KeyCode.None || ev.character != (int)mainKey))
                return false;

            ev.Use();
            return true;
        }
    }
}
