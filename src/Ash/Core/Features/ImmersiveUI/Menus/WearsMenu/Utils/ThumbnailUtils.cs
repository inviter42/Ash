using System;
using Ash.GlobalUtils;
using Character;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Utils
{
    internal static class ThumbnailUtils
    {
        // TODO: allow user to select directories as a last resort, before displaying fallback?
        internal static Texture2D LoadThumbnail(WEAR_TYPE type, string prefab) {
            // ReSharper disable once DuplicatedSequentialIfBodies
            if (TryLoadThumbnailFromRepackData(prefab, out var texture))
                return texture;

            Ash.Logger.LogDebug($"Thumbnail was not found in Repack data. Attempting to load thumbnail from original game files.");

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (TryLoadThumbnailFromAssetBundle(type, prefab, out texture))
                return texture;

            Ash.Logger.LogDebug($"Thumbnail was not found in original game files. Fallback image will be displayed.");

            return null;
        }

        // TODO: allow user to select directories as a last resort, before displaying fallback?
        internal static Texture2D LoadThumbnail(ACCESSORY_TYPE type, string prefab) {
            // ReSharper disable once DuplicatedSequentialIfBodies
            if (TryLoadThumbnailFromRepackData(prefab, out var texture))
                return texture;

            Ash.Logger.LogDebug($"Thumbnail was not found in Repack data. Attempting to load thumbnail from original game files.");

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (TryLoadThumbnailFromAssetBundle(type, prefab, out texture))
                return texture;

            Ash.Logger.LogDebug($"Thumbnail was not found in original game files. Fallback image will be displayed.");

            return null;
        }

        private static bool TryLoadThumbnailFromAssetBundle(WEAR_TYPE type, string prefab, out Texture2D texture) {
            try {
                texture = AssetBundleManagement.AssetBundleManager.LoadThumbnail(type, prefab);
                return true;
            }
            catch (Exception e) {
                Ash.Logger.LogError(e);
                texture = null;
                return false;
            }
        }

        private static bool TryLoadThumbnailFromAssetBundle(ACCESSORY_TYPE type, string prefab, out Texture2D texture) {
            try {
                texture = AssetBundleManagement.AssetBundleManager.LoadThumbnail(type, prefab);
                return true;
            }
            catch (Exception e) {
                Ash.Logger.LogError(e);
                texture = null;
                return false;
            }
        }

        private static bool TryLoadThumbnailFromRepackData(string prefab, out Texture2D texture) {
            var path = $"abdata/thumnbnail_R/{prefab}.png";
            try {
                texture = FileUtils.LoadTextureFromFile(path, TextureFormat.ARGB32);
                return true;
            }
            catch (Exception e) {
                Ash.Logger.LogDebug(e.Message);
                texture = null;
                return false;
            }
        }
    }
}
