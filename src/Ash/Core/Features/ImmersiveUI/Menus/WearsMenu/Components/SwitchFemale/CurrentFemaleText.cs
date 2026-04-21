using Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.ImmersiveUI.Menus.WearsMenu.Components.SwitchFemale
{
    internal static class CurrentFemaleText
    {
        internal static Text CreateFemaleText(
            WearsMenuConfig config,
            GameObject root
        ) {
            var textGameObj = new GameObject("FemaleText", typeof(Text), typeof(LayoutElement));
            textGameObj.transform.SetParent(root.transform, false);

            var layoutElement = textGameObj.GetComponent<LayoutElement>();
            layoutElement.minHeight = config.FemaleLabelFontSize;

            var textComp = textGameObj.GetComponent<Text>();
            textComp.text = "";
            textComp.font =
                Ash.AshUI.ImmersiveUIFontsAssetBundle.LoadAsset<Font>(
                    "assets/frostedglass/fonts/corbel-light.ttf");
            textComp.fontSize = config.FemaleLabelFontSize;
            textComp.fontStyle = FontStyle.Normal;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = config.TextNormalColor;
            textComp.raycastTarget = false;

            return textComp;
        }
    }
}
