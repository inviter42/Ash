using System;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.Accessories;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.SwitchFemale;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.State;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Textures;
using Ash.Core.SceneManagement;
using Ash.GlobalUtils;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu
{
    internal class WearsMenuMain
    {
        internal readonly GameObject WearsMenuRoot;
        internal readonly WearsMenuStateControl StateControl;

        private GameObject FrostedGlassGameObj { get; set; }

        private WearAndAccessoryButtonsContainer FemaleButtonsContainer1;
        private WearAndAccessoryButtonsContainer FemaleButtonsContainer2;

        private GameObject WearAndAccessoryButtonsContainer { get; set; }

        private Text FemaleTextComponent;

        private Material IconMaskMaterial;
        private Material FrostedGlassMaterial;

        private readonly WearsMenuTextures Textures;
        private readonly WearsMenuConfig Config;

        private static readonly int FrostTex = Shader.PropertyToID("_FrostTex");
        private static readonly int FrostIntensity = Shader.PropertyToID("_FrostIntensity");
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Feather = Shader.PropertyToID("_Feather");

        internal WearsMenuMain(IuiMain iuiMain, GameObject canvasGameObj) {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null) {
                throw new Exception("Expected H_Scene is null.");
            }

            Config = IuiMain.WearsMenuConfig;
            Textures = new WearsMenuTextures();

            WearsMenuRoot = new GameObject("WearsMenuRoot");
            WearsMenuRoot.transform.SetParent(canvasGameObj.transform, false);

            IuiPositioningHelpers.AnchorsFillParent(WearsMenuRoot);

            StateControl = new WearsMenuStateControl(
                this,
                iuiMain
            );

            LoadIconMaskShader();
            CreateWearAndAccessoryButtonsContainer(WearsMenuRoot, hScene);
            CreateSwitchFemaleBlock(WearsMenuRoot, hScene);

            AssetBundleManagement.AssetBundleManager.UnloadWearThumbnailAssetBundles();
            AssetBundleManagement.AssetBundleManager.UnloadAccessoryThumbnailAssetBundles();

            WearsMenuRoot.SetActive(false);
        }

        ~WearsMenuMain() {
            Object.DestroyImmediate(IconMaskMaterial);
            Object.DestroyImmediate(FrostedGlassMaterial);
        }


        internal void RecalculateButtonsContainerBoundingBox() {
            WearAndAccessoryButtonsContainer.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            var bounds = TransformUtils.CalculateBoundingBoxSizeFromCenter(WearAndAccessoryButtonsContainer);
            var sizeDelta = new Vector2(bounds.size.x, bounds.size.y);

            WearAndAccessoryButtonsContainer.GetComponent<RectTransform>().sizeDelta =
                new Vector2(
                    Mathf.Clamp(sizeDelta.x, Config.ButtonsContainerMinWidth, Config.ButtonsContainerMaxWidth),
                    Mathf.Clamp(sizeDelta.y, Config.ButtonsContainerMinHeight, Config.ButtonsContainerMaxHeight)
                );
        }

        internal void UpdateFemaleText(bool fetchMain = false) {
            var scene = SceneTypeTracker.Scene as H_Scene;
            if (scene == null)
                throw new Exception("Expected H_Scene is null.");

            if (fetchMain) {
                FemaleTextComponent.text = FormatFemaleNameFromId(scene.mainMembers.GetFemale(0));
            }
            else {
                FemaleTextComponent.text =
                    FemaleTextComponent.text == FormatFemaleNameFromId(scene.mainMembers.GetFemale(0))
                        ? FormatFemaleNameFromId(scene.visitor.GetFemale())
                        : FormatFemaleNameFromId(scene.mainMembers.GetFemale(0));
            }
        }

        internal void UpdateContainerVisibilityAndRecalculateBounds(bool fetchMain = false) {
            var hScene = SceneTypeTracker.Scene as H_Scene;
            if (hScene == null)
                throw new Exception("Expected H_Scene is null.");

            var visibleContainer = FemaleButtonsContainer1.Root.activeSelf
                ? FemaleButtonsContainer1
                : FemaleButtonsContainer2;

            var hiddenContainer = visibleContainer == FemaleButtonsContainer1
                ? FemaleButtonsContainer2
                : FemaleButtonsContainer1;

            var isExpanded = visibleContainer?.InteractableRing.AccessoryContainer != null
                             && visibleContainer.InteractableRing.AccessoryContainer.activeSelf;

            if (fetchMain) {
                var mainFemale = hScene.mainMembers.GetFemale(0);

                FemaleButtonsContainer1.Root.SetActive(FemaleButtonsContainer1.HeroineId == mainFemale.heroineID);

                if (hScene.visitor != null)
                    FemaleButtonsContainer2?.Root.SetActive(FemaleButtonsContainer2.HeroineId == mainFemale.heroineID);

                visibleContainer = FemaleButtonsContainer1.Root.activeSelf
                    ? FemaleButtonsContainer1
                    : FemaleButtonsContainer2;

                hiddenContainer = visibleContainer == FemaleButtonsContainer1
                    ? FemaleButtonsContainer2
                    : FemaleButtonsContainer1;
            }
            else {
                visibleContainer?.Root.SetActive(false);
                hiddenContainer?.Root.SetActive(true);

                // ReSharper disable once SwapViaDeconstruction
                var tmp = hiddenContainer;
                hiddenContainer = visibleContainer;
                visibleContainer = tmp;
            }

            if (hScene.visitor != null) {
                if (hiddenContainer?.InteractableRing.AccessoryContainer == null) {
                    if (visibleContainer?.InteractableRing.AccessoryContainer == null)
                        return;

                    FrostedGlassGameObj.SetActive(visibleContainer.InteractableRing.AccessoryContainer.activeSelf);
                }
                else {
                    if (visibleContainer?.InteractableRing.AccessoryContainer == null) {
                        FrostedGlassGameObj.SetActive(false);
                    }
                    else {
                        visibleContainer?.InteractableRing.AccessoryContainer.SetActive(isExpanded);
                        FrostedGlassGameObj.SetActive(isExpanded);
                    }
                }
            }

            RecalculateButtonsContainerBoundingBox();
        }

        internal void RecreateContainerForFemale(Female female) {
            if (FemaleButtonsContainer1.HeroineId == female.HeroineID)
                RecreateContainer(ref FemaleButtonsContainer1, female);
            else
                RecreateContainer(ref FemaleButtonsContainer2, female);

            RecalculateButtonsContainerBoundingBox();
        }

        private void RecreateContainer(ref WearAndAccessoryButtonsContainer container, Female female) {
            var isVisible = container.Root.activeSelf;
            var isExpanded = container.InteractableRing.AccessoryContainer != null
                             && container.InteractableRing.AccessoryContainer.activeSelf;

            container.Dispose();

            container = new WearAndAccessoryButtonsContainer(
                this,
                Config,
                Textures,
                IconMaskMaterial,
                WearAndAccessoryButtonsContainer,
                FrostedGlassGameObj,
                female
            );

            container.Root.SetActive(isVisible);

            if (container.InteractableRing.AccessoryContainer != null) {
                container.InteractableRing.AccessoryContainer.SetActive(isExpanded);
                if (isVisible) FrostedGlassGameObj.SetActive(isExpanded);
            } else if (isVisible) {
                FrostedGlassGameObj.SetActive(false);
            }
        }


        private void CreateWearAndAccessoryButtonsContainer(GameObject parent, H_Scene scene) {
            WearAndAccessoryButtonsContainer = new GameObject("Buttons", typeof(RectTransform));
            WearAndAccessoryButtonsContainer.transform.SetParent(parent.transform, false);

            FrostedGlassGameObj = CreateFrostedGlass(WearAndAccessoryButtonsContainer);
            FrostedGlassGameObj.SetActive(false);

            FemaleButtonsContainer1 = new WearAndAccessoryButtonsContainer(
                this,
                Config,
                Textures,
                IconMaskMaterial,
                WearAndAccessoryButtonsContainer,
                FrostedGlassGameObj,
                scene.mainMembers.GetFemale(0)
            );

            if (scene.visitor != null) {
                FemaleButtonsContainer2 = new WearAndAccessoryButtonsContainer(
                    this,
                    Config,
                    Textures,
                    IconMaskMaterial,
                    WearAndAccessoryButtonsContainer,
                    FrostedGlassGameObj,
                    scene.visitor.GetFemale()
                );

                FemaleButtonsContainer2.Root.SetActive(false);
            }

            XButton.CreateXButton(
                Config,
                Textures,
                () => StateControl.ToggleMenuVisibility(),
                WearAndAccessoryButtonsContainer
            );

            RecalculateButtonsContainerBoundingBox();

            IuiPositioningHelpers.AnchorsCenterIn(WearAndAccessoryButtonsContainer, Config.ButtonsContainerOffset);
        }

        private void CreateSwitchFemaleBlock(GameObject parent, H_Scene hScene) {
            var root = new GameObject("SwitchFemale", typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter), typeof(LayoutElement));
            root.transform.SetParent(parent.transform, false);

            var verticalLayoutGroup = root.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childControlHeight = true;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.spacing = Config.SwitchFemaleContainerSpacing;
            verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            var fitter = root.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            FemaleTextComponent = CurrentFemaleText.CreateFemaleText(
                Config,
                root
            );

            if (hScene.visitor != null) {
                SwitchFemaleButton.CreateSwitchFemaleButton(
                    this,
                    Config,
                    Textures,
                    root
                );
            }

            IuiPositioningHelpers.AnchorsCenterIn(root, Config.SwitchFemaleContainerOffset);
        }


        private GameObject CreateFrostedGlass(GameObject parent) {
            var frostedGlassShader =
                Ash.AshUI.ImmersiveUIShadersAssetBundle.LoadAsset<Shader>(
                    "assets/frostedglass/shaders/frostedglass.shader");

            if (frostedGlassShader == null) {
                Ash.Logger.LogError($"{nameof(frostedGlassShader)} is null");
                return null;
            }

            if (Textures.FrostedGlassAtlasMask == null) {
                Ash.Logger.LogError($"{nameof(Textures.FrostedGlassAtlasMask)} is null");
                return null;
            }

            var root = new GameObject("FrostedGlass", typeof(RectTransform), typeof(Image),
                typeof(Alpha8RaycastFilter));
            root.transform.SetParent(parent.transform, false);

            var frostedGlassRectTransform = root.GetComponent<RectTransform>();
            frostedGlassRectTransform.sizeDelta = new Vector2(
                Textures.FrostedGlassAtlasMask.width, Textures.FrostedGlassAtlasMask.height
            );

            FrostedGlassMaterial = new Material(frostedGlassShader);
            FrostedGlassMaterial.SetTexture(FrostTex, Textures.FrostedGlassAtlasMask);
            FrostedGlassMaterial.SetFloat(FrostIntensity, IuiSettings.SubMenusFrostIntensity);

            var frostedGlassBackground = root.GetComponent<Image>();
            frostedGlassBackground.material = FrostedGlassMaterial;
            frostedGlassBackground.raycastTarget = true;

            var raycastFilter = root.GetComponent<Alpha8RaycastFilter>();
            raycastFilter.AlphaHitTestMinimumThreshold = WearsMenuConfig.AccessoryPointerEventZoneAlphaHitTestMinimumThreshold;
            raycastFilter.SampleTexture = Textures.FrostedGlassAtlasMask;
            raycastFilter.Image = frostedGlassBackground;

            var frostedGlassTint = new GameObject("FrostedGlassTint", typeof(RectTransform), typeof(RawImage));
            frostedGlassTint.transform.SetParent(root.transform, false);

            var tint = frostedGlassTint.GetComponent<RawImage>();
            tint.texture = Textures.FrostedGlassTintTexture;
            tint.SetNativeSize();

            IuiPositioningHelpers.AnchorsCenterIn(root, WearsMenuConfig.AccessoryContainerOffset, new Vector2(0, 0.5f));

            return root;
        }


        private void LoadIconMaskShader() {
            if (IconMaskMaterial != null)
                return;

            var circleMaskSdfShader =
                Ash.AshUI.ImmersiveUIShadersAssetBundle.LoadAsset<Shader>(
                    "assets/frostedglass/shaders/circlemasksdf.shader");

            IconMaskMaterial = new Material(circleMaskSdfShader);
            IconMaskMaterial.SetFloat(Radius, WearsMenuConfig.IconMaskRadius);
            IconMaskMaterial.SetFloat(Feather, WearsMenuConfig.IconMaskFeather);
        }

        private string FormatFemaleNameFromId(Female female) {
            return female
                .heroineID
                .ToString()
                .ToLower()
                .ToTitleCase();
        }
    }
}
