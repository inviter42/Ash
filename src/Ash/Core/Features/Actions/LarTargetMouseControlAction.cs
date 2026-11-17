using System.Collections.Generic;
using System.Linq;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ash.Core.Features.Actions;

internal class LarTargetMouseControlAction : AbstractAction
{
    private const float DoubleClickThreshold = 0.3f;
    private const float ClickAndHoldThreshold = 0.5f;
    private const float DragThreshold = 2f;

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.Actions);

    private static bool PressValid;
    private static float PressTimestamp;
    private static Vector3 PressPosition;
    private static float LastClickTimestamp;
    private static Vector3 LastClickPosition;
    private static float DragAccumulation;

    /// <summary>
    /// Have to roll this clown car of an input handling, because sane approach with adding PhysicsRaycaster to the camera breaks existing
    /// game code that uses EventSystem.current.IsPointerOverGameObject() due to PhysicsRaycaster polluting hit results list
    /// </summary>
    internal override void QueryInput() {
        if (!Ash.PersistentSettings.EnableFemaleLarTargetMouseControl.Value)
            return;

        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        if (HotkeyUtils.HotkeyIsDown(KeyCode.Mouse0)) {
            PressTimestamp = Time.unscaledTime;
            PressPosition = Input.mousePosition;
            PressValid = !EventSystem.current.IsPointerOverGameObject(); // UI only
            DragAccumulation = 0f;
            return;
        }

        if (!PressValid)
            return;

        DragAccumulation += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).magnitude;
        if (!HotkeyUtils.HotkeyIsUp(KeyCode.Mouse0))
            return;

        PressValid = false;
        if (DragAccumulation > DragThreshold)
            return;

        if (!CastRayAndCheckForFemale(hScene, out var female))
            return;

        if (Time.unscaledTime - PressTimestamp > ClickAndHoldThreshold) {
            if (female.neckLookType == LookAtRotator.TYPE.HOLD) {
                if (ReferenceEquals(hScene.visitor.female, female)) {
                    female.ChangeNeckLook(LookAtRotator.TYPE.TARGET, hScene.mainMembers.visitorLookPos.transform, false);
                    female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, hScene.mainMembers.visitorLookPos.transform, false);
                } else {
                    female.ChangeNeckLook(LookAtRotator.TYPE.NO, null, false);
                    female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, null, false);
                }
            } else {
                female.ChangeNeckLook(LookAtRotator.TYPE.HOLD, null, false);
                female.ChangeEyeLook(LookAtRotator.TYPE.HOLD, null, false);
            }

            return;
        }

        var isDouble = Time.unscaledTime - LastClickTimestamp <= DoubleClickThreshold
                       && (PressPosition - LastClickPosition).magnitude <= DragThreshold;

        LastClickTimestamp = isDouble ? float.NegativeInfinity : Time.unscaledTime;
        LastClickPosition = PressPosition;

        if (!isDouble)
            return;

        if (!hScene.camera.camera)
            return;

        if (ReferenceEquals(hScene.visitor.female, female)) {
            if (female.neckLookType == LookAtRotator.TYPE.TARGET) {
                if (female.neckLookTarget == hScene.mainMembers.visitorLookPos.transform) {
                    female.ChangeNeckLook(LookAtRotator.TYPE.TARGET, hScene.camera.transform, false);
                    female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, hScene.camera.transform, false);
                } else {
                    female.ChangeNeckLook(LookAtRotator.TYPE.TARGET, hScene.mainMembers.visitorLookPos.transform, false);
                    female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, hScene.mainMembers.visitorLookPos.transform, false);
                }
            } else {
                female.ChangeNeckLook(LookAtRotator.TYPE.TARGET, hScene.mainMembers.visitorLookPos.transform, false);
                female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, hScene.mainMembers.visitorLookPos.transform, false);
            }
        } else {
            if (female.neckLookType == LookAtRotator.TYPE.NO) {
                female.ChangeNeckLook(LookAtRotator.TYPE.TARGET, hScene.camera.transform, false);
                female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, hScene.camera.transform, false);
            } else {
                female.ChangeNeckLook(LookAtRotator.TYPE.NO, null, false);
                female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, null, false);
            }
        }
    }

    internal override void Update() { }


    private static bool CastRayAndCheckForFemale(H_Scene hScene, out Female female) {
        female = null;

        if (!Physics.Raycast(hScene.camera.camera.ScreenPointToRay(Input.mousePosition), out var hit))
            return false;

        var females = new List<Female>(hScene.mainMembers.females) { hScene.visitor.female };
        female = females.FirstOrDefault(f => f.gameObject == hit.transform.gameObject);

        return female;
    }
}
