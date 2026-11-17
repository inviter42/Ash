using System;
using System.Text;
using Newtonsoft.Json;

namespace Ash.Utility.GlobalUtils
{
    internal static class SerializationUtils
    {
        internal static byte[] Serialize<T>(T obj) {
            try {
                return JsonSerializeToByteArray(obj);
            }
            catch (Exception e) {
                throw new Exception($"Unexpected exception during JSON deserialization: {e}");
            }
        }

        internal static string JsonSerializeToString<T>(T obj) {
            return JsonConvert.SerializeObject(obj);
        }

        internal static T Deserialize<T>(byte[] obj) {
            try {
                return JsonDeserializeFromByteArray<T>(obj);
            }
            catch (Exception e) {
                throw new Exception($"Unexpected exception during JSON deserialization: {e}");
            }
        }

        internal static T JsonDeserializeFromString<T>(string str) {
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
