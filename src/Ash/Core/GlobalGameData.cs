using System.Collections.Generic;
using Ash.Core.Features.Actions;
using Ash.Utility.GlobalUtils;

namespace Ash.Core;

internal static class GlobalGameData
{
    internal static readonly Dictionary<Female, AhegaoFaceAction.EyePreset> CustomEyesActive = new();
    internal static readonly Dictionary<Female, AhegaoFaceAction.EyesExpressionPreset> CustomEyesExpressionActive = new();
    internal static readonly Dictionary<Female, AhegaoFaceAction.MouthExpressionPreset> CustomMouthExpressionActive = new();

    internal static readonly Dictionary<Female, float> CustomFlushOverride = new();

    internal static bool IsCustomEyesPresetActive(Female female) {
        return CustomEyesActive.GetValueOrDefaultValue(female, AhegaoFaceAction.EyePreset.None)
            != AhegaoFaceAction.EyePreset.None;
    }

    internal static bool IsCustomEyesExpressionPresetActive(Female female) {
        return CustomEyesExpressionActive.GetValueOrDefaultValue(female, AhegaoFaceAction.EyesExpressionPreset.None)
               != AhegaoFaceAction.EyesExpressionPreset.None;
    }

    internal static bool IsCustomMouthExpressionPresetActive(Female female) {
        return CustomMouthExpressionActive.GetValueOrDefaultValue(female, AhegaoFaceAction.MouthExpressionPreset.None)
               != AhegaoFaceAction.MouthExpressionPreset.None;
    }

    internal static bool AllCustomFaceComponentsActive(Female female) {
        return IsCustomEyesPresetActive(female)
               && IsCustomEyesExpressionPresetActive(female)
               && IsCustomMouthExpressionPresetActive(female);
    }
}
