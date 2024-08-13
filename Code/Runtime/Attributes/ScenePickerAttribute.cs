#if !DISABLE_NIGAMES_INSPECTOR_ATTRIBUTES_FEATURE
using System.Diagnostics;
using UnityEngine;

namespace NiGames.Essentials
{
    [Conditional("UNITY_EDITOR")]
    public sealed class ScenePickerAttribute : PropertyAttribute
    {
    }
}
#endif