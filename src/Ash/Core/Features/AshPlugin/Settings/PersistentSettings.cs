using Ash.Utility.GlobalUtils;
using Newtonsoft.Json;

namespace Ash.Core.Features.AshPlugin.Settings;

internal class PersistentSettings
{
    [JsonProperty]
    internal Setting<bool> ShouldMuteBackgroundFemale = new(false);

    [JsonProperty]
    internal Setting<bool> DisableFemaleAutoEjaculation = new(false);

    [JsonProperty]
    internal Setting<bool> SkippingSpurtStateEnabled = new(false);

    [JsonProperty]
    internal Setting<bool> DisableFemaleHVoiceBark = new(false);

    [JsonProperty]
    internal Setting<bool> DisableFemaleInactionBark = new(false);

    [JsonProperty]
    internal Setting<bool> DisableFemaleVoiceBarkAtSceneStart = new(false);

    [JsonProperty]
    internal Setting<bool> DisableFemaleVoiceBarkAtSceneEnd = new(false);

    [JsonProperty]
    internal Setting<bool> DisableFemaleVoiceBarkAfterExtract = new(false);

    [JsonProperty]
    internal Setting<bool> DisableFemaleVoiceBarkAfterEjaIn = new(false);

    [JsonProperty]
    internal Setting<bool> DisableFemaleSpermDripAfterExtract = new(false);

    [JsonProperty]
    internal Setting<bool> InterruptVoiceClipImmediatelyUponGagChange = new(false);

    [JsonProperty]
    internal Setting<bool> EnableFemaleLarTargetMouseControl = new(false);

    [JsonProperty]
    internal Setting<bool> FixIncorrectShowMouthLiquidState = new(false);

    [JsonProperty]
    internal Setting<bool> FixTearsShaderColorBinding = new(false);

    [JsonProperty]
    internal Setting<float> TearsAlphaInterpolationMax = new(0.5882353f);

    [JsonProperty]
    internal Setting<bool> FixVirginBloodZFighting = new(false);

    [JsonProperty]
    internal Setting<bool> FixSpermZFighting = new(false);

    [JsonProperty]
    internal Setting<bool> FixFlushZFighting = new(false);

    [JsonProperty]
    internal Setting<bool> ApplyNewLarCalculationLogic = new(false);

    [JsonProperty]
    internal Setting<bool> OverrideNeckRotatorSettings = new(false);

    [JsonProperty]
    internal Setting<float> NeckRotatorMaxUpValue = new(50f);

    [JsonProperty]
    internal Setting<float> NeckRotatorMaxDownValue = new(20f);

    [JsonProperty]
    internal Setting<float> NeckBoneMaxYawMagnitudeValue = new(50f);

    [JsonProperty]
    internal Setting<float> HeadBoneMaxYawMagnitudeValue = new(50f);

    [JsonProperty]
    internal Setting<bool> OverrideEyesRotatorSettings = new(false);

    [JsonProperty]
    internal Setting<float> EyesRotatorMaxUpValue = new(45f);

    [JsonProperty]
    internal Setting<float> EyesRotatorMaxDownValue = new(12f);

    [JsonProperty]
    internal Setting<float> EyesBoneMaxPositiveYawValue = new(50f);

    [JsonProperty]
    internal Setting<float> EyesBoneMaxNegativeYawValue = new(50f);

    [JsonProperty]
    internal Setting<bool> IsImmersiveUiEnabled = new(false);

    [JsonProperty]
    internal Setting<bool> ConfineCursorToWindowEnabled = new(false);

    [JsonProperty]
    internal Setting<bool> ThumbnailBackgroundRemovalEnabled = new(false);

    [JsonProperty]
    internal Setting<int> DirtyTalkMinValue = new(0);

    [JsonProperty]
    internal Setting<int> DirtyTalkMaxValue = new(0);

    [JsonProperty]
    internal Setting<bool> AhegaoFacesEnabled = new(false);

    [JsonProperty]
    internal Setting<float> AhegaoMinDurationValue = new(3.0f);

    [JsonProperty]
    internal Setting<float> AhegaoMaxDurationValue = new(5.0f);

    [JsonProperty]
    internal Setting<float> AhegaoDecayValue = new(0.95f);

    [JsonProperty]
    internal Setting<float> AhegaoFlushAppearanceRateValue = new(0.0125f);

    [JsonProperty]
    internal Setting<float> AhegaoFlushDecayRateValue = new(0.001f);

    [JsonProperty]
    internal Setting<float> AhegaoFlushValueCapValue = new(1f);

    [JsonProperty]
    internal Setting<float> AhegaoClimaxTriggerBaseProbabilityValue = new(0f);

    [JsonProperty]
    internal Setting<float> AhegaoClimaxTriggerProbabilityIncrementValue = new(0.2f);

    [JsonProperty]
    internal Setting<float> ParticleSystemStartDelayMinValue = new(1);

    [JsonProperty]
    internal Setting<float> ParticleSystemStartDelayMaxValue = new(2);

    [JsonProperty]
    internal Setting<float> ParticleSystemGravityModifierValue = new(0.4f);

    [JsonProperty]
    internal Setting<float> ParticleSystemStartSizeMultiplierValue = new(0.003f);

    [JsonProperty]
    internal Setting<float> ParticleSystemRateOverTimeMultiplierMinValue = new(1);

    [JsonProperty]
    internal Setting<float> ParticleSystemRateOverTimeMultiplierMaxValue = new(7);

    [JsonProperty]
    internal Setting<int> ParticleSystemCollisionQuality = new(0);

    [JsonProperty]
    internal Setting<bool> EnableBetterPenetration = new(false);

    [JsonProperty]
    internal Setting<bool> RemoveAnimationMinSpeedCapValue = new(false);

    [JsonProperty]
    internal Setting<bool> ShowMaleSackValue = new(true);

    [JsonProperty]
    internal Setting<float> AnimationWheelSpeedValue = new(0.1f);

    [JsonProperty]
    internal Setting<int> BoneDensity = new(10);

    [JsonProperty]
    internal Setting<float> RootBoneLength = new(1f);

    [JsonProperty]
    internal Setting<float> BoneXyInvSqrtScaleFactor = new(1f);

    [JsonProperty]
    internal Setting<float> BoneXyLinearScaleFactor = new(1f);

    [JsonProperty]
    internal Setting<float> ColliderDistanceOffset = new(0f);

    [JsonProperty]
    internal Setting<bool> EnableBonesTelemetry = new(false);
}

internal class Setting<T> {
    [JsonProperty("Value")]
    // ReSharper disable once InconsistentNaming
    internal T _value;

    // ignore to prevent IO.Save() trigger during object construction
    [JsonIgnore]
    public T Value {
        get => _value;
        set {
            if (_value.Equals(value))
                return;

            _value = value;
            IO.Save(Ash.PersistentSettings, IO.SettingsFileName);
        }
    }

    public Setting(T defaultValue) => _value = defaultValue;
}
