using UnityEngine;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper.Utils
{
    public static class InGameUIButtonsSearch
    {
        public static RectTransform FindButtonRectTransform(GameObject gameObj, string name) {
            if (gameObj == null) {
                Ash.Logger.LogWarning($"{name} game object reference is null");
                return null;
            }

            var toggle = gameObj.transform.Find("Toggle");

            if (toggle == null) {
                Ash.Logger.LogWarning($"{name} > Toggle game object reference is null");
                return null;
            }

            var rectTrans = toggle.GetComponent<RectTransform>();

            // ReSharper disable once InvertIf
            if (rectTrans == null) {
                Ash.Logger.LogWarning($"{name} > Toggle > RectTransform component reference is null");
                return null;
            }

            return rectTrans;
        }

        public static GameObject FindButtonHideable(GameObject gameObj, string name) {
            if (gameObj == null) {
                Ash.Logger.LogWarning($"{name} game object reference is null");
                return null;
            }

            var hideable = gameObj.transform.Find("Hideable").gameObject;

            // ReSharper disable once InvertIf
            if (hideable == null) {
                Ash.Logger.LogWarning($"{name} > Toggle game object reference is null");
                return null;
            }

            return hideable;
        }
    }
}
