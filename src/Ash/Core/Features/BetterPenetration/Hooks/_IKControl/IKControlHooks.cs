using HarmonyLib;
using UnityEngine;

namespace Ash.Core.Features.BetterPenetration.Hooks._IKControl
{
    [HarmonyPatch]
    internal class IKControlHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(IK_Control), nameof(IK_Control.CalcTin))]
        // ReSharper disable once InconsistentNaming
        internal static bool CalcTinPrefix(IK_Control __instance) {
            if (!Ash.PersistentSettings.EnableBetterPenetration.Value)
                return true;

            if (__instance.tinRoot == null || __instance.tinTarget == null || !__instance.TinEnable)
                return false;

            VectorUtility.Vector3_ToYawPitch(__instance.tinRoot.parent.InverseTransformPoint(__instance.tinTarget.position), out var yaw, out var pitch);
            __instance.tinRoot.localRotation = Quaternion.Euler(pitch, yaw, 0.0f);

            Ash.BoneInjector.UpdateIKBonesTransforms(__instance.fbik.animator.transform.parent.GetComponent<Male>());

            return false;
        }
    }
}
