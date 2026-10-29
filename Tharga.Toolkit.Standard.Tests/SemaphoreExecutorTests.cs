using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class SemaphoreExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_returns_result()
    {
        var executor = new SemaphoreExecutor<string>();
        var result = await executor.ExecuteAsync("key", () => Task.FromResult(42));
        result.Should().Be(42);
    }

    [Fact]
    public async Task ExecuteAsync_same_key_runs_sequentially()
    {
        var executor = new SemaphoreExecutor<string>();
        var order = new ConcurrentQueue<int>();

        var task1 = executor.ExecuteAsync("same", async () =>
        {
            order.Enqueue(1);
            await Task.Delay(50);
            order.Enqueue(2);
            return 1;
        });

        var task2 = executor.ExecuteAsync("same", async () =>
        {
            order.Enqueue(3);
            await Task.Delay(10);
            order.Enqueue(4);
            return 2;
        });

        await Task.WhenAll(task1, task2);

        var items = order.ToArray();
        // Task1 should complete (1,2) before task2 starts (3,4)
        items.Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public async Task ExecuteAsync_different_keys_run_concurrently()
    {
        var executor = new SemaphoreExecutor<string>();
        var concurrentCount = 0;
        var maxConcurrent = 0;
        var lockObj = new object();

        async Task<int> Work(string key)
        {
            return await executor.ExecuteAsync(key, async () =>
            {
                lock (lockObj)
                {
                    concurrentCount++;
                    if (concurrentCount > maxConcurrent)
                        maxConcurrent = concurrentCount;
                }

                await Task.Delay(50);

                lock (lockObj)
                {
                    concurrentCount--;
                }

                return 1;
            });
        }

        var tasks = new[]
        {
            Work("key1"),
            Work("key2"),
            Work("key3")
        };

        await Task.WhenAll(tasks);

        // With different keys, at least 2 should have run concurrently
        maxConcurrent.Should().BeGreaterThanOrEqualTo(2);
    }

    private static readonly TimeSpan SignalTimeout = TimeSpan.FromSeconds(5);

    [Fact(Timeout = 20000)]
    public async Task ExecuteAsync_on_separate_executors_with_the_same_key_does_not_block()
    {
        var first = new SemaphoreExecutor<string>();
        var second = new SemaphoreExecutor<string>();
        var firstIsRunning = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirst = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var firstTask = first.ExecuteAsync("separate", async () =>
        {
            firstIsRunning.TrySetResult(true);
            return await releaseFirst.Task;
        });
        await firstIsRunning.Task.WaitAsync(SignalTimeout, TestContext.Current.CancellationToken);

        var result = await second.ExecuteAsync("separate", () => Task.FromResult(2)).WaitAsync(SignalTimeout, TestContext.Current.CancellationToken);

        result.Should().Be(2);
        releaseFirst.TrySetResult(true);
        await firstTask.WaitAsync(SignalTimeout, TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 20000)]
    public async Task ExecuteAsync_nested_on_another_executor_with_the_same_key_completes()
    {
        var outer = new SemaphoreExecutor<string>();
        var inner = new SemaphoreExecutor<string>();

        var result = await outer.ExecuteAsync("nested", () => inner.ExecuteAsync("nested", () => Task.FromResult(1))).WaitAsync(SignalTimeout, TestContext.Current.CancellationToken);

        result.Should().Be(1);
    }

    [Fact(Timeout = 60000)]
    public async Task ExecuteAsync_same_key_never_runs_concurrently_while_entries_are_added_and_removed()
    {
        var executor = new SemaphoreExecutor<int>();
        var running = new int[4];
        var overlaps = 0;

        async Task<int> Work(int key)
        {
            return await executor.ExecuteAsync(key, async () =>
            {
                if (Interlocked.Increment(ref running[key]) > 1) Interlocked.Increment(ref overlaps);
                await Task.Yield();
                Interlocked.Decrement(ref running[key]);
                return key;
            });
        }

        var tasks = Enumerable.Range(0, 100_000).Select(i => Task.Run(() => Work(i % running.Length))).ToArray();
        await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(50), TestContext.Current.CancellationToken);

        overlaps.Should().Be(0);
    }
}
