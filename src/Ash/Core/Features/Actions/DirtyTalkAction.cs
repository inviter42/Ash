using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using H;
using UnityEngine;

namespace Ash.Core.Features.Actions;

internal class DirtyTalkAction : AbstractAction
{
    private float Timer;

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.Actions);

    internal override void QueryInput() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        if (hScene.mainMembers.StateMgr.NowStateID != H_STATE.LOOP)
            return;

        if (!HotkeyUtils.HotkeyIsDown(Ash.ConfigEntryTriggerDirtyTalk.Value.MainKey))
            return;

        hScene.mainMembers.VoiceExpression(H_Voice.TYPE.ACT_TALK);
    }

    internal override void Update() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        var minVal = Ash.PersistentSettings.DirtyTalkMinValue.Value;
        var maxVal = Ash.PersistentSettings.DirtyTalkMaxValue.Value;

        if (minVal == 0 && maxVal == 0 || hScene.mainMembers.StateMgr.NowStateID != H_STATE.LOOP)
            return;

        if (Timer <= 0) // if timer has reached 0 - randomly choose new value
            Timer = Random.Range(minVal, maxVal);

        // update timer
        Timer = Mathf.Max(0, Timer - Time.deltaTime);

        if (Timer == 0)
            hScene.mainMembers.VoiceExpression(H_Voice.TYPE.ACT_TALK);
    }
}
