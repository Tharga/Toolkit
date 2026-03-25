using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests;

public class IdentityKeyTests
{
    [Fact]
    public void Constructor_stores_value()
    {
        var key = new IdentityKey("test-value");
        key.Value.Should().Be("test-value");
    }

    [Fact]
    public void Record_equality_same_value()
    {
        var key1 = new IdentityKey("abc");
        var key2 = new IdentityKey("abc");
        key1.Should().Be(key2);
    }

    [Fact]
    public void Record_equality_different_value()
    {
        var key1 = new IdentityKey("abc");
        var key2 = new IdentityKey("xyz");
        key1.Should().NotBe(key2);
    }

    [Fact]
    public void GetHashCode_same_for_equal_keys()
    {
        var key1 = new IdentityKey("abc");
        var key2 = new IdentityKey("abc");
        key1.GetHashCode().Should().Be(key2.GetHashCode());
    }
}
