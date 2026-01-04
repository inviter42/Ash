using Ash.Core.SceneManagement;
using Ash.Core.UI;
using Ash.HarmonyHooks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using KKAPI;
using UnityEngine;
using BepInEx.Bootstrap;
using MoreAccessoriesPH;

namespace Ash
{
    [BepInPlugin(GUID, PluginName, Version)]
    // Tell BepInEx that this plugin needs KKAPI of at least the specified version.
    // If not found, this plugi will not be loaded and a warning will be shown.
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class Ash : BaseUnityPlugin
    {
        // Project information
        public const string PluginName = "Ash";
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public const string GUID = "inviter42.anotherscenehelper";
        public const string Version = "0.2.0";

        // ReSharper disable once InconsistentNaming
        private const string MoreAccessoriesGUID = "com.joan6694.illusionplugins.moreaccessories";

        internal new static ManualLogSource Logger;

        internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleWindowHotkey { get; private set; }

        internal static GameObject AshGameObj;
        internal static AshUI AshUI;

        internal static MoreAccessories MoreAccessoriesInstance;

        private static Harmony Harmony;

        private void Awake() {
            InitializePlugin();
        }

        private void InitializePlugin() {
            Logger = base.Logger;
            Harmony = new Harmony($"{GUID}.harmony");

            // Setup hotkey binding
            ConfigEntryToggleWindowHotkey = Config.Bind(
                "Keyboard shortcuts",
                "Open/Close Window",
                new KeyboardShortcut(KeyCode.BackQuote)
            );

            // Register hooks
            Harmony.PatchAll(typeof(CharacterHooks));

            // Initialize UI
            InitPluginUI();

            // initialize MoreAccessories pointer
            MoreAccessoriesInstance = GetMoreAccessoriesInstance();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void InitPluginUI() {
            AshGameObj = new GameObject("AshGameObj");
            AshGameObj.AddComponent<SceneDataTracker>();
            AshUI = AshGameObj.AddComponent<AshUI>();
            DontDestroyOnLoad(AshGameObj);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private MoreAccessories GetMoreAccessoriesInstance() {
            if (Chainloader.PluginInfos.TryGetValue(MoreAccessoriesGUID, out var pluginInfo))
                return pluginInfo.Instance as MoreAccessories;

            return null;
        }
    }
}
