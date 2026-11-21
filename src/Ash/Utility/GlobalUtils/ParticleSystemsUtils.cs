using Ash.Core.Features.AshPlugin.Settings;
using UnityEngine;

namespace Ash.Utility.GlobalUtils;

internal static class ParticleSystemsUtils
{
    internal static void AdjustFemaleSpermDripParticleSystemSettings(ParticleSystem ps) {
        var subEmitters = ps.subEmitters;

        SetParticleSystem(
            ps,
            Ash.PersistentSettings.PsSimulationSpeedValue,
            Ash.PersistentSettings.PsStartDelayMinValue,
            Ash.PersistentSettings.PsStartDelayMaxValue,
            Ash.PersistentSettings.PsGravityModifierValue,
            Ash.PersistentSettings.PsStartSizeMultiplierValue,
            Ash.PersistentSettings.ParticleSystemCollisionQuality
        );

        SetEmissionModule(
            ps,
            Ash.PersistentSettings.PsRateOverTimeMultiplierMinValue,
            Ash.PersistentSettings.PsRateOverTimeMultiplierMaxValue
        );

        for (var i = 0; i < subEmitters.subEmittersCount; i++) {
            var se = subEmitters.GetSubEmitterSystem(i);
            SetParticleSystem(
                se,
                Ash.PersistentSettings.SeStartSizeMultiplierValue,
                Ash.PersistentSettings.ParticleSystemCollisionQuality
            );

            SetEmissionModule(se, Ash.PersistentSettings.SeEmissionBurstCountValue);
        }
    }

    private static void SetParticleSystem(
        ParticleSystem ps,
        Setting<float> simulationSpeed,
        Setting<float> startDelayMin,
        Setting<float> startDelayMax,
        Setting<float> gravityModifier,
        Setting<float> startSizeMultiplier,
        Setting<int> collisionQuality
    ) {
        var mainModule = ps.main;
        var collisionModule = ps.collision;

        mainModule.simulationSpeed = simulationSpeed.Value;

        mainModule.startDelay = new ParticleSystem.MinMaxCurve(
            startDelayMin.Value,
            startDelayMax.Value
        );

        mainModule.gravityModifier = gravityModifier.Value;
        mainModule.startSizeMultiplier = startSizeMultiplier.Value;

        collisionModule.bounceMultiplier = 0;
        collisionModule.dampenMultiplier = 1;
        collisionModule.quality = (ParticleSystemCollisionQuality)collisionQuality.Value;
    }

    private static void SetParticleSystem(
        ParticleSystem ps,
        Setting<float> startSizeMultiplier,
        Setting<int> collisionQuality
    ) {
        var mainModule = ps.main;
        var collisionModule = ps.collision;

        mainModule.gravityModifier = 1;
        mainModule.simulationSpeed = 1;
        mainModule.startSizeMultiplier = startSizeMultiplier.Value;

        collisionModule.bounceMultiplier = 0;
        collisionModule.dampenMultiplier = 1;
        collisionModule.quality = (ParticleSystemCollisionQuality)collisionQuality.Value;
    }

    private static void SetEmissionModule(ParticleSystem ps, Setting<float> rateOverTimeMultiplierMin, Setting<float> rateOverTimeMultiplierMax) {
        var emissionModule = ps.emission;
        emissionModule.rateOverTimeMultiplier = Random.Range(
            rateOverTimeMultiplierMin.Value,
            rateOverTimeMultiplierMax.Value
        );
    }

    private static void SetEmissionModule(ParticleSystem ps, Setting<int> burstCount) {
        ps.emission.SetBursts(new[] { new ParticleSystem.Burst(0.0f, (short)burstCount.Value) });
    }
}
