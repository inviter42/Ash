using UnityEngine;

namespace Ash.GlobalUtils
{
    internal static class ColorUtils
    {
        internal static Color Color32Af(byte r, byte g, byte b, float alpha = 1f) {
            return new Color32(r, g, b, (byte)(255 * alpha));
        }

        internal static uint Color32ToUint(Color32 color) {
            // bit-shift RGBA channels to occupy non-overlapping sections in 32-bit uint
            return (uint)(
                (color.r << 24)
                | (color.g << 16)
                | (color.b << 8)
                | color.a
            );
        }

        internal static Color32 UintToColor32(uint color) {
            return new Color32(
                (byte)(color >> 24),
                (byte)(color >> 16),
                (byte)(color >> 8),
                (byte)(color >> 0)
            );
        }
    }
}
