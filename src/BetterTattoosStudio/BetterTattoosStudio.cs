using BepInEx;
using BepInEx.Logging;
using BetterTattoosStudio.Core.ExtDataManagement;
using BetterTattoosStudio.Hooks._Body;
using BetterTattoosStudio.Hooks._Female;
using BetterTattoosStudio.Hooks._Head;
using BetterTattoosStudio.Hooks._Male;
using HarmonyLib;
using KKAPI;
using UnityEngine;

namespace BetterTattoosStudio
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class BetterTattoosStudio : BaseUnityPlugin
    {
        public const string PluginName = "BetterTattoosStudio";

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public const string GUID = "inviter42.bettertattoosstudio";
        public const string Version = "1.0.0";

        internal new static ManualLogSource Logger;

        private static GameObject BetterTattoosStudioGameObj;

        private static Harmony Harmony;

        private void Awake() {
            if (Application.productName != "PlayHomeStudio") {
                base.Logger.LogWarning($"BetterTattoosStudio plugin is studio only, loading has been interrupted.");
                return;
            }

            InitializePlugin();
        }

        private void InitializePlugin() {
            Logger = base.Logger;
            Harmony = new Harmony($"{GUID}.harmony");

            // Register hooks
            Harmony.PatchAll(typeof(BodyHooks));
            Harmony.PatchAll(typeof(HeadHooks));
            Harmony.PatchAll(typeof(MaleHooks));
            Harmony.PatchAll(typeof(FemaleHooks));

            CreateRootGameObject();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void CreateRootGameObject() {
            BetterTattoosStudioGameObj = new GameObject(
                "BetterTattoosStudio",
                typeof(TattooDataManager)
            );

            DontDestroyOnLoad(BetterTattoosStudioGameObj);
        }
    }
}
