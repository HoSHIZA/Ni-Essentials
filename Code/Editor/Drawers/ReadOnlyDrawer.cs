#if !DISABLE_NIGAMES_INSPECTOR_ATTRIBUTES_FEATURE
using UnityEditor;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public sealed class ReadOnlyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = base.CreatePropertyGUI(property);
            
            field.SetEnabled(false);

            return field;
        }
    }
}
#endif
