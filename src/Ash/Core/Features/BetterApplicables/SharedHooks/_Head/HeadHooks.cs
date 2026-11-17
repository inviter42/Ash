using System;
using System.Collections.Generic;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedUtils;
using Ash.Utility.GlobalUtils;
using Character;
using HarmonyLib;
using KKAPI.Maker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.BetterApplicables.SharedHooks._Head;

[HarmonyPatch]
internal class HeadHooks
{
    private static readonly int BaseTex = Shader.PropertyToID("_BaseTex");
    private static readonly int OffsetH = Shader.PropertyToID("_OffsetH");
    private static readonly int OffsetS = Shader.PropertyToID("_OffsetS");
    private static readonly int OffsetV = Shader.PropertyToID("_OffsetV");
    private static readonly int CheekTex = Shader.PropertyToID("_CheekTex");
    private static readonly int CheekColor = Shader.PropertyToID("_CheekColor");
    private static readonly int EyeShadowTex = Shader.PropertyToID("_EyeShadowTex");
    private static readonly int EyeShadowColor = Shader.PropertyToID("_EyeShadowColor");
    private static readonly int LipTex = Shader.PropertyToID("_LipTex");
    private static readonly int LipColor = Shader.PropertyToID("_LipColor");
    private static readonly int MoleTex = Shader.PropertyToID("_MoleTex");
    private static readonly int MoleColor = Shader.PropertyToID("_MoleColor");
    private static readonly int TattooTex = Shader.PropertyToID("_TattooTex");
    private static readonly int TattooColor = Shader.PropertyToID("_TattooColor");

    private static readonly Dictionary<int, Texture> SkinTextureDict = new();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Head), nameof(Head.RendSkinTexture))]
    // ReSharper disable once InconsistentNaming
    internal static bool RendSkinTexturePrefix(Head __instance) {
        // restore skinBaseTex if added sperm mats changed it
        if (__instance.human.sex == SEX.MALE)
            return true;

        var head = __instance.human.customParam.head;
        if (!SkinTextureDict.ContainsKey(head.faceTexID)) {
            var prefabData = CustomDataManager.GetFaceSkin_Female(head.faceTexID);
            var bundleController = new AssetBundleController();
            bundleController.OpenFromFile(prefabData.assetbundleDir, prefabData.assetbundleName);
            SkinTextureDict[head.faceTexID] = bundleController.LoadAsset<Material>(prefabData.prefab).mainTexture;
            bundleController.Close();
            Resources.UnloadUnusedAssets();
        }

        __instance.skinBaseTex = SkinTextureDict.GetValueOrDefaultValue(head.faceTexID, null);

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Head), nameof(Head.Load), typeof(HeadParameter))]
    // ReSharper disable once InconsistentNaming
    internal static void HeadLoadPostfix(Head __instance, HeadParameter param) {
        // restore sperm mats, because head is destroyed at Head.Load
        if (__instance.human is not Female female)
            return;

        var lv = female.sperms[0];
        if (lv == 0) {
            female.DeleteSpermMaterials(female.head.Rend_skin, new[] {
                female.spermMat_face_lo.name, female.spermMat_face_hi.name
            });
            return;
        }

        Female.ChangeSpermMaterial(female.head.Rend_skin, lv, female.spermMat_face_lo, female.spermMat_face_hi);
        female.head.SetupFaceMaterialAndTexture();
        foreach (var componentsInChild in female.head.Obj.GetComponentsInChildren<Renderer>(true)) {
            var idx = Array.FindIndex(componentsInChild.sharedMaterials, mat => mat.name.StartsWith("cf_M_f_hohoaka"));
            if (idx < 0) continue;

            female.flushMat = componentsInChild.sharedMaterials[idx];
        }
        female.SetFlush(female.FlushRate, true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Head), nameof(Head.RendSkinTexture_Female))]
    // ReSharper disable once InconsistentNaming
    internal static bool RendSkinTexture_FemalePrefix(Head __instance) {
        var head = __instance.human.customParam.head;
        var faceTattooFemale = CustomDataManager.GetFaceTattoo_Female(head.tattooID);
        var eyeShadow = CustomDataManager.GetEyeShadow(head.eyeshadowTexID);
        var cheek = CustomDataManager.GetCheek(head.cheekTexID);
        var lip = CustomDataManager.GetLip(head.lipTexID);
        var mole = CustomDataManager.GetMole(head.moleTexID);
        var lipColor = head.lipColor;
        var mat = new Material(CustomDataManager.skinBlendShader_Face);
        var sRgbWrite = GL.sRGBWrite;

        GL.sRGBWrite = true;
        Graphics.SetRenderTarget(__instance.skinTex);
        GL.Clear(false, true, Color.black);
        Graphics.SetRenderTarget(null);

        if (__instance.lipTex == null)
            lipColor.a = 0.0f;

        mat.SetTexture(BaseTex, __instance.skinBaseTex);
        mat.SetFloat(OffsetH, __instance.human.customParam.body.skinColor.offset_h);
        mat.SetFloat(OffsetS, __instance.human.customParam.body.skinColor.offset_s);
        mat.SetFloat(OffsetV, __instance.human.customParam.body.skinColor.offset_v);

        ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
            MakerAPI.InsideMaker
                ? Ash.BetterCheekShadowMakerExtensionHead.UnsavedChanges
                : Ash.BetterCheekShadowDataManager.GetSerializableTextureDataList(__instance.human, Part.Head),
            __instance,
            Part.Head,
            BetterCategory.CheekShadow,
            Ash.BetterCheekShadowMakerExtensionHead,
            mat,
            CheekTex,
            CheekColor,
            "_CheekTex"
        );

        ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
            MakerAPI.InsideMaker
                ? Ash.BetterEyeShadowMakerExtensionHead.UnsavedChanges
                : Ash.BetterEyeShadowDataManager.GetSerializableTextureDataList(__instance.human, Part.Head),
            __instance,
            Part.Head,
            BetterCategory.EyeShadow,
            Ash.BetterEyeShadowMakerExtensionHead,
            mat,
            EyeShadowTex,
            EyeShadowColor,
            "_EyeShadowTex"
        );

        mat.SetTexture(LipTex, __instance.lipTex);
        if (__instance.lipTex != null)
            __instance.SetTattooOffsetAndTiling(mat, "_LipTex", 1024, 1024, __instance.lipTex.width, __instance.lipTex.height, lip.pos.x, lip.pos.y);
        mat.SetColor(LipColor, lipColor);

        ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
            MakerAPI.InsideMaker
                ? Ash.BetterMoleMakerExtensionHead.UnsavedChanges
                : Ash.BetterMoleDataManager.GetSerializableTextureDataList(__instance.human, Part.Head),
            __instance,
            Part.Head,
            BetterCategory.Mole,
            Ash.BetterMoleMakerExtensionHead,
            mat,
            MoleTex,
            MoleColor,
            "_MoleTex"
        );

        ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
            MakerAPI.InsideMaker
                ? Ash.BetterTattooMakerExtensionHead.UnsavedChanges
                : Ash.BetterTattooDataManager.GetSerializableTextureDataList(__instance.human, Part.Head),
            __instance,
            Part.Head,
            BetterCategory.Tattoo,
            Ash.BetterTattooMakerExtensionHead,
            mat,
            TattooTex,
            TattooColor,
            "_TattooTex"
        );

        Graphics.Blit(__instance.skinBaseTex, __instance.skinTex, mat, 0);

        GL.sRGBWrite = sRgbWrite;

        __instance.skinMaterial.mainTexture = __instance.skinTex;

        if (faceTattooFemale != null)
            faceTattooFemale.isNew = false;

        if (eyeShadow != null)
            eyeShadow.isNew = false;

        if (cheek != null)
            cheek.isNew = false;

        if (lip != null)
            lip.isNew = false;

        if (mole != null)
            mole.isNew = false;

        Object.Destroy(mat);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Head), nameof(Head.RendSkinTexture_Male))]
    // ReSharper disable once InconsistentNaming
    internal static bool RendSkinTexture_MalePrefix(Head __instance) {
        var head = __instance.human.customParam.head;
        var faceTattooMale = CustomDataManager.GetFaceTattoo_Male(head.tattooID);
        var mat = new Material(CustomDataManager.skinBlendShader_Male);
        var sRgbWrite = GL.sRGBWrite;

        GL.sRGBWrite = true;
        Graphics.SetRenderTarget(__instance.skinTex);
        GL.Clear(false, true, Color.black);
        Graphics.SetRenderTarget(null);

        mat.SetTexture(BaseTex, __instance.skinBaseTex);
        mat.SetFloat(OffsetH, __instance.human.customParam.body.skinColor.offset_h);
        mat.SetFloat(OffsetS, __instance.human.customParam.body.skinColor.offset_s);
        mat.SetFloat(OffsetV, __instance.human.customParam.body.skinColor.offset_v);
        mat.SetTexture(TattooTex, __instance.tattooTex);

        ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
            MakerAPI.InsideMaker
                ? Ash.BetterTattooMakerExtensionHead.UnsavedChanges
                : Ash.BetterTattooDataManager.GetSerializableTextureDataList(__instance.human, Part.Head),
            __instance,
            Part.Head,
            BetterCategory.Tattoo,
            Ash.BetterTattooMakerExtensionHead,
            mat,
            TattooTex,
            TattooColor,
            "_TattooTex"
        );

        Graphics.Blit(__instance.skinBaseTex, __instance.skinTex, mat, 0);

        GL.sRGBWrite = sRgbWrite;

        __instance.skinMaterial.mainTexture = __instance.skinTex;

        if (faceTattooMale != null)
            faceTattooMale.isNew = false;

        Object.Destroy(mat);

        return false;
    }
}
