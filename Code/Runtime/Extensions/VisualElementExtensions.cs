using UnityEngine.UIElements;

namespace NiGames.Essentials
{
    public static class VisualElementExtensions
    {
        public static void SetDisplay(this VisualElement visualElement, bool display)
        {
            if (visualElement == null) return;
            
            visualElement.style.display = new StyleEnum<DisplayStyle>(display ? DisplayStyle.Flex : DisplayStyle.None);
        }
        
        public static void SendChangeEvent<T>(this VisualElement visualElement, T previous, T current)
        {
            using var changeEvent = ChangeEvent<T>.GetPooled(previous, current);

            changeEvent.target = visualElement;
            visualElement.SendEvent(changeEvent);
        }
        
        public static VisualElement GetRootElement(this VisualElement visualElement, string name = null, string className = null)
        {
            var hasName = string.IsNullOrEmpty(name);
            var hasClassName = string.IsNullOrEmpty(className);

            while (visualElement.parent != null)
            {
                visualElement = visualElement.parent;

                if (hasName && hasClassName)
                {
                    if (visualElement.ClassListContains(className) && visualElement.name == name) break;
                }
                else if (hasName)
                {
                    if (visualElement.name == name) break;
                }
                else if (hasClassName)
                {
                    if (visualElement.ClassListContains(className)) break;
                }
            }

            return visualElement;
        }
    }
}