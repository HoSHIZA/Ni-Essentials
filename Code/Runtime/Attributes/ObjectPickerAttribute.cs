#if !NIGAMES_INSPECTOR_ATTRIBUTES_DISABLE
using System;
using System.Diagnostics;
using UnityEngine;

namespace NiGames.Essentials
{
    [Conditional("UNITY_EDITOR")]
    public sealed class ObjectPickerAttribute : PropertyAttribute
    {
        public readonly Type BaseType;
        
        public ObjectPickerAttribute()
        {
        }
        
        public ObjectPickerAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}
#endif