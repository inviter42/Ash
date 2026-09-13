using System.Reflection;
using Ash.Core;
using Ash.Core.Features.Actions;
using Ash.Core.Features.AshPlugin.Main;
using Ash.Core.Features.AshPlugin.Settings;
using Ash.Core.Features.BetterTattoos;
using Ash.Core.Features.BetterTattoos.MakerExtensions;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using BepInEx;
using BepInEx.Configuration;
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
        public const string Version = "1.4.2";

        internal static Ash Instance { get; private set; }
        internal new static AshLogger Logger;

        internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleWindowHotkey { get; private set; }
        internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleImmersiveUIHotkey { get; private set; }
        internal static ConfigEntry<KeyboardShortcut> ConfigEntryTriggerDirtyTalk { get; private set; }
        internal static ConfigEntry<bool> ConfigEntrySkipToTitleSceneEnabled { get; private set; }

        internal static PersistentSettings PersistentSettings { get; private set; }

        internal static GameObject AshGameObj;
        internal static AshUI AshUI;
        internal static TattooExtensionBody TattooExtensionBody;
        internal static TattooExtensionHead TattooExtensionHead;

        internal static MoreAccessories MoreAccessoriesInstance;

        // ReSharper disable once InconsistentNaming
        private const string MoreAccessoriesGUID = "com.joan6694.illusionplugins.moreaccessories";

        private static Harmony Harmony;

        private void Awake() {
            if (Application.productName != "PlayHome") {
                base.Logger.LogWarning($"Ash plugin is main game only, loading has been interrupted.");
                return;
            }

            InitializePlugin();
        }

        private void InitializePlugin() {
            Instance = this;
            Logger = new AshLogger(LoggingSettings.LoggingModules.Global);
            Harmony = new Harmony($"{GUID}.harmony");

            PersistentSettings = IO.Load<PersistentSettings>(IO.SettingsFileName);

            SetupBepInExConfigShortcutsBinds();

            // Register hooks
            Harmony.PatchAll(Assembly.GetExecutingAssembly());

            InitPluginUI();

            // initialize MoreAccessories pointer
            MoreAccessoriesInstance = GetMoreAccessoriesInstance();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void InitPluginUI() {
            AshGameObj = new GameObject(
                "Ash",
                typeof(AshUI),
                typeof(SceneTypeTracker),
                typeof(ActionsManager),
                typeof(TattooDataManager),
                typeof(TattooExtensionBody),
                typeof(TattooExtensionHead)
            );

            AshUI = AshGameObj.GetComponent<AshUI>();

            TattooExtensionHead = AshGameObj.GetComponent<TattooExtensionHead>();
            TattooExtensionBody = AshGameObj.GetComponent<TattooExtensionBody>();

            DontDestroyOnLoad(AshGameObj);

            GlobalPluginData.PerformShaderCacheWarmup();
            SceneTypeTracker.SceneUnloaded += GlobalPluginData.InvalidateTextureCache;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private MoreAccessories GetMoreAccessoriesInstance() {
            if (!Chainloader.PluginInfos.TryGetValue(MoreAccessoriesGUID, out var pluginInfo))
                return null;

            return pluginInfo.Instance as MoreAccessories;
        }

        private void SetupBepInExConfigShortcutsBinds() {
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

            ConfigEntryTriggerDirtyTalk = Config.Bind(
                "Shortcuts",
                "Trigger dirty talk",
                new KeyboardShortcut(KeyCode.T)
            );

            ConfigEntrySkipToTitleSceneEnabled = Config.Bind(
                "Global Settings",
                "Enable intro skip",
                false
            );
        }
    }
}
