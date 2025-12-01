using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests.Hash;

public class HashStringTests
{
    [Theory]
    [InlineData(HashFormat.Base64, HashType.MD5, "f8VicOenD6gaWTW3Lqy+KQ==")]
    [InlineData(HashFormat.Base64, HashType.SHA1, "bc1M4j2I4u6VaLpUbAB8Y9kTHBs=")]
    [InlineData(HashFormat.Base64, HashType.SHA256, "VZrq0IJk1XldOQlxjN0Fq9SVcuhP5VWQ7vMaiKCP3/0=")]
    [InlineData(HashFormat.Base64, HashType.SHA384, "rRSq8lAgvvL9Tj617AxQJyzf1mB0sO0DfJoRJUMhqsBymYU3S+6qW4ClBNBIvhhk")]
    [InlineData(HashFormat.Base64, HashType.SHA512, "IbT0vZ5k7TVcPrZ2oo6+2vbY8XvcNlmVsxkJcVMEQIBRa9CDv8zmYSGjByZGmUyEMMw4K43FQ+hIgBg7+FbP9Q==")]
    public void HashStringTypes(HashFormat format, HashType type, string expected)
    {
        //Arrange
        var value = "A";

        //Act
        var hash = value.ToHash(format, type);

        //Assert
        hash.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(HashFormat.Hex, HashType.MD5, "7FC56270E7A70FA81A5935B72EACBE29")]
    [InlineData(HashFormat.HexLower, HashType.MD5, "7fc56270e7a70fa81a5935b72eacbe29")]
    [InlineData(HashFormat.Base64, HashType.MD5, "f8VicOenD6gaWTW3Lqy+KQ==")]
    [InlineData(HashFormat.Base64UrlSafe, HashType.MD5, "f8VicOenD6gaWTW3Lqy-KQ")]
    [InlineData(HashFormat.HexWithDashes, HashType.MD5, "7F-C5-62-70-E7-A7-0F-A8-1A-59-35-B7-2E-AC-BE-29")]
    [InlineData(HashFormat.Base32, HashType.MD5, "P7CWE4HHU4H2QGSZGW3S5LF6FE======")]
    public void HashStringFormats(HashFormat format, HashType type, string expected)
    {
        //Arrange
        var value = "A";

        //Act
        var hash = value.ToHash(format, type);

        //Assert
        hash.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(HashFormat.Hex, HashType.MD5)]
    [InlineData(HashFormat.HexLower, HashType.MD5)]
    [InlineData(HashFormat.Base64, HashType.MD5)]
    [InlineData(HashFormat.Base64UrlSafe, HashType.MD5)]
    [InlineData(HashFormat.HexWithDashes, HashType.MD5)]
    [InlineData(HashFormat.Base32, HashType.MD5)]
    public void ChangeHashString(HashFormat format, HashType type)
    {
        //Arrange
        var value = "A";
        var hash = value.ToHash(format, type);

        //Act
        var converted = hash.ChangeFormat(HashFormat.Base64);

        //Assert
        converted.Value.Should().Be("f8VicOenD6gaWTW3Lqy+KQ==");
    }

    [Fact]
    public void ChangeEmptyHashString()
    {
        //Arrange
        var value = "";
        var hash = value.ToHash(HashFormat.Base64);

        //Act
        var converted = hash.ChangeFormat(HashFormat.Base64);

        //Assert
        converted.Should().BeNull();
    }

    [Theory]
    [InlineData(HashFormat.Hex, HashType.MD5)]
    [InlineData(HashFormat.HexLower, HashType.MD5)]
    [InlineData(HashFormat.Base64, HashType.MD5)]
    [InlineData(HashFormat.Base64UrlSafe, HashType.MD5)]
    [InlineData(HashFormat.HexWithDashes, HashType.MD5)]
    [InlineData(HashFormat.Base32, HashType.MD5)]
    public void EmptyStringFormat(HashFormat format, HashType type)
    {
        //Arrange
        var value = "";

        //Act
        var hash = value.ToHash(format, type);

        //Assert
        hash.Should().BeNull();
    }

    [Theory]
    [InlineData(HashType.MD5)]
    [InlineData(HashType.SHA1)]
    [InlineData(HashType.SHA256)]
    [InlineData(HashType.SHA384)]
    [InlineData(HashType.SHA512)]
    public void EmptyString(HashType type)
    {
        //Arrange
        var value = "";

        //Act
        var hash = value.ToHash(type);

        //Assert
        hash.Should().BeNull();
    }

    [Fact]
    public void ChangeNullHashString()
    {
        //Arrange
        string value = null;
        var hash = value.ToHash(HashFormat.Base64);

        //Act
        var converted = hash.ChangeFormat(HashFormat.Base64);

        //Assert
        converted.Should().BeNull();
    }

    [Fact]
    public void NullString()
    {
        //Arrange
        string value = null;

        //Act
        var hash = value.ToHash(HashFormat.Base64);

        //Assert
        hash.Should().BeNull();
    }

    [Fact]
    public void NullBytes()
    {
        //Arrange
        string value = null;

        //Act
        var hash = value.ToHash();

        //Assert
        hash.Should().BeNull();
    }
}