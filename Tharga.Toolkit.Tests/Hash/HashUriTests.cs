using System;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests.Hash;

public class HashUriTests
{
    [Fact]
    public void UriHashString()
    {
        //Arrange
        var uri = new Uri("http://thargelion.com");

        //Act
        var hash = uri.ToHash(HashFormat.Base64, HashType.MD5, Encoding.ASCII);

        //Arrange
        hash.Value.Should().Be("igC8jgPDadDorMolZZfJ0w==");
    }

    [Fact]
    public void NullUriHash()
    {
        //Arrange
        Uri uri = null;

        //Act
        var hash = uri.ToHash(HashType.MD5, Encoding.ASCII);

        //Arrange
        hash.Should().BeNull();
    }

    [Fact]
    public void NullUriHashString()
    {
        //Arrange
        Uri uri = null;

        //Act
        var hash = uri.ToHash(HashFormat.Base32, HashType.MD5, Encoding.ASCII);

        //Arrange
        hash.Should().BeNull();
    }
}