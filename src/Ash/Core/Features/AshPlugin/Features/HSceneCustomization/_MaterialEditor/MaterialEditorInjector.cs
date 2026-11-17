using System.Collections.Generic;
using System.Linq;
using MaterialEditorAPI;

namespace Ash.Core.Features.AshPlugin.Features.HSceneCustomization._MaterialEditor
{

    internal static class MaterialEditorInjector
    {
        private static readonly Dictionary<string, MaterialAPI.ShaderPropertyType> CustomPropertyList = new  Dictionary<string, MaterialAPI.ShaderPropertyType> {
            ["Fresnel"] = MaterialAPI.ShaderPropertyType.Float,
        };

        internal static void InjectProperties() {
            if (MaterialEditorPluginBase.XMLShaderProperties == null)
                return;

            if (!MaterialEditorPluginBase.XMLShaderProperties.ContainsKey("default")) {
                MaterialEditorPluginBase.XMLShaderProperties["default"] =
                    new Dictionary<string, MaterialEditorPluginBase.ShaderPropertyData>();
            }

            var defaultRegistry = MaterialEditorPluginBase.XMLShaderProperties["default"];

            foreach (var kvp in CustomPropertyList.Where(kvp => !string.IsNullOrEmpty(kvp.Key) && !defaultRegistry.ContainsKey(kvp.Key))) {
                defaultRegistry[kvp.Key] = new MaterialEditorPluginBase.ShaderPropertyData(kvp.Key, kvp.Value);
            }
        }
    }
}
