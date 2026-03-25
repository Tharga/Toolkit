using System;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class DurationStringOptionsExtensionsTests
{
    [Fact]
    public void Get_by_english_culture_returns_english_options()
    {
        var options = DurationStringOptionsExtensions.Get("en-US");
        options.PreString.Should().Be("In");
        options.PostString.Should().Be("ago");
        options.UnitOptions.Should().ContainKey(EUnit.Second);
        options.UnitOptions[EUnit.Second].Value.Should().Be("second");
    }

    [Fact]
    public void Get_by_swedish_culture_returns_swedish_options()
    {
        var options = DurationStringOptionsExtensions.Get("sv-SE");
        options.PreString.Should().Be("Om");
        options.PostString.Should().Be("sedan");
        options.UnitOptions[EUnit.Second].Value.Should().Be("sekund");
    }

    [Fact]
    public void Get_by_Language_En()
    {
        var options = DurationStringOptionsExtensions.Get(Language.En);
        options.PreString.Should().Be("In");
        options.Now.Should().ContainKey(EUnit.Day);
        options.Resent.Should().ContainKey(EUnit.Second);
        options.Soon.Should().ContainKey(EUnit.Minute);
    }

    [Fact]
    public void Get_by_Language_Sv()
    {
        var options = DurationStringOptionsExtensions.Get(Language.Sv);
        options.PreString.Should().Be("Om");
        options.Now[EUnit.Day].Should().Be("Idag");
    }

    [Fact]
    public void Get_throws_for_unknown_culture()
    {
        var act = () => DurationStringOptionsExtensions.Get("xx-XX");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Get_all_units_are_present_in_english()
    {
        var options = DurationStringOptionsExtensions.Get(Language.En);
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
