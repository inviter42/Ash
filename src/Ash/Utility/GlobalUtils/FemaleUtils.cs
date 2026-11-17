using System.Collections.Generic;
using System.Linq;
using Ash.Core;
using Ash.Core.Features.Actions;
using Ash.Core.Features.AshPlugin.Features.BugFixes.Hooks._LookAtRotator;
using Character;
using UnityEngine;

namespace Ash.Utility.GlobalUtils;

internal static class FemaleUtils
{

    internal static void ApplyEyeRotation(Female female, KeyValuePair<AhegaoFaceAction.EyePreset,Vector3[]> preset, float transition = 0.1f) {
        GlobalGameData.CustomEyesActive[female] = preset.Key;

        female.eyeLookType = female.eyeLook.calcType = LookAtRotator.TYPE.NO;
        female.eyeLookTarget = female.eyeLook.target = null;
        female.eyeLookForce = false;
        female.eyeLook.noRate = 0f;
        female.eyeLook.targetPos = Vector3.zero;

        LookAtRotatorHooks.SetPresetOverride(
            female.eyeLook,
            preset.Value.Select(Quaternion.Euler).ToArray()
        );
    }

    internal static void ApplyEyesExpression(Female female, KeyValuePair<AhegaoFaceAction.EyesExpressionPreset, int> preset, float transition = 0.1f) {
        GlobalGameData.CustomEyesExpressionActive[female] = preset.Key;

        female.blink.LimitMax = 1;
        female.blink.Hold = true;
        female.ExpressionPlay(0, GlobalPluginData.EyeExpressionListFemale[preset.Value].anime, transition);
    }

    internal static void ApplyMouthExpression(Female female, KeyValuePair<AhegaoFaceAction.MouthExpressionPreset, int> preset, float transition = 0.1f, bool forceHold = false) {
        GlobalGameData.CustomMouthExpressionActive[female] = preset.Key;

        female.lipSync.RelaxOpen = 0f;
        female.ExpressionPlay(1, GlobalPluginData.MouthExpressionListFemale[preset.Value].anime, transition);
        female.lipSync.Hold = !AhegaoFaceAction.IsMouthExpressionPresetOnLipSyncWhitelist(preset.Key) || forceHold;
        female.head.ResetMouth(female.Gag);
        const TONGUE_TYPE type = TONGUE_TYPE.FACE;
        female.SetTongueType(type);
    }


    internal static void ResetEyeRotation(Female female, float transition = 0.1f) {
        LookAtRotatorHooks.ClearPresetOverride(female.eyeLook);
        female.ChangeEyeLook(LookAtRotator.TYPE.TARGET, SceneUtils.GetVisitorDefaultLookTarget(), false);

        GlobalGameData.CustomEyesActive[female] = AhegaoFaceAction.EyePreset.None;
    }

    internal static void ResetEyesExpression(Female female, float transition = 0.1f) {
        female.blink.Hold = false;
        female.eyeState = "Eye_Def";
        female.ExpressionPlay(0, female.eyeState, transition);

        GlobalGameData.CustomEyesExpressionActive[female] = AhegaoFaceAction.EyesExpressionPreset.None;
    }

    internal static void ResetMouthExpression(Female female, float transition = 0.1f) {
        female.lipSync.Hold = female.Gag;
        female.mouthState = "Mouth_Def";
        female.ExpressionPlay(1, female.mouthState, transition);

        GlobalGameData.CustomMouthExpressionActive[female] = AhegaoFaceAction.MouthExpressionPreset.None;
    }
}
