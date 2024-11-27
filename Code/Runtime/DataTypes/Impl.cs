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

        public Impl(T value)
        {
            _value = value as Object;
        }
        
        public T Value
        {
            get => _value as T;
            set => _value = value as Object;
        }
        
        public bool IsValid()
        {
            return _value is null or T;
        }

        #region Implicit Operators
        
        public static implicit operator T(Impl<T> value)
        {
            return value.Value;
        }

        public static implicit operator Impl<T>(T value)
        {
            return new Impl<T>(value);
        }
        
        #endregion
    }
}