using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace NiGames.Essentials.Editor
{
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Gets the type of ManagedReference of the field.
        /// </summary>
        public static Type GetManagedFieldType(this SerializedProperty property)
        {
            if (property == null) return null;
            if (property.propertyType != SerializedPropertyType.ManagedReference) return null;
            if (string.IsNullOrEmpty(property.managedReferenceFieldTypename)) return null;

            var typeSplit = property.managedReferenceFieldTypename.Split(' ');

            if (typeSplit.Length != 2) return null;

            var assemblyName = typeSplit[0];
            var typeName = typeSplit[1];

            var assembly = Assembly.Load(assemblyName);

            if (assembly == null) return null;

            var type = assembly.GetType(typeName);

            return type;
        }
        
        /// <summary>
        /// Gets the value type of the ManagedReference of the field.
        /// </summary>
        public static Type GetManagedValueType(this SerializedProperty property)
        {
            if (property == null) return null;
            if (property.propertyType != SerializedPropertyType.ManagedReference) return null;
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename)) return null;
            
            var typeSplit = property.managedReferenceFullTypename.Split(' ');
            
            if (typeSplit.Length != 2) return null;
            
            var assemblyName = typeSplit[0];
            var typeName = typeSplit[1];
            
            var assembly = Assembly.Load(assemblyName);
            
            if (assembly == null) return null;
            
            var type = assembly.GetType(typeName);
            
            return type;
        }
        
        /// <summary>
        /// Returns all visible children of <see cref="SerializedProperty"/>.
        /// </summary>
        public static IEnumerable<SerializedProperty> GetChildProperties(this SerializedProperty property)
        {
            if (property.isArray)
            {
                for (var i = 0; i < property.arraySize; i++)
                {
                    yield return property.GetArrayElementAtIndex(i);
                }
            }
            else if (string.IsNullOrEmpty(property.propertyPath))
            {
                var iterator = property.Copy();
                var valid = iterator.NextVisible(true);

                while (valid)
                {
                    yield return iterator.Copy();

                    valid = iterator.NextVisible(false);
                }
            }
            else
            {
                var iterator = property.Copy();
                var end = iterator.GetEndProperty();
                var valid = iterator.NextVisible(true);

                while (valid && !SerializedProperty.EqualContents(iterator, end))
                {
                    yield return iterator.Copy();
                    
                    valid = iterator.NextVisible(false);
                }
            }
        }
    }
}