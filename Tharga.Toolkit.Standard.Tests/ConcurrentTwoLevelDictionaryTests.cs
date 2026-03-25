using FluentAssertions;
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
}
