using System;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests;

public class DurationExtensionsTest
{
    [Theory]
    [InlineData(ETestCase.Now)]
    [InlineData(ETestCase.UtcNow)]
    [InlineData(ETestCase.Min)]
    [InlineData(ETestCase.Max)]
    [InlineData(ETestCase.Zero)]
    [InlineData(ETestCase.Null)]
    public void ToDurationStringDefault(ETestCase testCase)
    {
        //Arrange
        var now = testCase.GetDateTime();

        //Act
        var result = now.ToDurationString();

        //Assert
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData(ETestCase.Now)]
    [InlineData(ETestCase.UtcNow)]
    [InlineData(ETestCase.Min)]
    [InlineData(ETestCase.Max)]
    [InlineData(ETestCase.Zero)]
    [InlineData(ETestCase.Null)]
    public void ToLocalDurationStringDefault(ETestCase testCase)
    {
        //Arrange
        var now = testCase.GetDateTime();

        //Act
        var result = now.ToLocalDurationString();

        //Assert
        result.Should().NotBeNull();
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
}