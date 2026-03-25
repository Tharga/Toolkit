using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class EnumerationTests
{
    private class TestEnumeration : Enumeration
    {
        public static readonly TestEnumeration First = new(1, "First");
        public static readonly TestEnumeration Second = new(2, "Second");
        public static readonly TestEnumeration Third = new(3, "Third");

        private TestEnumeration(int id, string name) : base(id, name) { }
    }

    private class OtherEnumeration : Enumeration
    {
        public static readonly OtherEnumeration First = new(1, "First");

        private OtherEnumeration(int id, string name) : base(id, name) { }
    }

    [Fact]
    public void GetAll_returns_all_static_fields()
    {
        var all = Enumeration.GetAll<TestEnumeration>().ToList();
        all.Should().HaveCount(3);
        all.Should().Contain(TestEnumeration.First);
        all.Should().Contain(TestEnumeration.Second);
        all.Should().Contain(TestEnumeration.Third);
    }

    [Fact]
    public void ToString_returns_name()
    {
        TestEnumeration.First.ToString().Should().Be("First");
    }

    [Fact]
    public void Id_returns_id()
    {
        TestEnumeration.Second.Id.Should().Be(2);
    }

    [Fact]
    public void Equals_returns_true_for_same_instance()
    {
        TestEnumeration.First.Equals(TestEnumeration.First).Should().BeTrue();
    }

    [Fact]
    public void Equals_returns_false_for_different_id()
    {
        TestEnumeration.First.Equals(TestEnumeration.Second).Should().BeFalse();
    }

    [Fact]
    public void Equals_returns_false_for_different_type_same_id()
    {
        TestEnumeration.First.Equals(OtherEnumeration.First).Should().BeFalse();
    }

    [Fact]
    public void Equals_returns_false_for_null()
    {
        TestEnumeration.First.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void CompareTo_orders_by_id()
    {
        TestEnumeration.First.CompareTo(TestEnumeration.Second).Should().BeNegative();
        TestEnumeration.Second.CompareTo(TestEnumeration.First).Should().BePositive();
        TestEnumeration.First.CompareTo(TestEnumeration.First).Should().Be(0);
    }
}
