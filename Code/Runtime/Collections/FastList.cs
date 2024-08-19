using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NiGames.Essentials
{
    /// <summary>
    /// Wrapper over an array with minimal list functionality.
    /// </summary>
    public struct FastList<T>
    {
        private T[] _items;
        private int _count;
        
        public static readonly FastList<T> Empty = default;
        
        public T this[int index]
        {
            [MethodImpl(256)]
            get => _items[index];
            [MethodImpl(256)]
            set => _items[index] = value;
        }
        
        public T[] this[Range index]
        {
            [MethodImpl(256)]
            get => _items.AsSpan()[index].ToArray();
        }
        
        public int Count => _count;
        public int Length => _items.Length;
        
        public FastList(int capacity = 8)
        {
            _items = new T[capacity];
            _count = 0;
        }
        
        [MethodImpl(256)]
        public ref T ElementAt(int index)
        {
            return ref _items[index];
        }
        
        [MethodImpl(256)]
        public void Add(T item)
        {
            if (_items == null)
            {
                _items = new T[8];
            }
            else if (_items.Length == _count)
            {
                Array.Resize(ref _items, _items.Length * 2);
            }
            
            _items[_count++] = item;
        }
        
        [MethodImpl(256)]
        public void Remove(T item)
        {
            for (var i = 0; i < Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                {
                    _count--;
                    if (i < _count)
                    {
                        Array.Copy(_items, i + 1, _items, i, _count - i);
                    }
                    
                    _items[_count] = default;
                    
                    return;
                }
            }
            
            throw new ArgumentException(nameof(item));
        }
        
        [MethodImpl(256)]
        public void RemoveLast()
        {
            _count--;
            _items[_count] = default;
        }
        
        [MethodImpl(256)]
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _count--;
            if (index < _count)
            {
                Array.Copy(_items, index + 1, _items, index, _count - index);
            }

            _items[_count] = default;
        }
        
        [MethodImpl(256)]
        public void RemoveAtSwapBack(int index)
        {
            if (index < 0 || index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            _count--;
            if (index < _count)
            {
                _items[index] = _items[_count];
            }

            _items[_count] = default;
        }
        
        [MethodImpl(256)]
        public void SetCapacity(int capacity)
        {
            Array.Resize(ref _items, capacity);
        }
        
        [MethodImpl(256)]
        public void EnsureCapacity(int capacity)
        {
            if (capacity > _items.Length)
            {
                SetCapacity(capacity);
            }
        }

        [MethodImpl(256)]
        public int IndexOf(T item)
        {
            for (var i = 0; i < Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                {
                    return i;
                }
            }
            
            return -1;
        }
        
        [MethodImpl(256)]
        public bool Contains(T value)
        {
            return _items.Contains(value);
        }
        
        [MethodImpl(256)]
        public void Clear()
        {
            _items.AsSpan().Clear();
            _count = 0;
        }
        
        public T[] AsArray() => new Span<T>(_items, 0, _count).ToArray();
        public Span<T> AsSpan() => new(_items, 0, _count);
    }
}