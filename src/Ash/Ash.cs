using Ash.Core.SceneManagement;
using Ash.Core.Settings;
using Ash.Core.UI;
using Ash.GlobalUtils;
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
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class Ash : BaseUnityPlugin
    {
        public const string PluginName = "Ash";
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public const string GUID = "inviter42.anotherscenehelper";
        public const string Version = "1.2.3";

        // ReSharper disable once InconsistentNaming
        private const string MoreAccessoriesGUID = "com.joan6694.illusionplugins.moreaccessories";

        internal new static ManualLogSource Logger;

        internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleWindowHotkey { get; private set; }

        internal static PersistentSettings Settings { get; private set; }

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

            Settings = IO.Load<PersistentSettings>(IO.SettingsFileName);

            // Setup hotkey binding
            ConfigEntryToggleWindowHotkey = Config.Bind(
                "Keyboard shortcuts",
                "Open/Close Window",
                new KeyboardShortcut(KeyCode.BackQuote)
            );

            // Register hooks
            Harmony.PatchAll(typeof(FemaleHooks));
            Harmony.PatchAll(typeof(WearsHooks));
            Harmony.PatchAll(typeof(AccessoriesHooks));
            Harmony.PatchAll(typeof(HMembersHooks));

            // Initialize UI
            InitPluginUI();

            // initialize MoreAccessories pointer
            MoreAccessoriesInstance = GetMoreAccessoriesInstance();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void InitPluginUI() {
            AshGameObj = new GameObject("AshGameObj");
            AshGameObj.AddComponent<SceneTypeTracker>();
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
