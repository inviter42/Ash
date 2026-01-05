using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ash.Core.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace Ash.Core.Features.GameUIControls.UI.Helpers.InGameUIManagementHelper
{
    public static class InGameUIObjectManagement
    {
        public static readonly List<GameObject> TargetGameObjects = new List<GameObject>();
        public static readonly List<GameObject> InactiveGameObjects = new List<GameObject>();

        public static Image TalkImage;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject MaleGage = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject FemaleGage = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once InconsistentNaming
        public static GameObject MaleXTCLock = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once InconsistentNaming
        public static GameObject FemaleXTCLock = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject Pad = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject ActButtons = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject LeftMiddleToggles = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject SwapGameObj = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject PositionsGameObj = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject ClothingGameObj = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject GagGameObj = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static GameObject MoveGameObj = null;

        // Must have these exact params to match the shape of its event binding!
        public static void UpdateData(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {
            if (SceneTypeTracker.TypeOfCurrentScene != SceneTypeTracker.SceneTypes.H) {
                Ash.Logger.LogDebug($"Skipping update for the scene type '{SceneTypeTracker.TypeOfCurrentScene}'");
                return;
            }

            UpdateGameObjectCache();

            UpdateTalkImgRef();

            UpdateAllGameObjectRefs();
        }

        private static void UpdateGameObjectCache() {
            Ash.Logger.LogDebug("Scene loaded. Updating cache...");

            TargetGameObjects.Clear();

            var arrayOfTargetGameObjectNames = InGameUIStrings.GetAllStaticStringFieldValues();

            var gameObjectsArray = UObject.FindObjectsOfType<GameObject>().ToArray()
                .Where(gameObj =>
                    gameObj != null
                    && arrayOfTargetGameObjectNames.Contains(gameObj.name)
                    && gameObj.layer == 5
                );

            TargetGameObjects.AddRange(gameObjectsArray);

            Ash.Logger.LogDebug($"Done. New cache has {TargetGameObjects.Count} entries.");
        }

        private static void UpdateTalkImgRef() {
            if (SceneTypeTracker.TypeOfCurrentScene != SceneTypeTracker.SceneTypes.H)
                return;

            if (TalkImage != null)
                return;

            var talkGo = TargetGameObjects.Find(o => o.name == InGameUIStrings.TalkGameObjName);
            if (talkGo == null) {
                Ash.Logger.LogWarning($"'{InGameUIStrings.TalkGameObjName}' GameObject reference is not found.");
                return;
            }

            TalkImage = talkGo.GetComponent<Image>();

            if (TalkImage == null)
                Ash.Logger.LogWarning($"'{InGameUIStrings.TalkGameObjName}' Image reference is not found.");
        }

        private static void UpdateAllGameObjectRefs() {
            Ash.Logger.LogDebug("Update Game Object refs");

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("MaleGage", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.MaleGageGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("FemaleGage", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.FemaleGageGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("MaleXTCLock", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.MaleXTCLockGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("FemaleXTCLock", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.FemaleXTCLockGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("Pad", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.PadGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("ActButtons", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.ActButtonsGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("LeftMiddleToggles", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.LeftMiddleTogglesGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("SwapGameObj", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.SwapCharGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("PositionsGameObj", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.PositionGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("ClothingGameObj", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.ClothingGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("GagGameObj", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.GagGameObjName
            );

            UpdateGameObjectRef(
                typeof(InGameUIObjectManagement).GetField("MoveGameObj", BindingFlags.Static | BindingFlags.Public),
                InGameUIStrings.MoveGameObjName
            );
        }

        // OBJECTS MANAGEMENT HELPER METHODS
        private static void UpdateGameObjectRef(FieldInfo field, string name) {
            // Check field exists
            if (field == null) {
                Ash.Logger.LogWarning($"A field for {name} is null");
                return;
            }

            field.SetValue(null, TargetGameObjects.Find(o => o.name == name));
        }
    }
}
