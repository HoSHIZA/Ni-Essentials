using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NiGames.Essentials.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu(Constants.Menu.UI + "Draggable Rect")]
    public sealed class DraggableRect : UIBehaviour
    {
        [SerializeField] private bool _interactable = true;
        
        [Space]
        [SerializeField] private DraggableRestriction _restriction = DraggableRestriction.Strict;
        
        [Title("Components")]
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _handle;
        
        [Title("Events")]
        [SerializeField] private DraggableEvent _onBeginDrag;
        [SerializeField] private DraggableEvent _onDrag;
        [SerializeField] private DraggableEvent _onEndDrag;
        
        private bool _isDrag;
        private Vector3 _startPosition;
        
        private DraggableRectHandler _dragHandle;
        
        private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform ? _rectTransform : (RectTransform)transform;

        public bool Interactable
        {
            [MethodImpl(256)]
            get => _interactable;
            [MethodImpl(256)]
            set => InteractableChange(value);
        }

        public DraggableRestriction Restriction
        {
            [MethodImpl(256)]
            get => _restriction;
            [MethodImpl(256)]
            set => _restriction = value;
        }

        public RectTransform Target
        {
            [MethodImpl(256)]
            get => _target;
            [MethodImpl(256)]
            set => SetTarget(value);
        }

        public RectTransform Handle
        {
            [MethodImpl(256)]
            get => _handle;
            [MethodImpl(256)]
            set => SetHandle(value);
        }

        protected override void Start()
        {
            SetTarget(_target ? _target : RectTransform);
            SetHandle(_handle ? _handle : RectTransform);
        }

        protected override void OnDestroy()
        {
            RemoveListeners();
        }

        public override bool IsActive()
        {
            return base.IsActive() && Interactable;
        }

        public void BeginDrag(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            if (eventData.currentInputModule.input.mousePresent)
            {
                if (eventData.button != PointerEventData.InputButton.Left) return;
            }
#endif
            
            if (!IsActive()) return;

            _isDrag = true;

            _startPosition = Target.localPosition;

            _onBeginDrag.Invoke(this);
        }
        
        public void Drag(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            if (eventData.currentInputModule.input.mousePresent)
            {
                if (eventData.button != PointerEventData.InputButton.Left) return;
            }
#endif
            
            if (!_isDrag) return;
            if (eventData.used) return;

            eventData.Use();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, 
                eventData.position, eventData.pressEventCamera, out var currentPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, 
                eventData.pressPosition, eventData.pressEventCamera, out var originalPoint);

            Drag(currentPoint - originalPoint);
            
            _onDrag.Invoke(this);
        }

        public void Drag(Vector2 delta)
        {
            delta.y *= -1;

            var angle = Target.localRotation.eulerAngles.z * Mathf.Deg2Rad;
            var dragDelta = new Vector2(
                delta.x * Mathf.Cos(angle) + delta.y * Mathf.Sin(angle),
                delta.x * Mathf.Sin(angle) - delta.y * Mathf.Cos(angle));

            var position = new Vector3(_startPosition.x + dragDelta.x, _startPosition.y + dragDelta.y, _startPosition.z);

            position = _restriction == DraggableRestriction.Strict ? RestrictPosition(position) : position;

            Target.localPosition = position;

            CopyRectTransformValues();
        }

        public void EndDrag(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            if (eventData.currentInputModule.input.mousePresent)
            {
                if (eventData.button != PointerEventData.InputButton.Left) return;
            }
#endif
            
            if (!_isDrag) return;
            
            Drag(eventData);

            _isDrag = false;
            
            _onEndDrag.Invoke(this);
        }

        private void SetTarget(RectTransform target)
        {
            if (!target) return;

            _target = target;

            RectTransform.SetParent(_target.parent, false);

            CopyRectTransformValues();
        }

        private void SetHandle(RectTransform handle)
        {
            if (_handle != null)
            {
                RemoveListeners();
                
                Destroy(_dragHandle);
            }

            _handle = handle;
            _dragHandle = _handle.TryGetComponent<DraggableRectHandler>(out var dragHandle) 
                ? dragHandle 
                : _handle.gameObject.AddComponent<DraggableRectHandler>();

            AddListeners();
        }

        private void InteractableChange(bool interactable)
        {
            if (_interactable == interactable) return;
            
            _interactable = interactable;
            
            if (!IsActive()) return;
            
            OnInteractableChanged(Interactable);
        }

        private void OnInteractableChanged(bool interactable)
        {
            if (interactable) return;

            EndDrag(null);
        }

        private void AddListeners()
        {
            if (!_dragHandle) return;
            
            _dragHandle.OnBeginDragEvent.AddListener(BeginDrag);
            _dragHandle.OnDragEvent.AddListener(Drag);
            _dragHandle.OnEndDragEvent.AddListener(EndDrag);
        }

        private void RemoveListeners()
        {
            if (!_dragHandle) return;
            
            _dragHandle.OnBeginDragEvent.RemoveListener(BeginDrag);
            _dragHandle.OnDragEvent.RemoveListener(Drag);
            _dragHandle.OnEndDragEvent.RemoveListener(EndDrag);
        }

        private Vector3 RestrictPosition(Vector3 position)
        {
            var parent = (RectTransform)Target.parent;

            if (!parent) return position;

            var parentSize = parent.rect.size;
            var parentPivot = parent.pivot;
            
            var targetSize = Target.rect.size;
            var targetPivot = Target.pivot;
            
            var minX = -(parentSize.x * parentPivot.x) + targetSize.x * targetPivot.x;
            var maxX = parentSize.x - (1f - parentPivot.x) - targetSize.x * (1f - targetPivot.x);
            
            var minY = -(parentSize.y * parentPivot.y) + targetSize.y * targetPivot.y;
            var maxY = parentSize.y - (1f - parentPivot.y) - targetSize.y * (1f - targetPivot.y);

            return new Vector3(Mathf.Clamp(position.x, minX, maxX), Mathf.Clamp(position.y, minY, maxY), position.z);
        }

        private void CopyRectTransformValues()
        {
            RectTransform.localPosition = Target.localPosition;
            RectTransform.localRotation = Target.localRotation;
            RectTransform.localScale = Target.localScale;
            RectTransform.sizeDelta = Target.sizeDelta;
            RectTransform.anchorMin = Target.anchorMin;
            RectTransform.anchorMax = Target.anchorMax;
            RectTransform.pivot = Target.pivot;
        }

        public enum DraggableRestriction
        {
            None = 0,
            Strict = 1,
        }
        
        [Serializable]
        internal sealed class DraggableEvent : UnityEvent<DraggableRect>
        {
        }
    }
}
