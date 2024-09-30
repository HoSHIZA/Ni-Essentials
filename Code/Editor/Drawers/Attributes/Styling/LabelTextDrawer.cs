using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(LabelTextAttribute))]
    public sealed class LabelTextDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = (LabelTextAttribute)attribute;
            
            return new PropertyField(property, attr.Label);
        }
    }
}