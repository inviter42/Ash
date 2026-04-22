using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures
{
    internal static class IuiTextureGen
    {
        internal class Size
        {
            internal readonly int Width;
            internal readonly int Height;

            internal Size(int width, int height) {
                Width = width;
                Height = height;
            }

            public static Size operator *(Size left, int right) {
                return new Size(
                    left.Width * right,
                    left.Height * right
                );
            }

            public static Size operator *(Size left, float right) {
                return new Size(
                    (int)(left.Width * right),
                    (int)(left.Height * right)
                );
            }

            public static Size operator +(Size left, int right) {
                return new Size(
                    left.Width + right,
                    left.Height + right
                );
            }

            public static Size operator +(Size left, float right) {
                return new Size(
                    (int)(left.Width + right),
                    (int)(left.Height + right)
                );
            }
        }

        internal class TextureInfo
        {
            internal Size TextureResolution { get; private set; }
            internal Size Dimensions { get; private set; }
            internal Vector2 AtlasOffset { get; private set; }

            internal TextureInfo(Size textureResolution, Size dimensions, Vector2 atlasOffset = default) {
                TextureResolution = textureResolution;
                Dimensions = dimensions;
                AtlasOffset = atlasOffset;
            }
        }

        internal class RectTextureInfo : TextureInfo
        {
            internal int BorderRadius { get; private set; }
            internal Color ActiveColor { get; private set; }
            internal Color InactiveColor { get; private set; }

            internal RectTextureInfo(Size textureResolution, Size dimensions, int borderRadius, Color activeColor,
                Color inactiveColor, Vector2 atlasOffset = default) : base(textureResolution, dimensions, atlasOffset) {
                BorderRadius = borderRadius;
                ActiveColor = activeColor;
                InactiveColor = inactiveColor;
            }
        }

        internal class CircleTextureInfo : TextureInfo
        {
            internal Color ActiveColor { get; private set; }
            internal Color InactiveColor { get; private set; }

            internal CircleTextureInfo(Size textureResolution, Size dimensions, Color activeColor, Color inactiveColor,
                Vector2 atlasOffset = default) : base(textureResolution, dimensions, atlasOffset) {
                ActiveColor = activeColor;
                InactiveColor = inactiveColor;
            }
        }

        internal abstract class RingTextureInfo : TextureInfo
        {
            internal int InnerDiameter { get; private set; }

            internal RingTextureInfo(Size textureResolution, Size dimensions, int innerDiameter,
                Vector2 atlasOffset = default) : base(textureResolution, dimensions, atlasOffset) {
                InnerDiameter = innerDiameter;
            }
        }

        internal class ColorRingTextureInfo : RingTextureInfo
        {
            internal Color ActiveColor { get; private set; }
            internal Color InactiveColor { get; private set; }

            internal ColorRingTextureInfo(Size textureResolution, Size dimensions, int innerDiameter, Color activeColor,
                Color inactiveColor, Vector2 atlasOffset = default)
                : base(textureResolution, dimensions, innerDiameter, atlasOffset) {
                ActiveColor = activeColor;
                InactiveColor = inactiveColor;
            }
        }

        internal class GradientRingTextureInfo : RingTextureInfo
        {
            internal Gradient ActiveGradient { get; private set; }
            internal Color InactiveColor { get; private set; }
            internal float RotationDeg { get; private set; }

            internal GradientRingTextureInfo(Size textureResolution, Size dimensions, int innerDiameter,
                Gradient activeGradient, Color inactiveColor, float rotationDeg = 0f, Vector2 atlasOffset = default)
                : base(textureResolution, dimensions, innerDiameter, atlasOffset) {
                ActiveGradient = activeGradient;
                InactiveColor = inactiveColor;
                RotationDeg = rotationDeg;
            }
        }

        internal class XTextureInfo : TextureInfo
        {
            internal int LineThickness { get; private set; }
            internal Color ActiveColor { get; private set; }
            internal Color InactiveColor { get; private set; }

            internal XTextureInfo(Size textureResolution, Size dimensions, int lineThickness, Color activeColor,
                Color inactiveColor, Vector2 atlasOffset = default) : base(textureResolution, dimensions, atlasOffset) {
                LineThickness = lineThickness;
                ActiveColor = activeColor;
                InactiveColor = inactiveColor;
            }
        }

        internal class SectorTextureInfo : ColorRingTextureInfo
        {
            internal float StartAngleDegrees { get; private set; }
            internal float EndAngleDegrees { get; private set; }

            internal SectorTextureInfo(
                Size textureResolution,
                Size dimensions,
                int innerDiameter,
                Color activeColor,
                Color inactiveColor,
                float startAngleDegrees,
                float endAngleDegrees,
                Vector2 atlasOffset = default
            ) : base(
                textureResolution,
                dimensions,
                innerDiameter,
                activeColor,
                inactiveColor,
                atlasOffset
            ) {
                StartAngleDegrees = startAngleDegrees;
                EndAngleDegrees = endAngleDegrees;
            }
        }

        internal static Texture2D CreateEmptyTexture(Size textureResolution, TextureFormat textureFormat) {
            var color = new Color[textureResolution.Width * textureResolution.Height];
            var texture = new Texture2D(textureResolution.Width, textureResolution.Height, textureFormat, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            for(var i = 0; i < color.Length; i++)
                color[i] = Color.clear;

            texture.SetPixels(color);
            texture.Apply();

            return texture;
        }

        internal static Texture2D GenerateRingTexture(
            ColorRingTextureInfo textureInfo,
            float antialiasingRange = 1f
        ) {
            var textureWidth = textureInfo.TextureResolution.Width;
            var textureHeight = textureInfo.TextureResolution.Height;
            var innerRadius = textureInfo.InnerDiameter / 2f;
            var outerRadius = textureInfo.Dimensions.Width / 2f;

            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[textureWidth * textureHeight];
            var center = new Vector2((textureWidth - 1) / 2f, (textureHeight - 1) / 2f);

            for (var y = 0; y < textureHeight; y++) {
                for (var x = 0; x < textureWidth; x++) {
                    var pixelPos = new Vector2(x, y);

                    var outerDist = IuiSdfs.CalculateCircleSdf(pixelPos, center, outerRadius);
                    var innerDist = innerRadius - Vector2.Distance(pixelPos, center);

                    var outerAlpha = 1.0f - Mathf.Clamp01((outerDist + antialiasingRange) / antialiasingRange);
                    var innerAlpha = 1.0f - Mathf.Clamp01((innerDist + antialiasingRange) / antialiasingRange);

                    var ringAlpha = outerAlpha * innerAlpha;

                    var index = y * textureWidth + x;
                    pixels[index] = Color.Lerp(inactiveColor, activeColor, ringAlpha);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }

        internal static Texture2D GenerateRingGradientTexture(
            GradientRingTextureInfo textureInfo,
            float antialiasingRange = 1f
        ) {
            var textureWidth = textureInfo.TextureResolution.Width;
            var textureHeight = textureInfo.TextureResolution.Height;
            var innerRadius = textureInfo.InnerDiameter / 2f;
            var outerRadius = textureInfo.Dimensions.Width / 2f;

            var activeGradient = textureInfo.ActiveGradient;
            var inactiveColor = textureInfo.InactiveColor;

            var rotationRad = (textureInfo.RotationDeg + 90f) * Mathf.Deg2Rad;
            var rotationDir = new Vector2(Mathf.Cos(rotationRad), Mathf.Sin(rotationRad));

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var colors = new Color32[textureWidth * textureHeight];
            var center = new Vector2((textureWidth - 1) / 2f, (textureHeight - 1) / 2f);

            for (var y = 0; y < textureHeight; y++) {
                for (var x = 0; x < textureWidth; x++) {
                    var pixelPos = new Vector2(x, y);
                    var relativePos = pixelPos - center;

                    var projection = Vector2.Dot(relativePos, rotationDir);
                    var tGradient = Mathf.InverseLerp(outerRadius, -outerRadius, projection);
                    var gradientColor = activeGradient.Evaluate(tGradient);

                    var outerDist = IuiSdfs.CalculateCircleSdf(pixelPos, center, outerRadius);
                    var innerDist = innerRadius - Vector2.Distance(pixelPos, center);

                    var outerAlpha = 1.0f - Mathf.Clamp01((outerDist + antialiasingRange) / antialiasingRange);
                    var innerAlpha = 1.0f - Mathf.Clamp01((innerDist + antialiasingRange) / antialiasingRange);

                    var ringAlpha = outerAlpha * innerAlpha;

                    var index = y * textureWidth + x;
                    colors[index] = Color.Lerp(inactiveColor, gradientColor, ringAlpha);
                }
            }

            texture.SetPixels32(colors);
            texture.Apply();

            return texture;
        }

        internal static Texture2D GenerateCircleTexture(
            CircleTextureInfo textureInfo,
            float antialiasingRange = 1f
        ) {
            var textureWidth = textureInfo.TextureResolution.Width;
            var diameter = textureInfo.Dimensions.Width;
            var radius = diameter / 2f;
            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var texture = new Texture2D(textureWidth, textureWidth, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[textureWidth * textureWidth];
            var center = new Vector2((textureWidth - 1) / 2f, (textureWidth - 1) / 2f);

            for (var y = 0; y < textureWidth; y++) {
                for (var x = 0; x < textureWidth; x++) {
                    var pixelPos = new Vector2(x, y);
                    var distance = IuiSdfs.CalculateCircleSdf(pixelPos, center, radius);

                    var aaAlpha = 1.0f - Mathf.Clamp01((distance + antialiasingRange) / antialiasingRange);

                    var pixelColor = activeColor;
                    pixelColor.a = activeColor.a * aaAlpha;

                    var index = y * textureWidth + x;
                    pixels[index] = distance >= 0 ? inactiveColor : pixelColor;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }

        internal static Texture2D GenerateSectorTexture(SectorTextureInfo textureInfo, float antialiasingRange = 1f) {
            var textureWidth = textureInfo.TextureResolution.Width;
            var textureHeight = textureInfo.TextureResolution.Height;
            var innerRadius = textureInfo.InnerDiameter / 2f;
            var outerRadius = textureInfo.Dimensions.Width / 2f;
            var startAngle = textureInfo.StartAngleDegrees;
            var endAngle = textureInfo.EndAngleDegrees;

            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[textureWidth * textureHeight];
            var center = new Vector2((textureWidth - 1) / 2f, (textureHeight - 1) / 2f);

            startAngle = (startAngle % 360 + 360) % 360;
            endAngle = (endAngle % 360 + 360) % 360;

            for (var y = 0; y < textureHeight; y++) {
                for (var x = 0; x < textureWidth; x++) {
                    var pixelPos = new Vector2(x, y);
                    var dir = pixelPos - center;
                    var dist = dir.magnitude;

                    var outerDistSdf = dist - outerRadius;
                    var innerDistSdf = innerRadius - dist;

                    var outerAlpha = 1.0f - Mathf.Clamp01((outerDistSdf + antialiasingRange) / antialiasingRange);
                    var innerAlpha = 1.0f - Mathf.Clamp01((innerDistSdf + antialiasingRange) / antialiasingRange);

                    var radialAlpha = outerAlpha * innerAlpha;

                    var angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
                    angle = (angle + 360f) % 360f;

                    float angularAlpha;
                    var angleStep = antialiasingRange * 360f / (2f * Mathf.PI * Mathf.Max(dist, 0.1f));

                    if (startAngle <= endAngle) {
                        angularAlpha = SmoothStepThreshold(angle, startAngle, endAngle, angleStep);
                    }
                    else {
                        angularAlpha = Mathf.Max(
                            SmoothStepThreshold(angle, startAngle, 360f, angleStep),
                            SmoothStepThreshold(angle, 0f, endAngle, angleStep)
                        );
                    }

                    var finalAlpha = radialAlpha * angularAlpha;

                    var index = y * textureWidth + x;
                    pixels[index] = Color.Lerp(inactiveColor, activeColor, finalAlpha);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }

        internal static Texture2D GenerateRectangleTexture(
            RectTextureInfo textureInfo,
            float antialiasingRange = 2f
        ) {
            var textureWidth = textureInfo.TextureResolution.Width;
            var textureHeight = textureInfo.TextureResolution.Height;
            var rectWidth = textureInfo.Dimensions.Width;
            var rectHeight = textureInfo.Dimensions.Height;
            var borderRadius = textureInfo.BorderRadius;
            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            var pixels = new Color32[textureWidth * textureHeight];

            var center = new Vector2(textureWidth / 2f - 0.5f, textureHeight / 2f - 0.5f);
            var halfW = rectWidth / 2f;
            var halfH = rectHeight / 2f;

            for (var y = 0; y < textureHeight; y++) {
                for (var x = 0; x < textureWidth; x++) {
                    var dx = Mathf.Abs(x - center.x);
                    var dy = Mathf.Abs(y - center.y);

                    var dist = IuiSdfs.CalculateRoundedRectSdf(dx, dy, halfW, halfH, borderRadius);
                    var t = Mathf.InverseLerp(antialiasingRange / 2.0f, -antialiasingRange / 2.0f, dist);

                    var pixelColor = Color.Lerp(inactiveColor, activeColor, t);

                    pixels[y * textureWidth + x] = pixelColor;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }

        internal static Texture2D GenerateRectangleStrokeTexture(
            RectTextureInfo textureInfo,
            float strokeWidth,
            float antialiasingRange = 1f
        ) {
            var textureWidth = Mathf.CeilToInt(textureInfo.TextureResolution.Width);
            var textureHeight = Mathf.CeilToInt(textureInfo.TextureResolution.Height);

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[textureWidth * textureHeight];

            var centerX = (textureWidth - 1) / 2f;
            var centerY = (textureHeight - 1) / 2f;

            var halfW = textureInfo.Dimensions.Width / 2f;
            var halfH = textureInfo.Dimensions.Height / 2f;

            float borderRadius = textureInfo.BorderRadius;

            for (var y = 0; y < textureHeight; y++) {
                for (var x = 0; x < textureWidth; x++) {
                    var dx = Mathf.Abs(x - centerX);
                    var dy = Mathf.Abs(y - centerY);

                    var innerW = halfW - strokeWidth;
                    var innerH = halfH - strokeWidth;
                    var innerR = Mathf.Max(0, borderRadius - strokeWidth);

                    var innerDist = IuiSdfs.CalculateRoundedRectSdf(dx, dy, innerW, innerH, innerR);
                    var outerDist = IuiSdfs.CalculateRoundedRectSdf(dx, dy, halfW, halfH, borderRadius);

                    var innerAlpha = Mathf.Clamp01(innerDist / antialiasingRange);
                    var outerAlpha = Mathf.Clamp01(-outerDist / antialiasingRange);

                    var strokeMask = innerAlpha * outerAlpha;

                    var finalColor = textureInfo.ActiveColor;
                    finalColor.a *= strokeMask;
                    pixels[y * textureWidth + x] = finalColor;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }

        internal static Texture2D GenerateXTexture(
            XTextureInfo textureInfo,
            float antialiasingRange = 2f
        ) {
            var textureWidth = textureInfo.TextureResolution.Width;
            var textureHeight = textureInfo.TextureResolution.Height;
            var thickness = textureInfo.LineThickness;
            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false) {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[textureWidth * textureHeight];
            var center = new Vector2(textureWidth / 2f - 0.5f, textureHeight / 2f - 0.5f);

            for (var y = 0; y < textureHeight; y++) {
                for (var x = 0; x < textureWidth; x++) {
                    var px = x - center.x;
                    var py = y - center.y;

                    // 45deg rotation
                    const float invSqrt2 = 0.70710678118f;
                    var d1 = Mathf.Abs((px - py) * invSqrt2);
                    var d2 = Mathf.Abs((px + py) * invSqrt2);

                    var dist = Mathf.Min(d1, d2) - thickness / 2f;
                    var t = Mathf.InverseLerp(antialiasingRange / 2.0f, -antialiasingRange / 2.0f, dist);

                    pixels[y * textureWidth + x] = Color.Lerp(inactiveColor, activeColor, t);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }

        internal static Gradient CreateLinearGradient(Color colorA, Color colorB, float opacityMidpoint) {
            var gradient = new Gradient();

            var colorKeys = new GradientColorKey[2];
            colorKeys[0].color = colorA;
            colorKeys[0].time = 0.0f; // 0%
            colorKeys[1].color = colorB;
            colorKeys[1].time = 1.0f; // 100%

            var midpointAlpha = Mathf.Lerp(colorA.a, colorB.a, 0.5f);

            var alphaKeys = new GradientAlphaKey[3];
            alphaKeys[0] = new GradientAlphaKey(colorA.a, 0.0f); // 0%
            alphaKeys[1] = new GradientAlphaKey(midpointAlpha, opacityMidpoint); // opacity midpoint
            alphaKeys[2] = new GradientAlphaKey(colorB.a, 1.0f); // 100%

            gradient.SetKeys(colorKeys, alphaKeys);

            return gradient;
        }

        // atlas methods

        internal static void StampRingTexture(ColorRingTextureInfo textureInfo, Texture2D texture,
            float antialiasingRange = 1f) {
            var outerRadius = textureInfo.Dimensions.Width / 2f;
            var innerRadius = textureInfo.InnerDiameter / 2f;
            var offset = textureInfo.AtlasOffset;

            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var zeroIndexOffset = new Vector2(0.5f, 0.5f);
            var center = new Vector2(
                outerRadius + offset.x,
                outerRadius + offset.y
            ) - zeroIndexOffset;

            var minX = Mathf.Max(0, Mathf.FloorToInt(center.x - outerRadius - antialiasingRange));
            var minY = Mathf.Max(0, Mathf.FloorToInt(center.y - outerRadius - antialiasingRange));
            var maxX = Mathf.Min(texture.width - 1, Mathf.CeilToInt(center.x + outerRadius + antialiasingRange));
            var maxY = Mathf.Min(texture.height - 1, Mathf.CeilToInt(center.y + outerRadius + antialiasingRange));

            var blockWidth = maxX - minX + 1;
            var blockHeight = maxY - minY + 1;

            if (blockWidth <= 0 || blockHeight <= 0)
                return;

            var blockPixels = texture.GetPixels(minX, minY, blockWidth, blockHeight);

            for (var y = 0; y < blockHeight; y++) {
                for (var x = 0; x < blockWidth; x++) {
                    var pixelPos = new Vector2(x + minX, y + minY);

                    var outerDist = IuiSdfs.CalculateCircleSdf(pixelPos, center, outerRadius);
                    var innerDist = innerRadius - Vector2.Distance(pixelPos, center);

                    var outerAlpha = 1.0f - Mathf.Clamp01((outerDist + antialiasingRange) / antialiasingRange);
                    var innerAlpha = 1.0f - Mathf.Clamp01((innerDist + antialiasingRange) / antialiasingRange);

                    var aaAlpha = outerAlpha * innerAlpha;

                    var index = y * blockWidth + x;
                    var sampled = blockPixels[index].a;
                    var finalValue = Mathf.Max(sampled, aaAlpha);

                    blockPixels[index] = Color.Lerp(inactiveColor, activeColor, finalValue);
                }
            }

            texture.SetPixels(minX, minY, blockWidth, blockHeight, blockPixels);
            texture.Apply(false);
        }

        internal static void StampRingGradientTexture(
            GradientRingTextureInfo textureInfo,
            Texture2D texture,
            float antialiasingRange = 1f
        ) {
            var outerRadius = textureInfo.Dimensions.Width / 2f;
            var innerRadius = textureInfo.InnerDiameter / 2f;
            var offset = textureInfo.AtlasOffset;

            var activeGradient = textureInfo.ActiveGradient;
            var inactiveColor = textureInfo.InactiveColor;

            var rotationRad = (textureInfo.RotationDeg + 90f) * Mathf.Deg2Rad;
            var rotationDir = new Vector2(Mathf.Cos(rotationRad), Mathf.Sin(rotationRad));

            var zeroIndexOffset = new Vector2(0.5f, 0.5f);
            var center = new Vector2(
                outerRadius + offset.x,
                outerRadius + offset.y
            ) - zeroIndexOffset;

            var minX = Mathf.Max(0, Mathf.FloorToInt(center.x - outerRadius - antialiasingRange));
            var minY = Mathf.Max(0, Mathf.FloorToInt(center.y - outerRadius - antialiasingRange));
            var maxX = Mathf.Min(texture.width - 1, Mathf.CeilToInt(center.x + outerRadius + antialiasingRange));
            var maxY = Mathf.Min(texture.height - 1, Mathf.CeilToInt(center.y + outerRadius + antialiasingRange));

            var blockWidth = maxX - minX + 1;
            var blockHeight = maxY - minY + 1;

            if (blockWidth <= 0 || blockHeight <= 0)
                return;

            var blockPixels = texture.GetPixels(minX, minY, blockWidth, blockHeight);

            for (var y = 0; y < blockHeight; y++) {
                for (var x = 0; x < blockWidth; x++) {
                    var pixelPos = new Vector2(x + minX, y + minY);
                    var relativePos = pixelPos - center;

                    var projection = Vector2.Dot(relativePos, rotationDir);
                    var tGradient = Mathf.InverseLerp(outerRadius, -outerRadius, projection);
                    var gradientColor = activeGradient.Evaluate(tGradient);

                    var outerDist = IuiSdfs.CalculateCircleSdf(pixelPos, center, outerRadius);
                    var innerDist = innerRadius - Vector2.Distance(pixelPos, center);

                    var outerAlpha = 1.0f - Mathf.Clamp01((outerDist + antialiasingRange) / antialiasingRange);
                    var innerAlpha = 1.0f - Mathf.Clamp01((innerDist + antialiasingRange) / antialiasingRange);

                    var ringAlpha = outerAlpha * innerAlpha;

                    var index = y * blockWidth + x;
                    blockPixels[index] = Color.Lerp(inactiveColor, gradientColor, ringAlpha);
                }
            }

            texture.SetPixels(minX, minY, blockWidth, blockHeight, blockPixels);
            texture.Apply(false);
        }

        internal static void StampCircleTexture(CircleTextureInfo textureInfo, Texture2D texture,
            float antialiasingRange = 1f) {
            var radius = textureInfo.Dimensions.Width / 2f;
            var offset = textureInfo.AtlasOffset;
            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var zeroIndexOffset = new Vector2(0.5f, 0.5f);
            var center = new Vector2(
                radius + offset.x,
                radius + offset.y
            ) - zeroIndexOffset;

            var minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radius - antialiasingRange));
            var minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radius - antialiasingRange));
            var maxX = Mathf.Min(texture.width - 1, Mathf.CeilToInt(center.x + radius + antialiasingRange));
            var maxY = Mathf.Min(texture.height - 1, Mathf.CeilToInt(center.y + radius + antialiasingRange));

            var blockWidth = maxX - minX + 1;
            var blockHeight = maxY - minY + 1;

            if (blockWidth <= 0 || blockHeight <= 0)
                return;

            var blockPixels = texture.GetPixels(minX, minY, blockWidth, blockHeight);

            for (var y = 0; y < blockHeight; y++) {
                for (var x = 0; x < blockWidth; x++) {
                    var pixelPos = new Vector2(x + minX, y + minY);
                    var dist = IuiSdfs.CalculateCircleSdf(pixelPos, center, radius);

                    var aaAlpha = 1.0f - Mathf.Clamp01((dist + antialiasingRange) / antialiasingRange);

                    var index = y * blockWidth + x;
                    var sampled = blockPixels[index].a;
                    var finalValue = Mathf.Max(sampled, aaAlpha);

                    blockPixels[index] = Color.Lerp(inactiveColor, activeColor, finalValue);
                }
            }

            texture.SetPixels(minX, minY, blockWidth, blockHeight, blockPixels);
            texture.Apply();
        }

        internal static void StampHalfCircleTexture(CircleTextureInfo textureInfo, Texture2D texture,
            float antialiasingRange = 1f) {
            var radius = textureInfo.Dimensions.Width / 2f;
            var offset = textureInfo.AtlasOffset;
            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var zeroIndexOffset = new Vector2(0.5f, 0.5f);
            var center = new Vector2(
                radius + offset.x,
                radius + offset.y
            ) - zeroIndexOffset;

            var minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radius - antialiasingRange));
            var minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radius - antialiasingRange));
            var maxX = Mathf.Min(texture.width - 1, Mathf.CeilToInt(center.x + radius + antialiasingRange));
            var maxY = Mathf.Min(texture.height - 1, Mathf.CeilToInt(center.y + radius + antialiasingRange));

            var blockWidth = maxX - minX + 1;
            var blockHeight = maxY - minY + 1;

            if (blockWidth <= 0 || blockHeight <= 0)
                return;

            var blockPixels = texture.GetPixels(minX, minY, blockWidth, blockHeight);

            for (var y = 0; y < blockHeight; y++) {
                for (var x = 0; x < blockWidth; x++) {
                    var pixelPos = new Vector2(x + minX, y + minY);
                    var dist = IuiSdfs.CalculateCircleSdf(pixelPos, center, radius);

                    var aaAlpha = 1.0f - Mathf.Clamp01((dist + antialiasingRange) / antialiasingRange);

                    var index = y * blockWidth + x;
                    var sampled = blockPixels[index].a;
                    var finalValue = Mathf.Max(sampled, aaAlpha);

                    if (pixelPos.x >= center.x)
                        blockPixels[index] = Color.Lerp(inactiveColor, activeColor, finalValue);
                }
            }

            texture.SetPixels(minX, minY, blockWidth, blockHeight, blockPixels);
            texture.Apply();
        }

        internal static void StampRectangleTexture(RectTextureInfo textureInfo, Texture2D texture,
            float antialiasingRange = 1f) {
            var rectWidth = textureInfo.Dimensions.Width;
            var rectHeight = textureInfo.Dimensions.Height;
            var borderRadius = textureInfo.BorderRadius;
            var zeroIndexOffset = new Vector2(0.5f, 0.5f);
            var offset = textureInfo.AtlasOffset - zeroIndexOffset;
            var activeColor = textureInfo.ActiveColor;
            var inactiveColor = textureInfo.InactiveColor;

            var minX = Mathf.Max(0, Mathf.FloorToInt(offset.x - antialiasingRange));
            var minY = Mathf.Max(0, Mathf.FloorToInt(offset.y - antialiasingRange));
            var maxX = Mathf.Min(texture.width - 1, Mathf.CeilToInt(offset.x + rectWidth + antialiasingRange));
            var maxY = Mathf.Min(texture.height - 1, Mathf.CeilToInt(offset.y + rectHeight + antialiasingRange));

            var blockWidth = maxX - minX + 1;
            var blockHeight = maxY - minY + 1;

            if (blockWidth <= 0 || blockHeight <= 0)
                return;

            var blockPixels = texture.GetPixels(minX, minY, blockWidth, blockHeight);

            var halfW = rectWidth / 2f;
            var halfH = rectHeight / 2f;
            var center = new Vector2(offset.x + halfW, offset.y + halfH);

            for (var y = 0; y < blockHeight; y++) {
                for (var x = 0; x < blockWidth; x++) {
                    float globalX = x + minX;
                    float globalY = y + minY;

                    var dx = Mathf.Abs(globalX - center.x);
                    var dy = Mathf.Abs(globalY - center.y);

                    var dist = IuiSdfs.CalculateRoundedRectSdf(dx, dy, halfW, halfH, borderRadius);
                    var aaAlpha = 1.0f - Mathf.Clamp01((dist + antialiasingRange) / antialiasingRange);

                    var index = y * blockWidth + x;
                    var sampled = blockPixels[index].a;
                    var finalValue = Mathf.Max(sampled, aaAlpha);

                    blockPixels[index] = Color.Lerp(inactiveColor, activeColor, finalValue);
                }
            }

            texture.SetPixels(minX, minY, blockWidth, blockHeight, blockPixels);
            texture.Apply(false);
        }

        // aa helpers

        private static float SmoothStepThreshold(float angle, float start, float end, float step) {
            var startSmooth = Mathf.Clamp01((angle - (start - step)) / step);
            var endSmooth = 1.0f - Mathf.Clamp01((angle - end) / step);
            return Mathf.Clamp01(startSmooth * endSmooth);
        }
    }
}
