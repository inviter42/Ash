using System.Linq;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared;
using BetterApplicablesStudio.Core.BetterApplicables.SharedComponents;
using BetterApplicablesStudio.GlobalUtils;
using Character;
using UnityEngine;

namespace BetterApplicablesStudio.Core.BetterApplicables.SharedUtils
{
    internal static class ApplicablesTextureUtils
    {
        private static readonly AssetBundle BetterApplicablesShadersAssetBundle = AssetBundleUtils.LoadBundleFromResource("BetterApplicablesStudio.Resources.better_applicables_shaders");

        private static readonly Shader TextureStackingShader = BetterApplicablesShadersAssetBundle.LoadAsset<Shader>("assets/betterapplicables/shaders/texturestacking.shader");

        private static readonly Material TextureStackingMaterial = new Material(TextureStackingShader);

        private const string TexturePropertyName = "_ApplicableTex";
        private const string TintPropertyName = "_Tint";

        private static readonly int Tex = Shader.PropertyToID(TexturePropertyName);
        private static readonly int Tint = Shader.PropertyToID(TintPropertyName);


        internal static void ConfigureMaterialSlotForMultiTexture(
            SerializableTextureData[] dataArray,
            CharaShapeCustomBase shape,
            Part part,
            BetterCategory category,
            Material mat,
            int texId,
            int colorId,
            string propertyName
        ) {
            if (dataArray?.Count(data => data != null) > 1) {
                BetterApplicablesStudio.Logger.LogDebug($"[{(shape.human.sex == SEX.FEMALE ? "Female" : "Male")}] Multi-texture {category} rendering branch");
                StackTexturesAndWriteOutputToTextureCache(category, shape, part, dataArray);

                mat.SetTexture(texId, ApplicablesTextureCaches.GetCachedTexture2D(category, shape.human, part));

                mat.SetColor(colorId, Color.white);
            }
            else {
                BetterApplicablesStudio.Logger.LogDebug($"[{(shape.human.sex == SEX.FEMALE ? "Female" : "Male")}] Multi-texture {category} rendering branch");
                var data = dataArray?.FirstOrDefault(d => d != null);
                var textureAsset = GetTextureAsset(data);
                var tint = data?.Tint ?? Color.clear;

                mat.SetTexture(texId, textureAsset);
                mat.SetColor(colorId, tint);

                SetOffsetAndTiling(mat, propertyName, data);
            }
        }

        private static void SetOffsetAndTiling(
            Material mat,
            string propertyName,
            SerializableTextureData data
        ) {
            if (mat == null || data == null)
                return;

            var result = CalculateOffsetAndScale(GetTextureAsset(data), data.AbOffset, data.UserOffset, data.UserScale);

            mat.SetTextureOffset(propertyName, result[0]);
            mat.SetTextureScale(propertyName, result[1]);
        }

        private static Vector2[] CalculateOffsetAndScale(
            Texture eyeShadowTex,
            Vector2 abOffset,
            Vector2 userOffset,
            Vector2 userScale
        ) {
            var baseW = 1024;
            var baseH = 1024;
            var texW = eyeShadowTex.width;
            var texH = eyeShadowTex.height;
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

        private static Texture GetTextureAsset(SerializableTextureData data) {
            return data == null
                ? null
                : AssetBundleLoader.LoadAsset<Texture2D>(GlobalData.assetBundlePath, data.AssetBundleName, data.TextureName);
        }

        private static void StackTexturesAndWriteOutputToTextureCache(BetterCategory category, CharaShapeCustomBase shape, Part part,
            SerializableTextureData[] dataArray) {
            BetterApplicablesStudio.Logger.LogDebug($"Updating multi-texture cache for {category} ({part})");
            BetterApplicablesStudio.Logger.LogDebug($"Total number of active applicable layers <{dataArray.Count(d => d != null)}>");

            foreach (var data in dataArray.Where(d => d != null)) {
                BetterApplicablesStudio.Logger.LogDebug(
                    $"Asset data {data.AssetBundleName}/{data.TextureName} "
                    + $"({data.Tint.r}, {data.Tint.g}, "
                    + $"{data.Tint.b}, {data.Tint.a})");
            }

            var width = shape.skinTex.width;
            var height = shape.skinTex.height;
            var format = RenderTextureFormat.ARGB32;

            var rtA = RenderTexture.GetTemporary(width, height, 0, format, RenderTextureReadWrite.sRGB);
            var rtB = RenderTexture.GetTemporary(width, height, 0, format, RenderTextureReadWrite.sRGB);

            Graphics.SetRenderTarget(rtA);
            GL.Clear(false, true, Color.clear);
            Graphics.SetRenderTarget(null);

            var currentSource = rtA;
            var currentTarget = rtB;

            foreach (var data in dataArray) {
                var applicableTex = GetTextureAsset(data);
                if (applicableTex == null)
                    continue;

                TextureStackingMaterial.SetTexture(Tex, applicableTex);
                TextureStackingMaterial.SetColor(Tint, data.Tint);

                SetOffsetAndTiling(TextureStackingMaterial, TexturePropertyName, data);

                Graphics.Blit(currentSource, currentTarget, TextureStackingMaterial);

                // ReSharper disable once SwapViaDeconstruction
                var temp = currentSource;
                currentSource = currentTarget;
                currentTarget = temp;
            }

            ApplicablesTextureCaches.UpdateTextureCache(category, shape.human, part, TextureUtils.SaveToTexture2D(currentSource));

            RenderTexture.ReleaseTemporary(rtA);
            RenderTexture.ReleaseTemporary(rtB);
        }
    }
}
