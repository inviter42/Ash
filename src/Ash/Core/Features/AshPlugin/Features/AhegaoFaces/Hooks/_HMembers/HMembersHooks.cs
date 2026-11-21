using Ash.Utility.GlobalUtils;
using H;
using HarmonyLib;
using UnityEngine;

namespace Ash.Core.Features.AshPlugin.Features.AhegaoFaces.Hooks._HMembers;

[HarmonyPatch]
internal class HMembersHooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(H_Members), nameof(H_Members.Update_FemaleFlush), typeof(Female))]
    // ReSharper disable once InconsistentNaming
    internal static bool Update_FemaleFlushPrefix(H_Members __instance, Female female) {
        if (!Ash.PersistentSettings.AhegaoFacesEnabled.Value)
            return true;

        if (GlobalGameData.AllCustomFaceComponentsActive(female))
            GlobalGameData.CustomFlushOverride[female] = Mathf.Min(
                Ash.PersistentSettings.AhegaoFlushValueCapValue.Value,
                GlobalGameData.CustomFlushOverride.GetValueOrDefaultValue(female, 0f) + Ash.PersistentSettings.AhegaoFlushAppearanceRateValue.Value
            );

        if (GlobalGameData.CustomFlushOverride.GetValueOrDefaultValue(female, 0f) == 0)
            return true;

        var v = Mathf.Max(0, GlobalGameData.CustomFlushOverride.GetValueOrDefaultValue(female, 0f)
                                - Ash.PersistentSettings.AhegaoFlushDecayRateValue.Value);

        GlobalGameData.CustomFlushOverride[female] = v;

        female.SetFlush(v);

        return false;
    }
}
