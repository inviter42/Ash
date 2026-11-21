using System.Collections.Generic;
using System.Linq;
using Ash.Core.Tooling.SceneManagement;
using Ash.Logging;
using Ash.Utility.Extensions;
using Ash.Utility.GlobalUtils;
using Character;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ash.Core.Features.Actions;

internal class AhegaoFaceAction : AbstractAction
{
    private static readonly Dictionary<EyePreset, Vector3[]> EyePresets = new() {
        [EyePreset.Belle] = new[] { new Vector3(-12f, 8f, 0f), new Vector3(-16f, -22f, 0f) },
        [EyePreset.BelleReverse] = new[] { new Vector3(-16f, 22f, 0f), new Vector3(-12f, -8f, 0f) },
        [EyePreset.CrossedUp] = new[] { new Vector3(-7f, 22f, 0f), new Vector3(-7f, -22f, 0f) },
        [EyePreset.CrossedHorizon] = new[] { new Vector3(0f, 27f, 0f), new Vector3(0f, -27f, 0f) },
        [EyePreset.CrossedDown] = new[] { new Vector3(15f, 17f, 0f), new Vector3(15f, -17f, 0f) },
        [EyePreset.NoseObserver] = new[] { new Vector3(24f, 17f, 0f), new Vector3(24f, -17f, 0f) },
    };
    private static readonly Dictionary<EyesExpressionPreset, int> EyesExpressionPresets = new() {
        [EyesExpressionPreset.Shy] = 2,
        [EyesExpressionPreset.Awkward] = 5,
        [EyesExpressionPreset.Disappointed] = 18,
        [EyesExpressionPreset.Tsundere] = 23,
        [EyesExpressionPreset.Bawling] = 29,
        [EyesExpressionPreset.InPain] = 30,
        [EyesExpressionPreset.FeelingIt] = 36,
        [EyesExpressionPreset.AhegaoA] = 37,
        [EyesExpressionPreset.AhegaoB] = 38,
    };
    private static readonly Dictionary<MouthExpressionPreset, int> MouthExpressionPresets = new() {
        [MouthExpressionPreset.Smiling] = 2,
        [MouthExpressionPreset.Smirk] = 4,
        [MouthExpressionPreset.InPain] = 16,
        [MouthExpressionPreset.GrittedTeeth] = 17,
        [MouthExpressionPreset.AhegaoA] = 19,
        [MouthExpressionPreset.AhegaoB] = 20,
        [MouthExpressionPreset.AhegaoD] = 22,
        [MouthExpressionPreset.AhegaoE] = 23,
        [MouthExpressionPreset.Pouting] = 24,
        [MouthExpressionPreset.TongueC] = 31,
        [MouthExpressionPreset.TongueD] = 32,
        [MouthExpressionPreset.TongueE] = 33,
    };

    private static float[] EyePresetsWeights = new float[EyePresets.Count].Select(_ => 1f / EyePresets.Count).ToArray();
    private static float[] EyesExpressionPresetsWeights = new float[EyesExpressionPresets.Count].Select(_ => 1f / EyesExpressionPresets.Count).ToArray();
    private static float[] MouthExpressionPresetsWeights = new float[MouthExpressionPresets.Count].Select(_ => 1f / MouthExpressionPresets.Count).ToArray();

    private static readonly Dictionary<Female, float> AhegaoTriggerProbabilities = new();

    private static readonly MouthExpressionPreset[] LipSyncWhitelist = {
        MouthExpressionPreset.AhegaoD,
        MouthExpressionPreset.AhegaoE,
    };

    private static Female ActiveFemale;
    private static float DurationTimer;

    private static readonly AshLogger Logger = new(LoggingSettings.LoggingModules.Actions);


    internal override void QueryInput() {
        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        if (!HotkeyUtils.HotkeyIsDown(Ash.ConfigEntryTriggerAhegaoFace.Value.MainKey))
            return;

        ActiveFemale = hScene.mainMembers.GetFemale(0);

        TryActivateAhegaoExpression(ActiveFemale, Ash.PersistentSettings.AhegaoDecayValue.Value, true);

        DurationTimer = Random.Range(
            Ash.PersistentSettings.AhegaoMinDurationValue.Value,
            Ash.PersistentSettings.AhegaoMaxDurationValue.Value
        );
    }

    internal override void Update() {
        if (SceneTypeTracker.Scene is not H_Scene)
            return;

        if (DurationTimer <= 0)
            return;

        DurationTimer = Mathf.Max(0, DurationTimer - Time.deltaTime);

        if (DurationTimer > 0)
            return;

        DeactivateAhegaoExpression(ActiveFemale);
    }


    internal static void TryActivateAhegaoExpression(Female female, float decay, bool forceActivation = false) {
        if (!Ash.PersistentSettings.AhegaoFacesEnabled.Value)
            return;

        if (SceneTypeTracker.Scene is not H_Scene hScene)
            return;

        if (!forceActivation) {
            var probabilityToTrigger = AhegaoTriggerProbabilities.GetValueOrDefaultValue(
                female, Ash.PersistentSettings.AhegaoClimaxTriggerBaseProbabilityValue.Value);
            var threshold = 1 - probabilityToTrigger;

            var roll = Random.Range(0f, 1f);
            var baseStr = $"[Ahegao] Rolled '{roll:F4}' and at least '{threshold:F4}' was required. Roll had {probabilityToTrigger * 100}% probability to succeed. Result: ";
            if (roll > threshold) {
                Logger.LogInfo(baseStr + "SUCCESS");
            } else {
                AhegaoTriggerProbabilities[female] = Mathf.Clamp01(probabilityToTrigger + Ash.PersistentSettings.AhegaoClimaxTriggerProbabilityIncrementValue.Value);
                Logger.LogInfo(baseStr + $"FAILURE -> Increasing probability to {AhegaoTriggerProbabilities[female] * 100}%");
                return;
            }
        }

        var mouthBusy = hScene.mainMembers.param.UseMouth(SEX.FEMALE);

        // weight down selected by X%
        RandomUtils.WeightedRandomWithChoiceDecay(EyePresetsWeights, decay).Deconstruct(out var eyeIndex, out EyePresetsWeights);
        RandomUtils.WeightedRandomWithChoiceDecay(EyesExpressionPresetsWeights, decay).Deconstruct(out var eyesExpressionIndex, out EyesExpressionPresetsWeights);

        var eyesPreset = EyePresets.ElementAt(eyeIndex);
        var eyesExpressionPreset = EyesExpressionPresets.ElementAt(eyesExpressionIndex);

        FemaleUtils.ApplyEyeRotation(female, eyesPreset, Random.Range(0.15f, 0.25f));
        FemaleUtils.ApplyEyesExpression(female, eyesExpressionPreset, Random.Range(0.15f, 0.25f));

        KeyValuePair<MouthExpressionPreset, int> mouthExpressionPreset = default;
        if (!mouthBusy) {
            RandomUtils.WeightedRandomWithChoiceDecay(MouthExpressionPresetsWeights, decay).Deconstruct(out var mouthIndex, out MouthExpressionPresetsWeights);
            mouthExpressionPreset = MouthExpressionPresets.ElementAt(mouthIndex);

            FemaleUtils.ApplyMouthExpression(female, mouthExpressionPreset, Random.Range(0.15f, 0.25f));
        }

        Logger.LogDebug(
            $"Play EyePreset [{eyesPreset.Key.ToString()}], "
            + $"EyesExpressionPreset [{eyesExpressionPreset.Key.ToString()}] "
            + $"MouthExpressionPreset [{mouthExpressionPreset.Key.ToString()}] "
        );
    }

    internal static void DeactivateAhegaoExpression(Female female) {
        if (GlobalGameData.IsCustomEyesPresetActive(female))
            FemaleUtils.ResetEyeRotation(female, Random.Range(0.2f, 0.3f));

        if (GlobalGameData.IsCustomEyesExpressionPresetActive(female))
            FemaleUtils.ResetEyesExpression(female, Random.Range(0.26f, 0.38f));

        if (GlobalGameData.IsCustomMouthExpressionPresetActive(female))
            FemaleUtils.ResetMouthExpression(female, Random.Range(0.35f, 0.42f));
    }

    internal static bool IsMouthExpressionPresetOnLipSyncWhitelist(MouthExpressionPreset preset) {
        return LipSyncWhitelist.Contains(preset);
    }


    internal enum EyePreset
    {
        None,
        Belle,
        BelleReverse,
        CrossedUp,
        CrossedHorizon,
        CrossedDown,
        NoseObserver,
    }

    internal enum EyesExpressionPreset
    {
        None,
        Blank,
        Joy,
        Shy,
        SlightSmile,
        Smile,
        Awkward,
        Smirk,
        Smug,
        Posing,
        Boastful,
        WinkLeft,
        WinkRight,
        SquintingLeft,
        SquintingRight,
        NarrowedEyes,
        Unimpressed,
        Disdain,
        Dumbfounded,
        Disappointed,
        Surprised,
        Serious,
        Thinking,
        Scheming,
        Tsundere,
        Grumpy,
        Angry,
        Furious,
        Perplexed,
        Sad,
        Bawling,
        InPain,
        Agony,
        TwistedLeft,
        TwistedRight,
        Embarrassed,
        FeelingIt,
        Dreamy,
        AhegaoA,
        AhegaoB,
        AhegaoC,
    }

    internal enum MouthExpressionPreset
    {
        None,
        Blank,
        Joy,
        Smiling,
        AwkwardSmile,
        Smirk,
        Smug,
        Surprised,
        Serious,
        Scheming,
        Tsundere,
        Grumpy,
        Angry,
        Furious,
        Troubled,
        Sad,
        Bawling,
        InPain,
        GrittedTeeth,
        Screaming,
        AhegaoA,
        AhegaoB,
        AhegaoC,
        AhegaoD,
        AhegaoE,
        Pouting,
        Pursed,
        Whistling,
        KissA,
        KissB,
        TongueA,
        TongueB,
        TongueC,
        TongueD,
        TongueE,
    }
}
