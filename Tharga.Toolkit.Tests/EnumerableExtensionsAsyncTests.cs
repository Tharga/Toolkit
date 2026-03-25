using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests;

public class EnumerableExtensionsAsyncTests
{
    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }

    private static async IAsyncEnumerable<T> EmptyAsyncEnumerable<T>()
    {
        await Task.CompletedTask;
        yield break;
    }

    [Fact]
    public async Task TakeRandomAsync_returns_element_from_sequence()
    {
        var source = ToAsyncEnumerable(10, 20, 30);
        var result = await source.TakeRandomAsync();
        result.Should().BeOneOf(10, 20, 30);
    }

    [Fact]
    public async Task TakeRandomAsync_returns_default_for_empty()
    {
        var source = EmptyAsyncEnumerable<int>();
        var result = await source.TakeRandomAsync();
        result.Should().Be(default);
    }

    [Fact]
    public async Task TakeRandomAsync_returns_single_element()
    {
        var source = ToAsyncEnumerable(42);
        var result = await source.TakeRandomAsync();
        result.Should().Be(42);
    }

    [Fact]
    public async Task RandomOrderAsync_returns_all_elements()
    {
        var source = ToAsyncEnumerable(1, 2, 3, 4, 5);
        var result = new List<int>();
        await foreach (var item in source.RandomOrderAsync())
        {
            result.Add(item);
        }

        result.Should().HaveCount(5);
        result.Should().Contain(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public async Task RandomOrderAsync_empty_returns_empty()
    {
        var source = EmptyAsyncEnumerable<int>();
        var result = new List<int>();
        await foreach (var item in source.RandomOrderAsync())
        {
            result.Add(item);
        }

        result.Should().BeEmpty();
    }
}
