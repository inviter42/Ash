using UnityEngine;

namespace Ash.Core.Features.ImmersiveUI
{
    internal static class IuiSettings
    {

        // CommandBuffer settings

        /*
         * BlurSpread [Range(0.1f, 5f)]
         * Controls the distance between the points of UV coordinates of the filter.
         */
        internal const float BlurSpread = 2.0f;

        // Radial Menu settings
        internal static readonly Vector2 RadialMenuOrigin = new Vector2((float)Screen.width / 2, (float)Screen.height / 2);

        // [frosted effect settings]

        internal const float RadialMenuFrostIntensity = 0.3f;

        internal const float RadialMenuDeadzoneThreshold = 4f;
        internal const float RadialMenuHighlightAnimationSpeed = 10f;

        // Styles Menu settings
        internal const float SubMenusFrostIntensity = 0.65f;
    }
}
