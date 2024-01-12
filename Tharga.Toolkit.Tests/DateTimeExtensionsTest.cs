using System;
using FluentAssertions;
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
    [InlineData(0, EUnit.Millisecond, "Now")]
    [InlineData(-1, EUnit.Second, "Resent")]
    [InlineData(1, EUnit.Second, "Soon")]
    [InlineData(-1000, EUnit.Minute, "Resent")]
    [InlineData(1000, EUnit.Minute, "Soon")]
    [InlineData(-60000, EUnit.Hour, "Resent")]
    [InlineData(60000, EUnit.Hour, "Soon")]
    [InlineData(-3600000, EUnit.Day, "Resent")]
    [InlineData(3600000, EUnit.Day, "Soon")]
    [InlineData(-86400000, EUnit.Week, "Resent")]
    [InlineData(86400000, EUnit.Week, "Soon")]
    [InlineData(-604800000, EUnit.Month, "Resent")]
    [InlineData(604800000, EUnit.Month, "Soon")]
    [InlineData(-2592000000, EUnit.Year, "Resent")]
    [InlineData(2592000000, EUnit.Year, "Soon")]
    public void ToShortDurationString(long milliseconds, EUnit minUnit, string expected)
    {
        //Arrange
        var utcNow = DateTime.UtcNow;
        var dateTime = utcNow.AddMilliseconds(milliseconds);

        //Act
        var result = dateTime.ToDurationString(new DurationOptions { MinUnit = minUnit, MaxUnit = EUnit.Year, BaseValue = utcNow });

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(-1440, "8 weeks ago")]
    [InlineData(-720, "4 weeks ago")]
    [InlineData(-337, "2 weeks ago")]
    [InlineData(-169, "1 week ago")]
    [InlineData(-49, "2 days ago")]
    [InlineData(-25, "1 day ago")]
    [InlineData(25, "In 1 day")]
    [InlineData(49, "In 2 days")]
    [InlineData(179, "In 1 week")]
    [InlineData(347, "In 2 weeks")]
    [InlineData(720, "In 4 weeks")]
    [InlineData(1440, "In 8 weeks")]
    public void ToDurationString(int addHours, string expected)
    {
        //Arrange
        var utcNow = DateTime.UtcNow;
        var dateTime = utcNow.AddHours(addHours);

        //Act
        var result = dateTime.ToDurationString(new DurationOptions { MaxUnit = EUnit.Week, BaseValue = utcNow });

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(EUnit.Year, "1 year ago")]
    [InlineData(EUnit.Month, "13 months ago")]
    [InlineData(EUnit.Week, "57 weeks ago")]
    [InlineData(EUnit.Day, "400 days ago")]
    [InlineData(EUnit.Hour, "9600 hours ago")]
    [InlineData(EUnit.Minute, "576000 minutes ago")]
    [InlineData(EUnit.Second, "34560000 seconds ago")]
    [InlineData(EUnit.Millisecond, "34560000000 milliseconds ago")]
    public void ToDurationString_MaxUnit(EUnit maxUnit, string expected)
    {
        //Arrange
        var utcNow = DateTime.UtcNow;
        var dateTime = utcNow.AddDays(-400);

        //Act
        var result = dateTime.ToDurationString(new DurationOptions { MaxUnit = maxUnit, MinUnit = EUnit.Millisecond, BaseValue = utcNow });

        //Assert
        result.Should().Be(expected);
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
}