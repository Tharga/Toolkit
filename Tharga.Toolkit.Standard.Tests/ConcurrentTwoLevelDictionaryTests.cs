using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class ConcurrentTwoLevelDictionaryTests
{
    [Fact]
    public void AddOrUpdate_adds_new_item()
    {
        var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
        var result = dict.AddOrUpdate("main", "sub", 42);
        result.Before.Should().Be(default);
        result.After.Should().Be(42);
    }

    [Fact]
    public void AddOrUpdate_updates_existing_item()
    {
        var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
        dict.AddOrUpdate("main", "sub", 42);
        var result = dict.AddOrUpdate("main", "sub", 99);
        result.Before.Should().Be(42);
        result.After.Should().Be(99);
    }

    [Fact]
    public void AddOrUpdate_adds_sub_key_to_existing_main()
    {
        var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
        dict.AddOrUpdate("main", "sub1", 1);
        var result = dict.AddOrUpdate("main", "sub2", 2);
        result.Before.Should().Be(default);
        result.After.Should().Be(2);
    }

    [Fact]
    public void TryGetSubDictonary_returns_true_for_existing_key()
    {
        var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
        dict.AddOrUpdate("main", "sub", 42);

        dict.TryGetSubDictonary("main", out var sub).Should().BeTrue();
        sub.Should().ContainKey("sub");
        sub["sub"].Should().Be(42);
    }

    [Fact]
    public void TryGetSubDictonary_returns_false_for_missing_key()
    {
        var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
        dict.TryGetSubDictonary("missing", out _).Should().BeFalse();
    }

    [Fact]
    public void TryRemove_removes_and_returns_sub_dictionary()
    {
        var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
        dict.AddOrUpdate("main", "sub", 42);

        dict.TryRemove("main", out var removed).Should().BeTrue();
        removed.Should().ContainKey("sub");
        dict.TryGetSubDictonary("main", out _).Should().BeFalse();
    }

    [Fact]
    public void TryRemove_returns_false_for_missing_key()
    {
        var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
        dict.TryRemove("missing", out _).Should().BeFalse();
    }

    [Fact]
    public async Task TryRemove_concurrent_with_AddOrUpdate_never_loses_the_write()
    {
        const int iterations = 20000;
        var lost = 0;

        for (var i = 0; i < iterations; i++)
        {
            var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
            dict.AddOrUpdate("main", "existing", 1);
            using var barrier = new Barrier(2);
            var seenByRemover = false;

            var adder = Task.Run(() =>
            {
                barrier.SignalAndWait();
                dict.AddOrUpdate("main", "added", 2);
            });
            var remover = Task.Run(() =>
            {
                barrier.SignalAndWait();
                dict.TryRemove("main", out var removed);
                seenByRemover = removed.ContainsKey("added");
            });
            await Task.WhenAll(adder, remover);

            var storedAfterwards = dict.TryGetSubDictonary("main", out var current) && current.ContainsKey("added");
            if (!seenByRemover && !storedAfterwards) lost++;
        }

        lost.Should().Be(0);
    }
}
