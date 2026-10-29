using AwesomeAssertions;
using Microsoft.Extensions.Options;
using Tharga.Toolkit.Password;
using Xunit;

namespace Tharga.Toolkit.Tests.Password;

public class ApiKeyServiceTests
{
    [Theory]
    [InlineData("MyUsername", 1)]
    [InlineData("strange:username", 10)]
    public void Basic(string username, int hashSize)
    {
        //Arrange
        var sut = new ApiKeyService(Options.Create(new ApiKeyOptions{ HashSize = hashSize }));
        var apiKey = sut.BuildApiKey(username, null);
        var hash = sut.Encrypt(apiKey);

        //Act
        var result = sut.Verify(apiKey, hash);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_accepts_a_hash_stored_in_the_legacy_format()
    {
        //Arrange
        const string legacyHash = "$HASH|V1$10000$vmliBWX+Gt1tOpafbe91SikMwOYD25+IP4k9U1jHraQ0ZGZa";
        var sut = new ApiKeyService(Options.Create(new ApiKeyOptions()));

        //Act
        var result = sut.Verify("myPassword", legacyHash);

        //Assert
        result.Should().BeTrue();
        sut.Verify("wrongPassword", legacyHash).Should().BeFalse();
    }

    [Fact]
    public void Encrypt_writes_the_current_format_with_the_configured_iterations()
    {
        //Arrange
        var sut = new ApiKeyService(Options.Create(new ApiKeyOptions { Iterations = 1000 }));

        //Act
        var hash = sut.Encrypt(sut.BuildApiKey("MyUsername", null));

        //Assert
        hash.Should().StartWith("$HASH|V2$SHA512$1000$");
    }

    [Theory]
    [InlineData("MyUsername")]
    [InlineData("strange:username")]
    public void ExtractUsername(string username)
    {
        //Arrange
        var sut = new ApiKeyService(Options.Create(new ApiKeyOptions()));
        var apiKey = sut.BuildApiKey(username, null);

        //Act
        var user = sut.GetUsername(apiKey);

        //Assert
        user.Should().Be(username);
    }
}