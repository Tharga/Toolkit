using System;
using AwesomeAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests;

public class DateTimeExtensionsTest
{
    [Fact]
    public void ToDateTimeString()
    {
        //Arrange
        var dateTime = DateTime.UtcNow;

        //Act
        var result = dateTime.ToDateTimeString();

        //Assert
        result.Should().Be($"{dateTime.ToShortDateString()} {dateTime.ToLongTimeString()}", "Parsed DateTime to 'Date Time String' is not right.");
    }

    [Fact]
    public void ToTimeString()
    {
        //Arrange
        var dateTime = DateTime.UtcNow;

        //Act
        var diff = dateTime.AddSeconds(400) - dateTime;
        var result = diff.ToTimeString();

        //Assert
        result.Should().Be($"{diff.Hours}:{diff.Minutes:00}:{diff.Seconds:00}", "Parsed TimeSpan to 'Time String' is not right");
    }

    [Theory]
    [InlineData(400, "0:06:40")]
    [InlineData(86400, "24:00:00")]
    [InlineData(90000, "25:00:00")]
    [InlineData(-91800, "-25:30:00")]
    [InlineData(-1, "-0:00:01")]
    public void ToTimeString_uses_total_hours_and_preserves_negative_sign(long seconds, string expected)
    {
        var duration = TimeSpan.FromSeconds(seconds);

        duration.ToTimeString().Should().Be(expected);
    }

    [Fact]
    public void ToTimeString_handles_TimeSpan_MinValue()
    {
        TimeSpan.FromTicks(long.MinValue).ToTimeString().Should().Be("-256204778:48:05");
    }


    [Theory]
    [InlineData(0, "0 ms")]
    [InlineData(1, "1 ms")]
    [InlineData(2, "2 ms")]
    [InlineData(999, "999 ms")]
    [InlineData(1000, "1000 ms")]
    [InlineData(1001, "1 second")]
    [InlineData(2000, "2 seconds")]
    [InlineData(59999, "59 seconds")]
    [InlineData(60000, "1 minute")]
    [InlineData(120000, "2 minutes")]
    [InlineData(3599999, "59 minutes")]
    [InlineData(3600000, "1 hour")]
    [InlineData(7200000, "2 hours")]
    [InlineData(86399999, "23 hours")]
    [InlineData(86400000, "1 day")]
    [InlineData(172800000, "2 days")]
    public void ToTimeSpanString(long milliseconds, string expected)
    {
        //Arrange
        var duration = TimeSpan.FromMilliseconds(milliseconds);

        //Act
        var result = duration.ToTimeSpanString();

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(ETestCase.Max)]
    [InlineData(ETestCase.Min)]
    [InlineData(ETestCase.Null)]
    [InlineData(ETestCase.Zero)]
    public void ToTimeSpanStringDefault(ETestCase testCase)
    {
        //Arrange
        var duration = testCase.GetTimeSpan();

        //Act
        var result = duration.ToTimeSpanString();

        //Assert
        result.Should().NotBeNull();
    }
}
