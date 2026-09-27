using System;
using Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedHooks._CustomEdit;
using Ash.Logging;
using Character;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.BetterCheekShadows.Extensions
{
    internal class BetterCheekShadowMakerExtensionHead : ApplicableMakerExtensionBase
    {
        protected override AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ExtDataCheekShadows);
        protected override MakerCategory MakerCategory => MakerConstants.Face.Makeup;
        protected override int SubCategoryIndex => 1;
        protected override string UICategoryRootGameObjectPath => "EditMode/Canvas/Face/Mains/Makeup";
        protected override float OffsetMinValue => -2048;
        protected override float OffsetMaxValue => 2048;
        protected override Part Part => Part.Head;

        protected override Action RenderSkinTexture => MakerAPI.GetMakerBase().human.head.RendSkinTexture;

        protected override string LayerSelectorsGroupId => "HeadCheekShadowLayerSelectionGroup";


        protected override void SubscribeToEvents() {
            Ash.BetterCheekShadowDataManager.ExtDataFromTheCard += UpdateDataAndGuiStates;
            Ash.BetterCheekShadowDataManager.ExtDataFromVanilla += UpdateDataAndGuiStates;

            CustomEditHooks.CheekShadowChanged += OnSelectionChanged;
            CustomEditHooks.CheekShadowColorChanged += OnTintSelectionChanged;
        }

        protected override void UnsubscribeFromEvents() {
            Ash.BetterCheekShadowDataManager.ExtDataFromTheCard -= UpdateDataAndGuiStates;
            Ash.BetterCheekShadowDataManager.ExtDataFromVanilla -= UpdateDataAndGuiStates;

            CustomEditHooks.CheekShadowChanged -= OnSelectionChanged;
            CustomEditHooks.CheekShadowColorChanged += OnTintSelectionChanged;
        }


        protected override SerializableTextureData[] InitializeUnsavedChanges() {
            return Ash.BetterCheekShadowDataManager.GetSerializableTextureDataListCopy(MakerAPI.GetMakerBase().human, Part)
                   ?? new SerializableTextureData[Ash.BetterCheekShadowDataManager.NumberOfSlots];
        }

        protected override SerializableTextureData CreateSerializableData(CustomSelectSet set) {
            if (set.id == 0)
                return null;

            var combinedTextureData = MakerAPI.GetMakerSex() == SEX.FEMALE
                // ReSharper disable once ConditionalTernaryEqualBranch
                ? CustomDataManager.GetCheek(set.id)
                : CustomDataManager.GetCheek(set.id); // if we want to add cheek-shadows to males, it needs a custom getter!

            return new SerializableTextureData(
                combinedTextureData.id,
                combinedTextureData.assetbundleName,
                combinedTextureData.textureName,
                MakerAPI.GetMakerBase().face.colorChange_Cheek.color,
                combinedTextureData.pos.x,
                combinedTextureData.pos.y,
                Vector2.zero,
                Vector2.one
            );
        }

        protected override void UpdateNativeGuiControlsState() {
            var editMode = MakerAPI.GetMakerBase();
            var set = editMode.thumnbs_cheek.Find(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));
            var thumbIndex = editMode.thumnbs_cheek.FindIndex(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));

            editMode.face.selSets_Cheek.selectID = set?.id ?? -1;
            editMode.face.selSets_Cheek.select.select.SelectNo = thumbIndex;
            editMode.face.selSets_Cheek.toggle.dataName.text = set == null ? string.Empty : set.name;
            editMode.face.selSets_Cheek.toggle.thumnbnailImage.sprite = set?.thumbnail_S;
            editMode.face.selSets_Cheek.select.Close();

            editMode.face.colorChange_Cheek.SetColor(UnsavedChanges[ActiveLayer]?.Tint ?? Color.white);
            editMode.face.colorChange_Cheek.colorUI.Close();

            Logger.LogDebug($"[UpdateNativeGuiControlsState()] Current head cheek-shadow id is {editMode.face.selSets_Cheek.selectID}");
        }
    }
}
