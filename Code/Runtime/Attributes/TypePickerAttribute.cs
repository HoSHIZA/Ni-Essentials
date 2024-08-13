﻿using System;
using System.Diagnostics;
using UnityEngine;

namespace NiGames.Essentials
{
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
    
#if !DISABLE_NIGAMES_INSPECTOR_ATTRIBUTES_FEATURE
    [Conditional("UNITY_EDITOR")]
    public sealed class TypePickerAttribute : PropertyAttribute
    {
        public readonly Type BaseType;
        
        public TypeFilterMask TypeFilter { get; set; }
        
        public TypePickerAttribute()
        {
        }
        
        public TypePickerAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
#endif
}