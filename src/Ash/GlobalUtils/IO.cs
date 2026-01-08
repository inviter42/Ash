using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneOf;

namespace Ash.GlobalUtils
{
    public static class IO
    {
        public const string InterItemRulesFileName = "Ash.InterItemRules.json";
        public const string HPosRulesFileName = "Ash.HPosItemRules.json";
        public const string SettingsFileName = "Ash.Settings.json";

        public static void Save<T>(T data, string fileName) {
            var filePath = Path.Combine(Paths.ConfigPath, fileName);
            var envelope = new PersistenceEnvelope<T> {
                Version = Ash.Version,
                Data = data
            };

            try {
                var json = JsonConvert.SerializeObject(envelope, typeof(object), JsonConfig.GetGlobalSettings());
                File.WriteAllText(filePath, json);
            }
            catch (Exception e) {
                Ash.Logger.LogWarning($"Failed to save JSON - {e.Message}");
            }
        }

        public static T Load<T>(string fileName) where T : new() {
            var data = Migrations.GetVersionSpecificFileData(fileName);
            var currentFilePath = data.Key;
            var currentFileName = data.Value;

            if (!File.Exists(currentFilePath)) {
                Ash.Logger.LogWarning($"Unable to load data - file {fileName} doesn't exist on disk.");
                return new T();
            }

            try {
                var json = File.ReadAllText(currentFilePath);
                var jObject = JObject.Parse(json);
                var fileVersion = GetFileVersion(jObject);
                PersistenceEnvelope<T> envelope;

                try {
                    envelope = JsonConvert.DeserializeObject<PersistenceEnvelope<T>>(json, JsonConfig.GetMigrationSettings(currentFileName, fileVersion));
                }
                catch (Exception) {
                    T intermediate;
                    try {
                        intermediate = JsonConvert.DeserializeObject<T>(json, JsonConfig.GetMigrationSettings(currentFileName, fileVersion));
                    }
                    catch (Exception e) {
                        Ash.Logger.LogError($"Deserialization failed with exception {e.Message}");
                        throw;
                    }

                    envelope = new PersistenceEnvelope<T> {
                        Version = Ash.Version,
                        Data = intermediate
                    };
                }

                Save(envelope.Data, fileName);
                if (currentFileName == fileName)
                    return envelope.Data;

                File.Delete(currentFilePath);
                return envelope.Data;
            }
            catch (Exception e) {
                Ash.Logger.LogError($"Failed to load JSON - {e.Message}");
                throw;
            }
        }

        private class OneOfConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => typeof(IOneOf).IsAssignableFrom(objectType);

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                if (value is IOneOf oneOf) {
                    serializer.Serialize(writer, oneOf.Value, typeof(object));
                }
                else {
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
                }
                else { // handle ambiguous primitive types
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
                        catch { /*ignored*/
                        }
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

        private static class JsonConfig
        {
            private static readonly JsonSerializerSettings GlobalSettings = new JsonSerializerSettings {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new OneOfConverter() },
            };

            public static JsonSerializerSettings GetGlobalSettings() {
                return GlobalSettings;
            }

            public static JsonSerializerSettings GetMigrationSettings(string fileName, string fileVersion) {
                return new JsonSerializerSettings {
                    ConstructorHandling = GlobalSettings.ConstructorHandling,
                    Formatting = GlobalSettings.Formatting,
                    NullValueHandling = GlobalSettings.NullValueHandling,
                    TypeNameHandling = GlobalSettings.TypeNameHandling,
                    Converters = GlobalSettings.Converters,
                    Binder = new Migrations.MigrationBinder(fileName, fileVersion)
                };
            }
        }

        private class PersistenceEnvelope<T>
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Version { get; set; }
            public T Data { get; set; }
        }

        private static string GetFileVersion(JObject obj) {
            return obj["Version"]?.Value<string>() ?? "1.1.0";
        }
    }
}
