using System;

namespace NiGames.Essentials
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ScriptableObjectSingletonAttribute : Attribute
    {
        public string ResourcesPath;
    }
}