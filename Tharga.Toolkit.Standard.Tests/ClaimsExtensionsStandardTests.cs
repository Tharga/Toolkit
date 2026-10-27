using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AwesomeAssertions;
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

        var identity = principal.Identities.Single();
        identity.FindAll(ClaimTypes.Role).Select(c => c.Value).Should().Contain("Admin");
    }

    [Fact]
    public void AddRoleForDomain_does_not_add_role_when_domain_does_not_match()
    {
        var principal = CreatePrincipal(new Claim("email", "test@other.com"));
        principal.AddRoleForDomain("Admin", "example.com");

        var identity = principal.Identities.Single();
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

        var identity = principal.Identities.Single();
        identity.FindAll(ClaimTypes.Role).Where(c => c.Value == "Admin").Should().HaveCount(1);
    }

    [Fact]
    public void AddRoleForDomain_throws_for_null_principal()
    {
        var act = () => ((ClaimsPrincipal)null).AddRoleForDomain("Admin", "example.com");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddRoleForDomain_does_not_use_name_claim()
    {
        var principal = CreatePrincipal(new Claim("name", "test@example.com"));
        principal.AddRoleForDomain("Admin", "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void AddRoleForDomain_does_not_use_preferred_username_by_default()
    {
        var principal = CreatePrincipal(new Claim("preferred_username", "test@example.com"));
        principal.AddRoleForDomain("Admin", "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void AddRoleForDomain_does_not_add_role_when_email_verified_is_false()
    {
        var principal = CreatePrincipal(
            new Claim("email", "test@example.com"),
            new Claim("email_verified", "false"));
        principal.AddRoleForDomain("Admin", "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void AddRoleForDomain_adds_role_when_email_verified_is_true()
    {
        var principal = CreatePrincipal(
            new Claim("email", "test@example.com"),
            new Claim("email_verified", "true", ClaimValueTypes.Boolean));
        principal.AddRoleForDomain("Admin", "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Select(c => c.Value).Should().Contain("Admin");
    }

    [Fact]
    public void AddRoleForDomain_uses_mapped_email_claim_type()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Email, "test@example.com"));
        principal.AddRoleForDomain("Admin", "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Select(c => c.Value).Should().Contain("Admin");
    }

    [Fact]
    public void AddRoleForDomain_uses_emails_array_claim()
    {
        var principal = CreatePrincipal(new Claim("emails", "[\"test@example.com\"]"));
        principal.AddRoleForDomain("Admin", "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Select(c => c.Value).Should().Contain("Admin");
    }

    [Fact]
    public void AddRoleForDomain_uses_preferred_username_when_trusted()
    {
        var principal = CreatePrincipal(new Claim("preferred_username", "test@example.com"));
        principal.AddRoleForDomain("Admin", new RoleForDomainOptions { TrustPreferredUsername = true }, "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Select(c => c.Value).Should().Contain("Admin");
    }

    [Fact]
    public void AddRoleForDomain_prefers_email_claim_over_trusted_preferred_username()
    {
        var principal = CreatePrincipal(
            new Claim("email", "test@other.com"),
            new Claim("preferred_username", "test@example.com"));
        principal.AddRoleForDomain("Admin", new RoleForDomainOptions { TrustPreferredUsername = true }, "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void AddRoleForDomain_does_not_fall_back_to_preferred_username_when_email_is_not_verified()
    {
        var principal = CreatePrincipal(
            new Claim("email", "test@example.com"),
            new Claim("email_verified", "false"),
            new Claim("preferred_username", "test@example.com"));
        principal.AddRoleForDomain("Admin", new RoleForDomainOptions { TrustPreferredUsername = true }, "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void AddRoleForDomain_does_not_use_name_claim_when_preferred_username_is_trusted()
    {
        var principal = CreatePrincipal(new Claim("name", "test@example.com"));
        principal.AddRoleForDomain("Admin", new RoleForDomainOptions { TrustPreferredUsername = true }, "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void AddRoleForDomain_with_null_options_uses_defaults()
    {
        var principal = CreatePrincipal(new Claim("preferred_username", "test@example.com"));
        principal.AddRoleForDomain("Admin", (RoleForDomainOptions)null, "example.com");

        principal.Identities.Single().FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    // --- GetDisplayName tests ---

    [Fact]
    public void GetDisplayName_returns_name_claim()
    {
        var claims = new List<Claim> { new("name", "Daniel Bohlin") };
        claims.GetDisplayName().Should().Be("Daniel Bohlin");
    }

    [Fact]
    public void GetDisplayName_returns_ClaimTypes_Name_when_no_name_claim()
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, "Daniel Bohlin") };
        claims.GetDisplayName().Should().Be("Daniel Bohlin");
    }

    [Fact]
    public void GetDisplayName_prefers_name_over_ClaimTypes_Name()
    {
        var claims = new List<Claim>
        {
            new("name", "From OIDC"),
            new(ClaimTypes.Name, "From WS-Fed")
        };
        claims.GetDisplayName().Should().Be("From OIDC");
    }

    [Fact]
    public void GetDisplayName_returns_nickname_when_no_name()
    {
        var claims = new List<Claim> { new("nickname", "Danny") };
        claims.GetDisplayName().Should().Be("Danny");
    }

    [Fact]
    public void GetDisplayName_combines_given_name_and_family_name()
    {
        var claims = new List<Claim>
        {
            new("given_name", "Daniel"),
            new("family_name", "Bohlin")
        };
        claims.GetDisplayName().Should().Be("Daniel Bohlin");
    }

    [Fact]
    public void GetDisplayName_returns_given_name_alone_when_no_family_name()
    {
        var claims = new List<Claim> { new("given_name", "Daniel") };
        claims.GetDisplayName().Should().Be("Daniel");
    }

    [Fact]
    public void GetDisplayName_combines_ClaimTypes_GivenName_and_Surname()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.GivenName, "Daniel"),
            new(ClaimTypes.Surname, "Bohlin")
        };
        claims.GetDisplayName().Should().Be("Daniel Bohlin");
    }

    [Fact]
    public void GetDisplayName_returns_ClaimTypes_GivenName_alone()
    {
        var claims = new List<Claim> { new(ClaimTypes.GivenName, "Daniel") };
        claims.GetDisplayName().Should().Be("Daniel");
    }

    [Fact]
    public void GetDisplayName_returns_preferred_username_when_not_email()
    {
        var claims = new List<Claim> { new("preferred_username", "dannybee") };
        claims.GetDisplayName().Should().Be("dannybee");
    }

    [Fact]
    public void GetDisplayName_skips_preferred_username_when_email_like()
    {
        var claims = new List<Claim>
        {
            new("preferred_username", "daniel@example.com"),
            new("email", "daniel@example.com")
        };
        // Should fall through to email prefix fallback
        claims.GetDisplayName().Should().Be("Daniel");
    }

    [Fact]
    public void GetDisplayName_falls_back_to_email_prefix_with_title_case()
    {
        var claims = new List<Claim> { new("email", "daniel.bohlin@example.com") };
        claims.GetDisplayName().Should().Be("Daniel Bohlin");
    }

    [Fact]
    public void GetDisplayName_email_prefix_replaces_hyphens()
    {
        var claims = new List<Claim> { new("email", "anna-karin@example.com") };
        claims.GetDisplayName().Should().Be("Anna Karin");
    }

    [Fact]
    public void GetDisplayName_email_prefix_replaces_underscores()
    {
        var claims = new List<Claim> { new("email", "john_doe@example.com") };
        claims.GetDisplayName().Should().Be("John Doe");
    }

    [Fact]
    public void GetDisplayName_returns_null_when_no_claims()
    {
        var claims = new List<Claim> { new("custom", "value") };
        claims.GetDisplayName().Should().BeNull();
    }

    [Fact]
    public void GetDisplayName_from_ClaimsPrincipal()
    {
        var principal = CreatePrincipal(new Claim("name", "Daniel Bohlin"));
        principal.GetDisplayName().Should().Be("Daniel Bohlin");
    }

    [Fact]
    public void GetDisplayName_from_ClaimsIdentity()
    {
        var identity = new ClaimsIdentity(new[] { new Claim("name", "Daniel Bohlin") });
        identity.GetDisplayName().Should().Be("Daniel Bohlin");
    }

    [Fact]
    public void GetDisplayName_throws_for_null_claims()
    {
        var act = () => ((IEnumerable<Claim>)null).GetDisplayName();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetDisplayName_throws_for_null_principal()
    {
        var act = () => ((ClaimsPrincipal)null).GetDisplayName();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetDisplayName_throws_for_null_identity()
    {
        var act = () => ((ClaimsIdentity)null).GetDisplayName();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetDisplayName_skips_whitespace_only_name_claim()
    {
        var claims = new List<Claim>
        {
            new("name", "   "),
            new("nickname", "Danny")
        };
        claims.GetDisplayName().Should().Be("Danny");
    }

    [Fact]
    public void GetDisplayName_priority_4_over_5()
    {
        var claims = new List<Claim>
        {
            new("given_name", "OIDC Given"),
            new(ClaimTypes.GivenName, "WSFed Given")
        };
        claims.GetDisplayName().Should().Be("OIDC Given");
    }
}
