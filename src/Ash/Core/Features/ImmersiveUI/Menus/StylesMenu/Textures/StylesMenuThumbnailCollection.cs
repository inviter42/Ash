using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures
{
    internal class StylesMenuThumbnailCollection
    {
        internal readonly Dictionary<string, Texture2D> ThumbnailsDictionary = new Dictionary<string, Texture2D>();

         internal StylesMenuThumbnailCollection() {
            foreach (var path in Ash.AshUI.ImmersiveUIThumbnailsAssetBundle.GetAllAssetNames())
                ThumbnailsDictionary.Add(
                    Path.GetFileNameWithoutExtension(path),
                    Ash.AshUI.ImmersiveUIThumbnailsAssetBundle.LoadAsset<Texture2D>(path)
                );
        }
    }
}
