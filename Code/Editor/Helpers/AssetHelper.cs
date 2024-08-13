using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;

namespace NiGames.Essentials.Editor
{
    public static class AssetHelper
    {
        private static readonly Dictionary<string, AssetsWithPaths> _assetsCache = new Dictionary<string, AssetsWithPaths>();
        
        private static readonly Regex _assetPathRegex = new Regex(@".*/(Assets/.*/)[^/]+", RegexOptions.Compiled);
        private static readonly Regex _packagePathRegex = new Regex(@".*/PackageCache(/[^/]+)@[^/]+(/.*/)[^/]+", RegexOptions.Compiled);
        
        /// <summary>
        /// Returns a <see cref="AssetsWithPaths"/> structure that contains elements and element paths for use in GenericMenu.
        /// </summary>
        public static AssetsWithPaths GetAssetsWithPaths(Type assetType)
        {
            var name = assetType.AssemblyQualifiedName!;
            
            if (_assetsCache.TryGetValue(name, out var list)) return list;
            
            list = new AssetsWithPaths(assetType);
            _assetsCache.Add(name, list);

            return list;
        }
        
        public static string GetScriptPath([CallerFilePath] string filename = "")
        {
            return GetAssetPath(filename);
        }
        
        public static string GetAssetPath(string fullPath)
        {
            var normalized = fullPath.Replace('\\', '/');
            
            var assetMatch = _assetPathRegex.Match(normalized);
            if (assetMatch.Success)
            {
                return assetMatch.Groups[1].Value;
            }
            
            var packageMatch = _packagePathRegex.Match(normalized);
            if (packageMatch.Success)
            {
                return $"Package{packageMatch.Groups[1].Value}{packageMatch.Groups[2].Value}";
            }

            return "Unknown";
        }
        
        /// <summary>
        /// A struct that creates and describes assets and paths to them.
        /// </summary>
        public struct AssetsWithPaths
        {
            public readonly Type Type;
            public readonly Object[] Items;
            public readonly string[] Paths;
            
            public AssetsWithPaths(Type type)
            {
                Type = type;
                
                var objects = AssetDatabaseUtility.LoadAllAssetsWithType(type);
                
                var paths = objects.Select(AssetDatabaseUtility.GetAssetPath);
                var commonPath = AssetDatabaseUtility.GetCommonAssetPath(paths.ToArray());
                
                Items = objects;
                Paths = objects.Select(t =>
                {
                    var path = AssetDatabaseUtility.GetAssetPath(t).Substring(commonPath.Length);
                    return path.Length > 0 ? path + t.name : t.name;
                }).ToArray();
            }
        }
    }
}