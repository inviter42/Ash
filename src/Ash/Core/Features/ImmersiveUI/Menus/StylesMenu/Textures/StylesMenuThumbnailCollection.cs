using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures
{
    internal class StylesMenuThumbnailCollection
    {
        internal readonly Dictionary<string, Texture2D> ThumbnailsDictionary = new Dictionary<string, Texture2D>();

         internal StylesMenuThumbnailCollection() {
            foreach (var path in GlobalPluginData.ImmersiveUIThumbnailsAssetBundle.GetAllAssetNames())
                ThumbnailsDictionary.Add(
                    Path.GetFileNameWithoutExtension(path),
                    GlobalPluginData.ImmersiveUIThumbnailsAssetBundle.LoadAsset<Texture2D>(path)
                );
        }
    }
}
