using System.Collections.Generic;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Utility.GlobalUtils;
using Character;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.SharedComponents;

internal static class ApplicablesTextureCaches
{
    private static readonly Dictionary<BetterCategory, Dictionary<HEROINE, Dictionary<Part, Texture2D>>> FemaleTextureCache =
        new() {
            [BetterCategory.EyeShadow] = new Dictionary<HEROINE, Dictionary<Part, Texture2D>> {
                [HEROINE.RITSUKO] = new(),
                [HEROINE.AKIKO] = new(),
                [HEROINE.YUKIKO] = new(),
                [HEROINE.MARIKO] = new(),
            },
            [BetterCategory.CheekShadow] = new Dictionary<HEROINE, Dictionary<Part, Texture2D>> {
                [HEROINE.RITSUKO] = new(),
                [HEROINE.AKIKO] = new(),
                [HEROINE.YUKIKO] = new(),
                [HEROINE.MARIKO] = new(),
            },
            [BetterCategory.Tattoo] = new Dictionary<HEROINE, Dictionary<Part, Texture2D>> {
                [HEROINE.RITSUKO] = new(),
                [HEROINE.AKIKO] = new(),
                [HEROINE.YUKIKO] = new(),
                [HEROINE.MARIKO] = new(),
            },
            [BetterCategory.Mole] = new Dictionary<HEROINE, Dictionary<Part, Texture2D>> {
                [HEROINE.RITSUKO] = new(),
                [HEROINE.AKIKO] = new(),
                [HEROINE.YUKIKO] = new(),
                [HEROINE.MARIKO] = new(),
            }
        };

    private static readonly Dictionary<BetterCategory, Dictionary<MALE_ID, Dictionary<Part, Texture2D>>> MaleTextureCache =
        new() {
            [BetterCategory.EyeShadow] = new Dictionary<MALE_ID, Dictionary<Part, Texture2D>> {
                [MALE_ID.HERO] = new(),
                [MALE_ID.KOUICHI] = new(),
                [MALE_ID.MOB_A] = new(),
                [MALE_ID.MOB_B] = new(),
                [MALE_ID.MOB_C] = new(),
            },
            [BetterCategory.CheekShadow] = new Dictionary<MALE_ID, Dictionary<Part, Texture2D>> {
                [MALE_ID.HERO] = new(),
                [MALE_ID.KOUICHI] = new(),
                [MALE_ID.MOB_A] = new(),
                [MALE_ID.MOB_B] = new(),
                [MALE_ID.MOB_C] = new(),
            },
            [BetterCategory.Tattoo] = new Dictionary<MALE_ID, Dictionary<Part, Texture2D>> {
                [MALE_ID.HERO] = new(),
                [MALE_ID.KOUICHI] = new(),
                [MALE_ID.MOB_A] = new(),
                [MALE_ID.MOB_B] = new(),
                [MALE_ID.MOB_C] = new(),
            },
            [BetterCategory.Mole] = new Dictionary<MALE_ID, Dictionary<Part, Texture2D>> {
                [MALE_ID.HERO] = new(),
                [MALE_ID.KOUICHI] = new(),
                [MALE_ID.MOB_A] = new(),
                [MALE_ID.MOB_B] = new(),
                [MALE_ID.MOB_C] = new(),
            }
        };

    internal static Texture2D GetCachedTexture2D(BetterCategory category, Human human, Part part) {
        return human.sex == SEX.FEMALE
            ? FemaleTextureCache[category]
                ?.GetValueOrDefaultValue(((Female)human).heroineID, null)
                ?.GetValueOrDefaultValue(part, null)
            : MaleTextureCache[category]
                ?.GetValueOrDefaultValue(((Male)human).maleID, null)
                ?.GetValueOrDefaultValue(part, null);
    }

    internal static void UpdateTextureCache(BetterCategory category, Human human, Part part, Texture2D texture) {
        if (texture == null || human == null)
            return;

        if (human.sex == SEX.FEMALE)
            FemaleTextureCache[category][((Female)human).heroineID][part] = texture;
        else
            MaleTextureCache[category][((Male)human).maleID][part] = texture;
    }
}
