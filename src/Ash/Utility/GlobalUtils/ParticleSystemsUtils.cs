using UnityEngine;

namespace Ash.Utility.GlobalUtils
{
    internal static class ParticleSystemsUtils
    {
        internal static void AdjustFemaleSpermDripParticleSystemSettings(ParticleSystem ps) {
            var mainModule = ps.main;
            var collisionModule = ps.collision;
            var emissionModule = ps.emission;

            // var psRenderer = ps.GetComponent<ParticleSystemRenderer>();
            // var mat = psRenderer.material;

            // Ash.Logger.LogDebug($"render mode {psRenderer.renderMode}");
            // Ash.Logger.LogDebug($"material {mat.name}");
            // if (mat.shader) Ash.Logger.LogDebug($"shader {mat.shader.name}");
            // if (mat.HasProperty("_MainTex")) {
            //     var mainTex = mat.GetTexture("_MainTex");
            //     Ash.Logger.LogDebug($"mainTex {mainTex.name}");
            // }

            mainModule.startDelay = new ParticleSystem.MinMaxCurve(
                Ash.PersistentSettings.ParticleSystemStartDelayMinValue.Value,
                Ash.PersistentSettings.ParticleSystemStartDelayMaxValue.Value
            );

            mainModule.gravityModifier = Ash.PersistentSettings.ParticleSystemGravityModifierValue.Value;
            mainModule.startSizeMultiplier = Ash.PersistentSettings.ParticleSystemStartSizeMultiplierValue.Value;

            collisionModule.bounceMultiplier = 0;
            collisionModule.dampenMultiplier = 1;
            collisionModule.quality = (ParticleSystemCollisionQuality)Ash.PersistentSettings.ParticleSystemCollisionQuality.Value;

            emissionModule.rateOverTimeMultiplier = Random.Range(
                Ash.PersistentSettings.ParticleSystemRateOverTimeMultiplierMinValue.Value,
                Ash.PersistentSettings.ParticleSystemRateOverTimeMultiplierMaxValue.Value
            );
        }
    }
}
