using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegisToolChain.Utils
{
    public class BiMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _forward = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TValue, TKey> _reverse = new Dictionary<TValue, TKey>();

        public int Count => _forward.Count;

        public TValue this[TKey key]
        {
            get => _forward[key];
            set
            {
                if (_forward.TryGetValue(key, out var existingValue))
                {
                    if (!EqualityComparer<TValue>.Default.Equals(existingValue, value))
                        throw new ArgumentException($"Key '{key}' is already mapped to '{existingValue}'.");
                }
                else if (_reverse.TryGetValue(value, out var existingKey))
                {
                    if (!EqualityComparer<TKey>.Default.Equals(existingKey, key))
                        throw new ArgumentException($"Value '{value}' is already mapped to key '{existingKey}'.");
                }

                _forward[key] = value;
                _reverse[value] = key;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (_forward.ContainsKey(key))
                throw new ArgumentException("Duplicate key", nameof(key));
            if (_reverse.ContainsKey(value))
                throw new ArgumentException("Duplicate value", nameof(value));

            _forward[key] = value;
            _reverse[value] = key;
        }

        public bool TryGetByKey(TKey key, out TValue value) => _forward.TryGetValue(key, out value);

        public bool TryGetByValue(TValue value, out TKey key) => _reverse.TryGetValue(value, out key);

        public TValue GetByKey(TKey key) => _forward[key];

        public TKey GetByValue(TValue value) => _reverse[value];

        public bool ContainsKey(TKey key) => _forward.ContainsKey(key);

        public bool ContainsValue(TValue value) => _reverse.ContainsKey(value);

        public bool RemoveByKey(TKey key)
        {
            if (_forward.TryGetValue(key, out TValue value))
            {
                _forward.Remove(key);
                _reverse.Remove(value);
                return true;
            }
            return false;
        }

        public bool RemoveByValue(TValue value)
        {
            if (_reverse.TryGetValue(value, out TKey key))
            {
                _reverse.Remove(value);
                _forward.Remove(key);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        public IEnumerable<TKey> Keys => _forward.Keys;
        public IEnumerable<TValue> Values => _reverse.Keys;
    }

}
