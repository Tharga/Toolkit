using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Allows execution of one thread at a time, for a specific key.
    /// Threads with different keys will be executed in parallel.
    /// Thread with the same key will be executed one at a time.
    /// </summary>
    /// <remarks>
    /// Each instance has its own locks, so code that must not run at the same time for the same key has to share one instance.
    /// The lock is not reentrant: an action that calls <see cref="ExecuteAsync{T}"/> on the same instance with the same key waits for itself.
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    public class SemaphoreExecutor<TKey>
    {
        private readonly Dictionary<TKey, SemaphoreWrapper> _locks = new Dictionary<TKey, SemaphoreWrapper>();

        public async Task<T> ExecuteAsync<T>(TKey key, Func<Task<T>> action)
        {
            var semaphoreWrapper = AcquireWrapper(key);

            try
            {
                await semaphoreWrapper.Semaphore.WaitAsync();
                try
                {
                    return await action.Invoke();
                }
                finally
                {
                    semaphoreWrapper.Semaphore.Release();
                }
            }
            finally
            {
                ReleaseWrapper(key, semaphoreWrapper);
            }
        }

        private SemaphoreWrapper AcquireWrapper(TKey key)
        {
            lock (_locks)
            {
                if (!_locks.TryGetValue(key, out var semaphoreWrapper))
                {
                    semaphoreWrapper = new SemaphoreWrapper();
                    _locks.Add(key, semaphoreWrapper);
                }

                semaphoreWrapper.RefCount++;
                return semaphoreWrapper;
            }
        }

        private void ReleaseWrapper(TKey key, SemaphoreWrapper semaphoreWrapper)
        {
            lock (_locks)
            {
                semaphoreWrapper.RefCount--;
                if (semaphoreWrapper.RefCount == 0)
                {
                    _locks.Remove(key);
                }
            }
        }

        private class SemaphoreWrapper
        {
            public readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
            public int RefCount;
        }
    }
}
