using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.ItemsCoordinator.Hooks._Female
{
    internal class FemaleHooks
    {
        // Female instance is ready
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Female), nameof(Female.Start))]
        // ReSharper disable once InconsistentNaming
        internal static void FemaleStartPostfix(Female __instance) {
            ItemsCoordinator.ApplyRules(__instance, RulesManager.InterItemRuleSets);
        }
    }
}
