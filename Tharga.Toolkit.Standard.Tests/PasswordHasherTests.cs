using System;
using System.Security.Cryptography;
using System.Text;
using AwesomeAssertions;
using Tharga.Toolkit.Password;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class PasswordHasherTests
{
    // Created by the 1.17 PasswordHasher: PBKDF2-HMAC-SHA1, salt and hash sizes not recorded in the string.
    private const string LegacyHash = "$HASH|V1$10000$vmliBWX+Gt1tOpafbe91SikMwOYD25+IP4k9U1jHraQ0ZGZa";
    private const string LegacyHashWith32ByteSaltAndHash = "$HASH|V1$10000$2Pt4q9HB3ysrXG409X9eLHv0le4aLe/GinAlt9UErhxIDLUUsPR6Ugb5tUWbGnfnZdcAbo9PmsBwfVJIKTsYHQ==";
    private const string LegacyHashWith5000Iterations = "$HASH|V1$5000$foRN8iMsGI5knQknJW9mFhSysXUcobKbxXzV/f/d9BIKg+qd";

    [Fact]
    public void HashPassword_returns_formatted_hash()
    {
        var hash = PasswordHasher.HashPassword("myPassword");
        hash.Should().StartWith("$HASH|V2$SHA512$210000$");
    }

    [Fact]
    public void HashPassword_records_the_salt_and_the_hash_separately()
    {
        var parts = PasswordHasher.HashPassword("myPassword").Split('$');

        parts.Should().HaveCount(6);
        Convert.FromBase64String(parts[4]).Should().HaveCount(16);
        Convert.FromBase64String(parts[5]).Should().HaveCount(32);
    }

    [Fact]
    public void HashPassword_produces_a_standard_pbkdf2_sha512_hash()
    {
        var parts = PasswordHasher.HashPassword("myPassword", iterations: 1000).Split('$');
        var salt = Convert.FromBase64String(parts[4]);
        var hash = Convert.FromBase64String(parts[5]);

        var expected = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes("myPassword"), salt, 1000, HashAlgorithmName.SHA512, hash.Length);

        hash.Should().Equal(expected);
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
    public void VerifyPassword_reads_the_salt_and_hash_size_from_the_hash()
    {
        var hash = PasswordHasher.HashPassword("myPassword", saltSize: 32, hashSize: 64, iterations: 1000);
        PasswordHasher.VerifyPassword("myPassword", hash).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_works_with_custom_iterations()
    {
        var hash = PasswordHasher.HashPassword("myPassword", iterations: 5000);
        hash.Should().StartWith("$HASH|V2$SHA512$5000$");
        PasswordHasher.VerifyPassword("myPassword", hash).Should().BeTrue();
    }

    [Theory]
    [InlineData(LegacyHash, 16, 20)]
    [InlineData(LegacyHashWith32ByteSaltAndHash, 32, 32)]
    [InlineData(LegacyHashWith5000Iterations, 16, 20)]
    public void VerifyPassword_verifies_a_hash_stored_in_the_legacy_format(string hash, int saltSize, int hashSize)
    {
        PasswordHasher.VerifyPassword("myPassword", hash, saltSize, hashSize).Should().BeTrue();
        PasswordHasher.VerifyPassword("wrongPassword", hash, saltSize, hashSize).Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    public void VerifyPassword_returns_false_when_one_byte_of_the_stored_hash_differs(int index)
    {
        var parts = PasswordHasher.HashPassword("myPassword", iterations: 1000).Split('$');
        var hash = Convert.FromBase64String(parts[5]);
        hash[index] ^= 1;
        parts[5] = Convert.ToBase64String(hash);

        PasswordHasher.VerifyPassword("myPassword", string.Join("$", parts)).Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_throws_for_invalid_format()
    {
        var act = () => PasswordHasher.VerifyPassword("myPassword", "not-a-valid-hash");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void NeedsRehash_is_true_for_a_hash_in_the_legacy_format()
    {
        PasswordHasher.NeedsRehash(LegacyHash).Should().BeTrue();
        PasswordHasher.NeedsRehash(LegacyHash, 1000).Should().BeTrue();
    }

    [Fact]
    public void NeedsRehash_is_false_for_a_hash_with_the_current_defaults()
    {
        var hash = PasswordHasher.HashPassword("myPassword");
        PasswordHasher.NeedsRehash(hash).Should().BeFalse();
    }

    [Fact]
    public void NeedsRehash_is_true_when_the_hash_has_fewer_iterations_than_asked_for()
    {
        var hash = PasswordHasher.HashPassword("myPassword", iterations: 1000);

        PasswordHasher.NeedsRehash(hash).Should().BeTrue();
        PasswordHasher.NeedsRehash(hash, 1000).Should().BeFalse();
    }

    [Fact]
    public void NeedsRehash_throws_for_invalid_format()
    {
        var act = () => PasswordHasher.NeedsRehash("not-a-valid-hash");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void VerifyPassword_throws_for_an_unknown_algorithm()
    {
        var hash = PasswordHasher.HashPassword("myPassword", iterations: 1000).Replace("$SHA512$", "$MD5$");

        var act = () => PasswordHasher.VerifyPassword("myPassword", hash);

        act.Should().Throw<FormatException>();
    }
}
