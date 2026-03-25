using System;
using FluentAssertions;
using Tharga.Toolkit.Password;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_returns_formatted_hash()
    {
        var hash = PasswordHasher.HashPassword("myPassword");
        hash.Should().StartWith("$HASH|V1$10000$");
    }

    [Fact]
    public void HashPassword_produces_different_hashes_for_same_password()
    {
        var hash1 = PasswordHasher.HashPassword("myPassword");
        var hash2 = PasswordHasher.HashPassword("myPassword");
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_returns_true_for_correct_password()
    {
        var hash = PasswordHasher.HashPassword("myPassword");
        PasswordHasher.VerifyPassword("myPassword", hash).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_returns_false_for_wrong_password()
    {
        var hash = PasswordHasher.HashPassword("myPassword");
        PasswordHasher.VerifyPassword("wrongPassword", hash).Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_works_with_custom_salt_and_hash_size()
    {
        const int saltSize = 32;
        const int hashSize = 32;
        var hash = PasswordHasher.HashPassword("myPassword", saltSize, hashSize);
        PasswordHasher.VerifyPassword("myPassword", hash, saltSize, hashSize).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_works_with_custom_iterations()
    {
        var hash = PasswordHasher.HashPassword("myPassword", iterations: 5000);
        hash.Should().StartWith("$HASH|V1$5000$");
        PasswordHasher.VerifyPassword("myPassword", hash).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_throws_for_invalid_format()
    {
        var act = () => PasswordHasher.VerifyPassword("myPassword", "not-a-valid-hash");
        act.Should().Throw<FormatException>();
    }
}
