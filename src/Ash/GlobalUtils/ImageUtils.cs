using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.GlobalUtils
{
    internal static class ImageUtils
    {
        // todo: this probably needs to be optimized
        internal static void SetRawImageTransparency(RawImage ri, float alpha) {
            if (ri == null)
                return;

            var tempColor = ri.color;
            tempColor.a = alpha;
            ri.color = tempColor;
        }

        internal static void SetImageTransparency(Image image, float alpha) {
            if (image == null)
                return;

            var tempColor = image.color;
            tempColor.a = alpha;
            image.color = tempColor;
        }

        internal static Texture2D RemoveBackgroundFuzzy(Texture2D inputTex, float tolerance, float fuzziness) {
            Color[] pixels;
            try {
                pixels = inputTex.GetPixels();
            }
            catch (Exception e) {
                Ash.Logger.LogDebug($"Unable to remove background due to the following error: {e.Message}");
                return inputTex;
            }

            var bgColor = (Color)DetectBackgroundColor(inputTex);

            for (var i = 0; i < pixels.Length; i++) {
                var distance = Vector3.Distance(
                    new Vector3(pixels[i].r, pixels[i].g, pixels[i].b),
                    new Vector3(bgColor.r, bgColor.g, bgColor.b)
                );

                float selection;

                if (distance <= tolerance) {
                    selection = 1.0f;
                }
                else if (distance < tolerance + fuzziness) {
                    selection = 1.0f - (distance - tolerance) / fuzziness;
                }
                else {
                    selection = 0f;
                }

                var newAlpha = Mathf.Max(0, pixels[i].a - selection);
                if (newAlpha <= 0) {
                    pixels[i] = Color.clear;
                }
                else {
                    pixels[i].r = Mathf.Clamp01((pixels[i].r - bgColor.r * selection) / (1f - selection));
                    pixels[i].g = Mathf.Clamp01((pixels[i].g - bgColor.g * selection) / (1f - selection));
                    pixels[i].b = Mathf.Clamp01((pixels[i].b - bgColor.b * selection) / (1f - selection));
                    pixels[i].a = newAlpha;
                }
            }

            var output = new Texture2D(inputTex.width, inputTex.height, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            output.SetPixels(pixels);
            output.Apply();

            return output;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        internal static Color32 DetectBackgroundColor(Texture2D inputTex) {
            Color32[] pixels;
            try {
                pixels = inputTex.GetPixels32();
            }
            catch (Exception e) {
                Ash.Logger.LogDebug(e.Message);
                return Color.white;
            }

            var w = inputTex.width;
            var h = inputTex.height;

            var counts = new Dictionary<uint, int>();

            for (var y = 0; y < h; y++) {
                for (var x = 0; x < w; x++) {
                    if (x != 0 && x != w - 1 && y != 0 && y != h - 1)
                        continue;

                    var color = pixels[y * w + x];

                    var key = ColorUtils.Color32ToUint(color);

                    if (counts.ContainsKey(key))
                        counts[key]++;
                    else
                        counts[key] = 1;
                }
            }

            if (counts.Count == 0)
                return Color.white;

            uint bestKey = 0;
            var maxCount = -1;
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var keyValuePair in counts) {
                if (keyValuePair.Value <= maxCount)
                    continue;

                maxCount = keyValuePair.Value;
                bestKey = keyValuePair.Key;
            }

            return ColorUtils.UintToColor32(bestKey);
        }
    }
}
