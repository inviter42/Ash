using System.Linq;
using Ash.Core.Features.BetterTattoos.MakerExtensions;
using Ash.Core.Features.BetterTattoos.Utils;
using Ash.Logging;
using HarmonyLib;
using KKAPI.Maker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.BetterTattoos.Hooks._Body
{
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

            var part = TattooDataManager.Part.Body;
            var tattooDataArray = MakerAPI.InsideAndLoaded
                ? Ash.TattooExtensionBody.UnsavedChanges
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

            var part = TattooDataManager.Part.Body;
            var tattooDataArray = MakerAPI.InsideAndLoaded
                ? Ash.TattooExtensionBody.UnsavedChanges
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

            Graphics.Blit(__instance.skinMaterial.mainTexture, __instance.skinTex, mat, 0);

            GL.sRGBWrite = sRgbWrite;

            __instance.skinMaterial.mainTexture = __instance.skinTex;
            __instance.ChangeBumpRate();

            Object.Destroy(mat);

            return false;
        }
    }
}
