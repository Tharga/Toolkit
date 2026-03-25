using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class ClaimsExtensionsStandardTests
{
    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void GetIdentity_returns_sub_claim()
    {
        var claims = new List<Claim> { new("sub", "user-123") };
        var result = claims.GetIdentity();
        result.Identity.Should().Be("user-123");
        result.Type.Should().Be("sub");
    }

    [Fact]
    public void GetIdentity_returns_nameidentifier_first()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "id-from-nameid"),
            new("sub", "id-from-sub")
        };
        var result = claims.GetIdentity();
        result.Identity.Should().Be("id-from-nameid");
        result.Type.Should().Be(ClaimTypes.NameIdentifier);
    }

    [Fact]
    public void GetIdentity_returns_default_when_no_matching_claims()
    {
        var claims = new List<Claim> { new("custom", "value") };
        var result = claims.GetIdentity();
        result.Identity.Should().BeNull();
        result.Type.Should().BeNull();
    }

    [Fact]
    public void GetIdentities_returns_all_matching_claims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "id1"),
            new("sub", "id2"),
            new("oid", "id3")
        };
        var result = claims.GetIdentities().ToList();
        result.Should().HaveCount(3);
    }

    [Fact]
    public void GetIdentities_throws_for_null()
    {
        var act = () => ((IEnumerable<Claim>)null).GetIdentities().ToList();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetIdentity_from_ClaimsPrincipal()
    {
        var principal = CreatePrincipal(new Claim("sub", "user-456"));
        var result = principal.GetIdentity();
        result.Identity.Should().Be("user-456");
    }

    [Fact]
    public void GetIdentity_from_ClaimsIdentity()
    {
        var identity = new ClaimsIdentity(new[] { new Claim("sub", "user-789") });
        var result = identity.GetIdentity();
        result.Identity.Should().Be("user-789");
    }

    [Fact]
    public void GetEmail_returns_email_claim()
    {
        var claims = new List<Claim> { new("email", "test@example.com") };
        claims.GetEmail().Should().Be("test@example.com");
    }

    [Fact]
    public void GetEmail_falls_back_to_preferred_username()
    {
        var claims = new List<Claim> { new("preferred_username", "user@example.com") };
        claims.GetEmail().Should().Be("user@example.com");
    }

    [Fact]
    public void GetEmail_falls_back_to_name_with_at_sign()
    {
        var claims = new List<Claim> { new("name", "user@example.com") };
        claims.GetEmail().Should().Be("user@example.com");
    }

    [Fact]
    public void GetEmail_returns_null_when_no_email()
    {
        var claims = new List<Claim> { new("name", "John") };
        claims.GetEmail().Should().BeNull();
    }

    [Fact]
    public void GetEmail_from_ClaimsPrincipal()
    {
        var principal = CreatePrincipal(new Claim("email", "test@example.com"));
        principal.GetEmail().Should().Be("test@example.com");
    }

    [Fact]
    public void GetEmail_throws_for_null_principal()
    {
        var act = () => ((ClaimsPrincipal)null).GetEmail();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetEmailDomain_returns_domain()
    {
        var claims = new List<Claim> { new("email", "test@example.com") };
        claims.GetEmailDomain().Should().Be("example.com");
    }

    [Fact]
    public void GetEmailDomain_returns_null_when_no_email()
    {
        var claims = new List<Claim> { new("name", "John") };
        claims.GetEmailDomain().Should().BeNull();
    }

    [Fact]
    public void GetEmailDomain_from_ClaimsPrincipal()
    {
        var principal = CreatePrincipal(new Claim("email", "test@company.org"));
        principal.GetEmailDomain().Should().Be("company.org");
    }

    [Fact]
    public void GetEmailDomain_throws_for_null_principal()
    {
        var act = () => ((ClaimsPrincipal)null).GetEmailDomain();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddRoleForDomain_adds_role_when_domain_matches()
    {
        var principal = CreatePrincipal(new Claim("email", "test@example.com"));
        principal.AddRoleForDomain("Admin", "example.com");

        var identity = (ClaimsIdentity)principal.Identity;
        identity.FindAll(ClaimTypes.Role).Select(c => c.Value).Should().Contain("Admin");
    }

    [Fact]
    public void AddRoleForDomain_does_not_add_role_when_domain_does_not_match()
    {
        var principal = CreatePrincipal(new Claim("email", "test@other.com"));
        principal.AddRoleForDomain("Admin", "example.com");

        var identity = (ClaimsIdentity)principal.Identity;
        identity.FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void AddRoleForDomain_does_not_duplicate_role()
    {
        var principal = CreatePrincipal(
            new Claim("email", "test@example.com"),
            new Claim(ClaimTypes.Role, "Admin")
        );
        principal.AddRoleForDomain("Admin", "example.com");

        var identity = (ClaimsIdentity)principal.Identity;
        identity.FindAll(ClaimTypes.Role).Where(c => c.Value == "Admin").Should().HaveCount(1);
    }

    [Fact]
    public void AddRoleForDomain_throws_for_null_principal()
    {
        var act = () => ((ClaimsPrincipal)null).AddRoleForDomain("Admin", "example.com");
        act.Should().Throw<ArgumentNullException>();
    }
}
