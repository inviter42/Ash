using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.BetterTattoos.MakerExtensions.ExtendedControls
{
    internal class TattooSelectionToggle : ExtSelectionToggle
    {
        internal bool IsEmpty {
            get => isEmpty;
            set {
                isEmpty = value;
                UpdateColorBlocks();
            }
        }

        private bool isEmpty = true;

        private static readonly ColorBlock EmptyColorBlockOn = new ColorBlock {
            normalColor = new Color(0, 0, 0, 0.58f),
            highlightedColor = new Color(0, 0, 0, 0.58f),
            pressedColor = new Color(0, 0, 0, 0.58f),
            colorMultiplier = 1,
        };

        private static readonly ColorBlock EmptyColorBlockOff = new ColorBlock {
            normalColor = new Color(0.58f, 0.58f, 0.58f, 0.21f),
            highlightedColor = new Color(0.69f, 0.69f, 0.69f, 0.30f),
            pressedColor = new Color(0, 0, 0, 0.58f),
            colorMultiplier = 1,
        };

        private static readonly ColorBlock ActiveColorBlockOn = new ColorBlock {
            normalColor = new Color(0, 0.08f, 0.26f, 1),
            highlightedColor = new Color(0, 0.08f, 0.26f, 1),
            pressedColor = new Color(0, 0.08f, 0.26f, 1),
            colorMultiplier = 1,
        };

        private static readonly ColorBlock ActiveColorBlockOff = new ColorBlock {
            normalColor = new Color(0.12f, 0.30f, 0.67f, 1),
            highlightedColor = new Color(0.29f, 0.38f, 0.58f, 1),
            pressedColor = new Color(0, 0.08f, 0.26f, 1),
            colorMultiplier = 1,
        };

        private void UpdateColorBlocks() {
            ButtonOn.colors = IsEmpty ? EmptyColorBlockOn : ActiveColorBlockOn;
            ButtonOff.colors = IsEmpty ? EmptyColorBlockOff : ActiveColorBlockOff;
        }
    }
}
