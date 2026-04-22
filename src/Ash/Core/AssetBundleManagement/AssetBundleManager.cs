using System;
using System.Collections.Generic;
using System.IO;
using Ash.GlobalUtils;
using Character;
using UnityEngine;

namespace Ash.Core.AssetBundleManagement
{
    internal static class AssetBundleManager
    {
        private static readonly Dictionary<WEAR_TYPE, string> WearThumbnailsAbLookup =
            new Dictionary<WEAR_TYPE, string> {
                [WEAR_TYPE.TOP] = "thumbnail_weartop",
                [WEAR_TYPE.BOTTOM] = "thumbnail_wearbot",
                [WEAR_TYPE.BRA] = "thumbnail_wearbra",
                [WEAR_TYPE.SHORTS] = "thumbnail_wearshorts",
                [WEAR_TYPE.SWIM] = "thumbnail_wearswim",
                [WEAR_TYPE.SWIM_TOP] = "thumbnail_wearswimtop",
                [WEAR_TYPE.SWIM_BOTTOM] = "thumbnail_wearswimbot",
                [WEAR_TYPE.GLOVE] = "thumbnail_wearglove",
                [WEAR_TYPE.PANST] = "thumbnail_wearpanst",
                [WEAR_TYPE.SOCKS] = "thumbnail_wearsocks",
                [WEAR_TYPE.SHOES] = "thumbnail_wearshoes"
            };

        private static readonly Dictionary<ACCESSORY_TYPE, string> AccessoryThumbnailsAbLookup =
            new Dictionary<ACCESSORY_TYPE, string> {
                [ACCESSORY_TYPE.HEAD] = "thumbnail_accehead",
                [ACCESSORY_TYPE.EAR] = "thumbnail_acceear",
                [ACCESSORY_TYPE.GLASSES] = "thumbnail_accemegane",
                [ACCESSORY_TYPE.FACE] = "thumbnail_acceface",
                [ACCESSORY_TYPE.NECK] = "thumbnail_acceneck",
                [ACCESSORY_TYPE.SHOULDER] = "thumbnail_acceshoulder",
                [ACCESSORY_TYPE.CHEST] = "thumbnail_accebreast",
                [ACCESSORY_TYPE.WAIST] = "thumbnail_accewaist",
                [ACCESSORY_TYPE.BACK] = "thumbnail_acceback",
                [ACCESSORY_TYPE.ARM] = "thumbnail_accearm",
                [ACCESSORY_TYPE.HAND] = "thumbnail_accehand",
                [ACCESSORY_TYPE.LEG] = "thumbnail_acceleg"
            };

        private static readonly Dictionary<WEAR_TYPE, AssetBundle> WearThumbnailAbCache =
            new Dictionary<WEAR_TYPE, AssetBundle>();

        private static readonly Dictionary<ACCESSORY_TYPE, AssetBundle> AccessoryThumbnailAbCache =
            new Dictionary<ACCESSORY_TYPE, AssetBundle>();

        internal static Texture2D LoadThumbnail(WEAR_TYPE type, string prefab) {
            CacheAssetBundle(type, WearThumbnailsAbLookup, WearThumbnailAbCache);

            var assetBundle = WearThumbnailAbCache.GetValueOrDefaultValue(type, null);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (assetBundle == null)
                throw new Exception($"AssetBundle for type {type} is not found.");

            return LoadAsset<Texture2D>(assetBundle, prefab);
        }

        internal static Texture2D LoadThumbnail(ACCESSORY_TYPE type, string prefab) {
            CacheAssetBundle(type, AccessoryThumbnailsAbLookup, AccessoryThumbnailAbCache);

            var assetBundle = AccessoryThumbnailAbCache.GetValueOrDefaultValue(type, null);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (assetBundle == null)
                throw new Exception($"AssetBundle for type {type} is not found.");

            return LoadAsset<Texture2D>(assetBundle, prefab);
        }

        internal static void UnloadWearThumbnailAssetBundles(bool unloadAllLoadedObjects = false) {
            foreach (var keyValuePair in WearThumbnailAbCache)
                keyValuePair.Value.Unload(unloadAllLoadedObjects);

            WearThumbnailAbCache.Clear();
        }

        internal static void UnloadAccessoryThumbnailAssetBundles(bool unloadAllLoadedObjects = false) {
            foreach (var keyValuePair in AccessoryThumbnailAbCache)
                keyValuePair.Value.Unload(unloadAllLoadedObjects);

            AccessoryThumbnailAbCache.Clear();
        }

        private static T LoadAsset<T>(AssetBundle assetBundle, string assetName) where T : UnityEngine.Object {
            return assetBundle.LoadAsset<T>(assetName);
        }

        private static void CacheAssetBundle<T>(
            T type,
            Dictionary<T, string> lookupTable,
            Dictionary<T, AssetBundle> cache
        ) {
            var abFileName = lookupTable.GetValueOrDefaultValue(type, string.Empty);
            if (abFileName == string.Empty)
                throw new Exception($"AB filename for type {type} is not found.");

            var path = Path.Combine(Path.Combine(GlobalData.assetBundlePath, "thumnbnail/"), abFileName);
            if (!cache.ContainsKey(type))
                cache.Add(type, AssetBundle.LoadFromFile(path));
        }
    }
}
