using Ash.Core.SceneManagement;
using Ash.GlobalUtils;
using H;
using UnityEngine;

namespace Ash.Core.Features.Actions
{
    internal class DirtyTalkAction
    {
        private float Timer;

        internal void QueryInput() {
            if (!HotkeyUtils.HotkeyIsDown(Ash.ConfigEntryTriggerDirtyTalk.Value.MainKey))
                return;

            if (SceneTypeTracker.TypeOfCurrentScene != SceneTypeTracker.SceneTypes.H) {
                Ash.Logger.LogWarning($"Illegal scene {SceneTypeTracker.TypeOfCurrentScene}");
                return;
            }

            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (!hScene)
                return;

            if (hScene.mainMembers.StateMgr.NowStateID != H_STATE.LOOP)
                return;

            hScene.mainMembers.VoiceExpression(H_Voice.TYPE.ACT_TALK);
        }

        internal void Update() {
            var minVal = Ash.PersistentSettings.DirtyTalkMinValue.Value;
            var maxVal = Ash.PersistentSettings.DirtyTalkMaxValue.Value;

            if (minVal == 0 && maxVal == 0) {
                Timer = 0;
                return;
            }

            if (SceneTypeTracker.TypeOfCurrentScene != SceneTypeTracker.SceneTypes.H)
                return;

            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (!hScene)
                return;

            if (hScene.mainMembers.StateMgr.NowStateID != H_STATE.LOOP) {
                Timer = 0;
                return;
            }

            if (Timer == 0) // if timer has reached 0 - randomly choose new value
                Timer = Random.Range(minVal, maxVal);

            // update timer
            Timer = Mathf.Max(0, Timer - Time.deltaTime);

            if (Timer == 0)
                hScene.mainMembers.VoiceExpression(H_Voice.TYPE.ACT_TALK);
        }
    }
}
