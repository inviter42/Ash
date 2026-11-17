using Ash.Core.Tooling.SceneManagement;
using HarmonyLib;

namespace Ash.Core.Features.AshPlugin.Features.FemaleControls.Hooks._Female;

[HarmonyPatch]
internal class FemaleHooks
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Female), nameof(Female.Apply))]
    // ReSharper disable once InconsistentNaming
    internal static void ApplyPostfix(Female __instance) {
        if (Ash.PersistentSettings.OverrideNeckRotatorSettings.Value)
            InitNeckLar(__instance.neckLook);

        if (Ash.PersistentSettings.OverrideEyesRotatorSettings.Value)
            InitEyesLar(__instance.eyeLook);
    }

    // set neck/head pitch and yaw
    private static float NeckMaxUpInitialValue;
    private static float NeckMaxDownInitialValue;
    private static float NeckMaxLeftInitialValue;
    private static float NeckMaxRightInitialValue;
    private static float HeadMaxLeftInitialValue;
    private static float HeadMaxRightInitialValue;

    internal static void InitNeckLarForAllFemales() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        foreach (var mainFemale in hScene.mainMembers.females)
            InitNeckLar(mainFemale.neckLook);

        InitNeckLar(hScene.visitor?.female.neckLook);
    }

    internal static void ResetNeckLarForAllFemales() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        foreach (var mainFemale in hScene.mainMembers.females)
            ResetNeckLar(mainFemale.neckLook);

        ResetNeckLar(hScene.visitor?.female.neckLook);
    }

    private static void InitNeckLar(LookAtRotator lar) {
        if (lar == null)
            return;

        if (NeckMaxUpInitialValue == 0)
            NeckMaxUpInitialValue = lar.maxUp;

        if (NeckMaxDownInitialValue == 0)
            NeckMaxDownInitialValue = lar.maxDown;

        lar.maxUp = Ash.PersistentSettings.NeckRotatorMaxUpValue.Value;
        lar.maxDown = Ash.PersistentSettings.NeckRotatorMaxDownValue.Value;

        foreach (var rotateBone in lar.rotateBones) {
            switch (rotateBone.bone.name) {
                case "cf_J_Neck": {
                    if (NeckMaxLeftInitialValue == 0)
                        NeckMaxLeftInitialValue = rotateBone.maxLeft;

                    if (NeckMaxRightInitialValue == 0)
                        NeckMaxRightInitialValue = rotateBone.maxRight;

                    rotateBone.maxLeft = rotateBone.maxRight =
                        Ash.PersistentSettings.NeckBoneMaxYawMagnitudeValue.Value;

                    break;
                }
                case "cf_J_Head": {
                    if (HeadMaxLeftInitialValue == 0)
                        HeadMaxLeftInitialValue = rotateBone.maxLeft;

                    if (HeadMaxRightInitialValue == 0)
                        HeadMaxRightInitialValue = rotateBone.maxRight;

                    rotateBone.maxLeft = rotateBone.maxRight =
                        Ash.PersistentSettings.HeadBoneMaxYawMagnitudeValue.Value;
                    break;
                }
            }
        }
    }

    private static void ResetNeckLar(LookAtRotator lar) {
        if (lar == null)
            return;

        lar.maxUp = NeckMaxUpInitialValue;
        lar.maxDown = NeckMaxDownInitialValue;

        foreach (var rotateBone in lar.rotateBones) {
            switch (rotateBone.bone.name) {
                case "cf_J_Neck": {
                    rotateBone.maxLeft = NeckMaxLeftInitialValue;
                    rotateBone.maxRight = NeckMaxRightInitialValue;
                    break;
                }
                case "cf_J_Head": {
                    rotateBone.maxLeft = HeadMaxLeftInitialValue;
                    rotateBone.maxRight = HeadMaxRightInitialValue;
                    break;
                }
            }
        }
    }


    // set eyes pitch and yaw
    private static float EyesMaxUpInitialValue;
    private static float EyesMaxDownInitialValue;
    private static float EyeLeftMaxLeftInitialValue;
    private static float EyeLeftMaxRightInitialValue;
    private static float EyeRightMaxLeftInitialValue;
    private static float EyeRightMaxRightInitialValue;

    internal static void InitEyesLarForAllFemales() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        foreach (var mainFemale in hScene.mainMembers.females)
            InitEyesLar(mainFemale.eyeLook);

        InitEyesLar(hScene.visitor?.female.eyeLook);
    }

    internal static void ResetEyesLarForAllFemales() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        foreach (var mainFemale in hScene.mainMembers.females)
            ResetEyesLar(mainFemale.eyeLook);

        ResetEyesLar(hScene.visitor?.female.eyeLook);
    }

    private static void InitEyesLar(LookAtRotator lar) {
        if (lar == null)
            return;

        if (EyesMaxUpInitialValue == 0)
            EyesMaxUpInitialValue = lar.maxUp;

        if (EyesMaxDownInitialValue == 0)
            EyesMaxDownInitialValue = lar.maxDown;

        lar.maxUp = Ash.PersistentSettings.EyesRotatorMaxUpValue.Value;
        lar.maxDown = Ash.PersistentSettings.EyesRotatorMaxDownValue.Value;

        foreach (var rotateBone in lar.rotateBones) {
            switch (rotateBone.bone.name) {
                case "cf_J_look_L": {
                    if (EyeLeftMaxLeftInitialValue == 0)
                        EyeLeftMaxLeftInitialValue = rotateBone.maxLeft;

                    if (EyeLeftMaxRightInitialValue == 0)
                        EyeLeftMaxRightInitialValue = rotateBone.maxRight;

                    rotateBone.maxLeft = Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value;
                    rotateBone.maxRight = Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value;

                    break;
                }
                case "cf_J_look_R": {
                    if (EyeRightMaxLeftInitialValue == 0)
                        EyeRightMaxLeftInitialValue = rotateBone.maxLeft;

                    if (EyeRightMaxRightInitialValue == 0)
                        EyeRightMaxRightInitialValue = rotateBone.maxRight;

                    rotateBone.maxLeft = Ash.PersistentSettings.EyesBoneMaxPositiveYawValue.Value;
                    rotateBone.maxRight = Ash.PersistentSettings.EyesBoneMaxNegativeYawValue.Value;

                    break;
                }
            }
        }
    }

    private static void ResetEyesLar(LookAtRotator lar) {
        if (lar == null)
            return;

        lar.maxUp = EyesMaxUpInitialValue;
        lar.maxDown = EyesMaxDownInitialValue;

        foreach (var rotateBone in lar.rotateBones) {
            switch (rotateBone.bone.name) {
                case "cf_J_look_L": {
                    rotateBone.maxLeft = EyeLeftMaxLeftInitialValue;
                    rotateBone.maxRight = EyeLeftMaxRightInitialValue;
                    break;
                }
                case "cf_J_look_R": {
                    rotateBone.maxLeft = EyeRightMaxLeftInitialValue;
                    rotateBone.maxRight = EyeRightMaxRightInitialValue;
                    break;
                }
            }
        }
    }
}
