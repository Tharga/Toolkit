using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Tharga.Toolkit
{
    public class ConcurrentTwoLevelDictionary<TMainKey, TSubKey, TData>
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<TMainKey, ConcurrentDictionary<TSubKey, TData>> _mainStore = new ConcurrentDictionary<TMainKey, ConcurrentDictionary<TSubKey, TData>>();

        public (TData Before, TData After) AddOrUpdate(TMainKey mainKey, TSubKey subKey, TData data)
        {
            try
            {
                _lock.Wait();

                if (_mainStore.TryGetValue(mainKey, out var sub))
                {
                    if (sub.TryGetValue(subKey, out var current))
                    {
                        if (sub.TryUpdate(subKey, data, current))
                        {
                            return (current, data);
                        }

                        return (current, current);
                    }

                    if (sub.TryAdd(subKey, data))
                    {
                        return (default, data);
                    }

                    throw new InvalidOperationException($"Cannot add or find sub-item with key {subKey} inside item {mainKey}.");
                }

                if (_mainStore.TryAdd(mainKey, new ConcurrentDictionary<TSubKey, TData>(new Dictionary<TSubKey, TData> { { subKey, data } })))
                {
                    return (default, data);
                }

                throw new InvalidOperationException($"Cannot add or find item with key {mainKey}.");
            }
            finally
            {
                _lock.Release();
            }
        }

        public bool TryGetSubDictonary(TMainKey mainKey, out ConcurrentDictionary<TSubKey, TData> dictionary)
        {
            return _mainStore.TryGetValue(mainKey, out dictionary);
        }

        public bool TryRemove(TMainKey mainKey, out ConcurrentDictionary<TSubKey, TData> dictionary)
        {
            return _mainStore.TryRemove(mainKey, out dictionary);
        }
    }
}