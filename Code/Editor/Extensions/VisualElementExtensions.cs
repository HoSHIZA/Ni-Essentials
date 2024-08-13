using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    public static class VisualElementExtensions
    {
        public static void AddStyleSheet(this VisualElement element, string filename, [CallerFilePath] string callerFilename = "")
        {
            var path = $"{AssetHelper.GetAssetPath(callerFilename)}{filename}";
            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            
            if (stylesheet != null)
            {
                element.styleSheets.Add(stylesheet);
            }
            else
            {
                Debug.LogError($"StyleSheet '{path}' could not be found");
            }
        }
        
        public static void AddUxml(this VisualElement element, string filename, [CallerFilePath] string callerFilename = "")
        {
            var path = $"{AssetHelper.GetAssetPath(callerFilename)}{filename}";
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            
            if (uxml != null)
            {
                uxml.CloneTree(element);
            }
            else
            {
                Debug.LogError($"UXML '{path}' could not be found");
            }
        }

        #region Duplicates of Runtime Methods
        
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

        #endregion
    }
}