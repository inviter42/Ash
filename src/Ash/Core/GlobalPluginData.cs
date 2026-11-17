using System.Collections.Generic;
using Ash.Utility.GlobalUtils;
using UnityEngine;

namespace Ash.Core;

internal static class GlobalPluginData
{
    /// <summary>
    /// Caches
    /// </summary>
    internal static readonly Dictionary<ShaderName, Shader> ShaderCache = new();
    internal static readonly Dictionary<MaterialName, Material> MaterialCache = new();
    internal static readonly Dictionary<string, Texture2D> TextureCache = new();

    /// <summary>
    /// Asset Bundles
    /// </summary>
    internal static readonly AssetBundle ImmersiveUIShadersAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.immersive_ui_shaders");
    internal static readonly AssetBundle ImmersiveUIIconsAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.immersive_ui_icons");
    internal static readonly AssetBundle ImmersiveUIFontsAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.immersive_ui_fonts");
    internal static readonly AssetBundle ImmersiveUIThumbnailsAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.immersive_ui_thumbnails");
    internal static readonly AssetBundle BetterApplicablesShadersAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.better_applicables_shaders");
    internal static readonly AssetBundle StandaloneShadersAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.standalone_shaders");
    internal static readonly AssetBundle AlloyShadersMaterialsAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.alloy_shaders_materials");
    internal static readonly AssetBundle GameDataAssetBundle = AssetBundleUtils.LoadBundleFromResource("Ash.Resources.game_data");


    /// <summary>
    /// Game data
    /// </summary>
    internal static readonly List<AnimeAndNameSet> EyeExpressionListFemale =
        ParsingUtils.ParseTextAssetData(GameDataAssetBundle.LoadAsset<TextAsset>("assets/gamedata/eyeexpressionlist_f.txt"));

    internal static readonly List<AnimeAndNameSet> MouthExpressionListFemale =
        ParsingUtils.ParseTextAssetData(GameDataAssetBundle.LoadAsset<TextAsset>("assets/gamedata/mouthexpressionlist_f.txt"));

    internal static void Init() {
        CacheShaders();
        CacheMaterials();
    }

    internal static void InvalidateTextureCache() {
        TextureCache.Clear();
    }

    private static void CacheShaders() {
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

        ShaderCache.Add(
            ShaderName.BpExtendedTessellationOffsetCutout,
            StandaloneShadersAssetBundle.LoadAsset<Shader>("assets/standaloneshaders/bpextendedtessellationoffsetcutout.shader")
        );

        ShaderCache.Add(
            ShaderName.AlloyCoreOverlay,
            AlloyShadersMaterialsAssetBundle.LoadAsset<Shader>("assets/alloy/shaders/3.0/core/core overlay.shader")
        );
    }

    private static void CacheMaterials() {
        MaterialCache.Add(
            MaterialName.AlloyCoreOverlayCutoutNm,
            AlloyShadersMaterialsAssetBundle.LoadAsset<Material>("assets/materials/alloycoreoverlay__cutoutnm.mat")
        );

        MaterialCache.Add(
            MaterialName.AlloyCoreOverlayFadeNmParallaxRim,
            AlloyShadersMaterialsAssetBundle.LoadAsset<Material>("assets/materials/alloycoreoverlay__fadenmparallaxrim.mat")
        );

        MaterialCache.Add(
            MaterialName.AlloyCoreOverlayTransparent,
            AlloyShadersMaterialsAssetBundle.LoadAsset<Material>("assets/materials/alloycoreoverlay__transparent.mat")

        );
    }

    internal enum ShaderName
    {
        TextureStackingShader,
        SeparableBlur,
        FrostedGlass,
        CircleMaskSdf,
        BpExtendedTessellationOffsetCutout,
        AlloyCoreOverlay
    }

    internal enum MaterialName
    {
        AlloyCoreOverlayCutoutNm,
        AlloyCoreOverlayFadeNmParallaxRim,
        AlloyCoreOverlayTransparent
    }
}
