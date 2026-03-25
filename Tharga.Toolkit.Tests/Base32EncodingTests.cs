using System;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests;

public class Base32EncodingTests
{
    [Fact]
    public void Encode_known_vector()
    {
        // RFC 4648 test vectors
        var result = Base32Encoding.Encode(Encoding.ASCII.GetBytes("f"));
        result.Should().Be("MY======");
    }

    [Fact]
    public void Encode_known_vector_foobar()
    {
        var result = Base32Encoding.Encode(Encoding.ASCII.GetBytes("foobar"));
        result.Should().Be("MZXW6YTBOI======");
    }

    [Fact]
    public void Encode_empty_returns_empty()
    {
        Base32Encoding.Encode(Array.Empty<byte>()).Should().BeEmpty();
    }

    [Fact]
    public void Encode_null_returns_empty()
    {
        Base32Encoding.Encode(null).Should().BeEmpty();
    }

    [Fact]
    public void Decode_known_vector()
    {
        var result = Base32Encoding.Decode("MY======");
        Encoding.ASCII.GetString(result).Should().Be("f");
    }

    [Fact]
    public void Decode_known_vector_foobar()
    {
        var result = Base32Encoding.Decode("MZXW6YTBOI======");
        Encoding.ASCII.GetString(result).Should().Be("foobar");
    }

    [Fact]
    public void Decode_empty_returns_empty()
    {
        Base32Encoding.Decode("").Should().BeEmpty();
    }

    [Fact]
    public void Decode_null_returns_empty()
    {
        Base32Encoding.Decode(null).Should().BeEmpty();
    }

    [Fact]
    public void Decode_throws_for_invalid_character()
    {
        var act = () => Base32Encoding.Decode("1234");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Round_trip_preserves_data()
    {
        var original = Encoding.UTF8.GetBytes("Hello, Base32!");
        var encoded = Base32Encoding.Encode(original);
        var decoded = Base32Encoding.Decode(encoded);
        decoded.Should().Equal(original);
    }

    [Fact]
    public void Decode_is_case_insensitive()
    {
        var upper = Base32Encoding.Decode("MY======");
        var lower = Base32Encoding.Decode("my======");
        lower.Should().Equal(upper);
    }
}
