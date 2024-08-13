using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace NiGames.Essentials.Editor
{
    /// <summary>
    /// Class of utilities that slightly extends the capabilities of AssetDatabase.
    /// </summary>
    public static class AssetDatabaseUtility
    {
        /// <summary>
        /// Loads all assets with type T using AssetDatabase.
        /// </summary>
        /// <typeparam name="T">Type of assets to be loaded.</typeparam>
        /// <returns>An array of loaded assets.</returns>
        public static T[] LoadAllAssetsWithType<T>() where T : Object
        {
            var list = new List<T>();
            
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            
            foreach (var guid in guids)
            {
                T instance;
                
                try
                {
                    instance = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                }
                catch
                {
                    continue;
                }
                
                if (instance == null) continue;
                
                list.Add(instance);
            }
            
            return list.ToArray();
        }
        
        /// <summary>
        /// Loads all assets with type using AssetDatabase.
        /// </summary>
        /// <param name="type">Type of assets to be loaded.</param>
        /// <returns>An array of loaded assets.</returns>
        public static Object[] LoadAllAssetsWithType(Type type)
        {
            var list = new List<Object>();
            
            var guids = AssetDatabase.FindAssets($"t:{type}");
            
            foreach (var guid in guids)
            {
                Object instance;
                
                try
                {
                    instance = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type);
                }
                catch
                {
                    continue;
                }
                
                if (instance == null) continue;
                
                list.Add(instance);
            }
            
            return list.ToArray();
        }
        
        /// <summary>
        /// Searches for a common path among the passed paths and returns it.
        /// </summary>
        public static string GetCommonAssetPath(string[] paths)
        {
            if (paths == null || paths.Length == 0) return string.Empty;
            
            var commonPathParts = paths[0].Split('/');
            
            for (var i = 1; i < paths.Length; i++)
            {
                var pathParts = paths[i].Split('/');
                var commonLength = Math.Min(commonPathParts.Length, pathParts.Length);
                
                for (var j = 0; j < commonLength; j++)
                {
                    if (commonPathParts[j].Equals(pathParts[j], StringComparison.OrdinalIgnoreCase)) continue;
                    
                    Array.Resize(ref commonPathParts, j);
                    break;
                }
                
                if (commonPathParts.Length == 0) return string.Empty;
            }

            return Path.Combine(commonPathParts);
        }
        
        /// <summary>
        /// Returns the path to the asset.
        /// </summary>
        public static string GetAssetPath(Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            var slash = path.LastIndexOf('/');
            
            return path.Substring(0, slash + 1);
        }
    }
}