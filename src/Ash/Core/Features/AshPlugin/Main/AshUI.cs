using Ash.Core.Features.AshPlugin.Main.State;
using Ash.Core.Features.ImmersiveUI;
using Ash.Core.Tooling.SceneManagement;
using Ash.Utility.GlobalUtils;
using UnityEngine;

namespace Ash.Core.Features.AshPlugin.Main
{
    internal class AshUI : MonoBehaviour
    {


        internal IuiMain IuiMain;

        private void Awake() {
            SceneTypeTracker.SceneLoaded += CreateImmersiveUI;

            SceneTypeTracker.SceneUnloaded += WindowManager.UnloadWindow;
            SceneTypeTracker.SceneUnloaded += DestroyImmersiveUI;
        }

        private void Update() {
            AshUIStateControl.UpdateState();

            // imgui plugin window
            WindowManager.UpdateWindowVisibility();

            // immersive ui
            // ReSharper disable once InvertIf
            if (IuiMain != null) {
                IuiMain.StateControl.UpdateState();

                foreach (var enumerator in IuiMain.StateControl.YieldPostStateUpdateRoutineIterators())
                    StartCoroutine(enumerator);
            }
        }

        internal void CreateImmersiveUI() {
            if (!IuiMain.IsLegalScene)
                return;

            if (!Ash.PersistentSettings.IsImmersiveUiEnabled.Value) {
                Ash.Logger.LogDebug($"Immersive UI is disabled - skip running constructor");
                return;
            }

            if (IuiMain != null)
                return;

            IuiMain = new IuiMain();
        }

        internal void DestroyImmersiveUI() {
            if (IuiMain == null)
                return;

            DestroyImmediate(IuiMain.CanvasGameObj);
            IuiMain = null;
        }
    }
}
