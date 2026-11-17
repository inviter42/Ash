using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Character;
using HarmonyLib;

namespace Ash.Core.Tooling.SaveLoadManagement
{
    internal static class SaveLoadDataTracker
    {
        internal static SEX SexOfLoadingCharacter => CustomParameterHooks.SexOfLoadingCharacter;
        internal static MALE_ID MaleIdOfLoadingCharacter => CustomParameterHooks.MaleIdOfLoadingCharacter;
        internal static HEROINE FemaleIdOfLoadingCharacter => CustomParameterHooks.FemaleIdOfLoadingCharacter;

        internal static SEX SexOfSavingCharacter => CustomParameterHooks.SexOfSavingCharacter;
        internal static MALE_ID MaleIdOfSavingCharacter => CustomParameterHooks.MaleIdOfSavingCharacter;
        internal static HEROINE FemaleIdOfSavingCharacter => CustomParameterHooks.FemaleIdOfSavingCharacter;

        [HarmonyPatch]
        private class CustomParameterHooks
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static SEX SexOfLoadingCharacter;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static MALE_ID MaleIdOfLoadingCharacter;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static HEROINE FemaleIdOfLoadingCharacter;

            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static SEX SexOfSavingCharacter;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static MALE_ID MaleIdOfSavingCharacter;
            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static HEROINE FemaleIdOfSavingCharacter;

            [HarmonyPrefix]
            [HarmonyPatch(typeof(CustomParameter), nameof(CustomParameter.Load), typeof(BinaryReader))]
            // ReSharper disable once InconsistentNaming
            internal static bool CustomParameterLoadPrefix(CustomParameter __instance, BinaryReader reader) {
                var parentInstance = GlobalData.PlayData;
                if (parentInstance == null)
                    return true;

                // using game's native param getter to avoid using reflection
                var femaleParams = (from HEROINE heroineId in Enum.GetValues(typeof(HEROINE))
                        select new KeyValuePair<HEROINE, CustomParameter>(
                            heroineId,
                            GlobalData.PlayData.GetHeroineCustomParam(heroineId)
                        )
                    ).ToList();

                var maleParams = (from MALE_ID maleId in Enum.GetValues(typeof(MALE_ID))
                        select new KeyValuePair<MALE_ID, CustomParameter>(
                            maleId,
                            GlobalData.PlayData.GetMaleCustomParam(maleId)
                        )
                    ).ToList();

                SexOfLoadingCharacter = __instance.sex;

                var femaleParam = femaleParams.Find(param => ReferenceEquals(param.Value, __instance));
                var maleParam = maleParams.Find(param => ReferenceEquals(param.Value, __instance));

                if (femaleParam.Value != null) {
                    FemaleIdOfLoadingCharacter = femaleParam.Key;
                } else if (maleParam.Value != null) {
                    MaleIdOfLoadingCharacter = maleParam.Key;
                }

                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(CustomParameter), nameof(CustomParameter.Save), typeof(BinaryWriter))]
            // ReSharper disable once InconsistentNaming
            internal static bool CustomParameterSavePrefix(CustomParameter __instance, BinaryWriter writer) {
                var parentInstance = GlobalData.PlayData;
                if (parentInstance == null)
                    return true;

                // using game's native param getter to avoid using reflection
                var femaleParams = (from HEROINE heroineId in Enum.GetValues(typeof(HEROINE))
                        select new KeyValuePair<HEROINE, CustomParameter>(
                            heroineId,
                            GlobalData.PlayData.GetHeroineCustomParam(heroineId)
                        )
                    ).ToList();

                var maleParams = (from MALE_ID maleId in Enum.GetValues(typeof(MALE_ID))
                        select new KeyValuePair<MALE_ID, CustomParameter>(
                            maleId,
                            GlobalData.PlayData.GetMaleCustomParam(maleId)
                        )
                    ).ToList();

                SexOfSavingCharacter = __instance.sex;

                var femaleParam = femaleParams.Find(param => ReferenceEquals(param.Value, __instance));
                var maleParam = maleParams.Find(param => ReferenceEquals(param.Value, __instance));

                if (femaleParam.Value != null) {
                    FemaleIdOfSavingCharacter = femaleParam.Key;
                } else if (maleParam.Value != null) {
                    MaleIdOfSavingCharacter = maleParam.Key;
                }

                return true;
            }
        }
    }
}
