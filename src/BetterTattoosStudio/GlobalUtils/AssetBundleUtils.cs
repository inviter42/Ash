using System.Reflection;
using UnityEngine;

namespace BetterTattoosStudio.GlobalUtils
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
    }
}
