using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public sealed class OptionalDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var element = new VisualElement()
            {
                style = { flexDirection = FlexDirection.Row, flexShrink = 0 }
            };
            
            var hasValueProperty = property.FindPropertyRelative("_hasValue");
            var valueProperty = property.FindPropertyRelative("_value");
            
            var hasValueField = new Toggle(string.Empty)
            {
                style = { marginRight = 2f }
            };
            hasValueField.BindProperty(hasValueProperty);
            
            var valueField = new PropertyField(valueProperty, property.displayName)
            {
                style = { flexGrow = 1 }
            };
            
            hasValueField.RegisterValueChangedCallback(evt =>
            {
                valueField.SetEnabled(evt.newValue);
            });
            valueField.SetEnabled(hasValueField.value);
            
            element.Add(valueField);
            
            if (property.hasChildren)
            {
                hasValueField.style.alignSelf = Align.FlexStart;
                valueField.style.marginRight = -19;
                element.Add(hasValueField);
            }
            else
            {
                element.Insert(0, hasValueField);
            }
            
            return element;
        }
    }
}