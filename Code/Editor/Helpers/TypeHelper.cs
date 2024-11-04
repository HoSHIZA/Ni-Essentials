using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using Object = UnityEngine.Object;
using Type = System.Type;

namespace NiGames.Essentials.Editor
{
    public static class TypeHelper
    {
        private const BindingFlags CTOR_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        private static readonly Dictionary<string, TypesWithPaths> _typesWithPathsCache = new Dictionary<string, TypesWithPaths>();
        
        /// <summary>
        /// Works the same as <see cref="System.Activator.CreateInstance(Type)"/>, except that it allows you to create an instance
        /// of the type using a private constructor with no parameters.
        /// </summary>
        /// <returns>Created instance or null if it cannot be created.</returns>
        public static object CreateInstance(Type type)
        {
            if (type == null) return null;
            
            foreach (var ctor in type.GetConstructors(CTOR_BINDING_FLAGS))
            {
                if (ctor.GetParameters().Length != 0) continue;
                
                return ctor.Invoke(null);
            }
            
            return null;
        }
        
        /// <summary>
        /// Works the same as <see cref="System.Activator.CreateInstance{T}"/>, except that it allows you to create an instance
        /// of the type using a private constructor with no parameters.
        /// </summary>
        /// <returns>Created instance or null if it cannot be created.</returns>
        [MethodImpl(256)]
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }
        
        /// <inheritdoc cref="CreateInstance(System.Type)"/>
        public static object CreateInstance(string typeName, bool throwOnError = false)
        {
            var type = Type.GetType(typeName, throwOnError);
            
            if (type == null) return null;
            
            foreach (var ctor in type.GetConstructors(CTOR_BINDING_FLAGS))
            {
                if (ctor.GetParameters().Length != 0) continue;
                
                return ctor.Invoke(null);
            }
            
            return null;
        }
        
        /// <inheritdoc cref="GetTypeDisplayName"/>
        [MethodImpl(256)]
        public static string GetTypeDisplayName<T>()
        {
            return GetTypeDisplayName(typeof(T));
        }
        
        /// <summary>
        /// Returns the display name of the type, given the <see cref="DisplayNameAttribute"/> attribute.
        /// </summary>
        public static string GetTypeDisplayName(Type type)
        {
            if (type == null) return null;
            
            var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            var typeName = displayNameAttr != null && !string.IsNullOrWhiteSpace(displayNameAttr.DisplayName)
                ? displayNameAttr.DisplayName
                : type.Name;
            
            return typeName;
        }

        /// <summary>
        /// <inheritdoc cref="GetDerivedTypes"/>
        /// </summary>
        [MethodImpl(256)]
        public static Type[] GetDerivedTypes<T>(TypeFilterMask filter = TypeFilterMask.SerializablePreset)
        {
            return GetDerivedTypes(typeof(T), filter);
        }

        /// <summary>
        /// Returns all types that inherit from the passed base type. Taking into account the given parameters.
        /// </summary>
        public static Type[] GetDerivedTypes(Type baseType, TypeFilterMask filter = TypeFilterMask.SerializablePreset)
        {
            var @abstract = filter.HasFlagFast(TypeFilterMask.Abstract);
            var @class = filter.HasFlagFast(TypeFilterMask.Class);
            var valueType = filter.HasFlagFast(TypeFilterMask.ValueType);
            var serializable = filter.HasFlagFast(TypeFilterMask.Serializable);
            var hasCtor = filter.HasFlagFast(TypeFilterMask.HasConstructor);
            var hasAnyEmptyCtor = filter.HasFlagFast(TypeFilterMask.HasAnyEmptyConstructor);
            
            return TypeCache.GetTypesDerivedFrom(baseType)
                .Where(t =>
                    !typeof(Object).IsAssignableFrom(t) &&
                    GenericCheck(t) &&
                    AbstractCheck(t) &&
                    MemberTypeCheck(t) &&
                    SerializableCheck(t) &&
                    ConstructorCheck(t))
                .ToArray();
            
            [MethodImpl(256)]
            bool GenericCheck(Type t) => !t.IsGenericType;
            
            [MethodImpl(256)]
            bool AbstractCheck(Type t) => @abstract || !t.IsAbstract;
            
            [MethodImpl(256)]
            bool MemberTypeCheck(Type t) => valueType && @class || (valueType ? t.IsValueType : !t.IsValueType);
            
            [MethodImpl(256)]
            bool SerializableCheck(Type t) => !serializable || t.GetCustomAttribute<SerializableAttribute>() != null;
            
            [MethodImpl(256)]
            bool ConstructorCheck(Type t)
            {
                if (hasAnyEmptyCtor)
                {
                    return t
                        .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Any(c => !c.GetParameters().Any());
                }
                
                if (hasCtor)
                {
                    return t.GetConstructor(null) != null;
                }
                
                return true;
            }
        }
        
        /// <summary>
        /// Returns a <see cref="TypesWithPaths"/> structure that contains elements and element paths for use in GenericMenu.
        /// </summary>
        public static TypesWithPaths GetTypesWithPaths(Type baseType, TypeFilterMask filter = TypeFilterMask.SerializablePreset)
        {
            var typeId = ((byte)filter).ToString();
            var name = $"[{typeId}] {baseType.AssemblyQualifiedName}";
            
            if (_typesWithPathsCache.TryGetValue(name, out var list)) return list;
            
            list = new TypesWithPaths(baseType, filter);
            _typesWithPathsCache.Add(name, list);
            
            return list;
        }
        
        /// <summary>
        /// A struct that creates and describes elements and paths to them.
        /// </summary>
        public struct TypesWithPaths
        {
            public readonly Type BaseType;
            public readonly Type[] Items;
            public readonly string[] Paths;
            
            public TypesWithPaths(Type baseType, TypeFilterMask filter = TypeFilterMask.SerializablePreset)
            {
                BaseType = baseType;
                
                var derivedTypes = GetDerivedTypes(baseType, filter);
                
                var length = derivedTypes.Length;
                
                var types = new ValueTuple<Type, string>[length];
                for (var i = 0; i < length; i++)
                {
                    var type = derivedTypes[i];
                    
                    var typeName = GetTypeDisplayName(type);
                    var category = type.GetCustomAttribute<CategoryAttribute>()?.Category;
                    
                    var sb = new StringBuilder(typeName);
                    
                    if (category != null)
                    {
                        if (category != string.Empty)
                        {
                            sb.Insert(0, '/');
                            sb.Insert(0, category.TrimEnd('/'));
                        }
                    }
                    else
                    {
                        if (type != baseType)
                        {
                            if (derivedTypes.Any(t => t.BaseType == type))
                            {
                                sb.Append('/');
                                sb.Append(type.Name);
                            }
                        }

                        type = type.BaseType;
                        while (type != baseType && type != typeof(object))
                        {
                            sb.Insert(0, '/');
                            sb.Insert(0, type!.Name);
                            type = type.BaseType;
                        }
                    }

                    types[i] = new ValueTuple<Type, string>(derivedTypes[i], sb.ToString());
                }
                var sorted = types.OrderBy(t => t.Item2).ToArray();
                
                Paths = new string[length];
                Items = new Type[length];
                
                for (var i = 0; i < length; i++)
                {
                    Paths[i] = sorted[i].Item2;
                    Items[i] = sorted[i].Item1;
                }
            }
        }
    }
    
    public static class TypeFilterMaskExtensions
    {
        [MethodImpl(256)]
        public static bool HasFlagFast(this TypeFilterMask value, TypeFilterMask flag)
        {
            return (value & flag) != 0;
        }
    }
}