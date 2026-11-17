using UnityEngine;

namespace Ash.Core.Features.Actions
{
    internal class ActionsManager : MonoBehaviour
    {
        private DirtyTalkAction DirtyTalk;

        private void Awake() {
            DirtyTalk = new DirtyTalkAction();
        }

        private void Update() {
            DirtyTalk.QueryInput();
            DirtyTalk.Update();
        }
    }
}
