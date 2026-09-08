using System.Collections.Generic;
using Ash.Core.Features.BetterTattoos;
using UnityEngine;

namespace Ash.Core
{
    internal static class GlobalPluginData
    {
        internal static readonly Dictionary<ShaderName, Shader> ShaderCache = new Dictionary<ShaderName, Shader>();

        internal static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        internal static void InvalidateShaderCache() {
            ShaderCache.Clear();
        }

        internal static void InvalidateTextureCache() {
            TextureCache.Clear();
        }

        internal static void PerformShaderCacheWarmup() {
            ShaderCache.Add(
                ShaderName.FrostedGlass,
                Ash.AshUI.ImmersiveUIShadersAssetBundle.LoadAsset<Shader>("assets/frostedglass/shaders/frostedglass.shader")
            );

            ShaderCache.Add(
                ShaderName.SeparableBlur,
                Ash.AshUI.ImmersiveUIShadersAssetBundle.LoadAsset<Shader>("assets/frostedglass/shaders/separableblur.shader")
            );

            ShaderCache.Add(
                ShaderName.CircleMaskSdf,
                Ash.AshUI.ImmersiveUIShadersAssetBundle.LoadAsset<Shader>("assets/frostedglass/shaders/circlemasksdf.shader")
            );

            ShaderCache.Add(
                ShaderName.TattooStackingShader,
                TattooDataManager.MultipleTattoosShadersAssetBundle.LoadAsset<Shader>("assets/multipletattoos/shaders/tattoostacking.shader")
            );
        }

        internal enum ShaderName
        {
            TattooStackingShader,
            SeparableBlur,
            FrostedGlass,
            CircleMaskSdf
        }
    }
}
