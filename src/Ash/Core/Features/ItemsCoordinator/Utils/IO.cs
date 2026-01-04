using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneOf;

namespace Ash.Core.Features.ItemsCoordinator.Utils
{
    public static class IO
    {
        private const string ItemRulesFileName = "Ash.ItemRules.json";

        public static void Save<T>(T data) {
            var filePath = Path.Combine(Paths.ConfigPath, ItemRulesFileName);

            try {
                var json = JsonConvert.SerializeObject(data, typeof(object), JsonConfig.Settings);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e) {
                Ash.Logger.LogWarning($"Failed to save JSON - {e.Message}");
            }
        }

        public static T Load<T>() where T : new() {
            var filePath = Path.Combine(Paths.ConfigPath, ItemRulesFileName);

            try {
                if (!File.Exists(filePath)) {
                    Ash.Logger.LogWarning("Unable to load data - file doesn't exist on disk.");
                    return new T();
                }
            }
            catch (Exception e) {
                Ash.Logger.LogWarning($"Unable to load data - {e.Message}.");
                return new T();
            }

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json, JsonConfig.Settings);
        }

        public class OneOfConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => typeof(IOneOf).IsAssignableFrom(objectType);

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                if (value is IOneOf oneOf) {
                    serializer.Serialize(writer, oneOf.Value, typeof(object));
                } else {
                    writer.WriteNull();
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer) {
                var token = JToken.Load(reader);
                var unionTypes = objectType.GetGenericArguments();
                object val = null;
                var index = -1;

                // identify the types for deserialization
                if (token.Type == JTokenType.Object && token["$type"] != null) {
                    var fullTypeName = token["$type"].ToString();
                    // extract type name from metadata assigned via TypeNameHandling
                    var typeName = fullTypeName.Contains(",") ? fullTypeName.Split(',')[0].Trim() : fullTypeName;

                    for (var i = 0; i < unionTypes.Length; i++) {
                        if (unionTypes[i].FullName != typeName && unionTypes[i].Name != typeName)
                            continue;

                        // transform json data to object
                        val = token.ToObject(unionTypes[i], serializer);
                        // get OneOf's index for this type (T<index>)
                        index = i;
                        break;
                    }
                } else { // handle ambiguous primitive types
                    for (var i = 0; i < unionTypes.Length; i++) {
                        try {
                            if (unionTypes[i].IsEnum && token.Type == JTokenType.Integer) {
                                val = Enum.ToObject(unionTypes[i], token.Value<long>());
                                index = i;
                                break;
                            }

                            // ReSharper disable once InvertIf
                            if (unionTypes[i] == typeof(bool) && token.Type == JTokenType.Boolean) {
                                val = token.Value<bool>();
                                index = i;
                                break;
                            }
                        }
                        catch { /*ignored*/ }
                    }
                }

                if (index == -1 || val == null)
                    return null;

                // call factory OneOf method
                var methodName = "FromT" + index;
                var method = objectType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

                return method == null
                    ? throw new JsonException($"Could not find factory method {methodName} on {objectType.Name}")
                    : method.Invoke(null, new[] { val });
            }
        }
    }

    public static class JsonConfig
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters = new List<JsonConverter> { new IO.OneOfConverter() }
        };
    }
}
