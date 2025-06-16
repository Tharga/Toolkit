using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Tharga.Toolkit
{
    public class ObservableConcurrentDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        private readonly ConcurrentDictionary<TKey, TValue> _dict = new ConcurrentDictionary<TKey, TValue>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public TValue this[TKey key]
        {
            get => _dict[key];
            set
            {
                _dict[key] = value;
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, key));
                OnPropertyChanged(nameof(Values));
            }
        }

        public ICollection<TKey> Keys => _dict.Keys;
        public ICollection<TValue> Values => _dict.Values;
        public int Count => _dict.Count;
        public bool IsReadOnly => false;

        public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);

        public void Add(TKey key, TValue value)
        {
            if (_dict.TryAdd(key, value))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(Keys));
                OnPropertyChanged(nameof(Values));
            }
        }

        public bool Remove(TKey key)
        {
            if (_dict.TryRemove(key, out var value))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(Keys));
                OnPropertyChanged(nameof(Values));
                return true;
            }

            return false;
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
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((IDictionary<TKey, TValue>)_dict).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) =>
            CollectionChanged?.Invoke(this, e);

        protected virtual void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}