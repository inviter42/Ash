using System.Collections.Generic;
using BepInEx;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Ash.Core.Features.BetterApplicables.BaseComponents.MakerExtensions.ExtendedLayouts
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
            var containerGo = new GameObject("ExtGridLayout", typeof(LayoutElement), typeof(GridLayoutGroup));
            containerGo.transform.SetParent(parent, false);

            var grid = containerGo.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(40, 40);
            grid.spacing = Vector2.zero;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            grid.padding = new RectOffset(0, 0, 0, 0);

            GridTransform = containerGo.transform;

            foreach (var childControl in ChildControls)
                // keep world position to let the canvas scaler downsize the elements
                childControl.transform.SetParent(GridTransform, true);

            return containerGo;
        }
    }
}
