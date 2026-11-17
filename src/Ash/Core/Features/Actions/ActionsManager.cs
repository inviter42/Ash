using System.Collections.Generic;
using UnityEngine;

namespace Ash.Core.Features.Actions;

internal class ActionsManager : MonoBehaviour
{
    private readonly List<AbstractAction> Actions = new();

    private void Awake() {
        Actions.AddRange(new AbstractAction[] {
            new DirtyTalkAction(),
            new LarTargetMouseControlAction(),
            new AhegaoFaceAction()
        });
    }

    private void Update() {
        Actions.ForEach(action => {
            action.QueryInput();
            action.Update();
        });
    }
}
