#if !DISABLE_NIGAMES_INSPECTOR_ATTRIBUTES_FEATURE
using System;
using System.Diagnostics;
using UnityEngine;

namespace NiGames.Essentials
{
    [Conditional("UNITY_EDITOR")]
    public sealed class ReferenceAttribute : PropertyAttribute
    {
        public readonly Type BaseType;
        
        public bool DrawCustomReferenceDrawer { get; set; } = true;
        
        public ReferenceAttribute()
        {
        }
        
        public ReferenceAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}
#endif