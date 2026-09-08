using System;
using Ash.Core.Features.BetterTattoos.Hooks._CustomEdit;
using Character;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterTattoos.MakerExtensions
{
    internal class TattooExtensionBody : TattooExtensionBase
    {
        protected override MakerCategory MakerCategory => MakerConstants.Body.Tattoo;
        protected override float OffsetMinValue => -4096;
        protected override float OffsetMaxValue => 4096;
        protected override TattooDataManager.Part Part => TattooDataManager.Part.Body;

        protected override Action RenderSkinTexture => MakerAPI.GetMakerBase().human.body.RendSkinTexture;

        protected override string TattooLayerSelectorsGroupId => "BodyTattooLayerSelectionGroup";


        protected override void OnTattooLayerButtonPressed(int index) {
            ActiveTattooLayer = index;

            UpdateGuiControlsState();
            UpdateNativeGuiControlsState();

            RenderSkinTexture();
        }

        protected override void SubscribeToTattooEvents() {
            CustomEditHooks.BodyTattooChanged += OnTattooChanged;
            CustomEditHooks.BodyTattooColorChanged += OnTattooColorChanged;
        }

        protected override void UnsubscribeFromTattooEvents() {
            CustomEditHooks.BodyTattooChanged -= OnTattooChanged;
            CustomEditHooks.BodyTattooColorChanged -= OnTattooColorChanged;
        }


        protected override TattooDataManager.SerializableTattooData CreateTattooData(CustomSelectSet set) {
            if (set.id == 0)
                return null;

            var combinedTextureData = MakerAPI.GetMakerSex() == SEX.FEMALE
            ? CustomDataManager.GetBodyTattoo_Female(set.id)
            : CustomDataManager.GetBodyTattoo_Male(set.id);

            return new TattooDataManager.SerializableTattooData(
                combinedTextureData.id,
                combinedTextureData.assetbundleName,
                combinedTextureData.textureName,
                MakerAPI.GetMakerBase().body.color_tattoo.color,
                combinedTextureData.pos.x,
                combinedTextureData.pos.y,
                Vector2.zero,
                Vector2.one
            );
        }

        protected override void UpdateNativeGuiControlsState() {
            var editMode = MakerAPI.GetMakerBase();
            var set = editMode.thumnbs_bodyTattoo.Find(e => e.id == (UnsavedChanges[ActiveTattooLayer]?.Id ?? 0));
            var thumbIndex = editMode.thumnbs_bodyTattoo.FindIndex(e => e.id == (UnsavedChanges[ActiveTattooLayer]?.Id ?? 0));

            editMode.body.selSets_Tattoo.selectID = set?.id ?? -1;
            editMode.body.selSets_Tattoo.select.select.SelectNo = thumbIndex;
            editMode.body.selSets_Tattoo.toggle.dataName.text = set == null ? string.Empty : set.name;
            editMode.body.selSets_Tattoo.toggle.thumnbnailImage.sprite = set?.thumbnail_S;
            editMode.body.selSets_Tattoo.select.Close();

            editMode.body.color_tattoo.SetColor(UnsavedChanges[ActiveTattooLayer]?.TattooColor ?? Color.white);
            editMode.body.color_tattoo.colorUI.Close();

            Logger.LogDebug($"[UpdateNativeGuiControlsState()] Current body tattoo id is {editMode.body.selSets_Tattoo.selectID}");
        }
    }
}
