using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared
{
    public class SerializableTextureData
    {
        [JsonIgnore]
        internal Vector2 AbOffset {
            get => new Vector2(AbOffsetX, AbOffsetY);
            set {
                AbOffsetX = value.x;
                AbOffsetY = value.y;
            }
        }

        [JsonIgnore]
        internal Vector2 UserOffset {
            get => new Vector2(UserOffsetX, UserOffsetY);
            set {
                UserOffsetX = value.x;
                UserOffsetY = value.y;
            }
        }

        [JsonIgnore]
        internal Vector2 UserScale {
            get => new Vector2(UserScaleX, UserScaleY);
            set {
                UserScaleX = value.x;
                UserScaleY = value.y;
            }
        }

        [JsonIgnore]
        internal Color Tint {
            get {
                var c = new Color {
                    a = TintA,
                    r = TintR,
                    g = TintG,
                    b = TintB
                };
                return c;
            }
            set {
                TintA = value.a;
                TintR = value.r;
                TintG = value.g;
                TintB = value.b;
            }
        }

        [JsonProperty] internal int Id;
        [JsonProperty] internal string AssetBundleName;
        [JsonProperty] internal string TextureName;

        [JsonProperty] private float TintA;
        [JsonProperty] private float TintR;
        [JsonProperty] private float TintG;
        [JsonProperty] private float TintB;

        [JsonProperty] private float AbOffsetX;
        [JsonProperty] private float AbOffsetY;

        [JsonProperty] private float UserOffsetX;
        [JsonProperty] private float UserOffsetY;
        [JsonProperty] private float UserScaleX;
        [JsonProperty] private float UserScaleY;

        [JsonExtensionData]
#pragma warning disable CS0649 // Initialized by the JSON library
        private IDictionary<string, JToken> LegacyData;
#pragma warning restore CS0649 // Initialized by the JSON library

        [JsonConstructor]
        internal SerializableTextureData(
            int id,
            string assetBundleName,
            string textureName,
            Color tint,
            float abOffsetX,
            float abOffsetY,
            Vector2 offset,
            Vector2 scale
        ) {
            Id = id;
            AssetBundleName = assetBundleName;
            TextureName = textureName;
            Tint = tint;

            AbOffsetX = abOffsetX;
            AbOffsetY = abOffsetY;

            UserOffsetX = offset.x;
            UserOffsetY = offset.y;

            UserScaleX = scale.x;
            UserScaleY = scale.y;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context) {
            if (LegacyData == null)
                return;

            if (LegacyData.TryGetValue("TattooColorA", out var colorA) || LegacyData.TryGetValue("EyeShadowColorA", out colorA))
                TintA = colorA.Value<float>();

            if (LegacyData.TryGetValue("TattooColorR", out var colorR) || LegacyData.TryGetValue("EyeShadowColorR", out colorR))
                TintR = colorR.Value<float>();

            if (LegacyData.TryGetValue("TattooColorG", out var colorG) || LegacyData.TryGetValue("EyeShadowColorG", out colorG))
                TintG = colorG.Value<float>();

            if (LegacyData.TryGetValue("TattooColorB", out var colorB) || LegacyData.TryGetValue("EyeShadowColorB", out colorB))
                TintB = colorB.Value<float>();

            LegacyData.Clear();
        }
    }
}
