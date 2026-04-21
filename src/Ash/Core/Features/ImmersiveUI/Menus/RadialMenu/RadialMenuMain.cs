using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.GagMenu;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.State;
using Ash.Core.Features.ImmersiveUI.Menus.RadialMenu.Textures;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu;
using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu;
using Ash.GlobalUtils;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Ash.Core.Features.ImmersiveUI.Menus.RadialMenu
{
    internal class RadialMenuMain
    {
        internal GameObject MiddleHighlightGameObject;
        internal RectTransform MiddleHighlightRectTransform;
        internal GameObject SectorHighlightGameObject;
        internal RectTransform SectorHighlightRectTransform;
        internal GameObject CancelGroupGameObject;
        internal Text ActiveElementTextComponent;

        internal readonly GameObject RadialMenuRootGameObject;
        internal readonly RadialMenuStateControl StateControl;

        internal readonly List<RawImage> IconTextures = new List<RawImage>();
        internal readonly List<GameObject> IconActiveStrokeGameObjects = new List<GameObject>();

        internal readonly List<string> IconPathsFromBundle;

        private readonly RadialMenuTextures Textures;

        private readonly RadialMenuConfig Config;

        private Material FrostedGlassMaterial;

        private const string CancelText = "CANCEL";

        private static readonly int FrostTex = Shader.PropertyToID("_FrostTex");
        private static readonly int FrostIntensity = Shader.PropertyToID("_FrostIntensity");

        private static readonly List<string> IconsSortingOrder = new List<string> {
            "piemenu-repeat-2",
            "piemenu-shirt",
            "piemenu-message-circle-x",
            "piemenu-log-out",
            "piemenu-cog",
            "piemenu-heart",
        };

        internal RadialMenuMain(
            IuiMain iuiMain,
            GameObject canvasGameObj,
            StylesMenuMain stylesMenuMain,
            GagMenuMain gagMenuMain,
            WearsMenuMain wearsMenuMain
        ) {
            Config = IuiMain.RadialMenuConfig;

            IconPathsFromBundle = SortListByReference(
                IconsSortingOrder,
                AssetBundleUtils.GetPathsFromBundle(Ash.AshUI.ImmersiveUIIconsAssetBundle, "piemenu-")
            );

            Textures = new RadialMenuTextures(IconPathsFromBundle);

            var canvas = canvasGameObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.planeDistance = 0.5f;

            RadialMenuRootGameObject = new GameObject("RadialMenuRoot", typeof(RectTransform));
            RadialMenuRootGameObject.transform.SetParent(canvasGameObj.transform, false);

            IuiPositioningHelpers.AnchorsFillParent(RadialMenuRootGameObject);

            // frosted glass bg
            var frostedGlassGameObj = CreateFrostedGlass(RadialMenuRootGameObject);

            // create active element sector highlight
            CreateSectorHighlight(frostedGlassGameObj);

            // icons
            PopulatePieWithIcons(frostedGlassGameObj);

            // middle background
            var middleBackgroundGameObj = CreateMiddleBackground(RadialMenuRootGameObject);

            // populate middle background
            PopulateMiddleBackground(middleBackgroundGameObj);

            CreateMiddleHighlight(RadialMenuRootGameObject);

            RadialMenuRootGameObject.SetActive(false);

            StateControl = new RadialMenuStateControl(
                iuiMain,
                canvasGameObj,
                this,
                stylesMenuMain,
                gagMenuMain,
                wearsMenuMain
            );
        }

        ~RadialMenuMain() {
            Object.Destroy(FrostedGlassMaterial);
        }

        private GameObject CreateFrostedGlass(GameObject root) {
            var frostedGlassShader =
                Ash.AshUI.ImmersiveUIShadersAssetBundle.LoadAsset<Shader>(
                    "assets/frostedglass/shaders/frostedglass.shader");

            if (frostedGlassShader == null) {
                Ash.Logger.LogError($"{nameof(frostedGlassShader)} is null");
                return null;
            }

            if (Textures.FrostedGlassMask == null) {
                Ash.Logger.LogError($"{nameof(Textures.FrostedGlassMask)} is null");
                return null;
            }

            var frostedGlassGameObj = new GameObject("FrostedGlass", typeof(RectTransform), typeof(RawImage));
            frostedGlassGameObj.transform.SetParent(root.transform, true);

            var frostedGlassRectTransform = frostedGlassGameObj.GetComponent<RectTransform>();
            frostedGlassRectTransform.sizeDelta = new Vector2(
                Textures.FrostedGlassMask.width, Textures.FrostedGlassMask.height
            );

            FrostedGlassMaterial = new Material(frostedGlassShader);
            FrostedGlassMaterial.SetTexture(FrostTex, Textures.FrostedGlassMask);
            FrostedGlassMaterial.SetFloat(FrostIntensity, IuiSettings.RadialMenuFrostIntensity);

            var frostedRingBackground = frostedGlassGameObj.GetComponent<RawImage>();
            frostedRingBackground.material = FrostedGlassMaterial;

            IuiPositioningHelpers.AnchorsCenterIn(frostedGlassGameObj);

            var tintGameObj = new GameObject("FrostedGlassTint", typeof(RawImage));
            tintGameObj.transform.SetParent(frostedGlassGameObj.transform, false);

            var frostGlassTint = tintGameObj.GetComponent<RawImage>();
            frostGlassTint.texture = Textures.FrostGlassTintTexture;
            frostGlassTint.SetNativeSize();

            IuiPositioningHelpers.AnchorsCenterIn(tintGameObj);

            return frostedGlassGameObj;
        }

        private GameObject CreateMiddleBackground(GameObject root) {
            var middleBackgroundGameObj = new GameObject("MiddleCircle", typeof(RawImage));
            middleBackgroundGameObj.transform.SetParent(root.transform, false);

            var middleBackground = middleBackgroundGameObj.GetComponent<RawImage>();
            middleBackground.texture = Textures.MiddleBackgroundTexture;
            middleBackground.SetNativeSize();

            IuiPositioningHelpers.AnchorsCenterIn(middleBackgroundGameObj);

            return middleBackgroundGameObj;
        }

        private void CreateMiddleHighlight(GameObject root) {
            var middleHighlightGameObj = new GameObject("MiddleHighlight", typeof(RectTransform), typeof(RawImage));
            middleHighlightGameObj.transform.SetParent(root.transform, false);

            MiddleHighlightRectTransform = middleHighlightGameObj.GetComponent<RectTransform>();

            var middleHighlight = middleHighlightGameObj.GetComponent<RawImage>();
            middleHighlight.texture = Textures.MiddleHighlightTexture;
            middleHighlight.SetNativeSize();

            IuiPositioningHelpers.AnchorsCenterIn(middleHighlightGameObj);

            MiddleHighlightGameObject = middleHighlightGameObj;
        }

        private void PopulatePieWithIcons(GameObject parent) {
            var bundle = Ash.AshUI.ImmersiveUIIconsAssetBundle;
            // ReSharper disable once ConvertClosureToMethodGroup
            var model = IconPathsFromBundle
                .Select(path => bundle.LoadAsset<Texture2D>(path))
                .ToList();

            // middle-point between outer and inner radii in pixels
            var radius = Config.FrostedGlassInnerDiameterSize.Width / 2 +
                         (Config.FrostedGlassOuterDiameterSize.Width - Config.FrostedGlassInnerDiameterSize.Width) / 4;

            for (var i = 0; i < model.Count; i++) {
                // icon bg
                var iconName = Path.GetFileNameWithoutExtension(IconPathsFromBundle[i])
                    .Replace("piemenu-", "")
                    .ToTitleCase();

                var iconBgGameObj =
                    new GameObject($"IconBackground{iconName}", typeof(RectTransform), typeof(RawImage));
                iconBgGameObj.transform.SetParent(parent.transform, false);

                var iconBg = iconBgGameObj.GetComponent<RawImage>();
                iconBg.texture = Textures.IconBackgroundTexture;
                iconBg.SetNativeSize();

                // active stroke setup
                var activeElementStrokeGameObj = new GameObject($"IconActiveElementStroke{iconName}",
                    typeof(RectTransform), typeof(RawImage));

                activeElementStrokeGameObj.transform.SetParent(iconBgGameObj.transform, false);

                var activeElementStroke = activeElementStrokeGameObj.GetComponent<RawImage>();
                activeElementStroke.texture = Textures.ActiveElementStrokeTexture;
                activeElementStroke.SetNativeSize();

                activeElementStrokeGameObj.SetActive(false); // disable by default

                IconActiveStrokeGameObjects.Add(activeElementStrokeGameObj);

                // icon
                var iconGameObj = new GameObject($"Icon{iconName}", typeof(RectTransform), typeof(RawImage));
                iconGameObj.transform.SetParent(iconBgGameObj.transform, false);

                var icon = iconGameObj.GetComponent<RawImage>();
                icon.texture = model[i];

                var iconRectTransform = icon.GetComponent<RectTransform>();
                iconRectTransform.sizeDelta = new Vector2(Config.IconSize, Config.IconSize);

                ImageUtils.SetRawImageTransparency(icon, RadialMenuConfig.InactiveIconTransparencyValue);

                IconTextures.Add(icon);

                // angle for current icon starting from the top, winds clockwise
                var theta = Mathf.PI / 2 - 2 * Mathf.PI * i / model.Count;

                var x = Mathf.Cos(theta) * radius;
                var y = Mathf.Sin(theta) * radius;

                IuiPositioningHelpers.AnchorsCenterIn(iconBgGameObj, new Vector2(x, y));
            }
        }

        private void PopulateMiddleBackground(GameObject root) {
            var middleActionTextGameObj = new GameObject("MiddleActionText", typeof(ContentSizeFitter), typeof(Text));
            middleActionTextGameObj.transform.SetParent(root.transform, false);

            var middleActionTextFitter = middleActionTextGameObj.GetComponent<ContentSizeFitter>();
            middleActionTextFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            middleActionTextFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var middleActionTextComp = middleActionTextGameObj.GetComponent<Text>();
            middleActionTextComp.text = "";
            middleActionTextComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>("assets/frostedglass/fonts/tangerine-bold.ttf");
            middleActionTextComp.fontSize = Config.MiddleTextFontSize;
            middleActionTextComp.fontStyle = FontStyle.Italic;
            middleActionTextComp.alignment = TextAnchor.MiddleCenter;
            middleActionTextComp.color = ColorUtils.Color32Af(219, 207, 227);

            IuiPositioningHelpers.AnchorsCenterIn(middleActionTextGameObj);

            ActiveElementTextComponent = middleActionTextComp;

            var cancelGroupGameObj = new GameObject("MiddleCancelGroup", typeof(HorizontalLayoutGroup),
                typeof(ContentSizeFitter));
            cancelGroupGameObj.transform.SetParent(root.transform, false);

            var cancelGroupLayoutGroup = cancelGroupGameObj.GetComponent<HorizontalLayoutGroup>();
            cancelGroupLayoutGroup.childControlWidth = true;
            cancelGroupLayoutGroup.childControlHeight = true;
            cancelGroupLayoutGroup.childForceExpandWidth = false;
            cancelGroupLayoutGroup.childForceExpandHeight = false;
            cancelGroupLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            cancelGroupLayoutGroup.spacing = 4;

            var cancelGroupFitter = cancelGroupGameObj.GetComponent<ContentSizeFitter>();
            cancelGroupFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            cancelGroupFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var cancelIconGameObj = new GameObject("MiddleCancelImg", typeof(LayoutElement), typeof(RawImage));
            cancelIconGameObj.transform.SetParent(cancelGroupGameObj.transform, false);

            var cancelIcon = cancelIconGameObj.GetComponent<RawImage>();
            cancelIcon.texture =
                Ash.AshUI.ImmersiveUIIconsAssetBundle.LoadAsset<Texture2D>(
                    "assets/frostedglass/icons/mouse-right-2.png");

            var cancelIconLayoutElement = cancelIconGameObj.GetComponent<LayoutElement>();
            cancelIconLayoutElement.preferredWidth = Config.CancelGroupIconDimensions.Width;
            cancelIconLayoutElement.preferredHeight = Config.CancelGroupIconDimensions.Height;

            var cancelTextGameObj = new GameObject("MiddleCancelText", typeof(Text));
            cancelTextGameObj.transform.SetParent(cancelGroupGameObj.transform, false);

            var cancelTextComp = cancelTextGameObj.GetComponent<Text>();
            cancelTextComp.text = CancelText;
            cancelTextComp.fontSize = Config.CancelTextFontSize;
            cancelTextComp.color = ColorUtils.Color32Af(219, 207, 227);
            cancelTextComp.alignment = TextAnchor.MiddleCenter;
            cancelTextComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>("assets/frostedglass/fonts/corbel-regular.ttf");

            IuiPositioningHelpers.AnchorsCenterIn(cancelGroupGameObj, Config.CancelGroupPositionOffset);

            cancelGroupGameObj.SetActive(false);

            CancelGroupGameObject = cancelGroupGameObj;
        }

        private void CreateSectorHighlight(GameObject root) {
            var sectorHighlightGameObj = new GameObject("SectorHighlight", typeof(RectTransform), typeof(RawImage));
            sectorHighlightGameObj.transform.SetParent(root.transform, false);

            SectorHighlightRectTransform = sectorHighlightGameObj.GetComponent<RectTransform>();

            var sectorHighlight = sectorHighlightGameObj.GetComponent<RawImage>();
            sectorHighlight.texture = Textures.SectorHighlightTexture;
            sectorHighlight.SetNativeSize();

            IuiPositioningHelpers.AnchorsCenterIn(sectorHighlightGameObj);

            sectorHighlightGameObj.SetActive(false);

            SectorHighlightGameObject = sectorHighlightGameObj;
        }

        private static List<string> SortListByReference(List<string> reference, List<string> listToSort) {
            return reference.Concat(
                listToSort
                .Select(Path.GetFileNameWithoutExtension)
                .Where(fileName => !reference.Contains(fileName))
            ).ToList();
        }
    }
}
