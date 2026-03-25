using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class OrgNoExtensionsTests
{
    [Fact]
    public void TryParseOrgNo_returns_true_for_valid_org_number()
    {
        "5561234567".TryParseOrgNo(out var orgNo).Should().BeTrue();
        orgNo.Should().Be("556123-4567");
    }

    [Fact]
    public void TryParseOrgNo_strips_non_digit_characters()
    {
        "556123-4567".TryParseOrgNo(out var orgNo).Should().BeTrue();
        orgNo.Should().Be("556123-4567");
    }

    [Fact]
    public void TryParseOrgNo_returns_false_for_null()
    {
        ((string)null).TryParseOrgNo(out var orgNo, out var errorType).Should().BeFalse();
        orgNo.Should().BeNull();
        errorType.Should().Be(ErrorType.NoValue);
    }

    [Fact]
    public void TryParseOrgNo_returns_false_for_empty()
    {
        "".TryParseOrgNo(out var orgNo, out var errorType).Should().BeFalse();
        errorType.Should().Be(ErrorType.NoValue);
    }

    [Fact]
    public void TryParseOrgNo_returns_false_for_wrong_length()
    {
        "12345".TryParseOrgNo(out var orgNo, out var errorType).Should().BeFalse();
        errorType.Should().Be(ErrorType.InvalidFormat);
    }

    [Fact]
    public void TryParseOrgNo_returns_false_for_invalid_check_digit()
    {
        "5561234560".TryParseOrgNo(out var orgNo, out var errorType).Should().BeFalse();
        errorType.Should().Be(ErrorType.InvalidCheckDigit);
    }

    [Fact]
    public void TryParseOrgNo_simple_overload_returns_false_for_invalid()
    {
        "12345".TryParseOrgNo(out var orgNo).Should().BeFalse();
        orgNo.Should().BeNull();
    }
}
