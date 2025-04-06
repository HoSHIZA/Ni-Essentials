using System;
using System.Collections;
using System.Collections.Generic;

namespace NiGames.Essentials.Collections
{
    /// <summary>
    /// Represents a bidirectional dictionary that provides a two-way mapping between keys and values.
    /// Neither keys nor values can have duplicates.
    /// </summary>
    public class BidirectionalDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> _forward;
        private readonly Dictionary<TValue, TKey> _reverse;
        
        public BidirectionalDictionary()
        {
            _forward = new Dictionary<TKey, TValue>();
            _reverse = new Dictionary<TValue, TKey>();
        }
        
        public BidirectionalDictionary(int capacity)
        {
            _forward = new Dictionary<TKey, TValue>(capacity);
            _reverse = new Dictionary<TValue, TKey>(capacity);
        }
        
        public BidirectionalDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _forward = new Dictionary<TKey, TValue>(keyComparer);
            _reverse = new Dictionary<TValue, TKey>(valueComparer);
        }
        
        public BidirectionalDictionary(int capacity, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _forward = new Dictionary<TKey, TValue>(capacity, keyComparer);
            _reverse = new Dictionary<TValue, TKey>(capacity, valueComparer);
        }
        
        public TValue this[TKey key] => _forward[key];
        public TKey this[TValue value] => _reverse[value];
        
        /// <summary>
        /// Gets the number of key-value pairs in the dictionary.
        /// </summary>
        public int Count => _forward.Count;
        
        /// <summary>
        /// Gets a collection containing the keys in the dictionary.
        /// </summary>
        public ICollection<TKey> Keys => _forward.Keys;

        /// <summary>
        /// Gets a collection containing the values in the dictionary.
        /// </summary>
        public ICollection<TValue> Values => _forward.Values;

        /// <summary>
        /// Adds a key-value pair to the dictionary.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when key or value is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the key or value already exists in the dictionary.</exception>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (_forward.ContainsKey(key))
                throw new ArgumentException($"The key '{key}' already exists in the dictionary.", nameof(key));
            if (_reverse.ContainsKey(value))
                throw new ArgumentException($"The value '{value}' already exists in the dictionary.", nameof(value));

            _forward.Add(key, value);
            _reverse.Add(value, key);
        }

        /// <summary>
        /// Removes a key-value pair from the dictionary using the key.
        /// </summary>
        /// <returns>true if the key was found and removed; otherwise, false.</returns>
        public bool RemoveByKey(TKey key)
        {
            if (key == null || !_forward.Remove(key, out var value)) return false;

            _reverse.Remove(value);
            
            return true;
        }

        /// <summary>
        /// Removes a key-value pair from the dictionary using the value.
        /// </summary>
        /// <returns>true if the value was found and removed; otherwise, false.</returns>
        public bool RemoveByValue(TValue value)
        {
            if (value == null || !_reverse.Remove(value, out var key)) return false;

            _forward.Remove(key);
            
            return true;
        }

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetByKey(TKey key, out TValue value)
        {
            return _forward.TryGetValue(key, out value);
        }

        /// <summary>
        /// Attempts to get the key associated with the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="key">When this method returns, contains the key associated with the specified value,
        /// if the value is found; otherwise, the default value for the type of the key parameter.</param>
        /// <returns>true if the dictionary contains an element with the specified value; otherwise, false.</returns>
        public bool TryGetByValue(TValue value, out TKey key)
        {
            return _reverse.TryGetValue(value, out key);
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKey(TKey key)
        {
            return _forward.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified value.
        /// </summary>
        /// <returns>true if the dictionary contains an element with the specified value; otherwise, false.</returns>
        public bool ContainsValue(TValue value)
        {
            return _reverse.ContainsKey(value);
        }

        /// <summary>
        /// Removes all key-value pairs from the dictionary.
        /// </summary>
        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary's key-value pairs.
        /// </summary>
        /// <returns>An enumerator for the dictionary.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary's key-value pairs.
        /// </summary>
        /// <returns>An enumerator for the dictionary.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Tries to add a key-value pair to the dictionary.
        /// </summary>
        /// <returns>true if the key-value pair was added successfully; otherwise, false.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (key == null || value == null || _forward.ContainsKey(key) || _reverse.ContainsKey(value))
                return false;

            _forward.Add(key, value);
            _reverse.Add(value, key);
            
            return true;
        }

        /// <summary>
        /// Gets the value associated with the specified key, or a default value if the key is not found.
        /// </summary>
        /// <returns>The value associated with the specified key, or the default value if the key is not found.</returns>
        public TValue GetValueOrDefault(TKey key, TValue defaultValue = default)
        {
            return _forward.GetValueOrDefault(key, defaultValue);
        }

        /// <summary>
        /// Gets the key associated with the specified value, or a default key if the value is not found.
        /// </summary>
        /// <returns>The key associated with the specified value, or the default key if the value is not found.</returns>
        public TKey GetKeyOrDefault(TValue value, TKey defaultKey = default)
        {
            return _reverse.GetValueOrDefault(value, defaultKey);
        }
    }
}