using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests.Hash;

public class HashBytesTests
{
    [Fact]
    public void BytesHashString()
    {
        //Arrange
        var value = "A"u8.ToArray();

        //Act
        var hash = value.ToHash(HashFormat.Base64);

        //Assert
        hash.Value.Should().Be("f8VicOenD6gaWTW3Lqy+KQ==");
    }

    [Fact]
    public void BytesHash()
    {
        //Arrange
        var value = "A"u8.ToArray();

        //Act
        var hash = value.ToHash();

        //Assert
        hash.Value.Should().HaveCount(16);
    }

    [Theory]
    [InlineData(HashType.MD5)]
    [InlineData(HashType.SHA1)]
    [InlineData(HashType.SHA256)]
    [InlineData(HashType.SHA384)]
    [InlineData(HashType.SHA512)]
    public void EmptyBytes(HashType type)
    {
        //Arrange
        byte[] value = [];

        //Act
        var hash = value.ToHash(type);

        //Assert
        hash.Should().BeNull();
    }

    [Theory]
    [InlineData(HashType.MD5)]
    [InlineData(HashType.SHA1)]
    [InlineData(HashType.SHA256)]
    [InlineData(HashType.SHA384)]
    [InlineData(HashType.SHA512)]
    public void NullBytes(HashType type)
    {
        //Arrange
        byte[] value = null;

        //Act
        var hash = value.ToHash(type);

        //Assert
        hash.Should().BeNull();
    }
}