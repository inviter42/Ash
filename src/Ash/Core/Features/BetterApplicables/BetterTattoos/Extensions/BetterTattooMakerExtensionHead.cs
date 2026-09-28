using System;
using Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedHooks._CustomEdit;
using Ash.Logging;
using Character;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.BetterTattoos.Extensions
{
    internal class BetterTattooMakerExtensionHead : ApplicableMakerExtensionBase
    {
        protected override AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ExtDataTattoos);
        protected override MakerCategory MakerCategory => MakerConstants.Face.Tattoo;
        protected override string UICategoryRootGameObjectPath => "Canvas/Face/Mains/Tattoo";
        protected override float OffsetMinValue => -2048;
        protected override float OffsetMaxValue => 2048;
        protected override Part Part => Part.Head;

        protected override Action RenderSkinTexture => MakerAPI.GetMakerBase().human.head.RendSkinTexture;

        protected override string LayerSelectorsGroupId => "HeadTattooLayerSelectionGroup";


        protected override void SubscribeToEvents() {
            Ash.BetterTattooDataManager.ExtDataFromTheCard += UpdateDataAndGuiStates;
            Ash.BetterTattooDataManager.ExtDataFromVanilla += UpdateDataAndGuiStates;

            CustomEditHooks.FaceTattooChanged += OnSelectionChanged;
            CustomEditHooks.FaceTattooColorChanged += OnTintSelectionChanged;
        }

        protected override void UnsubscribeFromEvents() {
            Ash.BetterTattooDataManager.ExtDataFromTheCard -= UpdateDataAndGuiStates;
            Ash.BetterTattooDataManager.ExtDataFromVanilla -= UpdateDataAndGuiStates;

            CustomEditHooks.FaceTattooChanged -= OnSelectionChanged;
            CustomEditHooks.FaceTattooColorChanged += OnTintSelectionChanged;
        }


        protected override SerializableTextureData[] InitializeUnsavedChanges() {
            return Ash.BetterTattooDataManager.GetSerializableTextureDataListCopy(MakerAPI.GetMakerBase().human, Part)
                   ?? new SerializableTextureData[Ash.BetterTattooDataManager.NumberOfSlots];
        }

        protected override SerializableTextureData CreateSerializableData(CustomSelectSet set) {
            if (set.id == 0)
                return null;

            var combinedTextureData = MakerAPI.GetMakerSex() == SEX.FEMALE
                ? CustomDataManager.GetFaceTattoo_Female(set.id)
                : CustomDataManager.GetFaceTattoo_Male(set.id);

            return new SerializableTextureData(
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
            var set = editMode.thumnbs_faceTattoo.Find(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));
            var thumbIndex = editMode.thumnbs_faceTattoo.FindIndex(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));

            editMode.face.selSets_Tattoo.selectID = set?.id ?? -1;
            editMode.face.selSets_Tattoo.select.select.SelectNo = thumbIndex;
            editMode.face.selSets_Tattoo.toggle.dataName.text = set == null ? string.Empty : set.name;
            editMode.face.selSets_Tattoo.toggle.thumnbnailImage.sprite = set?.thumbnail_S;
            editMode.face.selSets_Tattoo.select.Close();

            editMode.face.colorChange_Tattoo.SetColor(UnsavedChanges[ActiveLayer]?.Tint ?? Color.white);
            editMode.face.colorChange_Tattoo.colorUI.Close();

            Logger.LogDebug($"[UpdateNativeGuiControlsState()] Current head tattoo id is {editMode.face.selSets_Tattoo.selectID}");
        }
    }
}
