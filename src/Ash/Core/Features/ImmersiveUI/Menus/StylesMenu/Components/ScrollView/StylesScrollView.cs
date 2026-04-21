using System.Collections.Generic;
using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Components.ScrollView
{
    internal static class StylesScrollView
    {
        private static readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;

        internal static List<GameObject> CreateScrollView(GameObject root, StylesMenuTextures textures) {
            // scroll view
            var scrollViewGameObject = new GameObject("ScrollView", typeof(RectTransform),
                typeof(CanvasRenderer), typeof(ScrollRect));
            scrollViewGameObject.transform.SetParent(root.transform, false);

            if (root.GetComponentInParent<GraphicRaycaster>() == null)
                root.transform.root.gameObject.AddComponent<GraphicRaycaster>();

            IuiPositioningHelpers.AnchorsFillParent(scrollViewGameObject);

            // viewport

            var viewportGameObj = CreateViewportGameObject(scrollViewGameObject);

            AddImageAndMask(viewportGameObj);

            var viewportRectTransform = AddViewportRectTransform(viewportGameObj);

            // content

            var contentGameObj = CreateContentGameObject(viewportGameObj);

            var contentRectTransform = AddContentRectTransform(contentGameObj);

            AddVerticalLayoutGroup(contentGameObj);

            AddContentSizeFitter(contentGameObj);

            var scrollRect = AddScrollRect(scrollViewGameObject, contentRectTransform, viewportRectTransform);

            StylesScrollBar.CreateScrollBar(scrollViewGameObject, scrollRect, textures);

            return new List<GameObject> { scrollViewGameObject, contentGameObj };
        }

        private static GameObject CreateViewportGameObject(GameObject scrollViewGameObject) {
            var viewportGameObj = new GameObject("ScrollViewViewport", typeof(RectTransform),
                typeof(CanvasRenderer), typeof(Image), typeof(Mask));
            viewportGameObj.transform.SetParent(scrollViewGameObject.transform, false);

            return viewportGameObj;
        }

        private static void AddImageAndMask(GameObject viewportGameObj) {
            viewportGameObj.GetComponent<Image>().raycastTarget = true;
            viewportGameObj.GetComponent<Mask>().showMaskGraphic = false;
        }

        private static RectTransform AddViewportRectTransform(GameObject viewportGameObj) {
            var viewportRectTransform = viewportGameObj.GetComponent<RectTransform>();
            IuiPositioningHelpers.AnchorsFillParent(viewportGameObj);

            return viewportRectTransform;
        }

        private static GameObject CreateContentGameObject(GameObject viewportGameObj) {
            var contentGameObj = new GameObject("ScrollViewContent", typeof(RectTransform),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            contentGameObj.transform.SetParent(viewportGameObj.transform, false);

            return contentGameObj;
        }

        private static RectTransform AddContentRectTransform(GameObject contentGameObj) {
            var contentRectTransform = contentGameObj.GetComponent<RectTransform>();
            contentRectTransform.anchorMin = new Vector2(0, 1);
            contentRectTransform.anchorMax = Vector2.one;
            contentRectTransform.pivot = new Vector2(0.5f, 1);

            return contentRectTransform;
        }

        private static void AddVerticalLayoutGroup(GameObject contentGameObj) {
            var verticalLayoutGroup = contentGameObj.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childControlHeight = true;
            verticalLayoutGroup.childForceExpandWidth = false;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
            verticalLayoutGroup.spacing = Config.StylesListItemSpacing;
            verticalLayoutGroup.padding = new RectOffset(
                -Config.ScrollbarLogicalComponentWidth/2 - Config.ScrollbarTextureWidth/2 -
                Config.BackgroundContainerMargins.left, 0,
                Config.StylesListPadding,
                Config.StylesListPadding); // offset for list items to be in a visual center
        }

        private static void AddContentSizeFitter(GameObject contentGameObj) {
            var contentSizeFitter = contentGameObj.GetComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private static ScrollRect AddScrollRect(GameObject scrollViewGameObject, RectTransform contentRectTransform,
            RectTransform viewportRectTransform) {
            // attach everything to ScrollRect
            var scrollRect = scrollViewGameObject.GetComponent<ScrollRect>();
            scrollRect.content = contentRectTransform;
            scrollRect.viewport = viewportRectTransform;
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 25f;

            return scrollRect;
        }
    }
}
