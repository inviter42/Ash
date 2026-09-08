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
        public const string Version = "1.4.0";

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

            PatchHooks();

            // Initialize UI
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

        private static void PatchHooks() {
            // Register hooks
#if DEBUG
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Main.Hooks.DevHooks));
#endif

            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.ItemsCoordinator.Hooks._HMember.HMemberHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.ItemsCoordinator.Hooks._Female.FemaleHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.ItemsCoordinator.Hooks._Wearables.WearsHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.ItemsCoordinator.Hooks._Wearables.AccessoriesHooks));

            Harmony.PatchAll(typeof(Core.Features.Fixes.Hooks._Female.FemaleHooks));
            Harmony.PatchAll(typeof(Core.Features.Fixes.Hooks._HMember.HMembersHooks));

            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HMember.HMemberHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HState.HStateExitHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HState.HStateExtractHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HState.HStateInsertedWaitHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HState.HStateInXtcAfterBase));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HState.HStateLoopHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HState.HStatePreInsertWaitHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HState.HStatePreTouchWaitHooks));
            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Features.HSceneSettings.Hooks._HState.HStateStartHooks));

            Harmony.PatchAll(typeof(Core.Features.BetterTattoos.Hooks._Body.BodyHooks));
            Harmony.PatchAll(typeof(Core.Features.BetterTattoos.Hooks._Head.HeadHooks));
            Harmony.PatchAll(typeof(Core.Features.BetterTattoos.Hooks._Female.FemaleHooks));
            Harmony.PatchAll(typeof(Core.Features.BetterTattoos.Hooks._Male.MaleHooks));
            Harmony.PatchAll(typeof(Core.Features.BetterTattoos.Hooks._CustomEdit.CustomEditHooks));
            Harmony.PatchAll(typeof(Core.Features.BetterTattoos.Hooks._EditMode.EditModeHooks));
            Harmony.PatchAll(typeof(Core.Features.BetterTattoos.Hooks._Scenes.EditSceneHooks));

            Harmony.PatchAll(typeof(Core.Features.ImmersiveUI.Hooks._Menus.ConfigMenuHooks));
            Harmony.PatchAll(typeof(Core.Features.ImmersiveUI.Hooks._Menus.PauseMenueHooks));
            Harmony.PatchAll(typeof(Core.Features.ImmersiveUI.Hooks._Scenes.HSceneHooks));
            Harmony.PatchAll(typeof(Core.Features.ImmersiveUI.Hooks._IllusionCamera.IllusionCameraHooks));

            Harmony.PatchAll(typeof(Core.Tooling.SceneManagement.Hooks._SceneControl.SceneControlHooks));
            Harmony.PatchAll(typeof(Core.Tooling.SceneManagement.Hooks._Female.FemaleHooks));

            Harmony.PatchAll(typeof(Core.Features.AshPlugin.Main.Hooks._Scenes.CautionSceneHooks));
        }
    }
}
