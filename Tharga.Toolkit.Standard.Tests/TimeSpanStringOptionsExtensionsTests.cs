using System;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class TimeSpanStringOptionsExtensionsTests
{
    [Fact]
    public void Get_english_culture_returns_english_options()
    {
        var options = TimeSpanStringOptionsExtensions.Get("en-US");
        options.UnitOptions.Should().ContainKey(EUnit.Second);
        options.UnitOptions[EUnit.Second].Value.Should().Be("second");
        options.UnitOptions[EUnit.Second].PluralSign.Should().Be("s");
    }

    [Fact]
    public void Get_swedish_culture_returns_swedish_options()
    {
        var options = TimeSpanStringOptionsExtensions.Get("sv-SE");
        options.UnitOptions[EUnit.Second].Value.Should().Be("sekund");
        options.UnitOptions[EUnit.Second].PluralSign.Should().Be("er");
    }

    [Fact]
    public void Get_throws_for_unknown_culture()
    {
        var act = () => TimeSpanStringOptionsExtensions.Get("xx-XX");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Get_all_units_are_present()
    {
        var options = TimeSpanStringOptionsExtensions.Get("en-US");
        options.UnitOptions.Should().ContainKey(EUnit.Millisecond);
        options.UnitOptions.Should().ContainKey(EUnit.Second);
        options.UnitOptions.Should().ContainKey(EUnit.Minute);
        options.UnitOptions.Should().ContainKey(EUnit.Hour);
        options.UnitOptions.Should().ContainKey(EUnit.Day);
        options.UnitOptions.Should().ContainKey(EUnit.Week);
        options.UnitOptions.Should().ContainKey(EUnit.Month);
        options.UnitOptions.Should().ContainKey(EUnit.Year);
    }
}
