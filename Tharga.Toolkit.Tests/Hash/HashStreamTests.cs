using FluentAssertions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tharga.Toolkit.Tests.Hash;

public class HashStreamTests
{
    [Fact]
    public async Task StreamHashString()
    {
        //Arrange
        var ms = new MemoryStream("A"u8.ToArray());

        //Act
        var hash = await ms.ToHashAsync(HashFormat.Base64);

        //Assert
        hash.Value.Should().Be("f8VicOenD6gaWTW3Lqy+KQ==");
    }

    [Fact]
    public async Task EmptyStreamHashString()
    {
        //Arrange
        var ms = new MemoryStream(""u8.ToArray());

        //Act
        var hash = await ms.ToHashAsync(HashFormat.Base64);

        //Assert
        hash.Should().BeNull();
    }

    [Fact]
    public async Task EmptyStream2HashString()
    {
        //Arrange
        var ms = new MemoryStream([]);

        //Act
        var hash = await ms.ToHashAsync(HashFormat.Base64);

        //Assert
        hash.Should().BeNull();
    }

    [Fact]
    public async Task NullStreamHashString()
    {
        //Arrange
        MemoryStream ms = null;

        //Act
        var act = async () => { _ = await ms.ToHashAsync(HashFormat.Base64); };

        //Assert
        await act.Should()
            .ThrowAsync<NullReferenceException>()
            .WithMessage("Stream is null.");
    }

    [Theory]
    [InlineData(HashType.MD5, "f8VicOenD6gaWTW3Lqy+KQ==")]
    [InlineData(HashType.SHA1, "bc1M4j2I4u6VaLpUbAB8Y9kTHBs=")]
    [InlineData(HashType.SHA256, "VZrq0IJk1XldOQlxjN0Fq9SVcuhP5VWQ7vMaiKCP3/0=")]
    [InlineData(HashType.SHA384, "rRSq8lAgvvL9Tj617AxQJyzf1mB0sO0DfJoRJUMhqsBymYU3S+6qW4ClBNBIvhhk")]
    [InlineData(HashType.SHA512, "IbT0vZ5k7TVcPrZ2oo6+2vbY8XvcNlmVsxkJcVMEQIBRa9CDv8zmYSGjByZGmUyEMMw4K43FQ+hIgBg7+FbP9Q==")]
    public async Task ToHashAndOutputAsyncTest(HashType hashType, string expected)
    {
        //Arrange
        var inputData = "A"u8.ToArray();
        var input = new MemoryStream(inputData);
        var output = new MemoryStream();

        //Act
        var hash = await input.ToHashAndOutputAsync(output, HashFormat.Base64, hashType);

        //Assert
        hash.Value.Should().Be(expected);
        var outputData = output.ToArray();
        outputData.SequenceEqual(inputData);
        Encoding.UTF8.GetString(outputData).Should().Be("A");
    }

    [Fact]
    public async Task ToHashAndOutputAsyncTestInputNull()
    {
        //Arrange
        var inputData = "A"u8.ToArray();
        MemoryStream input = null;
        var output = new MemoryStream();

        //Act
        var act = async () => { _ = await input.ToHashAndOutputAsync(output, HashFormat.Base64); };

        //Assert
        await act.Should()
            .ThrowAsync<NullReferenceException>()
            .WithMessage("Stream input is null.");
    }

    [Fact]
    public async Task ToHashAndOutputAsyncTestOutputNull()
    {
        //Arrange
        var inputData = "A"u8.ToArray();
        var input = new MemoryStream(inputData);
        MemoryStream output = null;

        //Act
        var hash = await input.ToHashAndOutputAsync(output, HashFormat.Base64);

        //Assert
        hash.Value.Should().Be("f8VicOenD6gaWTW3Lqy+KQ==");
    }

    [Fact]
    public async Task StreamNonSeekHashString()
    {
        //Arrange
        var ms = new NonSeekableInputStream("A"u8.ToArray());

        //Act
        var hash = await ms.ToHashAsync(HashFormat.Base64);

        //Assert
        hash.Value.Should().Be("f8VicOenD6gaWTW3Lqy+KQ==");
    }

    [Theory]
    [InlineData(HashType.MD5, "f8VicOenD6gaWTW3Lqy+KQ==")]
    [InlineData(HashType.SHA1, "bc1M4j2I4u6VaLpUbAB8Y9kTHBs=")]
    [InlineData(HashType.SHA256, "VZrq0IJk1XldOQlxjN0Fq9SVcuhP5VWQ7vMaiKCP3/0=")]
    [InlineData(HashType.SHA384, "rRSq8lAgvvL9Tj617AxQJyzf1mB0sO0DfJoRJUMhqsBymYU3S+6qW4ClBNBIvhhk")]
    [InlineData(HashType.SHA512, "IbT0vZ5k7TVcPrZ2oo6+2vbY8XvcNlmVsxkJcVMEQIBRa9CDv8zmYSGjByZGmUyEMMw4K43FQ+hIgBg7+FbP9Q==")]
    public async Task ToHashAndOutputAsyncNonSeekTest(HashType hashType, string expected)
    {
        //Arrange
        var inputData = "A"u8.ToArray();
        var input = new NonSeekableInputStream(inputData);
        var output = new NonSeekableOutputStream();

        //Act
        var hash = await input.ToHashAndOutputAsync(output, HashFormat.Base64, hashType);

        //Assert
        hash.Value.Should().Be(expected);
        var outputData = output.ToArray();
        outputData.SequenceEqual(inputData);
        Encoding.UTF8.GetString(outputData).Should().Be("A");
    }
}