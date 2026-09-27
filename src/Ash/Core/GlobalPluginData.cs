using System.Collections.Generic;
using Ash.Utility.GlobalUtils;
using UnityEngine;

namespace Ash.Core
{
    internal static class GlobalPluginData
    {
        internal static readonly Dictionary<ShaderName, Shader> ShaderCache = new Dictionary<ShaderName, Shader>();

        internal static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();


        internal static readonly AssetBundle ImmersiveUIShadersAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.immersive_ui_shaders");
        internal static readonly AssetBundle ImmersiveUIIconsAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.immersive_ui_icons");
        internal static readonly AssetBundle ImmersiveUIFontsAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.immersive_ui_fonts");
        internal static readonly AssetBundle ImmersiveUIThumbnailsAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.immersive_ui_thumbnails");

        internal static readonly AssetBundle BetterApplicablesShadersAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.better_applicables_shaders");


        internal static void InvalidateShaderCache() {
            ShaderCache.Clear();
        }

        internal static void InvalidateTextureCache() {
            TextureCache.Clear();
        }

        internal static void PerformShaderCacheWarmup() {
            ShaderCache.Add(
                ShaderName.FrostedGlass,
                ImmersiveUIShadersAssetBundle.LoadAsset<Shader>("assets/frostedglass/shaders/frostedglass.shader")
            );

            ShaderCache.Add(
                ShaderName.SeparableBlur,
                ImmersiveUIShadersAssetBundle.LoadAsset<Shader>("assets/frostedglass/shaders/separableblur.shader")
            );

            ShaderCache.Add(
                ShaderName.CircleMaskSdf,
                ImmersiveUIShadersAssetBundle.LoadAsset<Shader>("assets/frostedglass/shaders/circlemasksdf.shader")
            );

            ShaderCache.Add(
                ShaderName.TextureStackingShader,
                BetterApplicablesShadersAssetBundle.LoadAsset<Shader>("assets/betterapplicables/shaders/texturestacking.shader")
            );
        }

        internal enum ShaderName
        {
            TextureStackingShader,
            SeparableBlur,
            FrostedGlass,
            CircleMaskSdf
        }
    }
}
