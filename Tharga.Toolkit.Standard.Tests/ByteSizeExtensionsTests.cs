using System;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class ByteSizeExtensionsTests
{
    [Fact]
    public void ToReadableByteSize_zero_bytes()
    {
        0L.ToReadableByteSize().Should().Be("0 B");
    }

    [Fact]
    public void ToReadableByteSize_zero_bytes_full_unit()
    {
        0L.ToReadableByteSize(useFullUnit: true).Should().Be("0 Bytes");
    }

    [Fact]
    public void ToReadableByteSize_bytes()
    {
        512L.ToReadableByteSize().Should().Be("512 B");
    }

    [Fact]
    public void ToReadableByteSize_kilobytes()
    {
        1024L.ToReadableByteSize().Should().Be("1 KB");
    }

    [Fact]
    public void ToReadableByteSize_megabytes()
    {
        (1024L * 1024).ToReadableByteSize().Should().Be("1 MB");
    }

    [Fact]
    public void ToReadableByteSize_gigabytes()
    {
        (1024L * 1024 * 1024).ToReadableByteSize().Should().Be("1 GB");
    }

    [Fact]
    public void ToReadableByteSize_terabytes()
    {
        (1024L * 1024 * 1024 * 1024).ToReadableByteSize().Should().Be("1 TB");
    }

    [Fact]
    public void ToReadableByteSize_with_decimal_places()
    {
        var result = 1536L.ToReadableByteSize(decimalPlaces: 1);
        result.Should().EndWith("KB");
        result.Should().StartWith("1");
        result.Should().Contain("5");
    }

    [Fact]
    public void ToReadableByteSize_full_unit_name()
    {
        1024L.ToReadableByteSize(useFullUnit: true).Should().Be("1 Kilobytes");
    }

    [Fact]
    public void ToReadableByteSize_int_overload()
    {
        1024.ToReadableByteSize().Should().Be("1 KB");
    }

    [Fact]
    public void ToReadableByteSize_throws_for_negative()
    {
        var act = () => (-1L).ToReadableByteSize();
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
