using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BetterApplicablesStudio.Core.BetterApplicables.BetterCheekShadows.Managers;
using BetterApplicablesStudio.Core.BetterApplicables.BetterEyeShadows.Managers;
using BetterApplicablesStudio.Core.BetterApplicables.BetterMoles.Managers;
using BetterApplicablesStudio.Core.BetterApplicables.BetterTattoos.Managers;
using HarmonyLib;
using KKAPI;
using UnityEngine;

namespace BetterApplicablesStudio
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class BetterApplicablesStudio : BaseUnityPlugin
    {
        public const string PluginName = "BetterApplicablesStudio";

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public const string GUID = "inviter42.betterapplicablesstudio";
        public const string AshGUID = "inviter42.anotherscenehelper";
        public const string Version = "1.1.0";

        internal new static ManualLogSource Logger;

        internal static BetterTattooDataManager BetterTattooDataManager;
        internal static BetterEyeShadowDataManager BetterEyeShadowDataManager;
        internal static BetterCheekShadowDataManager BetterCheekShadowDataManager;
        internal static BetterMoleDataManager BetterMoleDataManager;

        private static GameObject BetterApplicablesStudioGameObj;

        private static Harmony Harmony;

        private void Awake() {
            if (Application.productName != "PlayHomeStudio") {
                base.Logger.LogWarning($"BetterApplicablesStudio plugin is studio only, loading has been interrupted.");
                return;
            }

            InitializePlugin();
        }

        private void InitializePlugin() {
            Logger = base.Logger;
            Harmony = new Harmony($"{GUID}.harmony");

            // Register hooks
            Harmony.PatchAll(Assembly.GetExecutingAssembly());

            CreateRootGameObject();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void CreateRootGameObject() {
            BetterApplicablesStudioGameObj = new GameObject(
                "BetterApplicablesStudio",
                typeof(BetterTattooDataManager),
                typeof(BetterEyeShadowDataManager),
                typeof(BetterCheekShadowDataManager),
                typeof(BetterMoleDataManager)
            );

            BetterTattooDataManager = BetterApplicablesStudioGameObj.GetComponent<BetterTattooDataManager>();
            BetterEyeShadowDataManager = BetterApplicablesStudioGameObj.GetComponent<BetterEyeShadowDataManager>();
            BetterCheekShadowDataManager = BetterApplicablesStudioGameObj.GetComponent<BetterCheekShadowDataManager>();
            BetterMoleDataManager = BetterApplicablesStudioGameObj.GetComponent<BetterMoleDataManager>();

            DontDestroyOnLoad(BetterApplicablesStudioGameObj);
        }
    }
}
