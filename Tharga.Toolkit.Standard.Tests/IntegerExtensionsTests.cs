using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class IntegerExtensionsTests
{
    [Theory]
    [InlineData(1, "Primary")]
    [InlineData(2, "Secondary")]
    [InlineData(3, "Tertiary")]
    [InlineData(4, "Quaternary")]
    [InlineData(5, "Quinary")]
    [InlineData(6, "Senary")]
    [InlineData(7, "Septenary")]
    [InlineData(8, "Octonary")]
    [InlineData(9, "Nonary")]
    [InlineData(10, "Denary")]
    public void GetNameForNumber_returns_correct_name(int number, string expected)
    {
        IntegerExtensions.GetNameForNumber(number).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "Number 0")]
    [InlineData(11, "Number 11")]
    [InlineData(-1, "Number -1")]
    public void GetNameForNumber_returns_fallback_for_out_of_range(int number, string expected)
    {
        IntegerExtensions.GetNameForNumber(number).Should().Be(expected);
    }
}
