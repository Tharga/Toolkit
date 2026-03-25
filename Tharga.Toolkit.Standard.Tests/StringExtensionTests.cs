using System;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class StringExtensionTests
{
    [Fact]
    public void NullIfEmpty_returns_null_for_empty_string()
    {
        "".NullIfEmpty().Should().BeNull();
    }

    [Fact]
    public void NullIfEmpty_returns_null_for_null()
    {
        ((string)null).NullIfEmpty().Should().BeNull();
    }

    [Fact]
    public void NullIfEmpty_returns_value_for_non_empty()
    {
        "hello".NullIfEmpty().Should().Be("hello");
    }

    [Fact]
    public void IsNullOrEmpty_returns_true_for_null()
    {
        ((string)null).IsNullOrEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_returns_true_for_empty()
    {
        "".IsNullOrEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_returns_false_for_non_empty()
    {
        "hello".IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public void IfEmpty_returns_fallback_for_null()
    {
        ((string)null).IfEmpty("fallback").Should().Be("fallback");
    }

    [Fact]
    public void IfEmpty_returns_fallback_for_empty()
    {
        "".IfEmpty("fallback").Should().Be("fallback");
    }

    [Fact]
    public void IfEmpty_returns_original_for_non_empty()
    {
        "hello".IfEmpty("fallback").Should().Be("hello");
    }

    [Fact]
    public void RandomString_returns_correct_length()
    {
        var result = 10.RandomString();
        result.Should().HaveLength(10);
    }

    [Fact]
    public void RandomString_uses_custom_characters()
    {
        var result = 20.RandomString(StringExtension.NumericCharacters);
        result.Should().MatchRegex("^[0-9]+$");
    }

    [Fact]
    public void Random_returns_string_within_length_range()
    {
        var result = StringExtension.AlphaNumericCharacters.Random(5, 10);
        result.Length.Should().BeInRange(5, 10);
    }

    [Fact]
    public void GetRandomString_returns_string_within_length_range()
    {
        var result = StringExtension.GetRandomString(8, 16);
        result.Length.Should().BeInRange(8, 16);
    }

    [Fact]
    public void GetRandomString_throws_for_empty_characters()
    {
        var act = () => StringExtension.GetRandomString(8, 16, "");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetRandomString_throws_for_invalid_length_range()
    {
        var act = () => StringExtension.GetRandomString(10, 5);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToBase64_encodes_string()
    {
        "hello".ToBase64().Should().Be("aGVsbG8=");
    }

    [Fact]
    public void FromBase64_decodes_string()
    {
        "aGVsbG8=".FromBase64().Should().Be("hello");
    }

    [Fact]
    public void ToBase64_and_FromBase64_round_trip()
    {
        const string original = "test string with special chars: åäö";
        original.ToBase64().FromBase64().Should().Be(original);
    }

    [Fact]
    public void ToBase64_returns_null_for_null()
    {
        ((string)null).ToBase64().Should().BeNull();
    }

    [Fact]
    public void FromBase64_returns_null_for_null()
    {
        ((string)null).FromBase64().Should().BeNull();
    }

    [Fact]
    public void Truncate_shortens_long_string()
    {
        "hello world".Truncate(5).Should().Be("hello");
    }

    [Fact]
    public void Truncate_returns_original_when_within_limit()
    {
        "hi".Truncate(5).Should().Be("hi");
    }

    [Fact]
    public void Truncate_returns_original_when_exact_length()
    {
        "hello".Truncate(5).Should().Be("hello");
    }
}
