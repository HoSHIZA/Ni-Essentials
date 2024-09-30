using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NiGames.Essentials.Components
{
    [DisallowMultipleComponent]
    [AddComponentMenu(Constants.Menu.Component.UI + "Drag/Draggable Rect Handler")]
    public sealed class DraggableRectHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private PointerUnityEvent _onBeginDragEvent;
        [SerializeField] private PointerUnityEvent _onDragEvent;
        [SerializeField] private PointerUnityEvent _onEndDragEvent;
        
        internal PointerUnityEvent OnBeginDragEvent => _onBeginDragEvent;
        internal PointerUnityEvent OnDragEvent => _onDragEvent;
        internal PointerUnityEvent OnEndDragEvent => _onEndDragEvent;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragEvent?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragEvent?.Invoke(eventData);
        }
        
        [Serializable]
        internal sealed class PointerUnityEvent : UnityEvent<PointerEventData>
        {
        }
    }
}
