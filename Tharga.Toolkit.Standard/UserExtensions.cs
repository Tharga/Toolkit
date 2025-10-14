using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

public static class UserExtensions
{
    public static string GetEmail(this ClaimsIdentity claimsIdentity)
    {
        return claimsIdentity.Claims.GetEmail();
    }

    public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.GetEmail();
    }

    public static string GetEmail(this IEnumerable<Claim> claims)
    {
        var arr = claims as Claim[] ?? claims.ToArray();

        var email = arr.FirstOrDefault(x => x.Type == "email")?.Value ??
                    arr.FirstOrDefault(x => x.Type == "emails")?.Value.Replace("[\"", "").Replace("\"]", "");

        if (string.IsNullOrEmpty(email))
        {
            var preferredUsername = arr.FirstOrDefault(x => x.Type == "preferred_username")?.Value;
            if (preferredUsername?.Contains("@") ?? false) email = preferredUsername;
        }

        return email;
    }

    public static string GetEmailDomain(this ClaimsIdentity item)
    {
        var email = item?.GetEmail();
        if (string.IsNullOrEmpty(email)) return null;

        try
        {
            return new System.Net.Mail.MailAddress(email).Host;
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public static string GetSub(this ClaimsPrincipal claimsPrincipal)
    {
        var key = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(key))
        {
            key = claimsPrincipal.FindFirst("sub")?.Value;
        }

        return key;
    }

    public static void AddRoleForDomain(this ClaimsPrincipal claimsPrincipal, string role, params string[] domains)
    {
        if (claimsPrincipal == null || claimsPrincipal.Identity == null) throw new ArgumentNullException(nameof(claimsPrincipal), "ClaimsPrincipal or its Identity cannot be null.");

        var claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;

        var emailDomain = claimsIdentity.GetEmailDomain();
        if (string.IsNullOrEmpty(emailDomain)) return;

        var roleClaims = claimsIdentity.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(x => x.Value);

        if (domains.Any(d => emailDomain.Equals(d, StringComparison.InvariantCultureIgnoreCase))
            && !roleClaims.Contains(role, StringComparer.InvariantCultureIgnoreCase))
        {
            claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role));
        }
    }
}