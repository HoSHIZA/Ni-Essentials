using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NiGames.Essentials
{
    /// <summary>
    /// A wrapper class that allows you to serialize interfaces and abstract classes in the inspector.
    /// </summary>
    [Serializable]
    public struct Impl<T> where T : class
    {
        [SerializeField] private Object _value;
        
        public T Value
        {
            get => _value as T;
            set => _value = value as Object;
        }
    }
}