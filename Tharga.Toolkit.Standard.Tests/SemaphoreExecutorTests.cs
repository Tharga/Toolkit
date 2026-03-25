using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
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
}
