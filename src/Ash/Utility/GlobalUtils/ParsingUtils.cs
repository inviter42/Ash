using System.Collections.Generic;
using UnityEngine;

namespace Ash.Utility.GlobalUtils;

internal static class ParsingUtils
{
    internal static List<AnimeAndNameSet> ParseTextAssetData(TextAsset textAsset) {
        var customDataListLoader = new CustomDataListLoader();
        var sets = new List<AnimeAndNameSet>();
        customDataListLoader.Load(textAsset);

        var attribute1 = customDataListLoader.GetAttributeNo("anime");
        var attribute2 = customDataListLoader.GetAttributeNo("name");

        for (var y = 0; y < customDataListLoader.GetDataNum(); ++y) {
            var data1 = customDataListLoader.GetData(attribute1, y);
            var data2 = customDataListLoader.GetData(attribute2, y);
            sets.Add(new AnimeAndNameSet(data1, data2));
        }

        return sets;
    }
}
