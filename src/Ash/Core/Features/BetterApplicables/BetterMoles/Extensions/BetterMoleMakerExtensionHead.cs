using System;
using Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions;
using Ash.Core.Features.BetterApplicables.BaseComponents.Managers.Shared;
using Ash.Core.Features.BetterApplicables.SharedHooks._CustomEdit;
using Ash.Logging;
using Character;
using KKAPI.Maker;
using UnityEngine;

namespace Ash.Core.Features.BetterApplicables.BetterMoles.Extensions
{
    internal class BetterMoleMakerExtensionHead : ApplicableMakerExtensionBase
    {
        protected override AshLogger Logger { get; } = new AshLogger(LoggingSettings.LoggingModules.ExtDataMoles);
        protected override MakerCategory MakerCategory => MakerConstants.Face.Makeup;
        protected override string UICategoryRootGameObjectPath => "EditMode/Canvas/Face/Mains/Mole";
        protected override float OffsetMinValue => -2048;
        protected override float OffsetMaxValue => 2048;
        protected override Part Part => Part.Head;

        protected override Action RenderSkinTexture => MakerAPI.GetMakerBase().human.head.RendSkinTexture;

        protected override string LayerSelectorsGroupId => "HeadMoleLayerSelectionGroup";


        protected override void SubscribeToEvents() {
            Ash.BetterMoleDataManager.ExtDataFromTheCard += UpdateDataAndGuiStates;
            Ash.BetterMoleDataManager.ExtDataFromVanilla += UpdateDataAndGuiStates;

            CustomEditHooks.MoleChanged += OnSelectionChanged;
            CustomEditHooks.MoleColorChanged += OnTintSelectionChanged;
        }

        protected override void UnsubscribeFromEvents() {
            Ash.BetterMoleDataManager.ExtDataFromTheCard -= UpdateDataAndGuiStates;
            Ash.BetterMoleDataManager.ExtDataFromVanilla -= UpdateDataAndGuiStates;

            CustomEditHooks.MoleChanged -= OnSelectionChanged;
            CustomEditHooks.MoleColorChanged += OnTintSelectionChanged;
        }


        protected override SerializableTextureData[] InitializeUnsavedChanges() {
            return Ash.BetterMoleDataManager.GetSerializableTextureDataListCopy(MakerAPI.GetMakerBase().human, Part)
                   ?? new SerializableTextureData[Ash.BetterMoleDataManager.NumberOfSlots];
        }

        protected override SerializableTextureData CreateSerializableData(CustomSelectSet set) {
            if (set.id == 0)
                return null;

            var combinedTextureData = MakerAPI.GetMakerSex() == SEX.FEMALE
                // ReSharper disable once ConditionalTernaryEqualBranch
                ? CustomDataManager.GetMole(set.id)
                : CustomDataManager.GetMole(set.id); // if we want to add eye-shadows to males, it needs a custom getter!

            return new SerializableTextureData(
                combinedTextureData.id,
                combinedTextureData.assetbundleName,
                combinedTextureData.textureName,
                MakerAPI.GetMakerBase().face.colorChange_Mole.color,
                combinedTextureData.pos.x,
                combinedTextureData.pos.y,
                Vector2.zero,
                Vector2.one
            );
        }

        protected override void UpdateNativeGuiControlsState() {
            var editMode = MakerAPI.GetMakerBase();
            var set = editMode.thumnbs_mole.Find(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));
            var thumbIndex = editMode.thumnbs_mole.FindIndex(e => e.id == (UnsavedChanges[ActiveLayer]?.Id ?? 0));

            editMode.face.selSets_Mole.selectID = set?.id ?? -1;
            editMode.face.selSets_Mole.select.select.SelectNo = thumbIndex;
            editMode.face.selSets_Mole.toggle.dataName.text = set == null ? string.Empty : set.name;
            editMode.face.selSets_Mole.toggle.thumnbnailImage.sprite = set?.thumbnail_S;
            editMode.face.selSets_Mole.select.Close();

            editMode.face.colorChange_Mole.SetColor(UnsavedChanges[ActiveLayer]?.Tint ?? Color.white);
            editMode.face.colorChange_Mole.colorUI.Close();

            Logger.LogDebug($"[UpdateNativeGuiControlsState()] Current head mole id is {editMode.face.selSets_Mole.selectID}");
        }
    }
}
