#if !DISABLE_NIGAMES_INSPECTOR_ATTRIBUTES_FEATURE
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(ObjectPickerAttribute))]
    public sealed class ObjectPickerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = (ObjectPickerAttribute)attribute;
            
            var field = new ObjectPickerField(property.displayName, attr.BaseType);
            field.BindProperty(property);
            
            return field;
        }
    }
}
#endif