using System.IO;
using System.Linq;
using Ash.Core.Features.BetterTattoos.MakerExtensions;
using Ash.Logging;
using Ash.Utility.GlobalUtils;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterTattoos.Utils
{
    internal static class TattooTextureUtils
    {
        private static readonly Shader TattooStackingShader =
            GlobalPluginData.ShaderCache.GetValueOrDefaultValue(GlobalPluginData.ShaderName.TattooStackingShader, null);

        private static readonly Material TattooStackingMaterial = new Material(TattooStackingShader);
        private static readonly int TattooTex = Shader.PropertyToID("_TattooTex");
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        internal static void SetTattooOffsetAndTiling(
            Material mat,
            string propertyName,
            TattooDataManager.SerializableTattooData data
        ) {
            if (mat == null || data == null)
                return;

            var result = CalculateOffsetAndScale(GetTextureAsset(data), data.AbOffset, data.UserOffset, data.UserScale);

            mat.SetTextureOffset(propertyName, result[0]);
            mat.SetTextureScale(propertyName, result[1]);
        }

        private static Vector2[] CalculateOffsetAndScale(
            Texture tattooTex,
            Vector2 abOffset,
            Vector2 userOffset,
            Vector2 userScale
        ) {
            var baseW = 1024;
            var baseH = 1024;
            var texW = tattooTex.width;
            var texH = tattooTex.height;
            var offsetPx = abOffset.x;
            var offsetPy = abOffset.y;

            // from PHCustomTexturePatch.dll
            if (offsetPx >= 10000.0 && offsetPy >= 10000.0 && offsetPx < 20000.0 && offsetPy < 20000.0) {
                baseW = 2048;
                baseH = 2048;
                offsetPx -= 10000f;
                offsetPy -= 10000f;
            }
            else if (offsetPx >= 20000.0 && offsetPy >= 20000.0 && offsetPx < 30000.0 && offsetPy < 30000.0) {
                baseW = 4096;
                baseH = 4096;
                offsetPx -= 20000f;
                offsetPy -= 20000f;
            }
            else if (offsetPx >= 30000.0 && offsetPy >= 30000.0) {
                baseW = 8196;
                baseH = 8196;
                offsetPx -= 30000f;
                offsetPy -= 30000f;
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            offsetPx += userOffset.x;
            offsetPy += userOffset.y;

            var scaleX = baseW / (float)texW;
            var scaleY = baseH / (float)texH;
            var offsetX = (float)-(offsetPx / (double)baseW) * scaleX;
            var offsetY = (float)-((baseH - (double)offsetPy - texH) / baseH) * scaleY;

            return new[] { new Vector2(offsetX, offsetY), new Vector2(scaleX / userScale.x, scaleY / userScale.y) };
        }

        internal static Texture GetTextureAsset(TattooDataManager.SerializableTattooData data) {
            if (data == null)
                return null;

            var cachedTexture = GlobalPluginData.TextureCache.GetValueOrDefaultValue(data.TextureName, null);

            return cachedTexture == null
                ? AssetBundleLoader.LoadAsset<Texture2D>(GlobalData.assetBundlePath, data.AssetBundleName, data.TextureName)
                : cachedTexture;
        }

        internal static void StackTattoosAndWriteOutputToTextureCache(Body body, TattooDataManager.Part part) {
            StackTattoosAndWriteOutputToTextureCache(
                body.human,
                body.skinTex,
                part,
                MakerAPI.InsideAndLoaded
                    ? Ash.TattooExtensionBody.UnsavedChanges
                    : TattooDataManager.GetTattooDataList(body.human, TattooDataManager.Part.Body)
            );
        }

        internal static void StackTattoosAndWriteOutputToTextureCache(Head head, TattooDataManager.Part part) {
            StackTattoosAndWriteOutputToTextureCache(
                head.human,
                head.skinTex,
                part,
                MakerAPI.InsideAndLoaded
                    ? Ash.TattooExtensionHead.UnsavedChanges
                    : TattooDataManager.GetTattooDataList(head.human, TattooDataManager.Part.Head)
            );
        }

        private static void StackTattoosAndWriteOutputToTextureCache(Human human, RenderTexture skinTex, TattooDataManager.Part part,
            TattooDataManager.SerializableTattooData[] data) {
            AshLogger.LogDebug(LoggingSettings.LoggingModules.HumanHooks, "Updating multi-texture cache");
            AshLogger.LogDebug(LoggingSettings.LoggingModules.HumanHooks, $"Total number of active tattoos layers <{data.Count(d => d != null)}>");

            foreach (var tattooData in data.Where(d => d != null)) {
                AshLogger.LogDebug(LoggingSettings.LoggingModules.HumanHooks,
                    $"Asset data {tattooData.AssetBundleName}/{tattooData.TextureName} "
                    + $"({tattooData.TattooColor.r}, {tattooData.TattooColor.g}, "
                    + $"{tattooData.TattooColor.b}, {tattooData.TattooColor.a})");
            }

            var width = skinTex.width;
            var height = skinTex.height;
            var format = RenderTextureFormat.ARGB32;

            var rtA = RenderTexture.GetTemporary(width, height, 0, format, RenderTextureReadWrite.sRGB);
            var rtB = RenderTexture.GetTemporary(width, height, 0, format, RenderTextureReadWrite.sRGB);

            Graphics.SetRenderTarget(rtA);
            GL.Clear(false, true, Color.clear);
            Graphics.SetRenderTarget(null);

            var currentSource = rtA;
            var currentTarget = rtB;

            foreach (var tattooData in data) {
                var tattooTex = GetTextureAsset(tattooData);
                if (tattooTex == null)
                    continue;

                TattooStackingMaterial.SetTexture(TattooTex, tattooTex);
                TattooStackingMaterial.SetColor(Color1, tattooData.TattooColor);

                SetTattooOffsetAndTiling(TattooStackingMaterial, "_TattooTex", tattooData);

                Graphics.Blit(currentSource, currentTarget, TattooStackingMaterial);

                // ReSharper disable once SwapViaDeconstruction
                var temp = currentSource;
                currentSource = currentTarget;
                currentTarget = temp;
            }

            if (MakerAPI.InsideAndLoaded)
                TattooExtensionBase.UpdateTattooTextureCache(part, currentSource);
            else
                TattooDataManager.UpdateTattooTextureCache(human, part, Utility.GlobalUtils.TextureUtils.SaveToTexture2D(currentSource));

            RenderTexture.ReleaseTemporary(rtA);
            RenderTexture.ReleaseTemporary(rtB);
        }

        // ReSharper disable once UnusedMember.Local
        private static void DumpRenderTexture(RenderTexture rt, string fileName) {
            var oldRT = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = oldRT;
            File.WriteAllBytes(Path.Combine(Application.dataPath, fileName), tex.EncodeToPNG());
            Object.Destroy(tex);
        }

        // ReSharper disable once UnusedMember.Local
        private static void DumpTexture2D(Texture tex, string fileName) {
            var debugRT = RenderTexture.GetTemporary(tex.width, tex.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(tex, debugRT);

            var previousActive = RenderTexture.active;
            RenderTexture.active = debugRT;

            var readableTex = new Texture2D(debugRT.width, debugRT.height, TextureFormat.ARGB32, false);
            readableTex.ReadPixels(new Rect(0, 0, debugRT.width, debugRT.height), 0, 0);
            readableTex.Apply();

            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(debugRT);

            File.WriteAllBytes(Path.Combine(Application.dataPath, fileName), readableTex.EncodeToPNG());
            Object.Destroy(readableTex);
        }
    }
}
