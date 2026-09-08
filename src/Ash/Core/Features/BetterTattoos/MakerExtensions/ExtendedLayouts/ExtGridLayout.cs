using System.Collections.Generic;
using BepInEx;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.BetterTattoos.MakerExtensions.ExtendedLayouts
{
    internal class ExtGridLayout : BaseGuiEntry
    {
        internal Transform GridTransform { get; private set; }

        internal List<GameObject> ChildControls { get; }

        internal ExtGridLayout(MakerCategory category, BaseUnityPlugin owner, List<GameObject> childControls)
            : base(category, owner) {
            ChildControls = childControls;
        }

        protected override void Initialize() { }

        public override void Dispose() {
            foreach (var childControl in ChildControls)
                Object.Destroy(childControl);

            base.Dispose();
        }

        protected override GameObject OnCreateControl(Transform parent) {
            var containerGo = new GameObject("ExtGridLayout");
            containerGo.transform.SetParent(parent, false);

            var layoutElement = containerGo.AddComponent<LayoutElement>();
            layoutElement.minWidth = 200;
            layoutElement.preferredWidth = 200;

            var grid = containerGo.AddComponent<GridLayoutGroup>();

            grid.cellSize = new Vector2(40, 40);
            grid.spacing = Vector2.zero;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            grid.padding = new RectOffset(0, 0, 10, 10);

            GridTransform = containerGo.transform;

            foreach (var childControl in ChildControls)
                childControl.transform.SetParent(GridTransform);

            return containerGo;
        }
    }
}
