using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public sealed class InfoBoxDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = (InfoBoxAttribute)attribute;
            var element = new VisualElement();

            var helpBox = new HelpBox(attr.Message, attr.Type switch
            {
                MessageType.Info => HelpBoxMessageType.Info,
                MessageType.Warning => HelpBoxMessageType.Warning,
                MessageType.Error => HelpBoxMessageType.Error,
                _ => HelpBoxMessageType.None,
            });

            var field = new PropertyField(property);
            
            element.Add(helpBox);
            element.Add(field);
            
            return element;
        }
    }
}