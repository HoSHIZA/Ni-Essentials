#if !NI_ESSENTIALS_INSPECTOR_ATTRIBUTES_DISABLE
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(ReferenceAttribute))]
    public sealed class ReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = (ReferenceAttribute)attribute;
            
            var baseType = attr.BaseType ?? fieldInfo.FieldType;
            var field = new ReferenceField(property.displayName, baseType, attr.DrawCustomReferenceDrawer);
            
            field.BindProperty(property);
            
            return field;
        }
    }
}
#endif