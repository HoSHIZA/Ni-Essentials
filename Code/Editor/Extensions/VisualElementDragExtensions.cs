using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    public enum DragState
    {
        Idle,
        Ready,
        Dragging,
    }

    public interface IDraggable
    {
        DragState DragState { get; set; }
        Object[] DragObjects { get; }
        object DragData { get; }
        string DragText { get; }
    }

    public interface IDragReceiver
    {
        void AcceptDrag(Object[] objects, object data);
        bool IsDragValid(Object[] objects, object data);
    }

    public static class VisualElementDragExtensions
    {
        public static void MakeDraggable<TDraggable>(this TDraggable draggable) where TDraggable : VisualElement, IDraggable
        {
            draggable.DragState = DragState.Idle;
            
            draggable.RegisterCallback<MouseDownEvent>(OnMouseDown);
            draggable.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            draggable.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        public static void MakeDragReceiver<TReceiver>(this TReceiver receiver) where TReceiver : VisualElement, IDragReceiver
        {
            receiver.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            receiver.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        private static void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.currentTarget is not IDraggable draggable) return;
            if (evt.button != (int)MouseButton.LeftMouse) return;
            
            draggable.DragState = DragState.Ready;
        }

        private static void OnMouseMove(MouseMoveEvent evt)
        {
            if (evt.currentTarget is not IDraggable draggable) return;
            if (draggable.DragState != DragState.Ready) return;
            
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = draggable.DragObjects;
            DragAndDrop.SetGenericData("Drag", draggable.DragData);
            DragAndDrop.StartDrag(draggable.DragText);
                
            draggable.DragState = DragState.Dragging;
        }

        private static void OnMouseUp(MouseUpEvent evt)
        {
            if (evt.currentTarget is not IDraggable draggable) return;
            if (draggable.DragState == DragState.Idle) return;
            if (evt.button != (int)MouseButton.LeftMouse) return;
            
            draggable.DragState = DragState.Idle;
        }
        
        private static void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (evt.currentTarget is not IDragReceiver receiver) return;
            
            var objects = DragAndDrop.objectReferences;
            var data = DragAndDrop.GetGenericData("Drag");
            
            DragAndDrop.visualMode = receiver.IsDragValid(objects, data)
                ? DragAndDropVisualMode.Generic
                : DragAndDropVisualMode.Rejected;
        }
        
        private static void OnDragPerform(DragPerformEvent evt)
        {
            if (evt.currentTarget is not IDragReceiver receiver) return;
            
            var objects = DragAndDrop.objectReferences;
            var data = DragAndDrop.GetGenericData("Drag");
            
            if (receiver.IsDragValid(objects, data))
            {
                DragAndDrop.AcceptDrag();
                receiver.AcceptDrag(objects, data);
            }
        }
    }
}