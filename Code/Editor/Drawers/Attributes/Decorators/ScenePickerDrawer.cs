using NiGames.Essentials.Editor.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(ScenePickerAttribute))]
    public sealed class ScenePickerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new ScenePickerField(property.displayName);
            field.BindProperty(property);
            
            return field;
        }
    }
}