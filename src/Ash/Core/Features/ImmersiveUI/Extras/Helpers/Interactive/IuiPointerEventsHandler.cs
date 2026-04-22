using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ash.Core.Features.ImmersiveUI.Extras.Helpers.Interactive
{
    internal class IuiPointerEventsHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
        IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        internal readonly List<System.Action<PointerEventData>> ActionOnPointerEnter =
            new List<System.Action<PointerEventData>>();

        internal readonly List<System.Action<PointerEventData>> ActionOnPointerExit =
            new List<System.Action<PointerEventData>>();

        internal readonly List<System.Action<PointerEventData>> ActionOnPointerClick =
            new List<System.Action<PointerEventData>>();

        internal readonly List<System.Action<PointerEventData>> ActionOnPointerDown =
            new List<System.Action<PointerEventData>>();

        internal readonly List<System.Action<PointerEventData>> ActionOnPointerUp =
            new List<System.Action<PointerEventData>>();

        internal readonly List<System.Action<PointerEventData>> ActionOnBeginDrag =
            new List<System.Action<PointerEventData>>();

        internal readonly List<System.Action<PointerEventData>> ActionOnEndDrag =
            new List<System.Action<PointerEventData>>();

        internal readonly List<System.Action<PointerEventData>> ActionOnDrag =
            new List<System.Action<PointerEventData>>();

        public void OnPointerEnter(PointerEventData eventData) {
            foreach (var action in ActionOnPointerEnter)
                action.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData) {
            foreach (var action in ActionOnPointerExit)
                action.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData) {
            foreach (var action in ActionOnPointerClick)
                action.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData) {
            foreach (var action in ActionOnPointerDown)
                action.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData) {
            foreach (var action in ActionOnPointerUp)
                action.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            foreach (var action in ActionOnBeginDrag)
                action.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            foreach (var action in ActionOnEndDrag)
                action.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            foreach (var action in ActionOnDrag)
                action.Invoke(eventData);
        }

        private void OnDisable() {
            foreach (var action in ActionOnPointerExit)
                action.Invoke(null);

            foreach (var action in ActionOnEndDrag)
                action.Invoke(null);
        }
    }
}
