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
    internal class BetterTattooMakerExtensionBody : ApplicableMakerExtensionBase
    {
        protected override AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ExtDataTattoos);
        protected override MakerCategory MakerCategory => MakerConstants.Body.Tattoo;
        protected override string UICategoryRootGameObjectPath => "Canvas/Body/Mains/Tattoo";
        protected override float OffsetMinValue => -4096;
        protected override float OffsetMaxValue => 4096;
        protected override Part Part => Part.Body;

        protected override Action RenderSkinTexture => MakerAPI.GetMakerBase().human.body.RendSkinTexture;

        protected override string LayerSelectorsGroupId => "BodyTattooLayerSelectionGroup";


        protected override void SubscribeToEvents() {
            Ash.BetterTattooDataManager.ExtDataFromTheCard += UpdateDataAndGuiStates;
            Ash.BetterTattooDataManager.ExtDataFromVanilla += UpdateDataAndGuiStates;

            CustomEditHooks.BodyTattooChanged += OnSelectionChanged;
            CustomEditHooks.BodyTattooColorChanged += OnTintSelectionChanged;
        }

        protected override void UnsubscribeFromEvents() {
            Ash.BetterTattooDataManager.ExtDataFromTheCard -= UpdateDataAndGuiStates;
            Ash.BetterTattooDataManager.ExtDataFromVanilla -= UpdateDataAndGuiStates;

            CustomEditHooks.BodyTattooChanged -= OnSelectionChanged;
            CustomEditHooks.BodyTattooColorChanged += OnTintSelectionChanged;
        }


        protected override SerializableTextureData[] InitializeUnsavedChanges() {
            return Ash.BetterTattooDataManager.GetSerializableTextureDataListCopy(MakerAPI.GetMakerBase().human, Part)
                   ?? new SerializableTextureData[Ash.BetterTattooDataManager.NumberOfSlots];
        }

        protected override SerializableTextureData CreateSerializableData(CustomSelectSet set) {
            if (set.id == 0)
                return null;

            var combinedTextureData = MakerAPI.GetMakerSex() == SEX.FEMALE
                ? CustomDataManager.GetBodyTattoo_Female(set.id)
                : CustomDataManager.GetBodyTattoo_Male(set.id);

            return new SerializableTextureData(
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
            var set = editMode.thumnbs_bodyTattoo.Find(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));
            var thumbIndex = editMode.thumnbs_bodyTattoo.FindIndex(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));

            editMode.body.selSets_Tattoo.selectID = set?.id ?? -1;
            editMode.body.selSets_Tattoo.select.select.SelectNo = thumbIndex;
            editMode.body.selSets_Tattoo.toggle.dataName.text = set == null ? string.Empty : set.name;
            editMode.body.selSets_Tattoo.toggle.thumnbnailImage.sprite = set?.thumbnail_S;
            editMode.body.selSets_Tattoo.select.Close();

            editMode.body.color_tattoo.SetColor(UnsavedChanges[ActiveLayer]?.Tint ?? Color.white);
            editMode.body.color_tattoo.colorUI.Close();

            Logger.LogDebug($"[UpdateNativeGuiControlsState()] Current body tattoo id is {editMode.body.selSets_Tattoo.selectID}");
        }
    }
}
