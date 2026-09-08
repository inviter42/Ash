using System.Linq;
using Ash.Core.Features.BetterTattoos.MakerExtensions;
using Ash.Core.Features.BetterTattoos.Utils;
using Ash.Logging;
using HarmonyLib;
using KKAPI.Maker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.BetterTattoos.Hooks._Head
{
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
            var eyeshadowColor = head.eyeshadowColor;
            var cheekColor = head.cheekColor;
            var lipColor = head.lipColor;
            var moleColor = head.moleColor;
            var mat = new Material(CustomDataManager.skinBlendShader_Face);
            var sRgbWrite = GL.sRGBWrite;

            GL.sRGBWrite = true;
            Graphics.SetRenderTarget(__instance.skinTex);
            GL.Clear(false, true, Color.black);
            Graphics.SetRenderTarget(null);

            if (__instance.eyeshadowTex == null)
                eyeshadowColor.a = 0.0f;

            if (__instance.cheekTex == null)
                cheekColor.a = 0.0f;

            if (__instance.lipTex == null)
                lipColor.a = 0.0f;

            if (__instance.moleTex == null)
                moleColor.a = 0.0f;

            mat.SetTexture(BaseTex, __instance.skinBaseTex);
            mat.SetFloat(OffsetH, __instance.human.customParam.body.skinColor.offset_h);
            mat.SetFloat(OffsetS, __instance.human.customParam.body.skinColor.offset_s);
            mat.SetFloat(OffsetV, __instance.human.customParam.body.skinColor.offset_v);
            mat.SetTexture(CheekTex, __instance.cheekTex);

            if (__instance.cheekTex != null)
                __instance.SetTattooOffsetAndTiling(mat, "_CheekTex", 1024, 1024, __instance.cheekTex.width, __instance.cheekTex.height, cheek.pos.x, cheek.pos.y);
            mat.SetColor(CheekColor, cheekColor);

            mat.SetTexture(EyeShadowTex, __instance.eyeshadowTex);
            if (__instance.eyeshadowTex != null)
                __instance.SetTattooOffsetAndTiling(mat, "_EyeShadowTex", 1024, 1024, __instance.eyeshadowTex.width, __instance.eyeshadowTex.height, eyeShadow.pos.x, eyeShadow.pos.y);
            mat.SetColor(EyeShadowColor, eyeshadowColor);

            mat.SetTexture(LipTex, __instance.lipTex);
            if (__instance.lipTex != null)
                __instance.SetTattooOffsetAndTiling(mat, "_LipTex", 1024, 1024, __instance.lipTex.width, __instance.lipTex.height, lip.pos.x, lip.pos.y);
            mat.SetColor(LipColor, lipColor);

            mat.SetTexture(MoleTex, __instance.moleTex);
            if (__instance.moleTex != null)
                __instance.SetTattooOffsetAndTiling(mat, "_MoleTex", 1024, 1024, __instance.moleTex.width, __instance.moleTex.height, mole.pos.x, mole.pos.y);
            mat.SetColor(MoleColor, moleColor);

            var part = TattooDataManager.Part.Head;
            var tattooDataArray = MakerAPI.InsideAndLoaded
                ? Ash.TattooExtensionHead.UnsavedChanges
                : TattooDataManager.GetTattooDataList(__instance.human, part);

            if (tattooDataArray?.Count(data => data != null) > 1) {
                AshLogger.LogDebug(LoggingSettings.LoggingModules.HumanHooks, "Multi-texture tattoo rendering branch");
                TattooTextureUtils.StackTattoosAndWriteOutputToTextureCache(__instance, part);

                if (MakerAPI.InsideAndLoaded)
                    mat.SetTexture(TattooTex, TattooExtensionBase.GetCachedRenderTexture(part));
                else
                    mat.SetTexture(TattooTex, TattooDataManager.GetCachedTexture2D(__instance.human, part));

                mat.SetColor(TattooColor, Color.white);
            }
            else {
                AshLogger.LogDebug(LoggingSettings.LoggingModules.HumanHooks, "Default tattoo rendering branch");
                var data = tattooDataArray?.FirstOrDefault(d => d != null);
                var tattooTexture = TattooTextureUtils.GetTextureAsset(data);
                var tattooColor = data?.TattooColor ?? Color.clear;

                mat.SetTexture(TattooTex, tattooTexture);
                mat.SetColor(TattooColor, tattooColor);

                TattooTextureUtils.SetTattooOffsetAndTiling(mat, "_TattooTex", data);
            }

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

            var part = TattooDataManager.Part.Head;
            var tattooDataArray = MakerAPI.InsideAndLoaded
                ? Ash.TattooExtensionHead.UnsavedChanges
                : TattooDataManager.GetTattooDataList(__instance.human, part);

            if (tattooDataArray?.Count(data => data != null) > 1) {
                AshLogger.LogDebug(LoggingSettings.LoggingModules.HumanHooks, "Multi-texture tattoo rendering branch");
                TattooTextureUtils.StackTattoosAndWriteOutputToTextureCache(__instance, part);

                if (MakerAPI.InsideAndLoaded)
                    mat.SetTexture(TattooTex, TattooExtensionBase.GetCachedRenderTexture(part));
                else
                    mat.SetTexture(TattooTex, TattooDataManager.GetCachedTexture2D(__instance.human, part));

                mat.SetColor(TattooColor, Color.white);
            }
            else {
                AshLogger.LogDebug(LoggingSettings.LoggingModules.HumanHooks, "Default tattoo rendering branch");
                var data = tattooDataArray?.FirstOrDefault(d => d != null);
                var tattooTexture = TattooTextureUtils.GetTextureAsset(data);
                var tattooColor = data?.TattooColor ?? Color.clear;

                mat.SetTexture(TattooTex, tattooTexture);
                mat.SetColor(TattooColor, tattooColor);

                TattooTextureUtils.SetTattooOffsetAndTiling(mat, "_TattooTex", data);
            }

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
