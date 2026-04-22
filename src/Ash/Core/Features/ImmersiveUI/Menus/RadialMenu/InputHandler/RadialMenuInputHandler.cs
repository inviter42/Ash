using System;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.InputHandler
{
    internal class RadialMenuInputHandler
    {
        private readonly RadialMenuMain RadialMenuMain;

        private Vector2 CursorPosOnPressed = Vector2.zero;
        private bool IsCursorPosDirty = true;

        public RadialMenuInputHandler(RadialMenuMain radialMenuMain) {
            RadialMenuMain = radialMenuMain;
        }

        internal bool IsHotkeyHeld() {
            return Input.GetKey(Ash.ConfigEntryToggleImmersiveUIHotkey.Value.MainKey);
        }

        internal bool IsHotkeyReleased() {
            return GlobalUtils.HotkeyUtils.HotkeyIsUp(Ash.ConfigEntryToggleImmersiveUIHotkey.Value.MainKey);
        }

        internal bool IsCanceled() {
            return Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Escape);
        }

        // mouse handling
        internal void UpdateCursorPosition() {
            if (RadialMenuMain is null)
                return;

            if (Ash.AshUI.IuiMain.IsAnySubMenuVisible)
                return;

            if (IsHotkeyHeld() && !IsCanceled())
                UpdateCursorPositionWhileHeld();
            else
                IsCursorPosDirty = true;
        }

        private void UpdateCursorPositionWhileHeld() {
            if (!IsCursorPosDirty)
                return;

            CursorPosOnPressed = Input.mousePosition;
            IsCursorPosDirty = false;
        }

        internal int GetAngleByVector() {
            var currentCursorPos = Input.mousePosition;

            // this will help move cursor in delta calculations to the menu origin point (center of the screen)
            // from where it actually is at the moment of menu activation
            var offset = IuiSettings.RadialMenuOrigin - CursorPosOnPressed;

            // calculate raw direction vector (delta)
            var deltaX = currentCursorPos.x - IuiSettings.RadialMenuOrigin.x + offset.x;
            var deltaY = currentCursorPos.y - IuiSettings.RadialMenuOrigin.y + offset.y;

            // calculate distance to check if cursor is in deadzone
            var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance < IuiSettings.RadialMenuDeadzoneThreshold)
                return -1;

            // calculate angle in radians
            var angleRadians = Math.Atan2(deltaY, deltaX);

            // covert to degrees 0-360
            var angleDegrees = angleRadians * (180 / Math.PI);

            // atan2 operates in cartesian (x,y), where 0deg is at (1,0) is on the right
            // adjust to have 0deg at the top
            angleDegrees -= 90f;

            if (angleDegrees < 0)
                angleDegrees += 360;

            if (angleDegrees >= 360f)
                angleDegrees -= 360;

            return (int)Math.Round(angleDegrees);
        }
    }
}
