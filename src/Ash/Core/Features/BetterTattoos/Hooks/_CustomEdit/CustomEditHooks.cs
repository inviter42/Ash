using System;
using Character;
using HarmonyLib;
using UnityEngine;

namespace Ash.Core.Features.BetterTattoos.Hooks._CustomEdit
{
    internal class CustomEditHooks
    {
        internal static event Action<CustomSelectSet> BodyTattooChanged;
        internal static event Action<Color> BodyTattooColorChanged;
        internal static event Action<CustomSelectSet> FaceTattooChanged;
        internal static event Action<Color> FaceTattooColorChanged;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BodyCustomEdit), nameof(BodyCustomEdit.OnChangeTattoo), typeof(CustomSelectSet))]
        // ReSharper disable once InconsistentNaming
        internal static bool BodyOnChangeTattooPrefix(BodyCustomEdit __instance, CustomSelectSet set) {
            if (!__instance.invoke)
                return false;

            BodyTattooChanged?.Invoke(set);

            __instance.human.body.RendSkinTexture();

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BodyCustomEdit), nameof(BodyCustomEdit.OnChangeColor_Tattoo), typeof(Color))]
        // ReSharper disable once InconsistentNaming
        internal static bool BodyOnChangeColor_TattooPrefix(BodyCustomEdit __instance, Color color) {
            if (!__instance.invoke)
                return false;

            BodyTattooColorChanged?.Invoke(color);

            __instance.human.customParam.body.tattooColor = color;
            __instance.human.body.RendSkinTexture();

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FaceCustomEdit), nameof(FaceCustomEdit.OnChangeTattoo), typeof(CustomSelectSet))]
        // ReSharper disable once InconsistentNaming
        internal static bool FaceOnChangeTattooPrefix(FaceCustomEdit __instance, CustomSelectSet set) {
            if (!__instance.invoke)
                return false;

            FaceTattooChanged?.Invoke(set);

            __instance.human.head.RendSkinTexture();

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FaceCustomEdit), nameof(FaceCustomEdit.OnChangeTattooColor), typeof(Color))]
        // ReSharper disable once InconsistentNaming
        internal static bool FaceOnChangeColor_TattooPrefix(FaceCustomEdit __instance, Color color) {
            if (!__instance.invoke)
                return false;

            FaceTattooColorChanged?.Invoke(color);

            __instance.human.customParam.head.tattooColor = color;
            __instance.human.head.RendSkinTexture();

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BodyCustomEdit), nameof(BodyCustomEdit.LoadedHuman))]
        // ReSharper disable once InconsistentNaming
        internal static bool LoadedHumanBody_Prefix(BodyCustomEdit __instance) {
            __instance.invoke = false;

            var body = __instance.human.customParam.body;
            var sliderDataArray = __instance.human.sex != SEX.FEMALE ? BodyCustomEdit.datas_M : BodyCustomEdit.datas_F;
            for (var index = 0; index < __instance.sliders.Length; ++index) {
                if (sliderDataArray[index].type == (BodyCustomEdit.TAB)__instance.nowTab)
                    __instance.sliders[index].Value = __instance.human.body.GetShape(sliderDataArray[index].id) * 100f;
            }

            __instance.selSets_Skin.SetSelectedFromDataID(body.bodyID);
            __instance.detailRate.SetValue(body.detailWeight * 100f);
            __instance.skin_H.SetValue(body.skinColor.offset_h * 100f);
            __instance.skin_S.SetValue(BodyCustomEdit.SVtoSliderVal(body.skinColor.offset_s));
            __instance.skin_V.SetValue(BodyCustomEdit.SVtoSliderVal(body.skinColor.offset_v));
            __instance.specular_skin.SetValue((float)(body.skinColor.metallic * 100.0 * 2.5));
            __instance.smooth_skin.SetValue((float)(body.skinColor.smooth * 100.0 * 1.25));
            __instance.bustSoft.SetValue(body.bustSoftness * 100f);
            __instance.bustWeight.SetValue(body.bustWeight * 100f);
            __instance.selSets_Nip.SetSelectedFromDataID(body.nipID);
            __instance.areolaSize.SetValue(body.areolaSize * 100f);
            __instance.nip_H.SetValue(body.nipColor.offset_h * 100f);
            __instance.nip_S.SetValue(BodyCustomEdit.SVtoSliderVal(body.nipColor.offset_s));
            __instance.nip_V.SetValue(BodyCustomEdit.SVtoSliderVal(body.nipColor.offset_v));
            __instance.nip_A.SetValue(body.nipColor.alpha * 100f);
            __instance.specular_nip.SetValue((float)(body.nipColor.metallic * 100.0 * 2.5));
            __instance.smooth_nip.SetValue((float)(body.nipColor.smooth * 100.0 * 1.25));
            __instance.selSets_UnderHair.SetSelectedFromDataID(body.underhairID);
            __instance.color_underhair.SetColor(body.underhairColor.mainColor);
            __instance.selSets_Sunburn.SetSelectedFromDataID(body.sunburnID);
            __instance.sunburn_H.SetValue(body.sunburnColor_H * 100f);
            __instance.sunburn_S.SetValue(BodyCustomEdit.SVtoSliderVal(body.sunburnColor_S));
            __instance.sunburn_V.SetValue(BodyCustomEdit.SVtoSliderVal(body.sunburnColor_V));
            __instance.sunburn_A.SetValue(body.sunburnColor_A * 100f);
            // __instance.selSets_Tattoo.SetSelectedFromDataID(body.tattooID);
            // __instance.color_tattoo.SetColor(body.tattooColor);
            __instance.nail_H.SetValue(body.nailColor.offset_h * 100f);
            __instance.nail_S.SetValue(BodyCustomEdit.SVtoSliderVal(body.nailColor.offset_s));
            __instance.nail_V.SetValue(BodyCustomEdit.SVtoSliderVal(body.nailColor.offset_v));
            __instance.specular_nail.SetValue(body.nailColor.metallic * 100f);
            __instance.smooth_nail.SetValue(body.nailColor.smooth * 100f);
            __instance.manicure_Color.SetColor(body.manicureColor.mainColor1);
            __instance.specular_manicure.SetValue(body.manicureColor.specular1 * 100f);
            __instance.smooth_manicure.SetValue(body.manicureColor.smooth1 * 100f);
            __instance.invoke = true;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FaceCustomEdit), nameof(FaceCustomEdit.LoadedHuman))]
        // ReSharper disable once InconsistentNaming
        internal static bool LoadedHumanFace_Prefix(FaceCustomEdit __instance) {
            __instance.invoke = false;

            var head = __instance.human.customParam.head;
            for (var index = 0; index < __instance.sliders.Length; ++index) {
                if (FaceCustomEdit.datas[index].type == (FaceCustomEdit.TAB)__instance.nowTab)
                    __instance.sliders[index].Value = __instance.human.head.GetShape(FaceCustomEdit.datas[index].id) * 100f;
            }

            __instance.selSets_FaceType.SetSelectedFromDataID(head.headID);
            __instance.selSets_SkinType.SetSelectedFromDataID(head.faceTexID);
            __instance.selSets_BumpType.SetSelectedFromDataID(head.detailID);
            __instance.bumpRate.SetValue(head.detailWeight * 100f);
            var sync = head.CheckEyeEqual();
            __instance.syncEyeSwitch.Value = sync;
            __instance.colorChange_ScleraL.SetColor(head.eyeScleraColorL);
            __instance.colorChange_ScleraR.SetColor(head.eyeScleraColorR);
            __instance.selSets_EyeL.SetSelectedFromDataID(head.eyeID_L);
            __instance.colorChange_IrisL.SetColor(head.eyeIrisColorL);
            __instance.pupilL.SetValue(head.eyePupilDilationL * 100f);
            __instance.eyeEmissiveL.SetValue(head.eyeEmissiveL * 100f);
            __instance.selSets_EyeR.SetSelectedFromDataID(head.eyeID_R);
            __instance.colorChange_IrisR.SetColor(head.eyeIrisColorR);
            __instance.pupilR.SetValue(head.eyePupilDilationR * 100f);
            __instance.eyeEmissiveR.SetValue(head.eyeEmissiveR * 100f);
            __instance.ChangeEyeUI(sync);
            __instance.selSets_EyeHighlight.SetSelectedFromDataID(head.eyeHighlightTexID);
            __instance.metallic_EyeHighlight.SetValue(head.eyeHighlightColor.specular1 * 100f);
            __instance.smooth_EyeHighlight.SetValue(head.eyeHighlightColor.smooth1 * 100f);
            __instance.colorChange_EyeHighlight.SetColor(head.eyeHighlightColor.mainColor1);
            var flag = __instance.human.head.IsGlossEyeHighlight();
            __instance.metallic_EyeHighlight.gameObject.SetActive(flag);
            __instance.smooth_EyeHighlight.gameObject.SetActive(flag);
            __instance.colorChange_EyeHighlight.gameObject.SetActive(!flag);
            __instance.selSets_Eyebrow.SetSelectedFromDataID(head.eyeBrowID);
            __instance.colorChange_Eyebrow.SetColor(head.eyeBrowColor.mainColor1);
            __instance.metallic_Eyebrow.SetValue(head.eyeBrowColor.specular1 * 100f);
            __instance.smooth_Eyebrow.SetValue(head.eyeBrowColor.smooth1 * 100f);
            __instance.selSets_Eyelash.SetSelectedFromDataID(head.eyeLashID);
            __instance.colorChange_Eyelash.SetColor(head.eyeLashColor.mainColor1);
            __instance.metallic_Eyelash.SetValue(head.eyeLashColor.specular1 * 100f);
            __instance.smooth_Eyelash.SetValue(head.eyeLashColor.smooth1 * 100f);
            __instance.selSets_Mole.SetSelectedFromDataID(head.moleTexID);
            __instance.colorChange_Mole.SetColor(head.moleColor);
            __instance.selSets_EyeShadow.SetSelectedFromDataID(head.eyeshadowTexID);
            __instance.colorChange_EyeShadow.SetColor(head.eyeshadowColor);
            __instance.selSets_Cheek.SetSelectedFromDataID(head.cheekTexID);
            __instance.colorChange_Cheek.SetColor(head.cheekColor);
            __instance.selSets_Lip.SetSelectedFromDataID(head.lipTexID);
            __instance.colorChange_Lip.SetColor(head.lipColor);
            // __instance.selSets_Tattoo.SetSelectedFromDataID(head.tattooID);
            // __instance.colorChange_Tattoo.SetColor(head.tattooColor);
            __instance.selSets_Beard.SetSelectedFromDataID(head.beardID);
            __instance.colorChange_Beard.SetColor(head.beardColor);
            __instance.invoke = true;

            return false;
        }
    }
}
