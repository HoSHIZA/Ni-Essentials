using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace NiGames.Essentials.Editor
{
    public static class PropertyDrawerHelper
    {
        private static readonly Dictionary<string, PropertyDrawer> _propertyDrawerCache = new Dictionary<string, PropertyDrawer>();
        
        /// <summary>
        /// Searches <see cref="PropertyDrawer"/> for the desired type. Only working <see cref="PropertyDrawer"/> marked with
        /// the <see cref="CustomPropertyDrawer"/> attribute participate in the search.
        /// </summary>
        public static PropertyDrawer GetPropertyDrawerForType(Type type)
        {
            if (type == null) return null;
            
            var typeName = type.AssemblyQualifiedName;
            
            if (string.IsNullOrEmpty(typeName)) return null;

            if (_propertyDrawerCache.TryGetValue(typeName, out var drawer))
            {
                return drawer;
            }
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes();
                
                foreach (var t in types)
                {
                    if (t.IsDefined(typeof(CustomPropertyDrawer), false))
                    {
                        var attributes = t.GetCustomAttributes(typeof(CustomPropertyDrawer), false) as CustomPropertyDrawer[];
                        
                        if (attributes == null) return null;
                        
                        foreach (var attribute in attributes)
                        {
                            if (attribute == null) continue;
                            
                            var fieldInfo = typeof(CustomPropertyDrawer)
                                .GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
                            var drawerTypeFor = fieldInfo?.GetValue(attribute) as Type;
                            
                            var useForChildrenField = typeof(CustomPropertyDrawer)
                                .GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);
                            var useForChildren = (bool)(useForChildrenField?.GetValue(attribute) ?? false);
                            
                            if (drawerTypeFor != null &&
                                (drawerTypeFor == type || (useForChildren && type.IsSubclassOf(drawerTypeFor))))
                            {
                                var instance = Activator.CreateInstance(t) as PropertyDrawer;
                                
                                return _propertyDrawerCache[typeName] = instance;
                            }
                        }
                    }
                }
            }
            
            return _propertyDrawerCache[typeName] = null;
        }
    }
}