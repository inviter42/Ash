using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Illusion.Extensions;

namespace Ash.GlobalUtils
{
    internal static class AssetBundleUtils
    {
        internal static AssetBundle LoadBundleFromResource(string resourceName) {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                if (stream == null)
                    return null;

                var data = new byte[stream.Length];
                // ReSharper disable once MustUseReturnValue
                stream.Read(data, 0, data.Length);
                return AssetBundle.LoadFromMemory(data);
            }
        }

        internal static List<string> GetPathsFromBundle(AssetBundle bundle, string filter = "") {
            var model = new List<string>();
            var assetPaths = bundle.GetAllAssetNames();
            foreach (var path in assetPaths) {
                if (path.Contains(filter))
                    model.Push(path);
            }

            return model;
        }
    }
}
