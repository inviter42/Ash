using System;
using Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedHooks._CustomEdit;
using Ash.Logging;
using Character;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.BetterEyeShadows.Extensions
{
    internal class BetterEyeShadowMakerExtensionHead : ApplicableMakerExtensionBase
    {
        protected override AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ExtDataEyeShadows);
        protected override MakerCategory MakerCategory => MakerConstants.Face.Makeup;
        protected override string UICategoryRootGameObjectPath => "Canvas/Face/Mains/Makeup";
        protected override float OffsetMinValue => -2048;
        protected override float OffsetMaxValue => 2048;
        protected override Part Part => Part.Head;

        protected override Action RenderSkinTexture => MakerAPI.GetMakerBase().human.head.RendSkinTexture;

        protected override string LayerSelectorsGroupId => "HeadEyeShadowLayerSelectionGroup";


        protected override void SubscribeToEvents() {
            Ash.BetterEyeShadowDataManager.ExtDataFromTheCard += UpdateDataAndGuiStates;
            Ash.BetterEyeShadowDataManager.ExtDataFromVanilla += UpdateDataAndGuiStates;

            CustomEditHooks.EyeShadowChanged += OnSelectionChanged;
            CustomEditHooks.EyeShadowColorChanged += OnTintSelectionChanged;
        }

        protected override void UnsubscribeFromEvents() {
            Ash.BetterEyeShadowDataManager.ExtDataFromTheCard -= UpdateDataAndGuiStates;
            Ash.BetterEyeShadowDataManager.ExtDataFromVanilla -= UpdateDataAndGuiStates;

            CustomEditHooks.EyeShadowChanged -= OnSelectionChanged;
            CustomEditHooks.EyeShadowColorChanged += OnTintSelectionChanged;
        }


        protected override SerializableTextureData[] InitializeUnsavedChanges() {
            return Ash.BetterEyeShadowDataManager.GetSerializableTextureDataListCopy(MakerAPI.GetMakerBase().human, Part)
                   ?? new SerializableTextureData[Ash.BetterEyeShadowDataManager.NumberOfSlots];
        }

        protected override SerializableTextureData CreateSerializableData(CustomSelectSet set) {
            if (set.id == 0)
                return null;

            var combinedTextureData = MakerAPI.GetMakerSex() == SEX.FEMALE
                // ReSharper disable once ConditionalTernaryEqualBranch
                ? CustomDataManager.GetEyeShadow(set.id)
                : CustomDataManager.GetEyeShadow(set.id); // if we want to add eye-shadows to males, it needs a custom getter!

            return new SerializableTextureData(
                combinedTextureData.id,
                combinedTextureData.assetbundleName,
                combinedTextureData.textureName,
                MakerAPI.GetMakerBase().face.colorChange_EyeShadow.color,
                combinedTextureData.pos.x,
                combinedTextureData.pos.y,
                Vector2.zero,
                Vector2.one
            );
        }

        protected override void UpdateNativeGuiControlsState() {
            var editMode = MakerAPI.GetMakerBase();
            var set = editMode.thumnbs_eyeshadow.Find(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));
            var thumbIndex = editMode.thumnbs_eyeshadow.FindIndex(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));

            editMode.face.selSets_EyeShadow.selectID = set?.id ?? -1;
            editMode.face.selSets_EyeShadow.select.select.SelectNo = thumbIndex;
            editMode.face.selSets_EyeShadow.toggle.dataName.text = set == null ? string.Empty : set.name;
            editMode.face.selSets_EyeShadow.toggle.thumnbnailImage.sprite = set?.thumbnail_S;
            editMode.face.selSets_EyeShadow.select.Close();

            editMode.face.colorChange_EyeShadow.SetColor(UnsavedChanges[ActiveLayer]?.Tint ?? Color.white);
            editMode.face.colorChange_EyeShadow.colorUI.Close();

            Logger.LogDebug($"[UpdateNativeGuiControlsState()] Current head eye-shadow id is {editMode.face.selSets_EyeShadow.selectID}");
        }
    }
}
