using FluentAssertions;
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