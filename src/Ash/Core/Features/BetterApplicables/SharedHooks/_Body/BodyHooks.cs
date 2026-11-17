using System.Collections.Generic;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedUtils;
using Ash.Utility.GlobalUtils;
using Character;
using HarmonyLib;
using KKAPI.Maker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.BetterApplicables.SharedHooks._Body;

[HarmonyPatch]
internal class BodyHooks
{

    private static readonly int BaseTex = Shader.PropertyToID("_BaseTex");
    private static readonly int OffsetH = Shader.PropertyToID("_OffsetH");
    private static readonly int OffsetS = Shader.PropertyToID("_OffsetS");
    private static readonly int OffsetV = Shader.PropertyToID("_OffsetV");
    private static readonly int SunburnTex = Shader.PropertyToID("_SunburnTex");
    private static readonly int SunburnH = Shader.PropertyToID("_SunburnH");
    private static readonly int SunburnS = Shader.PropertyToID("_SunburnS");
    private static readonly int SunburnV = Shader.PropertyToID("_SunburnV");
    private static readonly int SunburnA = Shader.PropertyToID("_SunburnA");
    private static readonly int TattooTex = Shader.PropertyToID("_TattooTex");
    private static readonly int TattooColor = Shader.PropertyToID("_TattooColor");

    private static readonly Dictionary<int, Texture> SkinTextureDict = new();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Body), nameof(Body.RendSkinTexture))]
    // ReSharper disable once InconsistentNaming
    internal static bool RendSkinTexturePrefix(Body __instance) {
        // restore skinBaseTex if added sperm mats changed it
        if (__instance.human.sex == SEX.MALE)
            return true;

        var body = __instance.human.customParam.body;
        if (!SkinTextureDict.ContainsKey(body.bodyID)) {
            var prefabData = CustomDataManager.GetBodySkin_Female(body.bodyID);
            var bundleController = new AssetBundleController();
            bundleController.OpenFromFile(prefabData.assetbundleDir, prefabData.assetbundleName);
            SkinTextureDict[body.bodyID] = bundleController.LoadAsset<Material>(prefabData.prefab).mainTexture;
            bundleController.Close();
            Resources.UnloadUnusedAssets();
        }

        __instance.skinBaseTex = SkinTextureDict.GetValueOrDefaultValue(body.bodyID, null);

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Body), nameof(Body.RendSkinTexture_Female))]
    // ReSharper disable once InconsistentNaming
    internal static bool RendSkinTexture_FemalePrefix(Body __instance) {
        var body = __instance.human.customParam.body;
        var num = body.sunburnColor_A;
        var mat = new Material(CustomDataManager.skinBlendShader_Body);
        var sRgbWrite = GL.sRGBWrite;

        GL.sRGBWrite = true;
        Graphics.SetRenderTarget(__instance.skinTex);
        GL.Clear(false, true, Color.white);
        Graphics.SetRenderTarget(null);

        if (__instance.sunburnTex == null)
            num = 0.0f;

        mat.SetTexture(BaseTex, __instance.skinBaseTex);
        mat.SetFloat(OffsetH, body.skinColor.offset_h);
        mat.SetFloat(OffsetS, body.skinColor.offset_s);
        mat.SetFloat(OffsetV, body.skinColor.offset_v);
        mat.SetTexture(SunburnTex, __instance.sunburnTex);
        mat.SetFloat(SunburnH, body.sunburnColor_H);
        mat.SetFloat(SunburnS, body.sunburnColor_S);
        mat.SetFloat(SunburnV, body.sunburnColor_V);
        mat.SetFloat(SunburnA, num);

        ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
            MakerAPI.InsideMaker
                ? Ash.BetterTattooMakerExtensionBody.UnsavedChanges
                : Ash.BetterTattooDataManager.GetSerializableTextureDataList(__instance.human, Part.Body),
            __instance,
            Part.Body,
            BetterCategory.Tattoo,
            Ash.BetterTattooMakerExtensionBody,
            mat,
            TattooTex,
            TattooColor,
            "_TattooTex"
        );

        Graphics.Blit(__instance.skinBaseTex, __instance.skinTex, mat, 0);

        GL.sRGBWrite = sRgbWrite;

        __instance.skinMaterial.mainTexture = __instance.skinTex;
        __instance.ChangeBumpRate();

        Object.Destroy(mat);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Body), nameof(Body.RendSkinTexture_Male))]
    // ReSharper disable once InconsistentNaming
    internal static bool RendSkinTexture_MalePrefix(Body __instance) {
        var body = __instance.human.customParam.body;
        var mat = new Material(CustomDataManager.skinBlendShader_Male);
        var sRgbWrite = GL.sRGBWrite;

        GL.sRGBWrite = true;
        Graphics.SetRenderTarget(__instance.skinTex);
        GL.Clear(false, true, Color.white);
        Graphics.SetRenderTarget(null);

        mat.SetTexture(BaseTex, __instance.skinBaseTex);
        mat.SetFloat(OffsetH, body.skinColor.offset_h);
        mat.SetFloat(OffsetS, body.skinColor.offset_s);
        mat.SetFloat(OffsetV, body.skinColor.offset_v);

        ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
            MakerAPI.InsideMaker
                ? Ash.BetterTattooMakerExtensionBody.UnsavedChanges
                : Ash.BetterTattooDataManager.GetSerializableTextureDataList(__instance.human, Part.Body),
            __instance,
            Part.Body,
            BetterCategory.Tattoo,
            Ash.BetterTattooMakerExtensionBody,
            mat,
            TattooTex,
            TattooColor,
            "_TattooTex"
        );

        Graphics.Blit(__instance.skinMaterial.mainTexture, __instance.skinTex, mat, 0);

        GL.sRGBWrite = sRgbWrite;

        __instance.skinMaterial.mainTexture = __instance.skinTex;
        __instance.ChangeBumpRate();

        Object.Destroy(mat);

        return false;
    }
}
