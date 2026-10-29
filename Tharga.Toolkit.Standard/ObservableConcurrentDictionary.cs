using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Tharga.Toolkit
{
    /// <summary>
    /// A thread-safe dictionary that raises collection and property change notifications when items are added, removed, or modified.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class ObservableConcurrentDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        private readonly ConcurrentDictionary<TKey, TValue> _dict = new ConcurrentDictionary<TKey, TValue>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the value for a key. Setting a new key raises <see cref="NotifyCollectionChangedAction.Add"/>;
        /// setting an existing key raises <see cref="NotifyCollectionChangedAction.Replace"/> with the old and the new value.
        /// </summary>
        public TValue this[TKey key]
        {
            get => _dict[key];
            set
            {
                var added = false;
                var oldValue = default(TValue);
                _dict.AddOrUpdate(key,
                    _ =>
                    {
                        added = true;
                        return value;
                    },
                    (_, existing) =>
                    {
                        added = false;
                        oldValue = existing;
                        return value;
                    });

                if (added)
                {
                    OnAdded(value);
                }
                else
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue));
                    OnPropertyChanged(nameof(Values));
                }
            }
        }

        public ICollection<TKey> Keys => _dict.Keys;
        public ICollection<TValue> Values => _dict.Values;
        public int Count => _dict.Count;
        public bool IsReadOnly => false;

        public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);

        /// <summary>
        /// Adds the key and value.
        /// </summary>
        /// <exception cref="ArgumentException">The key already exists. Use <see cref="TryAdd"/> to add only when the key is missing.</exception>
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException($"An item with the same key has already been added. Key: {key}", nameof(key));
        }

        /// <summary>
        /// Adds the key and value if the key does not exist yet, raising the same notifications as <see cref="Add(TKey, TValue)"/>.
        /// </summary>
        /// <returns><c>true</c> if the key and value were added; <c>false</c> if the key already exists.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (!_dict.TryAdd(key, value)) return false;

            OnAdded(value);
            return true;
        }

        public bool Remove(TKey key)
        {
            if (!_dict.TryRemove(key, out var value)) return false;

            OnRemoved(value);
            return true;
        }

        public void Clear()
        {
            _dict.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
        }

        public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public bool Contains(KeyValuePair<TKey, TValue> item) => _dict.Contains(item);

        /// <summary>
        /// Removes the key only when its value equals the value of <paramref name="item"/>.
        /// </summary>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!((ICollection<KeyValuePair<TKey, TValue>>)_dict).Remove(item)) return false;

            OnRemoved(item.Value);
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((IDictionary<TKey, TValue>)_dict).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void OnAdded(TValue value)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
        }

        private void OnRemoved(TValue value)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) =>
            CollectionChanged?.Invoke(this, e);

        protected virtual void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}