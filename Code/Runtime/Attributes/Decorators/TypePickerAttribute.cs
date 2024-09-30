using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace NiGames.Essentials
{
    [PublicAPI]
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class TypePickerAttribute : PropertyAttribute
    {
        internal readonly Type BaseType;
        
        public TypeFilterMask TypeFilter;
        
        public TypePickerAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
    
    [Flags]
    public enum TypeFilterMask : byte
    {
        All = byte.MaxValue,
        Default = Class | ValueType,
        
        SerializablePreset = Default | Serializable,
        SerializedReferencePreset = SerializablePreset | HasAnyEmptyConstructor,
        
        None = 0,
        Class = 1 << 0,
        ValueType = 1 << 1,
        Abstract = 1 << 2,
        Serializable = 1 << 3,
        HasConstructor = 1 << 4,
        HasAnyEmptyConstructor = 1 << 5,
    }
}
