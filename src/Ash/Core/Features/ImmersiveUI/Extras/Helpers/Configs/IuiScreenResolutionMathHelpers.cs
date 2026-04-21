using System;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.Textures;
using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI.Extras.Helpers.Configs
{
    internal static class IuiScreenResolutionMathHelpers
    {
        // get UI scale value relative to the screen dimensions
        internal static object GetRelativeValue(object value) {
            switch (value) {
                case int i: return GetRelativeValue(i);
                case Vector2 v: return GetRelativeValue(v);
                case IuiTextureGen.Size s: return GetRelativeValue(s);
                case RectOffset ro: return GetRelativeValue(ro);
                default: return value;
            }
        }

        private static int GetRelativeValue(int value) {
            var screenRef = Mathf.Min(Screen.width, Screen.height);
            const int referenceValue = 1080;

            return (int)Math.Round(screenRef * ((float)value / referenceValue));
        }

        private static Vector2 GetRelativeValue(Vector2 value) {
            var screenRef = Mathf.Min(Screen.width, Screen.height);
            const int referenceValue = 1080;

            var rX = (int)Math.Round(screenRef * (value.x / referenceValue));
            var rY = (int)Math.Round(screenRef * (value.y / referenceValue));

            return new Vector2(rX, rY);
        }

        private static IuiTextureGen.Size GetRelativeValue(IuiTextureGen.Size value) {
            var screenRef = Mathf.Min(Screen.width, Screen.height);
            const int referenceValue = 1080;

            var rWidth = (int)Math.Round(screenRef * ((float)value.Width / referenceValue));
            var rHeight = (int)Math.Round(screenRef * ((float)value.Height / referenceValue));

            return new IuiTextureGen.Size(rWidth, rHeight);
        }

        private static RectOffset GetRelativeValue(RectOffset value) {
            var screenRef = Mathf.Min(Screen.width, Screen.height);
            const int referenceValue = 1080;

            var rLeft = (int)Math.Round(screenRef * ((float)value.left / referenceValue));
            var rRight = (int)Math.Round(screenRef * ((float)value.right / referenceValue));
            var rTop = (int)Math.Round(screenRef * ((float)value.top / referenceValue));
            var rBottom = (int)Math.Round(screenRef * ((float)value.bottom / referenceValue));

            return new RectOffset(rLeft, rRight, rTop, rBottom);
        }
    }
}
