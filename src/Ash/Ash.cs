using Ash.Core.SceneManagement;
using Ash.Core.Settings;
using Ash.Core.UI;
using Ash.GlobalUtils;
using Ash.HarmonyHooks;
using Ash.HarmonyHooks._H;
using Ash.HarmonyHooks._H._HState;
using Ash.HarmonyHooks._Human;
using Ash.HarmonyHooks._Menus;
using Ash.HarmonyHooks._Scene;
using Ash.HarmonyHooks._Wearables;
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
        public const string Version = "1.3.0";

        internal new static ManualLogSource Logger;

        internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleWindowHotkey { get; private set; }
        internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleImmersiveUIHotkey { get; private set; }

        internal static PersistentSettings PersistentSettings { get; private set; }

        internal static GameObject AshGameObj;
        internal static AshUI AshUI;

        internal static MoreAccessories MoreAccessoriesInstance;

        // ReSharper disable once InconsistentNaming
        private const string MoreAccessoriesGUID = "com.joan6694.illusionplugins.moreaccessories";

        private static Harmony Harmony;

        private void Awake() {
            InitializePlugin();
        }

        private void InitializePlugin() {
            Logger = base.Logger;
            Harmony = new Harmony($"{GUID}.harmony");

            PersistentSettings = IO.Load<PersistentSettings>(IO.SettingsFileName);

            // Setup hotkey binding
            ConfigEntryToggleWindowHotkey = Config.Bind(
                "Shortcuts",
                "Open/Close Window",
                new KeyboardShortcut(KeyCode.BackQuote)
            );

            ConfigEntryToggleImmersiveUIHotkey = Config.Bind(
                "Shortcuts",
                "Open/Close Immersive UI",
                new KeyboardShortcut(KeyCode.Mouse2)
            );

            // Register hooks
#if DEBUG
            Harmony.PatchAll(typeof(DevHooks));
#endif
            Harmony.PatchAll(typeof(FemaleHooks));
            Harmony.PatchAll(typeof(WearsHooks));
            Harmony.PatchAll(typeof(AccessoriesHooks));
            Harmony.PatchAll(typeof(HMembersHooks));
            Harmony.PatchAll(typeof(IllusionCameraHooks));
            Harmony.PatchAll(typeof(SceneControlHooks));
            Harmony.PatchAll(typeof(PauseMenueHooks));
            Harmony.PatchAll(typeof(HStateLoopHooks));
            Harmony.PatchAll(typeof(HStatePreInsertWaitHooks));
            Harmony.PatchAll(typeof(HStatePreTouchWaitHooks));
            Harmony.PatchAll(typeof(HStateInsertedWaitHooks));
            Harmony.PatchAll(typeof(HStateStartHooks));
            Harmony.PatchAll(typeof(HStateExitHooks));
            Harmony.PatchAll(typeof(HSceneHooks));
            Harmony.PatchAll(typeof(ConfigMenuHooks));

            // Initialize UI
            InitPluginUI();

            // initialize MoreAccessories pointer
            MoreAccessoriesInstance = GetMoreAccessoriesInstance();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void InitPluginUI() {
            AshGameObj = new GameObject("Ash", typeof(AshUI), typeof(SceneTypeTracker));
            AshUI = AshGameObj.GetComponent<AshUI>();
            DontDestroyOnLoad(AshGameObj);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private MoreAccessories GetMoreAccessoriesInstance() {
            if (!Chainloader.PluginInfos.TryGetValue(MoreAccessoriesGUID, out var pluginInfo))
                return null;

            return pluginInfo.Instance as MoreAccessories;
        }
    }
}
