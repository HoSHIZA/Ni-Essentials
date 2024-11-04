using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    public static class VisualElementUtility
    {
        public static VisualElement GetUxml(string filename, [CallerFilePath] string callerFilename = "")
        {
            var path = $"{AssetHelper.GetAssetPath(callerFilename)}{filename}";
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            
            if (uxml != null)
            {
                return uxml.Instantiate().contentContainer;
            }
            
            Debug.LogError($"UXML '{path}' could not be found");
            return null;
        }
    }
}