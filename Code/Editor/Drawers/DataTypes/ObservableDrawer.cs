using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(Observable<>))]
    public class ObservableDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var valueProperty = property.FindPropertyRelative("_value");
            var field = new PropertyField(valueProperty, property.displayName);
            field.style.flexGrow = 1;
            
            return field;
        }
    }
}