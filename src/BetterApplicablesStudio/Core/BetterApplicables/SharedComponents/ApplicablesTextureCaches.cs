using System.Collections.Generic;
using BetterApplicablesStudio.Core.BetterApplicables.BaseComponents.Managers.Shared;
using BetterApplicablesStudio.GlobalUtils;
using Character;
using UnityEngine;

namespace BetterApplicablesStudio.Core.BetterApplicables.SharedComponents
{
    internal static class ApplicablesTextureCaches
    {
        private static readonly Dictionary<BetterCategory, Dictionary<Part, Texture2D>> FemaleTextureCache =
                new Dictionary<BetterCategory, Dictionary<Part, Texture2D>> {
                    [BetterCategory.EyeShadow] = new Dictionary<Part, Texture2D>(),
                    [BetterCategory.CheekShadow] = new Dictionary<Part, Texture2D>(),
                    [BetterCategory.Tattoo] = new Dictionary<Part, Texture2D>(),
                    [BetterCategory.Mole] = new Dictionary<Part, Texture2D>()
                };

        private static readonly Dictionary<BetterCategory, Dictionary<Part, Texture2D>> MaleTextureCache =
                new Dictionary<BetterCategory, Dictionary<Part, Texture2D>> {
                    [BetterCategory.EyeShadow] = new Dictionary<Part, Texture2D>(),
                    [BetterCategory.CheekShadow] = new Dictionary<Part, Texture2D>(),
                    [BetterCategory.Tattoo] = new Dictionary<Part, Texture2D>(),
                    [BetterCategory.Mole] = new Dictionary<Part, Texture2D>()
                };

        internal static Texture2D GetCachedTexture2D(BetterCategory category, Human human, Part part) {
            return human.sex == SEX.FEMALE
                ? FemaleTextureCache[category]
                    ?.GetValueOrDefaultValue(part, null)
                : MaleTextureCache[category]
                    ?.GetValueOrDefaultValue(part, null);
        }

        internal static void UpdateTextureCache(BetterCategory category, Human human, Part part, Texture2D texture) {
            if (texture == null || human == null)
                return;

            if (human.sex == SEX.FEMALE)
                FemaleTextureCache[category][part] = texture;
            else
                MaleTextureCache[category][part] = texture;
        }
    }
}
