using System;

namespace NiGames.Essentials
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MonoBehaviourSingletonAttribute : Attribute
    {
        public readonly bool DontDestroyOnLoad;

        public bool DestroyNonSingletonInstances = true;

        public MonoBehaviourSingletonAttribute()
        {
            DontDestroyOnLoad = true;
        }
        
        public MonoBehaviourSingletonAttribute(bool dontDestroyOnLoad)
        {
            DontDestroyOnLoad = dontDestroyOnLoad;
        }
    }
}