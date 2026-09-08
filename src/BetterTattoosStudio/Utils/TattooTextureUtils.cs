using System.Linq;
using BetterTattoosStudio.Core.ExtDataManagement;
using BetterTattoosStudio.GlobalUtils;
using UnityEngine;

namespace BetterTattoosStudio.Utils
{
    internal static class TattooTextureUtils
    {
        private static readonly Shader TattooStackingShader =
            TattooDataManager.MultipleTattoosShadersAssetBundle.LoadAsset<Shader>(
                "assets/multipletattoos/shaders/tattoostacking.shader");

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
            return data == null
                ? null
                : AssetBundleLoader.LoadAsset<Texture2D>(GlobalData.assetBundlePath, data.AssetBundleName, data.TextureName);
        }

        internal static Texture2D StackTattoosAndWriteOutput(Body body, TattooDataManager.Part part) {
            return StackTattoosAndWriteOutput(
                body.human,
                body.skinTex,
                part,
                TattooDataManager.TattooDataDict[TattooDataManager.Part.Body]
            );
        }

        internal static Texture2D StackTattoosAndWriteOutput(Head head, TattooDataManager.Part part) {
            return StackTattoosAndWriteOutput(
                head.human,
                head.skinTex,
                part,
                TattooDataManager.TattooDataDict[TattooDataManager.Part.Head]
            );
        }

        private static Texture2D StackTattoosAndWriteOutput(Human human, RenderTexture skinTex, TattooDataManager.Part part,
            TattooDataManager.SerializableTattooData[] data) {
            global::BetterTattoosStudio.BetterTattoosStudio.Logger.LogDebug("Updating multi-texture cache");
            global::BetterTattoosStudio.BetterTattoosStudio.Logger.LogDebug($"Total number of active tattoos layers <{data.Count(d => d != null)}>");

            foreach (var tattooData in data.Where(d => d != null)) {
                global::BetterTattoosStudio.BetterTattoosStudio.Logger.LogDebug(
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

            var output = TextureUtils.SaveToTexture2D(currentSource);

            RenderTexture.ReleaseTemporary(rtA);
            RenderTexture.ReleaseTemporary(rtB);

            return output;
        }
    }
}
