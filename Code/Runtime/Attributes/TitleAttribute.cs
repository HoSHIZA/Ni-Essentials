using System;
using System.Diagnostics;
using UnityEngine;

namespace NiGames.Essentials
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class TitleAttribute : PropertyAttribute
    {
        internal readonly string Header;
        internal readonly int LineHeight;
        
        public string LabelColor;
        public string LineColor;
        
        public TitleAttribute(string header, int lineHeight = 1)
        {
            Header = header;
            LineHeight = lineHeight;
        }
    }
}