using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class EnumerableExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_returns_true_for_null()
    {
        EnumerableExtensions.IsNullOrEmpty<int>(null).Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_returns_true_for_empty()
    {
        EnumerableExtensions.IsNullOrEmpty(Enumerable.Empty<int>()).Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_returns_false_for_non_empty()
    {
        EnumerableExtensions.IsNullOrEmpty(new[] { 1 }).Should().BeFalse();
    }

    [Fact]
    public void EmptyIfNull_returns_empty_for_null()
    {
        EnumerableExtensions.EmptyIfNull<int>(null).Should().BeEmpty();
    }

    [Fact]
    public void EmptyIfNull_returns_original_for_non_null()
    {
        var items = new[] { 1, 2, 3 };
        EnumerableExtensions.EmptyIfNull(items).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void RandomOrder_returns_all_elements()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var result = items.RandomOrder().ToList();
        result.Should().HaveCount(5);
        result.Should().Contain(items);
    }

    [Fact]
    public void TakeRandom_returns_null_default_for_null()
    {
        ((IEnumerable<int>)null).TakeRandom().Should().Be(default);
    }

    [Fact]
    public void TakeRandom_returns_default_for_empty()
    {
        Enumerable.Empty<int>().TakeRandom().Should().Be(default);
    }

    [Fact]
    public void TakeRandom_returns_element_from_collection()
    {
        var items = new[] { 10, 20, 30 };
        items.TakeRandom().Should().BeOneOf(10, 20, 30);
    }

    [Fact]
    public void TakeRandom_and_RandomOrder_stay_random_after_concurrent_use()
    {
        var items = Enumerable.Range(0, 100).ToArray();

        Parallel.For(0, 500_000, i =>
        {
            if (i % 100 == 0) items.RandomOrder().First();
            else items.TakeRandom();
        });

        var picks = Enumerable.Range(0, 1000).Select(_ => items.TakeRandom()).Distinct().Count();
        picks.Should().BeGreaterThan(1);
        items.RandomOrder().Should().BeEquivalentTo(items);
    }
}
