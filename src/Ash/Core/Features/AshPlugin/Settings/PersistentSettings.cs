using Ash.Utility.GlobalUtils;
using Newtonsoft.Json;

namespace Ash.Core.Features.AshPlugin.Settings;

internal class PersistentSettings
{
    // H-Scene settings
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


    // Scene Interactivity
    [JsonProperty]
    internal Setting<bool> EnableFemaleLarTargetMouseControl = new(false);


    // Bug fixes
    [JsonProperty]
    internal Setting<bool> InterruptVoiceClipImmediatelyUponGagChange = new(false);

    [JsonProperty]
    internal Setting<bool> FixIncorrectShowMouthLiquidState = new(false);


    // Shader fixes
    [JsonProperty]
    internal Setting<bool> FixTearsShaderColorBinding = new(false);

    [JsonProperty]
    internal Setting<float> TearsAlphaInterpolationMax = new(0.5882353f);

    [JsonProperty]
    internal Setting<bool> FixVirginBloodZFighting = new(false);

    [JsonProperty]
    internal Setting<bool> FixSpermZFighting = new(false);

    [JsonProperty]
    internal Setting<float> ReplacedSpermMaterialOpacity = new(0.24f);

    [JsonProperty]
    internal Setting<bool> FixFlushZFighting = new(false);


    // LAR settings
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


    // Immersive UI settings
    [JsonProperty]
    internal Setting<bool> IsImmersiveUiEnabled = new(false);

    [JsonProperty]
    internal Setting<bool> ThumbnailBackgroundRemovalEnabled = new(false);


    // Dirty talk
    [JsonProperty]
    internal Setting<int> DirtyTalkMinValue = new(0);

    [JsonProperty]
    internal Setting<int> DirtyTalkMaxValue = new(0);


    // Ahegao
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


    // Female Particle System settings
    [JsonProperty]
    internal Setting<float> PsSimulationSpeedValue = new(1f, 0.75f);

    [JsonProperty]
    internal Setting<float> PsStartDelayMinValue = new(0f, 1f);

    [JsonProperty]
    internal Setting<float> PsStartDelayMaxValue = new(0f, 2f);

    [JsonProperty]
    internal Setting<float> PsGravityModifierValue = new(1f, 1f);

    [JsonProperty]
    internal Setting<float> PsStartSizeMultiplierValue = new(1f, 0.01f);

    [JsonProperty]
    internal Setting<float> PsRateOverTimeMultiplierMinValue = new(75f, 2.4f);

    [JsonProperty]
    internal Setting<float> PsRateOverTimeMultiplierMaxValue = new(75f, 3.5f);

    [JsonProperty]
    internal Setting<float> SeStartSizeMultiplierValue = new(0.1f, 0.06f);

    [JsonProperty]
    internal Setting<int> SeEmissionBurstCountValue = new(1, 4);

    [JsonProperty]
    internal Setting<int> ParticleSystemCollisionQuality = new(0, 0);


    // Male ejaculation emitter

    // pack1
    [JsonProperty]
    internal Setting<bool> EjaculatorPack0AddToPoolValue = new(true);

    [JsonProperty]
    internal Setting<float> EjaculatorPack0LocalScaleValue = new(0.5f, 0.2f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack0TsRelaxDistanceValue = new(0.1f, 0.025f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack0TsMinDistanceValue = new(0.01f, 0.001f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack0TsMaxDistanceValue = new(0.1f);

    // pack2
    [JsonProperty]
    internal Setting<bool> EjaculatorPack1AddToPoolValue = new(true);

    [JsonProperty]
    internal Setting<float> EjaculatorPack1LocalScaleValue = new(0.5f, 0.08f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack1TsRelaxDistanceValue = new(0.1f, 0.03f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack1TsMinDistanceValue = new(0.01f, 0.001f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack1TsMaxDistanceValue = new(0.1f, 0.075f);

    // pack3
    [JsonProperty]
    internal Setting<bool> EjaculatorPack2AddToPoolValue = new(true);

    [JsonProperty]
    internal Setting<float> EjaculatorPack2LocalScaleValue = new(0.25f, 0.08f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack2TsRelaxDistanceValue = new(0.1f, 0.01f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack2TsMinDistanceValue = new(0.01f, 0.001f);

    [JsonProperty]
    internal Setting<float> EjaculatorPack2TsMaxDistanceValue = new(0.1f, 0.05f);

    // RigidEmitter
    [JsonProperty]
    internal Setting<int> EjaculatorRigidEmitterEmitNumValue = new(3);


    // BP
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


    // Plugin UI settings
    [JsonProperty]
    internal Setting<float> PluginUIColorRedValue = new(1f);

    [JsonProperty]
    internal Setting<float> PluginUIColorGreenValue = new(0.67f);

    [JsonProperty]
    internal Setting<float> PluginUIColorBlueValue = new(1f);

    [JsonProperty]
    internal Setting<bool> ConfineCursorToWindowEnabled = new(false);
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

    [JsonIgnore]
    public T Default { get; }

    [JsonIgnore]
    public T Recommended { get; }

    public Setting(T defaultValue) {
        _value = Default = defaultValue;
    }

    public Setting(T defaultValue, T recommended) {
        Default = defaultValue;
        _value = Recommended = recommended;
    }
}
