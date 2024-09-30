using NiGames.Essentials.Components;
using UnityEditor;
using UnityEditor.UI;

namespace NiGames.Essentials.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NonDrawingGraphic), false)]
    internal sealed class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}