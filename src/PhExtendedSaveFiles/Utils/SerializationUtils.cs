using System;
using System.Text;
using MessagePack;
using MessagePack.Resolvers;
using Newtonsoft.Json;

namespace PhExtendedSaveFiles.Utils
{
    public static class SerializationUtils
    {
        public static byte[] Serialize<T>(T obj) {
            try {
                return MessagePackSerializer.Serialize(obj, StandardResolver.Instance);
            }
            catch (FormatterNotRegisteredException) {
                return MessagePackSerializer.Serialize(obj, ContractlessStandardResolver.Instance);
            }
            catch (InvalidOperationException) {
                ExtendedSaveFiles.Logger.LogWarning("Only primitive types are supported. Using fallback JSON serializer.");
                return JsonSerializeToByteArray(obj);
            }
            catch (Exception e) {
                ExtendedSaveFiles.Logger.LogWarning($"Unexpected exception during JSON deserialization: {e}");
                return null;
            }
        }

        public static string JsonSerializeToString<T>(T obj) {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(byte[] obj) {
            try {
                return MessagePackSerializer.Deserialize<T>(obj, StandardResolver.Instance);
            }
            catch (FormatterNotRegisteredException) {
                return MessagePackSerializer.Deserialize<T>(obj, ContractlessStandardResolver.Instance);
            }
            catch (InvalidOperationException) {
                ExtendedSaveFiles.Logger.LogWarning("Only primitive types are supported. Using fallback JSON deserializer.");
                return JsonDeserializeFromByteArray<T>(obj);
            }
            catch (Exception e) {
                ExtendedSaveFiles.Logger.LogWarning($"Unexpected exception during JSON deserialization: {e}");
                return default;
            }
        }

        public static T JsonDeserializeFromString<T>(string str) {
            return JsonConvert.DeserializeObject<T>(str);
        }

        private static byte[] JsonSerializeToByteArray<T>(T data) {
            var str = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(str);
        }

        private static T JsonDeserializeFromByteArray<T>(byte[] data) {
            var str = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
