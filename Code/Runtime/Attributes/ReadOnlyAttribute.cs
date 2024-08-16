#if !NIGAMES_INSPECTOR_ATTRIBUTES_DISABLE
using System.Diagnostics;
using UnityEngine;

namespace NiGames.Essentials
{
    [Conditional("UNITY_EDITOR")]
    public sealed class ReadOnlyAttribute : PropertyAttribute
    {
    }
}
#endif