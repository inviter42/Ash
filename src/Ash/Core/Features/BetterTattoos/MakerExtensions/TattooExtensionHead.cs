using System;
using Ash.Core.Features.BetterTattoos.Hooks._CustomEdit;
using Character;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterTattoos.MakerExtensions
{
    internal class TattooExtensionHead : TattooExtensionBase
    {
        protected override MakerCategory MakerCategory => MakerConstants.Face.Tattoo;
        protected override float OffsetMinValue => -2048;
        protected override float OffsetMaxValue => 2048;
        protected override TattooDataManager.Part Part => TattooDataManager.Part.Head;

        protected override Action RenderSkinTexture => MakerAPI.GetMakerBase().human.head.RendSkinTexture;

        protected override string TattooLayerSelectorsGroupId => "HeadTattooLayerSelectionGroup";


        protected override void OnTattooLayerButtonPressed(int index)  {
            ActiveTattooLayer = index;

            UpdateGuiControlsState();
            UpdateNativeGuiControlsState();

            RenderSkinTexture();
        }

        protected override void SubscribeToTattooEvents() {
            CustomEditHooks.FaceTattooChanged += OnTattooChanged;
            CustomEditHooks.FaceTattooColorChanged += OnTattooColorChanged;
        }

        protected override void UnsubscribeFromTattooEvents() {
            CustomEditHooks.FaceTattooChanged -= OnTattooChanged;
            CustomEditHooks.FaceTattooColorChanged += OnTattooColorChanged;
        }


        protected override TattooDataManager.SerializableTattooData CreateTattooData(CustomSelectSet set) {
            if (set.id == 0)
                return null;

            var combinedTextureData = MakerAPI.GetMakerSex() == SEX.FEMALE
                ? CustomDataManager.GetFaceTattoo_Female(set.id)
                : CustomDataManager.GetFaceTattoo_Male(set.id);

            return new TattooDataManager.SerializableTattooData(
                combinedTextureData.id,
                combinedTextureData.assetbundleName,
                combinedTextureData.textureName,
                MakerAPI.GetMakerBase().face.colorChange_Tattoo.color,
                combinedTextureData.pos.x,
                combinedTextureData.pos.y,
                Vector2.zero,
                Vector2.one
            );
        }

        protected override void UpdateNativeGuiControlsState() {
            var editMode = MakerAPI.GetMakerBase();
            var set = editMode.thumnbs_faceTattoo.Find(e => e.id == (UnsavedChanges[ActiveTattooLayer]?.Id ?? 0));
            var thumbIndex = editMode.thumnbs_faceTattoo.FindIndex(e => e.id == (UnsavedChanges[ActiveTattooLayer]?.Id ?? 0));

            editMode.face.selSets_Tattoo.selectID = set?.id ?? -1;
            editMode.face.selSets_Tattoo.select.select.SelectNo = thumbIndex;
            editMode.face.selSets_Tattoo.toggle.dataName.text = set == null ? string.Empty : set.name;
            editMode.face.selSets_Tattoo.toggle.thumnbnailImage.sprite = set?.thumbnail_S;
            editMode.face.selSets_Tattoo.select.Close();

            editMode.face.colorChange_Tattoo.SetColor(UnsavedChanges[ActiveTattooLayer]?.TattooColor ?? Color.white);
            editMode.face.colorChange_Tattoo.colorUI.Close();

            Logger.LogDebug($"[UpdateNativeGuiControlsState()] Current head tattoo id is {editMode.face.selSets_Tattoo.selectID}");
        }
    }
}
