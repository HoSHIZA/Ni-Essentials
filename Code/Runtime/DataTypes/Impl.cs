using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NiGames.Essentials
{
    /// <summary>
    /// A wrapper class that allows you to serialize interfaces and abstract classes in the inspector.
    /// </summary>
    [Serializable]
    public struct Impl<T> : IEquatable<Impl<T>>, IEquatable<T> where T : class
    {
        [SerializeField] private Object _value;

        public T Value
        {
            get => _value as T;
            set => _value = value as Object;
        }

        public Impl(T value)
        {
            _value = value as Object;
        }

        public bool IsValid()
        {
            return _value && _value is T;
        }
        
        public bool TryGetValue(out T value)
        {
            value = IsValid() ? _value as T : null;
            
            return value != null;
        }

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is Impl<T> other) return Equals(other);
            if (obj is T value) return Equals(value);
            return false;
        }
        
        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return _value?.ToString() ?? "null";
        }

        #endregion

        #region Interface Implementations

        public bool Equals(Impl<T> other)
        {
            return ReferenceEquals(_value, other._value);
        }

        public bool Equals(T other)
        {
            return ReferenceEquals(_value, other);
        }

        #endregion
        
        #region Operators
        
        public static implicit operator T(Impl<T> value)
        {
            return value.Value;
        }

        public static implicit operator Impl<T>(T value)
        {
            return new Impl<T>(value);
        }

        public static bool operator ==(Impl<T> left, Impl<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Impl<T> left, Impl<T> right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(Impl<T> left, T right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Impl<T> left, T right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(T left, Impl<T> right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(T left, Impl<T> right)
        {
            return !right.Equals(left);
        }
        
        #endregion
    }
}