using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ash.Core.Tooling.SceneManagement;
using Ash.Utility.GlobalUtils;
using Character;
using HarmonyLib;
using UnityEngine;
using Utility;

namespace Ash.Core.Features.AshPlugin.Features.BugFixes.Hooks._Female;

[HarmonyPatch]
internal class FemaleHooks
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int MainColor = Shader.PropertyToID("_MainColor");
    private static readonly int Metal = Shader.PropertyToID("_Metal");
    private static readonly int Roughness = Shader.PropertyToID("_Roughness");
    private static readonly int Specularity = Shader.PropertyToID("_Specularity");
    private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
    private static readonly int RimWeight = Shader.PropertyToID("_RimWeight");
    private static readonly int Parallax = Shader.PropertyToID("_Parallax");
    private static readonly int ParallaxMap = Shader.PropertyToID("_ParallaxMap");
    private static readonly int HasBumpMap = Shader.PropertyToID("_HasBumpMap");
    private static readonly int BumpMode = Shader.PropertyToID("_BumpMode");
    private static readonly int Rim = Shader.PropertyToID("_Rim");
    private static readonly int RimBias = Shader.PropertyToID("_RimBias");
    private static readonly int RimPower = Shader.PropertyToID("_RimPower");
    private static readonly int RimColor = Shader.PropertyToID("_RimColor");
    private static readonly int Mode = Shader.PropertyToID("_Mode");
    private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
    private static readonly int OffsetFactor = Shader.PropertyToID("_OffsetFactor");
    private static readonly int OffsetUnits = Shader.PropertyToID("_OffsetUnits");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");


    // this fixes incorrect original return result for Mariko
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Female), nameof(Female.GetVolume))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static bool GetVolumePrefix(Female __instance, ref float __result) {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        __result = __instance.heroineID switch {
            HEROINE.RITSUKO => ConfigData.VolumeVoice_Ritsuko(),
            HEROINE.AKIKO => ConfigData.VolumeVoice_Akiko(),
            HEROINE.YUKIKO => ConfigData.VolumeVoice_Yukiko(),
            HEROINE.MARIKO => ConfigData.VolumeVoice_Mariko(),
            _ => 0.0f,
        };

        // apply custom volume modifier
        __result *= SceneDataModifiers.GetFemaleVolumeModifier(__instance.heroineID);

        // return false to skip the original method
        return false;
    }

    // tear color fix
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Female), nameof(Female.Update_FlushAndTear), typeof(bool))]
    // ReSharper disable once InconsistentNaming
    internal static void UpdateFlushAndTearPrefix(Female __instance, bool force) {
        var bFlush = 0.5882353f;
        if (Math.Abs(__instance.nowFlushRate - (double)__instance.FlushRate) > double.Epsilon || force) {
            __instance.nowFlushRate = force
                ? __instance.FlushRate
                : Tween.LinearMove(__instance.nowFlushRate, __instance.FlushRate, Time.deltaTime);

            __instance.flushMat.color = new Color(
                __instance.flushMat.color.r,
                __instance.flushMat.color.g,
                __instance.flushMat.color.b,
                Mathf.Lerp(0.0f, bFlush, __instance.nowFlushRate)
            );
        }

        if (Math.Abs(__instance.nowTearsRate - (double)__instance.TearsRate) < double.Epsilon && !force)
            return;

        var bTear = Ash.PersistentSettings.TearsAlphaInterpolationMax.Value;
        __instance.nowTearsRate = force
            ? __instance.TearsRate
            : Tween.LinearMove(__instance.nowTearsRate, __instance.TearsRate, Time.deltaTime);

        foreach (var tear in __instance.tears) {
            var color = new Color(
                tear.material.color.r,
                tear.material.color.g,
                tear.material.color.b,
                Mathf.Lerp(0.0f, bTear, __instance.nowTearsRate)
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
        if (Ash.PersistentSettings.FixVirginBloodZFighting.Value)
            ReplaceVirginBloodMaterial(__instance);

        if (Ash.PersistentSettings.FixSpermZFighting.Value)
            ReplaceSpermMaterials(__instance);

        if (Ash.PersistentSettings.FixFlushZFighting.Value)
            ReplaceFlushMaterial(__instance);
    }


    private static void ConfigureAlloyCoreOverlayCutoutNmMaterial(Material oldMaterial, Material newMaterial) {
        if (oldMaterial == null || newMaterial == null)
            return;

        newMaterial.name = oldMaterial.name;
        newMaterial.renderQueue = 3000;

        newMaterial.DisableKeyword("_ALPHABLEND_ON");
        newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        newMaterial.EnableKeyword("_ALPHATEST_ON");

        newMaterial.EnableKeyword("EFFECT_BUMP");
        newMaterial.SetFloat(HasBumpMap, 1f);

        newMaterial.SetFloat(Mode, 1f);
        newMaterial.SetOverrideTag("RenderType", "TransparentCutout");
        newMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
        newMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.Zero);
        newMaterial.SetInt(ZWrite, 1);
        newMaterial.SetFloat(OffsetFactor, -1f);
        newMaterial.SetFloat(OffsetUnits, -1f);

        newMaterial.SetTexture(MainTex, oldMaterial.GetTexture(MainTex));
        newMaterial.SetTextureScale("_MainTex", oldMaterial.GetTextureScale("_MainTex"));
        newMaterial.SetTextureOffset("_MainTex", oldMaterial.GetTextureOffset("_MainTex"));
        newMaterial.SetTexture(BumpMap, oldMaterial.GetTexture(BumpMap));

        newMaterial.SetColor(Color1, new Color(0.48f, 0f, 0f, 1f));
        newMaterial.SetFloat(Cutoff, 0.68f);
        newMaterial.SetFloat(Roughness, 0.015f);
        newMaterial.SetFloat(Specularity, 1f);
    }

    private static void ConfigureAlloyCoreOverlayFadeNmMaterial(Material oldMaterial, Material newMaterial) {
        if (oldMaterial == null || newMaterial == null)
            return;

        newMaterial.name = oldMaterial.name;
        newMaterial.renderQueue = 3000;

        newMaterial.DisableKeyword("_ALPHATEST_ON");
        newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        newMaterial.EnableKeyword("_ALPHABLEND_ON");

        newMaterial.EnableKeyword("EFFECT_BUMP");
        newMaterial.SetFloat(HasBumpMap, 1f);
        newMaterial.SetTexture(BumpMap, oldMaterial.GetTexture(BumpMap));

        newMaterial.EnableKeyword("_PARALLAXMAP");
        newMaterial.SetFloat(BumpMode, 0f);
        newMaterial.SetFloat(Parallax, 0.005f);
        newMaterial.SetTexture(ParallaxMap, oldMaterial.GetTexture(ParallaxMap));

        newMaterial.EnableKeyword("_RIM_ON");
        newMaterial.SetFloat(Rim, 1f);
        newMaterial.SetFloat(RimBias, 0f);
        newMaterial.SetFloat(RimPower, 4f);
        newMaterial.SetFloat(RimWeight, 0f);
        newMaterial.SetColor(RimColor, Color.white);

        newMaterial.SetFloat(Mode, 2f);
        newMaterial.SetOverrideTag("RenderType", "Transparent");
        newMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        newMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        newMaterial.SetInt(ZWrite, 0);

        newMaterial.SetFloat(OffsetFactor, -1f);
        newMaterial.SetFloat(OffsetUnits, -1f);

        newMaterial.SetTexture(MainTex, oldMaterial.GetTexture(MainTex));
        newMaterial.SetTextureScale("_MainTex", oldMaterial.GetTextureScale("_MainTex"));
        newMaterial.SetTextureOffset("_MainTex", oldMaterial.GetTextureOffset("_MainTex"));

        newMaterial.SetColor(Color1, new Color(1, 1, 1, 0.45f));
        newMaterial.SetFloat(Metal, 0.2f);
        newMaterial.SetFloat(Roughness, 0.1f);
        newMaterial.SetFloat(Specularity, 1f);
    }

    private static void ConfigureAlloyCoreOverlayTransparentMaterial(Material oldMaterial, Material newMaterial, Color color) {
        if (oldMaterial == null || newMaterial == null)
            return;

        newMaterial.name = oldMaterial.name;
        newMaterial.renderQueue = 3000;

        newMaterial.DisableKeyword("_ALPHATEST_ON");
        newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        newMaterial.EnableKeyword("_ALPHABLEND_ON");

        newMaterial.SetFloat(Mode, 3f);
        newMaterial.SetOverrideTag("RenderType", "Transparent");
        newMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        newMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        newMaterial.SetInt(ZWrite, 0);

        newMaterial.SetFloat(OffsetFactor, -1f);
        newMaterial.SetFloat(OffsetUnits, -1f);

        newMaterial.SetTexture(MainTex, oldMaterial.GetTexture(MainTex));
        newMaterial.SetTextureScale("_MainTex", oldMaterial.GetTextureScale("_MainTex"));
        newMaterial.SetTextureOffset("_MainTex", oldMaterial.GetTextureOffset("_MainTex"));

        newMaterial.SetColor(Color1, color);
        newMaterial.SetFloat(Metal, 0f);
        newMaterial.SetFloat(Roughness, 0.9f);
        newMaterial.SetFloat(Specularity, 1f);
    }

    // sperms z-fighting fix
    private static Material OriginalSpermMatFaceLo;
    private static Material OriginalSpermMatFaceHi;
    private static Material OriginalSpermMatBustLo;
    private static Material OriginalSpermMatBustHi;
    private static Material OriginalSpermMatBackLo;
    private static Material OriginalSpermMatBackHi;
    private static Material OriginalSpermMatCrotchLo;
    private static Material OriginalSpermMatCrotchHi;
    private static Material OriginalSpermMatHipLo;
    private static Material OriginalSpermMatHipHi;

    private static Material ReplacementSpermMatFaceLo;
    private static Material ReplacementSpermMatFaceHi;
    private static Material ReplacementSpermMatBustLo;
    private static Material ReplacementSpermMatBustHi;
    private static Material ReplacementSpermMatBackLo;
    private static Material ReplacementSpermMatBackHi;
    private static Material ReplacementSpermMatCrotchLo;
    private static Material ReplacementSpermMatCrotchHi;
    private static Material ReplacementSpermMatHipLo;
    private static Material ReplacementSpermMatHipHi;

    internal static void ReplaceSpermMaterialsForAllFemales() {
        foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>())
            ReplaceSpermMaterials(female);
    }

    internal static void ResetSpermMaterialsForAllFemales() {
        foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>())
            ResetSpermMaterials(female);
    }

    private static void ReplaceSpermMaterials(Female female) {
        var materialFromCache = GlobalPluginData.MaterialCache.GetValueOrDefaultValue(
            GlobalPluginData.MaterialName.AlloyCoreOverlayFadeNmParallaxRim, null);

        if (materialFromCache == null) {
            Ash.Logger.LogWarning("AlloyCoreOverlayTransparentNm material is null.");
            return;
        }

        if (OriginalSpermMatFaceLo == null)
            OriginalSpermMatFaceLo = new Material(female.spermMat_face_lo);

        if (OriginalSpermMatFaceHi == null)
            OriginalSpermMatFaceHi = new Material(female.spermMat_face_hi);

        if (OriginalSpermMatBustLo == null)
            OriginalSpermMatBustLo = new Material(female.spermMat_bust_lo);

        if (OriginalSpermMatBustHi == null)
            OriginalSpermMatBustHi = new Material(female.spermMat_bust_hi);

        if (OriginalSpermMatBackLo == null)
            OriginalSpermMatBackLo = new Material(female.spermMat_back_lo);

        if (OriginalSpermMatBackHi == null)
            OriginalSpermMatBackHi = new Material(female.spermMat_back_hi);

        if (OriginalSpermMatCrotchLo == null)
            OriginalSpermMatCrotchLo = new Material(female.spermMat_crotch_lo);

        if (OriginalSpermMatCrotchHi == null)
            OriginalSpermMatCrotchHi = new Material(female.spermMat_crotch_hi);

        if (OriginalSpermMatHipLo == null)
            OriginalSpermMatHipLo = new Material(female.spermMat_hip_lo);

        if (OriginalSpermMatHipHi == null)
            OriginalSpermMatHipHi = new Material(female.spermMat_hip_hi);


        if (ReplacementSpermMatFaceLo == null) {
            var newSpermMatFaceLo = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_face_lo, newSpermMatFaceLo);
            ReplacementSpermMatFaceLo = newSpermMatFaceLo;
        }

        if (ReplacementSpermMatFaceHi == null) {
            var newSpermMatFaceHi = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_face_hi, newSpermMatFaceHi);
            ReplacementSpermMatFaceHi = newSpermMatFaceHi;
        }

        if (ReplacementSpermMatBustLo == null) {
            var newSpermMatBustLo = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_bust_lo, newSpermMatBustLo);
            ReplacementSpermMatBustLo = newSpermMatBustLo;
        }

        if (ReplacementSpermMatBustHi == null) {
            var newSpermMatBustHi = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_bust_hi, newSpermMatBustHi);
            ReplacementSpermMatBustHi = newSpermMatBustHi;
        }

        if (ReplacementSpermMatBackLo == null) {
            var newSpermMatBackLo = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_back_lo, newSpermMatBackLo);
            ReplacementSpermMatBackLo = newSpermMatBackLo;
        }

        if (ReplacementSpermMatBackHi == null) {
            var newSpermMatBackHi = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_back_hi, newSpermMatBackHi);
            ReplacementSpermMatBackHi = newSpermMatBackHi;
        }

        if (ReplacementSpermMatCrotchLo == null) {
            var newSpermMatCrotchLo = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_crotch_lo, newSpermMatCrotchLo);
            ReplacementSpermMatCrotchLo = newSpermMatCrotchLo;
        }

        if (ReplacementSpermMatCrotchHi == null) {
            var newSpermMatCrotchHi = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_crotch_hi, newSpermMatCrotchHi);
            ReplacementSpermMatCrotchHi = newSpermMatCrotchHi;
        }

        if (ReplacementSpermMatHipLo == null) {
            var newSpermMatHipLo = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_hip_lo, newSpermMatHipLo);
            ReplacementSpermMatHipLo = newSpermMatHipLo;
        }

        if (ReplacementSpermMatHipHi == null) {
            var newSpermMatHipHi = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayFadeNmMaterial(female.spermMat_hip_hi, newSpermMatHipHi);
            ReplacementSpermMatHipHi = newSpermMatHipHi;
        }

        female.spermMat_face_lo.shader = ReplacementSpermMatFaceLo.shader;
        female.spermMat_face_lo.CopyPropertiesFromMaterial(ReplacementSpermMatFaceLo);
        female.spermMat_face_hi.shader = ReplacementSpermMatFaceHi.shader;
        female.spermMat_face_hi.CopyPropertiesFromMaterial(ReplacementSpermMatFaceHi);

        female.spermMat_bust_lo.shader = ReplacementSpermMatBustLo.shader;
        female.spermMat_bust_lo.CopyPropertiesFromMaterial(ReplacementSpermMatBustLo);
        female.spermMat_bust_hi.shader = ReplacementSpermMatBustHi.shader;
        female.spermMat_bust_hi.CopyPropertiesFromMaterial(ReplacementSpermMatBustHi);

        female.spermMat_back_lo.shader = ReplacementSpermMatBackLo.shader;
        female.spermMat_back_lo.CopyPropertiesFromMaterial(ReplacementSpermMatBackLo);
        female.spermMat_back_hi.shader = ReplacementSpermMatBackHi.shader;
        female.spermMat_back_hi.CopyPropertiesFromMaterial(ReplacementSpermMatBackHi);

        female.spermMat_crotch_lo.shader = ReplacementSpermMatCrotchLo.shader;
        female.spermMat_crotch_lo.CopyPropertiesFromMaterial(ReplacementSpermMatCrotchLo);
        female.spermMat_crotch_hi.shader = ReplacementSpermMatCrotchHi.shader;
        female.spermMat_crotch_hi.CopyPropertiesFromMaterial(ReplacementSpermMatCrotchHi);

        female.spermMat_hip_lo.shader = ReplacementSpermMatHipLo.shader;
        female.spermMat_hip_lo.CopyPropertiesFromMaterial(ReplacementSpermMatHipLo);
        female.spermMat_hip_hi.shader = ReplacementSpermMatHipHi.shader;
        female.spermMat_hip_hi.CopyPropertiesFromMaterial(ReplacementSpermMatHipHi);

        female.head.rend_skin.sharedMaterials = female.head.rend_skin.sharedMaterials
            .Select(mat => {
                if (mat.name.StartsWith(female.spermMat_face_lo.name)) return female.spermMat_face_lo;
                if (mat.name.StartsWith(female.spermMat_face_hi.name)) return female.spermMat_face_hi;

                return mat;
            })
            .ToArray();

        female.body.rend_skin.sharedMaterials = female.body.rend_skin.sharedMaterials
            .Select(mat => {
                if (mat.name.StartsWith(female.spermMat_bust_lo.name)) return female.spermMat_bust_lo;
                if (mat.name.StartsWith(female.spermMat_bust_hi.name)) return female.spermMat_bust_hi;
                if (mat.name.StartsWith(female.spermMat_back_lo.name)) return female.spermMat_back_lo;
                if (mat.name.StartsWith(female.spermMat_back_hi.name)) return female.spermMat_back_hi;
                if (mat.name.StartsWith(female.spermMat_crotch_lo.name)) return female.spermMat_crotch_lo;
                if (mat.name.StartsWith(female.spermMat_crotch_hi.name)) return female.spermMat_crotch_hi;
                if (mat.name.StartsWith(female.spermMat_hip_lo.name)) return female.spermMat_hip_lo;
                if (mat.name.StartsWith(female.spermMat_hip_hi.name)) return female.spermMat_hip_hi;

                return mat;
            })
            .ToArray();

        female.wears.ChangeBodyMaterial(female.body.rend_skin);
    }

    private static void ResetSpermMaterials(Female female) {
        if (OriginalSpermMatFaceLo != null) {
            female.spermMat_face_lo.shader = OriginalSpermMatFaceLo.shader;
            female.spermMat_face_lo.CopyPropertiesFromMaterial(OriginalSpermMatFaceLo);
        }

        if (OriginalSpermMatFaceHi != null) {
            female.spermMat_face_hi.shader = OriginalSpermMatFaceHi.shader;
            female.spermMat_face_hi.CopyPropertiesFromMaterial(OriginalSpermMatFaceHi);
        }

        if (OriginalSpermMatBustLo != null) {
            female.spermMat_bust_lo.shader = OriginalSpermMatBustLo.shader;
            female.spermMat_bust_lo.CopyPropertiesFromMaterial(OriginalSpermMatBustLo);
        }

        if (OriginalSpermMatBustHi != null) {
            female.spermMat_bust_hi.shader = OriginalSpermMatBustHi.shader;
            female.spermMat_bust_hi.CopyPropertiesFromMaterial(OriginalSpermMatBustHi);
        }

        if (OriginalSpermMatBackLo != null) {
            female.spermMat_back_lo.shader = OriginalSpermMatBackLo.shader;
            female.spermMat_back_lo.CopyPropertiesFromMaterial(OriginalSpermMatBackLo);
        }

        if (OriginalSpermMatBackHi != null) {
            female.spermMat_back_hi.shader = OriginalSpermMatBackHi.shader;
            female.spermMat_back_hi.CopyPropertiesFromMaterial(OriginalSpermMatBackHi);
        }

        if (OriginalSpermMatCrotchLo != null) {
            female.spermMat_crotch_lo.shader = OriginalSpermMatCrotchLo.shader;
            female.spermMat_crotch_lo.CopyPropertiesFromMaterial(OriginalSpermMatCrotchLo);
        }

        if (OriginalSpermMatCrotchHi != null) {
            female.spermMat_crotch_hi.shader = OriginalSpermMatCrotchHi.shader;
            female.spermMat_crotch_hi.CopyPropertiesFromMaterial(OriginalSpermMatCrotchHi);
        }

        if (OriginalSpermMatHipLo != null) {
            female.spermMat_hip_lo.shader = OriginalSpermMatHipLo.shader;
            female.spermMat_hip_lo.CopyPropertiesFromMaterial(OriginalSpermMatHipLo);
        }

        if (OriginalSpermMatHipHi != null) {
            female.spermMat_hip_hi.shader = OriginalSpermMatHipHi.shader;
            female.spermMat_hip_hi.CopyPropertiesFromMaterial(OriginalSpermMatHipHi);
        }


        female.head.rend_skin.sharedMaterials = female.head.rend_skin.sharedMaterials
            .Select(mat => {
                if (mat.name.StartsWith(female.spermMat_face_lo.name)) return female.spermMat_face_lo;
                if (mat.name.StartsWith(female.spermMat_face_hi.name)) return female.spermMat_face_hi;

                return mat;
            })
            .ToArray();

        female.body.rend_skin.sharedMaterials = female.body.rend_skin.sharedMaterials
            .Select(mat => {
                if (mat.name.StartsWith(female.spermMat_bust_lo.name)) return female.spermMat_bust_lo;
                if (mat.name.StartsWith(female.spermMat_bust_hi.name)) return female.spermMat_bust_hi;
                if (mat.name.StartsWith(female.spermMat_back_lo.name)) return female.spermMat_back_lo;
                if (mat.name.StartsWith(female.spermMat_back_hi.name)) return female.spermMat_back_hi;
                if (mat.name.StartsWith(female.spermMat_crotch_lo.name)) return female.spermMat_crotch_lo;
                if (mat.name.StartsWith(female.spermMat_crotch_hi.name)) return female.spermMat_crotch_hi;
                if (mat.name.StartsWith(female.spermMat_hip_lo.name)) return female.spermMat_hip_lo;
                if (mat.name.StartsWith(female.spermMat_hip_hi.name)) return female.spermMat_hip_hi;

                return mat;
            })
            .ToArray();

        female.head.SetupFaceMaterialAndTexture();
        female.body.SetupBodyMaterialAndTexture();

        female.wears.ChangeBodyMaterial(female.body.rend_skin);
    }


    // virgin blood z-fighting fix
    private static Material OriginalBloodMaterial;
    private static Material ReplacementBloodMaterial;
    private static Color OriginalBloodColor;

    internal static void ReplaceVirginBloodMaterialForAllFemales() {
        foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>())
            ReplaceVirginBloodMaterial(female);
    }

    internal static void ResetVirginBloodMaterialForAllFemales() {
        foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>())
            ResetVirginBloodMaterial(female);
    }

    private static void ReplaceVirginBloodMaterial(Female female) {
        var materialFromCache = GlobalPluginData.MaterialCache.GetValueOrDefaultValue(
            GlobalPluginData.MaterialName.AlloyCoreOverlayCutoutNm, null);

        if (materialFromCache == null) {
            Ash.Logger.LogWarning("AlloyCoreOverlayCutoutNm material is null.");
            return;
        }

        if (OriginalBloodMaterial == null) {
            OriginalBloodMaterial = new Material(female.virginBlood);

            OriginalBloodColor = female.virginBlood.HasProperty(MainColor)
                ? female.virginBlood.GetColor(MainColor)
                : female.virginBlood.HasProperty(Color1)
                    ? female.virginBlood.GetColor(Color1)
                    : Color.white;
        }

        if (ReplacementBloodMaterial == null) {
            var newMaterial = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayCutoutNmMaterial(female.virginBlood, newMaterial);
            ReplacementBloodMaterial = newMaterial;
        }

        female.virginBlood.shader = ReplacementBloodMaterial.shader;
        female.virginBlood.CopyPropertiesFromMaterial(ReplacementBloodMaterial);

        female.body.rend_skin.sharedMaterials = female.body.rend_skin.sharedMaterials
            .Select(mat => mat.name.StartsWith(female.virginBlood.name) ? female.virginBlood : mat)
            .ToArray();

        female.wears.ChangeBodyMaterial(female.body.rend_skin);
    }

    private static void ResetVirginBloodMaterial(Female female) {
        if (OriginalBloodMaterial == null)
            return;

        female.virginBlood.shader = OriginalBloodMaterial.shader;
        female.virginBlood.CopyPropertiesFromMaterial(OriginalBloodMaterial);

        female.body.rend_skin.sharedMaterials = female.body.rend_skin.sharedMaterials
            .Select(mat => mat.name.StartsWith(OriginalBloodMaterial.name) ? female.virginBlood : mat)
            .ToArray();

        female.wears.ChangeBodyMaterial(female.body.rend_skin);
    }


    // flush z-fighting fix
    private static Material OriginalFlushMaterial;
    private static Material ReplacementFlushMaterial;
    private static Color OriginalFlushColor;

    internal static void ReplaceFlushMaterialForAllFemales() {
        foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>())
            ReplaceFlushMaterial(female);
    }

    internal static void ResetFlushMaterialForAllFemales() {
        foreach (var female in SceneComponentRegistry.GetComponentsOfType<Female>())
            ResetFlushMaterial(female);
    }

    private static void ReplaceFlushMaterial(Female female) {
        var materialFromCache = GlobalPluginData.MaterialCache.GetValueOrDefaultValue(
            GlobalPluginData.MaterialName.AlloyCoreOverlayTransparent, null);

        if (materialFromCache == null) {
            Ash.Logger.LogWarning("AlloyCoreOverlayTransparent material is null.");
            return;
        }

        if (OriginalFlushMaterial == null) {
            OriginalFlushMaterial = new Material(female.flushMat);

            OriginalFlushColor = female.flushMat.HasProperty(MainColor)
                ? female.flushMat.GetColor(MainColor)
                : female.flushMat.HasProperty(Color1)
                    ? female.flushMat.GetColor(Color1)
                    : Color.white;
        }

        if (ReplacementFlushMaterial == null) {
            var newMaterial = new Material(materialFromCache);
            ConfigureAlloyCoreOverlayTransparentMaterial(female.flushMat, newMaterial, OriginalFlushColor);
            ReplacementFlushMaterial = newMaterial;
        }

        female.flushMat.shader = ReplacementFlushMaterial.shader;
        female.flushMat.CopyPropertiesFromMaterial(ReplacementFlushMaterial);

        female.head.rend_skin.sharedMaterials = female.head.rend_skin.sharedMaterials
            .Select(mat => mat.name.StartsWith(OriginalFlushMaterial.name) ? female.flushMat : mat)
            .ToArray();
    }

    private static void ResetFlushMaterial(Female female) {
        if (OriginalFlushMaterial == null)
            return;

        female.flushMat.shader = OriginalFlushMaterial.shader;
        female.flushMat.CopyPropertiesFromMaterial(OriginalFlushMaterial);

        female.head.rend_skin.sharedMaterials = female.head.rend_skin.sharedMaterials
            .Select(mat => mat.name.StartsWith(OriginalFlushMaterial.name) ? female.flushMat : mat)
            .ToArray();
    }
}
