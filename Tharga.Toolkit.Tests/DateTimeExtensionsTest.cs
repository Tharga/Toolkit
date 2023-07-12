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
    [InlineData(-337, "2 weeks ago")]
    [InlineData(-169, "1 week ago")]
    [InlineData(-49, "2 days ago")]
    [InlineData(-25, "1 day ago")]
    [InlineData(25, "In 1 day")]
    [InlineData(49, "In 2 days")]
    [InlineData(179, "In 1 week")]
    [InlineData(347, "In 2 weeks")]
    public void ToDurationString(int addHours, string expected)
    {
        //Arrange
        var dateTime = DateTime.UtcNow.AddHours(addHours);

        //Act
        var result = dateTime.ToDurationString(EMaxUnit.Week);

        //Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(EMaxUnit.Year, "1 year ago")]
    [InlineData(EMaxUnit.Month, "13 months ago")]
    [InlineData(EMaxUnit.Week, "57 weeks ago")]
    [InlineData(EMaxUnit.Day, "400 days ago")]
    [InlineData(EMaxUnit.Hour, "9600 hours ago")]
    [InlineData(EMaxUnit.Minute, "576000 minutes ago")]
    public void ToDurationString_MaxUnit(EMaxUnit maxUnit, string expected)
    {
        //Arrange
        var dateTime = DateTime.UtcNow.AddDays(-400);

        //Act
        var result = dateTime.ToDurationString(maxUnit);

        //Assert
        result.Should().Be(expected);
    }
}