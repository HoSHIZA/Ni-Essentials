using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiGames.Essentials
{
    /// <summary>
    /// Represents a value that may or may not exist, similar to Nullable but for reference types.
    /// </summary>
    /// <remarks>Can be serialized in Unity inspector.</remarks>
    [Serializable]
    public struct Optional<T> : IEquatable<Optional<T>>, IComparable<Optional<T>>, IComparable<T>
    {
        [SerializeField] private bool _hasValue;
        [SerializeField] private T _value;

        public bool HasValue => _hasValue;
        public T Value => _hasValue ? _value : default;

        public Optional(T value)
        {
            _value = value;
            _hasValue = value != null;
        }

        public T GetValueOrDefault()
        {
            return _hasValue ? _value : default;
        }

        public T GetValueOrDefault(T defaultValue)
        {
            return _hasValue ? _value : defaultValue;
        }

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is Optional<T> other) return Equals(other);
            return false;
        }
        
        public override int GetHashCode()
        {
            return _hasValue ? _value?.GetHashCode() ?? 0 : 0;
        }
        
        public override string ToString()
        {
            return _hasValue ? _value?.ToString() ?? "null" : "None";
        }

        #endregion
        
        #region Interface Implementations

        public bool Equals(Optional<T> other)
        {
            if (!_hasValue && !other._hasValue) return true;
            if (_hasValue != other._hasValue) return false;
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        public int CompareTo(Optional<T> other)
        {
            if (!_hasValue && !other._hasValue) return 0;
            if (!_hasValue) return -1;
            if (!other._hasValue) return 1;
            return Comparer<T>.Default.Compare(_value, other._value);
        }

        public int CompareTo(T other)
        {
            if (!_hasValue) return -1;
            return Comparer<T>.Default.Compare(_value, other);
        }
        
        #endregion

        #region Operators

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static implicit operator Optional<T>(bool value)
        {
            return new Optional<T> { _hasValue = value };
        }

        public static bool operator ==(Optional<T> left, Optional<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Optional<T> left, Optional<T> right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(Optional<T> left, T right)
        {
            return left._hasValue && EqualityComparer<T>.Default.Equals(left._value, right);
        }

        public static bool operator !=(Optional<T> left, T right)
        {
            return !(left == right);
        }

        public static bool operator ==(T left, Optional<T> right)
        {
            return right == left;
        }

        public static bool operator !=(T left, Optional<T> right)
        {
            return !(left == right);
        }

        #endregion
    }
}