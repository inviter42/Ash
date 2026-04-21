using UnityEngine;

namespace Ash.GlobalUtils
{
    internal static class HotkeyUtils
    {
        // ReSharper disable once MemberCanBeMadeStatic.Local
        internal static bool HotkeyIsDown(KeyCode mainKey) {
            var ev = Event.current;
            if (ev == null)
                return false;

            if (ev.isKey && ev.type == EventType.KeyDown) {
                if (ev.keyCode == mainKey || (ev.keyCode == KeyCode.None && ev.character == (int)mainKey)) {
                    ev.Use();
                    return true;
                }
            }

            if (ev.isMouse && ev.type == EventType.MouseDown) {
                if (Input.GetKeyDown(mainKey)) {
                    ev.Use();
                    return true;
                }
            }

            if (Input.GetKeyDown(mainKey)) {
                return true;
            }

            return false;
        }

        internal static bool HotkeyIsUp(KeyCode mainKey) {
            var ev = Event.current;
            if (ev == null)
                return false;

            if (ev.isKey && ev.type == EventType.KeyUp) {
                if (ev.keyCode == mainKey || (ev.keyCode == KeyCode.None && ev.character == (int)mainKey)) {
                    ev.Use();
                    return true;
                }
            }

            if (ev.isMouse && ev.type == EventType.MouseUp) {
                if (Input.GetKeyUp(mainKey)) {
                    ev.Use();
                    return true;
                }
            }

            if (Input.GetKeyUp(mainKey)) {
                return true;
            }

            return false;
        }
    }
}
