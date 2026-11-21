using System.Reflection;
using Ash.Core;
using Ash.Core.Features.Actions;
using Ash.Core.Features.AshPlugin.Features.HSceneCustomization._MaterialEditor;
using Ash.Core.Features.AshPlugin.Main;
// ReSharper disable once RedundantUsingDirective
using Ash.Core.Features.AshPlugin.Main.Hooks;
using Ash.Core.Features.AshPlugin.Settings;
using Ash.Core.Features.BetterApplicables.BetterCheekShadows.Extensions;
using Ash.Core.Features.BetterApplicables.BetterCheekShadows.Managers;
using Ash.Core.Features.BetterApplicables.BetterEyeShadows.Extensions;
using Ash.Core.Features.BetterApplicables.BetterEyeShadows.Managers;
using Ash.Core.Features.BetterApplicables.BetterMoles.Extensions;
using Ash.Core.Features.BetterApplicables.BetterMoles.Managers;
using Ash.Core.Features.BetterApplicables.BetterTattoos.Extensions;
using Ash.Core.Features.BetterApplicables.BetterTattoos.Managers;
using Ash.Core.Features.BetterPenetration;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KKAPI;
using UnityEngine;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using KK_Plugins.MaterialEditor;
using KKAPI.Utilities;
using MoreAccessoriesPH;

namespace Ash;

[BepInPlugin(GUID, PluginName, Version)]
[BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
[BepInDependency(MaterialEditorPlugin.PluginGUID, MaterialEditorPlugin.PluginVersion)]
public class Ash : BaseUnityPlugin
{
    public const string PluginName = "Ash";

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public const string GUID = "inviter42.anotherscenehelper";
    public const string Version = "1.5.9";

    internal static Ash Instance { get; private set; }
    internal new static AshLogger Logger;

    internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleWindowHotkey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> ConfigEntryToggleImmersiveUIHotkey { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> ConfigEntryTriggerDirtyTalk { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> ConfigEntryTriggerAhegaoFace { get; private set; }
    internal static ConfigEntry<KeyboardShortcut> ConfigEntryTriggerMaleEmitter { get; private set; }
    internal static ConfigEntry<bool> ConfigEntrySkipToTitleSceneEnabled { get; private set; }

    internal static ConfigEntry<LogLevel> ConfigEntryActionsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryAnimationControlsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryApplicablesMigrationsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryBetterPenetrationModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryBugFixesModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryDebugModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryExtDataCheekShadowsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryExtDataEyeShadowsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryExtDataMolesModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryExtDataTattoosModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryFemaleSettingsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryGlobalModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryHMembersHooksModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryHSceneSettingsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryHumanHooksModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryImmersiveUIModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryItemsCoordinatorModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryItemsVisibilityControlsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryMaleSettingsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntrySceneManagementModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryUISettingsModuleLogLevel { get; private set; }
    internal static ConfigEntry<LogLevel> ConfigEntryWearablesHooksModuleLogLevel { get; private set; }

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

    internal static BoneInjector BoneInjector;

    internal static MoreAccessories MoreAccessoriesInstance;

    // ReSharper disable once InconsistentNaming
    private const string MoreAccessoriesGUID = "com.joan6694.illusionplugins.moreaccessories";

    private static Harmony Harmony;

    private void Awake() {
        if (Application.productName != "PlayHome") {
            base.Logger.LogWarning("Ash plugin is main game only, loading has been interrupted.");
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

        GlobalPluginData.Init();

        MaterialEditorInjector.InjectCachedShaders();
        MaterialEditorInjector.InjectProperties();

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
            typeof(BetterMoleDataManager),
            typeof(BoneInjector)
#if DEBUG
                ,typeof(DevHooks.AnimationBaker)
#endif
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

        BoneInjector = AshGameObj.GetComponent<BoneInjector>();

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

        ConfigEntryTriggerAhegaoFace = Config.Bind(
            "Shortcuts",
            "Trigger ahegao face",
            new KeyboardShortcut(KeyCode.F)
        );

        ConfigEntryTriggerMaleEmitter = Config.Bind(
            "Shortcuts",
            "Trigger male emitter",
            new KeyboardShortcut(KeyCode.V)
        );

        ConfigEntrySkipToTitleSceneEnabled = Config.Bind(
            "Global Settings",
            "Enable intro skip",
            false
        );

        var logLevelConfigDescription = new ConfigDescription(
            "Enables verbose logging for troubleshooting",
            null,
            new ConfigurationManagerAttributes { IsAdvanced = true }
        );

        ConfigEntryGlobalModuleLogLevel = Config.Bind("Logging", "Global", LogLevel.All, logLevelConfigDescription);
        ConfigEntryItemsCoordinatorModuleLogLevel = Config.Bind("Logging", "ItemsCoordinator", LogLevel.All, logLevelConfigDescription);
        ConfigEntryExtDataTattoosModuleLogLevel = Config.Bind("Logging", "ExtDataTattoos", LogLevel.All, logLevelConfigDescription);
        ConfigEntryExtDataEyeShadowsModuleLogLevel = Config.Bind("Logging", "ExtDataEyeShadows", LogLevel.All, logLevelConfigDescription);
        ConfigEntryExtDataCheekShadowsModuleLogLevel = Config.Bind("Logging", "ExtDataCheekShadows", LogLevel.All, logLevelConfigDescription);
        ConfigEntryExtDataMolesModuleLogLevel = Config.Bind("Logging", "ExtDataMoles", LogLevel.All, logLevelConfigDescription);
        ConfigEntryActionsModuleLogLevel = Config.Bind("Logging", "Actions", LogLevel.All, logLevelConfigDescription);
        ConfigEntryUISettingsModuleLogLevel = Config.Bind("Logging", "GameUIControls", LogLevel.All, logLevelConfigDescription);
        ConfigEntryHSceneSettingsModuleLogLevel = Config.Bind("Logging", "HSceneSettings", LogLevel.All, logLevelConfigDescription);
        ConfigEntryBugFixesModuleLogLevel = Config.Bind("Logging", "BugFixes", LogLevel.All, logLevelConfigDescription);
        ConfigEntryAnimationControlsModuleLogLevel = Config.Bind("Logging", "AnimationControls", LogLevel.All, logLevelConfigDescription);
        ConfigEntryImmersiveUIModuleLogLevel = Config.Bind("Logging", "ImmersiveUI", LogLevel.All, logLevelConfigDescription);
        ConfigEntryItemsVisibilityControlsModuleLogLevel = Config.Bind("Logging", "ItemsVisibilityControls", LogLevel.All, logLevelConfigDescription);
        ConfigEntryMaleSettingsModuleLogLevel = Config.Bind("Logging", "MaleSettings", LogLevel.All, logLevelConfigDescription);
        ConfigEntrySceneManagementModuleLogLevel = Config.Bind("Logging", "SceneManagement", LogLevel.All, logLevelConfigDescription);
        ConfigEntryHMembersHooksModuleLogLevel = Config.Bind("Logging", "HMembersHooks", LogLevel.All, logLevelConfigDescription);
        ConfigEntryHumanHooksModuleLogLevel = Config.Bind("Logging", "HumanHooks", LogLevel.All, logLevelConfigDescription);
        ConfigEntryWearablesHooksModuleLogLevel = Config.Bind("Logging", "WearablesHooks", LogLevel.All, logLevelConfigDescription);
        ConfigEntryBetterPenetrationModuleLogLevel = Config.Bind("Logging", "BetterPenetration", LogLevel.All, logLevelConfigDescription);
        ConfigEntryApplicablesMigrationsModuleLogLevel = Config.Bind("Logging", "ApplicablesMigrations", LogLevel.All, logLevelConfigDescription);
        ConfigEntryFemaleSettingsModuleLogLevel = Config.Bind("Logging", "FemaleSettings", LogLevel.All, logLevelConfigDescription);
        ConfigEntryDebugModuleLogLevel = Config.Bind("Logging", "Debug", LogLevel.All, logLevelConfigDescription);

    }
}
