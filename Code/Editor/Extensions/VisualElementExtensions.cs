using System.IO;
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
        
        public static void AddStyleSheetWithSimilarName(this VisualElement element, string relativePath = null, [CallerFilePath] string callerFilename = "")
        {
            var path = callerFilename;
            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                path = Path.Combine(Path.GetDirectoryName(callerFilename)!, relativePath);
            }
            
            var filename = $"{Path.GetFileNameWithoutExtension(path)}.uss";
            path = $"{AssetHelper.GetAssetPath(path)}{filename}";
            
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
    }
}