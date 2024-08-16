using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NiGames.Essentials.Editor
{
    public static class ObjectHelper
    {
        /// <summary>
        /// Returns a <see cref="ObjectsWithPaths"/> structure that contains elements and element paths for use in GenericMenu.
        /// </summary>
        /// <param name="type">Type of object to search for.</param>
        /// <param name="includeDisabled">Include disabled objects in the search.</param>
        /// <returns></returns>
        public static ObjectsWithPaths GetObjectsWithPaths(Type type, bool includeDisabled)
        {
            return new ObjectsWithPaths(type, includeDisabled);
        }
        
        /// <summary>
        /// A class that creates and describes objects and paths to them.
        /// </summary>
        public sealed class ObjectsWithPaths
        {
            public readonly Type Type;
            public readonly Object[] Items;
            public readonly string[] Paths;

            public ObjectsWithPaths(Type type, bool includeDisabled)
            {
                Type = type;

                var objects = (includeDisabled
                            ? Resources.FindObjectsOfTypeAll(type)
#if UNITY_2023_1_OR_NEWER
                            : Object.FindObjectsByType(type, FindObjectsSortMode.None)
#else
                            : Object.FindObjectsOfType(type)
#endif
                    )
                    .Where(obj => obj.hideFlags is HideFlags.None or HideFlags.NotEditable)
                    .ToArray();
                
                var paths = objects.Select(static obj => GetObjectPath(obj));
                var commonPath = AssetDatabaseUtility.GetCommonAssetPath(paths.ToArray());
                
                Items = objects;
                Paths = objects.Select(t =>
                {
                    var path = AssetDatabaseUtility.GetAssetPath(t).Substring(commonPath.Length);
                    return path.Length > 0 ? $"{path}{t.name}" : t.name;
                }).ToArray();
            }
            
            private static string GetObjectPath(Object obj)
            {
                var sb = new StringBuilder();
                var gameObject = obj switch
                {
                    GameObject go => go,
                    Component component => component.gameObject,
                    _ => null
                };
                
                if (!gameObject) return sb.ToString();
                
                while (gameObject.transform.parent)
                {
                    gameObject = gameObject.transform.parent.gameObject;
                    
                    sb.Append(gameObject.name);
                    sb.Append('/');
                }
                
                return sb.ToString();
            }
        }
    }
}