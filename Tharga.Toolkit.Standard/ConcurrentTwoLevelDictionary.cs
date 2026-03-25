using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Tharga.Toolkit
{
    /// <summary>
    /// A thread-safe two-level dictionary that uses a main key to access a sub-dictionary keyed by a sub key.
    /// </summary>
    /// <typeparam name="TMainKey">The type of the main dictionary key.</typeparam>
    /// <typeparam name="TSubKey">The type of the sub-dictionary key.</typeparam>
    /// <typeparam name="TData">The type of the stored data values.</typeparam>
    public class ConcurrentTwoLevelDictionary<TMainKey, TSubKey, TData>
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<TMainKey, ConcurrentDictionary<TSubKey, TData>> _mainStore = new ConcurrentDictionary<TMainKey, ConcurrentDictionary<TSubKey, TData>>();

        /// <summary>
        /// Adds or updates a value in the two-level dictionary under the specified main key and sub key.
        /// </summary>
        /// <param name="mainKey">The main dictionary key.</param>
        /// <param name="subKey">The sub-dictionary key.</param>
        /// <param name="data">The data value to store.</param>
        /// <returns>A tuple containing the previous value (Before) and the new value (After).</returns>
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

        /// <summary>
        /// Attempts to retrieve the sub-dictionary associated with the specified main key.
        /// </summary>
        /// <param name="mainKey">The main dictionary key.</param>
        /// <param name="dictionary">When this method returns, contains the sub-dictionary if found; otherwise, null.</param>
        /// <returns>True if the sub-dictionary was found; otherwise, false.</returns>
        public bool TryGetSubDictonary(TMainKey mainKey, out ConcurrentDictionary<TSubKey, TData> dictionary)
        {
            return _mainStore.TryGetValue(mainKey, out dictionary);
        }

        /// <summary>
        /// Attempts to remove the sub-dictionary associated with the specified main key.
        /// </summary>
        /// <param name="mainKey">The main dictionary key.</param>
        /// <param name="dictionary">When this method returns, contains the removed sub-dictionary if successful; otherwise, null.</param>
        /// <returns>True if the sub-dictionary was removed; otherwise, false.</returns>
        public bool TryRemove(TMainKey mainKey, out ConcurrentDictionary<TSubKey, TData> dictionary)
        {
            return _mainStore.TryRemove(mainKey, out dictionary);
        }
    }
}