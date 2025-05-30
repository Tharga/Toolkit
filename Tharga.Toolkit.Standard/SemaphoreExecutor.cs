using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Allows execution of one thread at a time, for a specific key.
    /// Threads with different keys will be executed in parallel.
    /// Thread with the same key will be executed one at a time.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class SemaphoreExecutor<TKey>
    {
        private static readonly ConcurrentDictionary<TKey, SemaphoreWrapper> _locks = new ConcurrentDictionary<TKey, SemaphoreWrapper>();

        public async Task<T> ExecuteAsync<T>(TKey key, Func<Task<T>> action)
        {
            var semaphoreWrapper = _locks.GetOrAdd(key, _ => new SemaphoreWrapper());
            semaphoreWrapper.Increment();

            try
            {
                await semaphoreWrapper.Semaphore.WaitAsync();

                var result = await action.Invoke();
                return result;
            }
            finally
            {
                semaphoreWrapper.Semaphore.Release();

                if (semaphoreWrapper.Decrement() == 0)
                {
                    _locks.TryRemove(key, out _);
                }
            }
        }

        private class SemaphoreWrapper
        {
            public readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
            private int _refCount;

            public void Increment() => Interlocked.Increment(ref _refCount);
            public int Decrement() => Interlocked.Decrement(ref _refCount);
        }
    }
}
