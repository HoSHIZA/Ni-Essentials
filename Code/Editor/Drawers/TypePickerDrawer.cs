#if !DISABLE_NIGAMES_INSPECTOR_ATTRIBUTES_FEATURE
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(TypePickerAttribute))]
    public sealed class TypePickerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = (TypePickerAttribute)attribute;
            
            var field = new TypePickerField(property.displayName, attr.BaseType, attr.TypeFilter);
            field.BindProperty(property);
            
            return field;
        }
    }
}
#endif