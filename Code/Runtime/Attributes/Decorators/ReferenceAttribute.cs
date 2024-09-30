using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace NiGames.Essentials
{
    [PublicAPI]
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ReferenceAttribute : PropertyAttribute
    {
        internal readonly Type BaseType;
        
        /// <summary>
        /// Indicate whether to draw a Custom PropertyDrawer for the selected managed object.
        /// </summary>
        public bool DrawCustomReferenceDrawer = true;
        
        public ReferenceAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}