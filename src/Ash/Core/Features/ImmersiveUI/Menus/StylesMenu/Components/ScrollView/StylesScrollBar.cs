using Ash.Core.Features.ImmersiveUI.Extras.Helpers.UiPositioning;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Config;
using Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Textures;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.StylesMenu.Components.ScrollView
{
    internal static class StylesScrollBar
    {
        private static readonly StylesMenuConfig Config = IuiMain.StylesMenuConfig;

        internal static void CreateScrollBar(GameObject scrollGameObject, ScrollRect scrollRect, StylesMenuTextures textures) {
            var scrollbarGameObj = new GameObject("VerticalScrollbar", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Image), typeof(Scrollbar));
            scrollbarGameObj.transform.SetParent(scrollGameObject.transform, false);

            AddScrollbarRectTransform(scrollbarGameObj);

            AddScrollbarBackgroundDummyImage(scrollbarGameObj);

            var scrollbarBackgroundSpriteGameObj = CreateScrollbarBackgroundSpriteGameObject(scrollbarGameObj);

            AddScrollbarBackgroundImage(scrollbarBackgroundSpriteGameObj, textures);

            AddScrollbarBackgroundRectTransform(scrollbarBackgroundSpriteGameObj);

            var slidingAreaGameObj = CreateSlidingAreaGameObject(scrollbarGameObj);

            var handleGameObj = CreateHandleGameObject(slidingAreaGameObj);

            AddHandleRectTransform(handleGameObj);

            var visualHandle = CreateVisualHandleGameObject(handleGameObj);

            AddHandleImage(visualHandle, textures);

            AddVisualRectTransform(visualHandle);

            CreateScrollbarComponent(scrollbarGameObj, handleGameObj, scrollRect);
        }

        private static void AddScrollbarRectTransform(GameObject scrollbarGameObj) {
            var scrollbarRect = scrollbarGameObj.GetComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1, 0);
            scrollbarRect.anchorMax = Vector2.one;
            scrollbarRect.pivot = Vector2.one;
            scrollbarRect.sizeDelta = new Vector2(Config.ScrollbarLogicalComponentWidth, 0);
        }

        private static void AddScrollbarBackgroundDummyImage(GameObject scrollbarGameObj) {
            var scrollbarBackgroundDummyImage = scrollbarGameObj.GetComponent<Image>();
            scrollbarBackgroundDummyImage.color = Color.clear;
        }

        private static GameObject CreateScrollbarBackgroundSpriteGameObject(GameObject scrollbarGameObj) {
            var scrollbarBackgroundSpriteGameObj =
                new GameObject("ScrollbarBackgroundSprite", typeof(RectTransform), typeof(Image));
            scrollbarBackgroundSpriteGameObj.transform.SetParent(scrollbarGameObj.transform, false);

            return scrollbarBackgroundSpriteGameObj;
        }

        private static void AddScrollbarBackgroundImage(GameObject scrollbarBackgroundSpriteGameObj, StylesMenuTextures textures) {
            var scrollbarBackgroundImage = scrollbarBackgroundSpriteGameObj.GetComponent<Image>();
            scrollbarBackgroundImage.sprite = CreateScrollbarBackgroundSprite(textures.ScrollbarBackgroundTexture);
            scrollbarBackgroundImage.type = Image.Type.Sliced;
        }

        private static void AddScrollbarBackgroundRectTransform(GameObject scrollbarBackgroundSpriteGameObj) {
            var scrollbarBackgroundRectTransform = scrollbarBackgroundSpriteGameObj.GetComponent<RectTransform>();
            scrollbarBackgroundRectTransform.anchorMin = new Vector2(0.5f, 0);
            scrollbarBackgroundRectTransform.anchorMax = new Vector2(0.5f, 1);
            scrollbarBackgroundRectTransform.sizeDelta = new Vector2(Config.ScrollbarTextureWidth, 0);
        }

        private static GameObject CreateSlidingAreaGameObject(GameObject scrollbarGameObj) {
            var slidingAreaGameObj = new GameObject("SlidingArea", typeof(RectTransform));
            slidingAreaGameObj.transform.SetParent(scrollbarGameObj.transform, false);
            IuiPositioningHelpers.AnchorsFillParent(slidingAreaGameObj);

            return slidingAreaGameObj;
        }

        private static GameObject CreateHandleGameObject(GameObject slidingAreaGameObj) {
            var handleGameObj = new GameObject("HandleContainer", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Image));
            handleGameObj.transform.SetParent(slidingAreaGameObj.transform, false);

            handleGameObj.GetComponent<Image>().color = Color.clear;

            return handleGameObj;
        }

        private static void AddHandleRectTransform(GameObject handleGameObj) {
            var handleRectTransform = handleGameObj.GetComponent<RectTransform>();
            handleRectTransform.sizeDelta = Vector2.zero;
        }

        private static GameObject CreateVisualHandleGameObject(GameObject handleGameObj) {
            var visualHandle = new GameObject("VisualHandle", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Image));
            visualHandle.transform.SetParent(handleGameObj.transform, false);

            return visualHandle;
        }

        private static void AddHandleImage(GameObject visualHandle, StylesMenuTextures textures) {
            var handleImage = visualHandle.GetComponent<Image>();
            handleImage.sprite = CreateScrollHandleSprite(textures.ScrollbarHandleTexture);
            handleImage.type = Image.Type.Sliced;
        }

        private static void AddVisualRectTransform(GameObject visualHandle) {
            var visualRect = visualHandle.GetComponent<RectTransform>();
            visualRect.anchorMin = new Vector2(0.5f, 0);
            visualRect.anchorMax = new Vector2(0.5f, 1);
            visualRect.pivot = new Vector2(0.5f, 0.5f);
            visualRect.sizeDelta = new Vector2(Config.ScrollbarTextureWidth, 0);
        }

        private static void CreateScrollbarComponent(GameObject scrollbarGameObj, GameObject handleGameObj, ScrollRect scrollRect) {
            var scrollbar = scrollbarGameObj.GetComponent<Scrollbar>();
            scrollbar.handleRect = handleGameObj.GetComponent<RectTransform>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        }

        private static Sprite CreateScrollbarBackgroundSprite(Texture2D scrollbarBackgroundTexture) {
            return Sprite.Create(scrollbarBackgroundTexture,
                new Rect(0, 0, scrollbarBackgroundTexture.width, scrollbarBackgroundTexture.height),
                new Vector2(0.5f, 0.5f),
                100f, //default value
                0, //default value
                SpriteMeshType.FullRect,
                new Vector4(
                    0, // left
                    Config.ScrollbarTextureBorderRadius, // bottom
                    0, // right
                    Config.ScrollbarTextureBorderRadius // top
                )
            );
        }

        private static Sprite CreateScrollHandleSprite(Texture2D scrollbarHandleTexture) {
            return Sprite.Create(scrollbarHandleTexture,
                new Rect(0, 0, scrollbarHandleTexture.width, scrollbarHandleTexture.height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(
                    0, // left
                    Config.ScrollbarTextureBorderRadius, // bottom
                    0, // right
                    Config.ScrollbarTextureBorderRadius // top
                )
            );
        }
    }
}
