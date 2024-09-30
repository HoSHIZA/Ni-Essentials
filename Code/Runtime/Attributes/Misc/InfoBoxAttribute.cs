using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace NiGames.Essentials
{
    [PublicAPI]
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class InfoBoxAttribute : PropertyAttribute
    {
        internal readonly string Message;
        
        public MessageType Type;

        public InfoBoxAttribute(string message)
        {
            Message = message;
        }
        
        public InfoBoxAttribute(string message, MessageType type)
        {
            Message = message;
            Type = type;
        }
    }
    
    [PublicAPI]
    public enum MessageType
    {
        None,
        Info,
        Warning,
        Error,
    }
}