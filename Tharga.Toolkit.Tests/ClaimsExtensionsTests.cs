using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Tests;

public class ClaimsExtensionsTests
{
    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void GetKey_from_claims_with_sub()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var key = claims.GetKey();
        key.Should().NotBeNull();
        key.Value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetKey_returns_null_when_no_matching_claims()
    {
        var claims = new List<Claim> { new("custom", "value") };
        claims.GetKey().Should().BeNull();
    }

    [Fact]
    public void GetKey_throws_for_null_claims()
    {
        var act = () => ((IEnumerable<Claim>)null).GetKey();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetKey_from_ClaimsPrincipal()
    {
        var principal = CreatePrincipal(new Claim("sub", "user-456"));
        var key = principal.GetKey();
        key.Should().NotBeNull();
    }

    [Fact]
    public void GetKey_throws_for_null_principal()
    {
        var act = () => ((ClaimsPrincipal)null).GetKey();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetKey_from_ClaimsIdentity()
    {
        var identity = new ClaimsIdentity(new[] { new Claim("sub", "user-789") });
        var key = identity.GetKey();
        key.Should().NotBeNull();
    }

    [Fact]
    public void GetKey_throws_for_null_identity()
    {
        var act = () => ((ClaimsIdentity)null).GetKey();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VerifyKey_matches_identity()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var key = claims.GetKey();
        key.VerifyKey("user-123", "sub").Should().BeTrue();
    }

    [Fact]
    public void VerifyKey_returns_false_for_wrong_identity()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var key = claims.GetKey();
        key.VerifyKey("wrong-user", "sub").Should().BeFalse();
    }

    [Fact]
    public void VerifyKey_without_type_matches_any()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var key = claims.GetKey();
        key.VerifyKey("user-123").Should().BeTrue();
    }

    [Fact]
    public void VerifyKey_tuple_overload()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var key = claims.GetKey();
        key.VerifyKey(("user-123", "sub")).Should().BeTrue();
    }

    [Fact]
    public void VerifyKey_with_base64_value_directly()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var key = claims.GetKey();
        key.VerifyKey(key.Value).Should().BeTrue();
    }

    [Fact]
    public void GetIdentities_returns_all_identities()
    {
        var claims = new List<Claim>
        {
            new("sub", "sub-id"),
            new("oid", "oid-id")
        };
        var key = claims.GetKey();
        var identities = key.GetIdentities().ToList();
        identities.Should().Contain(x => x.Identity == "sub-id" && x.Type == "sub");
        identities.Should().Contain(x => x.Identity == "oid-id" && x.Type == "oid");
    }

    [Fact]
    public void GetIdentity_returns_value_for_type()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var key = claims.GetKey();
        key.GetIdentity("sub").Should().Be("user-123");
    }

    [Fact]
    public void GetIdentity_returns_null_for_missing_type()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var key = claims.GetKey();
        key.GetIdentity("oid").Should().BeNull();
    }
}
