using System.Reflection;
using Ash.Core;
using Ash.Core.Features.Actions;
using Ash.Core.Features.AshPlugin.Main;
using Ash.Core.Features.AshPlugin.Settings;
using Ash.Core.Features.BetterApplicables.BetterCheekShadows.Extensions;
using Ash.Core.Features.BetterApplicables.BetterCheekShadows.Managers;
using Ash.Core.Features.BetterApplicables.BetterEyeShadows.Extensions;
using Ash.Core.Features.BetterApplicables.BetterEyeShadows.Managers;
using Ash.Core.Features.BetterApplicables.BetterMoles.Extensions;
using Ash.Core.Features.BetterApplicables.BetterMoles.Managers;
using Ash.Core.Features.BetterApplicables.BetterTattoos.Extensions;
using Ash.Core.Features.BetterApplicables.BetterTattoos.Managers;
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
        public const string Version = "1.4.3";

        internal static Ash Instance { get; private set; }
        internal new static AshLogger Logger;

        internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleWindowHotkey { get; private set; }
        internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleImmersiveUIHotkey { get; private set; }
        internal static ConfigEntry<KeyboardShortcut> ConfigEntryTriggerDirtyTalk { get; private set; }
        internal static ConfigEntry<bool> ConfigEntrySkipToTitleSceneEnabled { get; private set; }

        internal static PersistentSettings PersistentSettings { get; private set; }

        internal static GameObject AshGameObj;
        internal static AshUI AshUI;

        internal static BetterTattooMakerExtensionHead BetterTattooMakerExtensionHead;
        internal static BetterTattooMakerExtensionBody BetterTattooMakerExtensionBody;
        internal static BetterTattooDataManager BetterTattooDataManager;

        internal static BetterEyeShadowMakerExtensionHead BetterEyeShadowMakerExtensionHead;
        internal static BetterEyeShadowDataManager BetterEyeShadowDataManager;

        internal static BetterCheekShadowMakerExtensionHead BetterCheekShadowMakerExtensionHead;
        internal static BetterCheekShadowDataManager BetterCheekShadowDataManager;

        internal static BetterMoleMakerExtensionHead BetterMoleMakerExtensionHead;
        internal static BetterMoleDataManager BetterMoleDataManager;

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

            GlobalPluginData.PerformShaderCacheWarmup();

            CreateRootObjAndAttachComponents();

            // initialize MoreAccessories pointer
            MoreAccessoriesInstance = GetMoreAccessoriesInstance();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void CreateRootObjAndAttachComponents() {
            AshGameObj = new GameObject(
                "Ash",
                typeof(AshUI),
                typeof(SceneTypeTracker),
                typeof(ActionsManager),
                typeof(BetterTattooMakerExtensionHead),
                typeof(BetterTattooMakerExtensionBody),
                typeof(BetterTattooDataManager),
                typeof(BetterEyeShadowMakerExtensionHead),
                typeof(BetterEyeShadowDataManager),
                typeof(BetterCheekShadowMakerExtensionHead),
                typeof(BetterCheekShadowDataManager),
                typeof(BetterMoleMakerExtensionHead),
                typeof(BetterMoleDataManager)
            );

            AshUI = AshGameObj.GetComponent<AshUI>();

            BetterTattooMakerExtensionHead = AshGameObj.GetComponent<BetterTattooMakerExtensionHead>();
            BetterTattooMakerExtensionBody = AshGameObj.GetComponent<BetterTattooMakerExtensionBody>();
            BetterTattooDataManager = AshGameObj.GetComponent<BetterTattooDataManager>();

            BetterEyeShadowMakerExtensionHead = AshGameObj.GetComponent<BetterEyeShadowMakerExtensionHead>();
            BetterEyeShadowDataManager = AshGameObj.GetComponent<BetterEyeShadowDataManager>();

            BetterCheekShadowMakerExtensionHead = AshGameObj.GetComponent<BetterCheekShadowMakerExtensionHead>();
            BetterCheekShadowDataManager = AshGameObj.GetComponent<BetterCheekShadowDataManager>();

            BetterMoleMakerExtensionHead = AshGameObj.GetComponent<BetterMoleMakerExtensionHead>();
            BetterMoleDataManager = AshGameObj.GetComponent<BetterMoleDataManager>();

            DontDestroyOnLoad(AshGameObj);

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
