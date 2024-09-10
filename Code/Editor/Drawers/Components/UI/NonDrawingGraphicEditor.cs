using NiGames.Essentials.Components;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace NiGames.Essentials.Editor.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NonDrawingGraphic), false)]
    public sealed class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;
            
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}