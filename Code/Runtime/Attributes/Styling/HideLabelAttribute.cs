using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace NiGames.Essentials
{
    [PublicAPI]
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class HideLabelAttribute : PropertyAttribute
    {
    }
}