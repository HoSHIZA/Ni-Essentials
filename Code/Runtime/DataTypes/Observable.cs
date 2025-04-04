using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiGames.Essentials
{
    /// <summary>
    /// A wrapper class that provides change notifications for its value.
    /// </summary>
    [Serializable]
    public class Observable<T> : IComparable, 
        IComparable<T>, IComparable<Observable<T>>, 
        IEquatable<T>, IEquatable<Observable<T>>
    {
        [SerializeField] private T _value;
        
        public event Action<T> BeforeValueChanged;
        public event Action<T> ValueChanged;
        
        public T Value
        {
            get => _value;
            set
            {
                BeforeValueChanged?.Invoke(value);
                
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;
                
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }

        public Observable(T initialValue = default)
        {
            _value = initialValue;
        }

        public void SetValueWithoutNotify(T value)
        {
            _value = value;
        }

        #region Overrides
        
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Observable<T>)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BeforeValueChanged, ValueChanged, _value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
        
        #endregion
        
        #region Interface Implementations

        public int CompareTo(object obj)
        {
            return CompareTo((Observable<T>)obj);
        }

        public int CompareTo(T other)
        {
            return Comparer<T>.Default.Compare(_value, other);
        }

        public int CompareTo(Observable<T> other)
        {
            return Comparer<T>.Default.Compare(_value, other.Value);
        }

        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(_value, other);
        }

        public bool Equals(Observable<T> other)
        {
            return other != null && EqualityComparer<T>.Default.Equals(_value, other.Value);
        }

        #endregion
        
        #region Operators
        
        public static implicit operator T(Observable<T> observable)
        {
            return observable.Value;
        }

        public static implicit operator Observable<T>(T initialValue)
        {
            return new Observable<T>(initialValue);
        }
        
        public static bool operator ==(Observable<T> left, Observable<T> right)
        {
            return EqualityComparer<Observable<T>>.Default.Equals(left, right);
        }

        public static bool operator ==(T left, Observable<T> right)
        {
            return EqualityComparer<T>.Default.Equals(left, right);
        }

        public static bool operator ==(Observable<T> left, T right)
        {
            return EqualityComparer<T>.Default.Equals(left, right);
        }

        public static bool operator !=(Observable<T> left, Observable<T> right)
        {
            return !(left == right);
        }

        public static bool operator !=(T left, Observable<T> right)
        {
            return !(left == right);
        }

        public static bool operator !=(Observable<T> left, T right)
        {
            return !(left == right);
        }
        
        #endregion
    }
} 