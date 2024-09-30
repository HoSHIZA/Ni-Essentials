using System;
using System.Diagnostics;
using UnityEngine;

namespace NiGames.Essentials
{
    [Conditional("UNITY_EDITOR")]
    public sealed class ReferenceAttribute : PropertyAttribute
    {
        public readonly Type BaseType;
        
        /// <summary>
        /// Indicate whether to draw a Custom PropertyDrawer for the selected managed object.
        /// </summary>
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