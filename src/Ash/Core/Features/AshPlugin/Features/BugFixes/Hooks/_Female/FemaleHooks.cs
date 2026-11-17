using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ash.Core.Tooling.SceneManagement;
using Ash.Utility.GlobalUtils;
using Character;
using HarmonyLib;
using UnityEngine;
using Utility;

namespace Ash.Core.Features.AshPlugin.Features.BugFixes.Hooks._Female
{
    [HarmonyPatch]
    internal class FemaleHooks
    {
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int MainColor = Shader.PropertyToID("_MainColor");
        private static readonly int SpecularColor = Shader.PropertyToID("_SpecularColor");
        private static readonly int BumpScale = Shader.PropertyToID("_BumpScale");
        private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
        private static readonly int Fresnel = Shader.PropertyToID("_Fresnel");
        private static readonly int Glossiness = Shader.PropertyToID("_Glossiness");


        // this fixes incorrect original return result for Mariko
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Female), nameof(Female.GetVolume))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static bool GetVolumePrefix(Female __instance, ref float __result) {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (__instance.heroineID) {
                case HEROINE.RITSUKO:
                    __result = ConfigData.VolumeVoice_Ritsuko();
                    break;
                case HEROINE.AKIKO:
                    __result = ConfigData.VolumeVoice_Akiko();
                    break;
                case HEROINE.YUKIKO:
                    __result = ConfigData.VolumeVoice_Yukiko();
                    break;
                case HEROINE.MARIKO:
                    __result = ConfigData.VolumeVoice_Mariko();
                    break;
                default:
                    __result = 0.0f;
                    break;
            }

            // apply custom volume modifier
            __result *= SceneDataModifiers.GetFemaleVolumeModifier(__instance.heroineID);

            // return false to skip the original method
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Female), nameof(Female.Update_FlushAndTear), typeof(bool))]
        // ReSharper disable once InconsistentNaming
        internal static void UpdateFlushAndTearPrefix(Female __instance, bool force) {
            var b = Ash.PersistentSettings.TearsAlphaInterpolationMax.Value;

            if (Math.Abs(__instance.nowFlushRate - (double)__instance.FlushRate) > double.Epsilon || force) {
                __instance.nowFlushRate = force
                    ? __instance.FlushRate
                    : Tween.LinearMove(__instance.nowFlushRate, __instance.FlushRate, Time.deltaTime);

                __instance.flushMat.color = new Color(
                    __instance.flushMat.color.r,
                    __instance.flushMat.color.g,
                    __instance.flushMat.color.b,
                    Mathf.Lerp(0.0f, b, __instance.nowFlushRate)
                );
            }

            if (Math.Abs(__instance.nowTearsRate - (double)__instance.TearsRate) < double.Epsilon && !force)
                return;

            __instance.nowTearsRate = force
                ? __instance.TearsRate
                : Tween.LinearMove(__instance.nowTearsRate, __instance.TearsRate, Time.deltaTime);

            foreach (var tear in __instance.tears) {
                var color = new Color(
                    tear.material.color.r,
                    tear.material.color.g,
                    tear.material.color.b,
                    Mathf.Lerp(0.0f, b, __instance.nowTearsRate)
                );

                tear.material.color = color;

                if (Ash.PersistentSettings.FixTearsShaderColorBinding.Value)
                    tear.material.SetColor(MainColor, color);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Female), nameof(Female.Apply))]
        // ReSharper disable once InconsistentNaming
        internal static void ApplyPostfix(Female __instance) {
            if (!Ash.PersistentSettings.FixVirginBloodZFighting.Value)
                return;

            ReplaceVirginBloodShader();
        }


        private static Material OriginalBloodMaterial;
        private static Material ReplacementBloodMaterial;
        private static Color OriginalBloodColor;


        internal static void ReplaceVirginBloodShader() {
            var shader = GlobalPluginData.ShaderCache.GetValueOrDefaultValue(
                GlobalPluginData.ShaderName.BpExtendedTessellationOffsetCutout, null);

            if (shader == null) {
                Ash.Logger.LogWarning($"Virgin blood shader is null.");
                return;
            }

            foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>()) {
                if (OriginalBloodMaterial == null) {
                    OriginalBloodMaterial = new Material(female.virginBlood);
                    if (female.virginBlood.HasProperty(MainColor)) { }

                    OriginalBloodColor = female.virginBlood.HasProperty(MainColor)
                        ? female.virginBlood.GetColor(MainColor)
                        : female.virginBlood.HasProperty(Color1)
                            ? female.virginBlood.GetColor(Color1)
                            : Color.white;
                }

                if (ReplacementBloodMaterial == null) {
                    SetShaderAndProperties(female.virginBlood, shader);
                    ReplacementBloodMaterial = new Material(female.virginBlood);
                }
                else {
                    female.virginBlood = ReplacementBloodMaterial;
                }

                female.body.rend_skin.sharedMaterials = female.body.rend_skin.sharedMaterials
                    .Select(mat => mat.name.StartsWith(female.virginBlood.name) ? ReplacementBloodMaterial : mat)
                    .ToArray();

                female.wears.ChangeBodyMaterial(female.body.rend_skin);
            }
        }

        internal static void ResetVirginBloodShader() {
            foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>()) {
                if (OriginalBloodMaterial == null)
                    return;

                female.virginBlood = OriginalBloodMaterial;
                female.body.rend_skin.sharedMaterials = female.body.rend_skin.sharedMaterials
                    .Select(mat => mat.name.StartsWith(female.virginBlood.name) ? OriginalBloodMaterial : mat)
                    .ToArray();

                female.wears.ChangeBodyMaterial(female.body.rend_skin);
            }
        }

        private static void SetShaderAndProperties(Material material, Shader shader) {
            if (material == null || shader == null)
                return;

            material.shader = shader;
            material.renderQueue = 3000;
            material.SetColor(Color1, OriginalBloodColor);
            material.SetColor(SpecularColor, new Color(0.55f, 0.55f, 0.55f, 1));
            material.SetFloat(BumpScale, 1f);
            material.SetFloat(Cutoff, 0.55f);
            material.SetFloat(Fresnel, 1.0f);
            material.SetFloat(Glossiness, 1.0f);
        }
    }
}
