using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(Impl<>), true)]
    internal sealed class ImplDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var valueProperty = property.FindPropertyRelative("_value");
            var type = fieldInfo.FieldType.GetGenericArguments()[0];
            
            var objectField = new ObjectField(property.displayName)
            {
                objectType = type,
                allowSceneObjects = true,
                value = valueProperty.objectReferenceValue,
            };
            objectField.SetInspectorAligned(true);

            objectField.RegisterValueChangedCallback(evt =>
            {
                var newValue = evt.newValue;
                if (newValue == null || type.IsInstanceOfType(newValue))
                {
                    valueProperty.objectReferenceValue = newValue;
                    valueProperty.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    objectField.SetValueWithoutNotify(valueProperty.objectReferenceValue);
                }
            });
            
            return objectField;
        }
    }
}