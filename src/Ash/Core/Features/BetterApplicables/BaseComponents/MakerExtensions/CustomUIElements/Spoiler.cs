using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions.CustomUIElements
{
    public class Spoiler : MonoBehaviour
    {
        internal GameObject SpoilerGroup { get; private set; }
        internal Button ToggleButton { get; private set; }
        internal GameObject ContentBody { get; private set; }
        internal Text ArrowText { get; private set; }

        private LayoutElement RootLayoutElement;
        private ContentSizeFitter RootSizeFitter;
        private bool IsExpanded;

        private const float ElementWidth = 240f;
        private const float HeaderHeight = 30f;

        internal static Spoiler Create(Transform parentCanvas, string title) {
            var groupObj = new GameObject("SpoilerGroup", typeof(RectTransform));
            groupObj.transform.SetParent(parentCanvas, false);

            var groupRect = groupObj.GetComponent<RectTransform>();
            groupRect.anchorMin = new Vector2(0f, 1f);
            groupRect.anchorMax = new Vector2(1f, 1f);
            groupRect.pivot = new Vector2(0f, 1f);
            groupRect.anchoredPosition = Vector2.zero;
            groupRect.sizeDelta = Vector2.zero;

            var groupLayout = groupObj.AddComponent<VerticalLayoutGroup>();
            groupLayout.childControlWidth = true;
            groupLayout.childControlHeight = true;
            groupLayout.childForceExpandWidth = true;
            groupLayout.childForceExpandHeight = false;
            groupLayout.spacing = 0f;

            var groupFitter = groupObj.AddComponent<ContentSizeFitter>();
            groupFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            groupFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var rootLayoutElement = groupObj.AddComponent<LayoutElement>();
            rootLayoutElement.minWidth = ElementWidth;
            rootLayoutElement.preferredWidth = ElementWidth;
            rootLayoutElement.minHeight = HeaderHeight;

            var buttonObj = new GameObject("SpoilerHeader", typeof(RectTransform));
            buttonObj.transform.SetParent(groupObj.transform, false);

            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.3f);

            var button = buttonObj.AddComponent<Button>();

            var buttonLayoutElement = buttonObj.AddComponent<LayoutElement>();
            buttonLayoutElement.minHeight = HeaderHeight;
            buttonLayoutElement.preferredHeight = HeaderHeight;

            var textObj = new GameObject("LabelText", typeof(RectTransform));
            textObj.transform.SetParent(buttonObj.transform, false);

            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = new Vector2(-20f, 0f);
            textRect.anchoredPosition = new Vector2(10f, 0f);

            var labelText = textObj.AddComponent<Text>();
            labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            labelText.text = "▶  " + title;
            labelText.fontSize = 12;
            labelText.color = Color.white;
            labelText.alignment = TextAnchor.MiddleLeft;

            var bodyObj = new GameObject("SpoilerBody", typeof(RectTransform));
            bodyObj.transform.SetParent(groupObj.transform, false);

            var bodyImage = bodyObj.AddComponent<Image>();
            bodyImage.color = new Color(0.15f, 0.15f, 0.15f, 0.15f);

            var bodyLayout = bodyObj.AddComponent<VerticalLayoutGroup>();
            bodyLayout.padding = new RectOffset(10, 10, 10, 10);
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = false;
            bodyLayout.childForceExpandHeight = false;
            bodyLayout.spacing = 6f;
            bodyLayout.childAlignment = TextAnchor.UpperCenter;

            var bodyFitter = bodyObj.AddComponent<ContentSizeFitter>();
            bodyFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            bodyFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var spoiler = groupObj.AddComponent<Spoiler>();
            spoiler.SpoilerGroup = groupObj;
            spoiler.ToggleButton = button;
            spoiler.ContentBody = bodyObj;
            spoiler.ArrowText = labelText;
            spoiler.RootLayoutElement = rootLayoutElement;
            spoiler.RootSizeFitter = groupFitter;

            return spoiler;
        }

        private void Start() {
            if (ToggleButton != null)
                ToggleButton.onClick.AddListener(Toggle);

            IsExpanded = false;
            UpdateState();
        }

        public void Toggle() {
            IsExpanded = !IsExpanded;
            UpdateState();
        }

        private void UpdateState() {
            if (ContentBody != null)
                ContentBody.SetActive(IsExpanded);

            if (ArrowText != null) {
                var cleanTitle = ArrowText.text.Replace("▼  ", "").Replace("▶  ", "");
                ArrowText.text = (IsExpanded ? "▼  " : "▶  ") + cleanTitle;
            }

            if (RootLayoutElement != null) {
                if (IsExpanded) {
                    if (RootSizeFitter != null)
                        RootSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    RootLayoutElement.preferredHeight = -1;
                } else {
                    if (RootSizeFitter != null)
                        RootSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    RootLayoutElement.preferredHeight = HeaderHeight;

                    var rectTransform = SpoilerGroup.GetComponent<RectTransform>();
                    if (rectTransform != null)
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, HeaderHeight);
                }
            }

            Canvas.ForceUpdateCanvases();

            if (transform.parent == null)
                return;

            var parentRect = transform.parent.GetComponent<RectTransform>();
            if (parentRect != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        }
    }
}
