using System.Linq;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared;
using BetterApplicablesStudio.Core.BetterApplicables.SharedComponents;
using BetterApplicablesStudio.Core.BetterApplicables.SharedUtils;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterApplicablesStudio.Core.BetterApplicables.SharedHooks._Head
{
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
                BetterApplicablesStudio.BetterCheekShadowDataManager.GetSerializableTextureDataList(Part.Head),
                __instance,
                Part.Head,
                BetterCategory.CheekShadow,
                mat,
                CheekTex,
                CheekColor,
                "_CheekTex"
            );

            ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
                BetterApplicablesStudio.BetterEyeShadowDataManager.GetSerializableTextureDataList(Part.Head),
                __instance,
                Part.Head,
                BetterCategory.EyeShadow,
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
                BetterApplicablesStudio.BetterMoleDataManager.GetSerializableTextureDataList(Part.Head),
                __instance,
                Part.Head,
                BetterCategory.Mole,
                mat,
                MoleTex,
                MoleColor,
                "_MoleTex"
            );

            ApplicablesTextureUtils.ConfigureMaterialSlotForMultiTexture(
                BetterApplicablesStudio.BetterTattooDataManager.GetSerializableTextureDataList(Part.Head),
                __instance,
                Part.Head,
                BetterCategory.Tattoo,
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
                BetterApplicablesStudio.BetterTattooDataManager.GetSerializableTextureDataList(Part.Head),
                __instance,
                Part.Head,
                BetterCategory.Tattoo,
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
}
