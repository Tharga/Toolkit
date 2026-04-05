using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Tharga.Toolkit
{
    public static class ClaimsExtensionsStandard
    {
        public static readonly string[] KeyClaimTypes =
        {
            ClaimTypes.NameIdentifier, // WS-Fed / ASP.NET Identity
            "sub",                     // OpenID Connect
            "oid",                     // Azure AD
            "nameid",                  // SAML / ADFS
            "uid"                      // Custom / LDAP
        };

        public static (string Identity, string Type) GetIdentity(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.GetIdentities().FirstOrDefault();
        }

        public static (string Identity, string Type) GetIdentity(this ClaimsIdentity claimsIdentity)
        {
            return claimsIdentity.Claims.GetIdentities().FirstOrDefault();
        }

        public static (string Identity, string Type) GetIdentity(this IEnumerable<Claim> claims)
        {
            return claims.GetIdentities().FirstOrDefault();
        }

        public static IEnumerable<(string Identity, string Type)> GetIdentities(this IEnumerable<Claim> claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var arr = claims as Claim[] ?? claims.ToArray();

            foreach (var keyClaimType in KeyClaimTypes)
            {
                var value = arr.FirstOrDefault(c => string.Equals(c.Type, keyClaimType, StringComparison.OrdinalIgnoreCase))?.Value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return (value, keyClaimType);
                }
            }
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
            return claimsPrincipal.Claims.GetEmail();
        }

        public static string GetEmail(this ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
            return claimsIdentity.Claims.GetEmail();
        }

        public static string GetEmail(this IEnumerable<Claim> claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var arr = claims as Claim[] ?? claims.ToArray();

            var email = arr.FirstOrDefault(x => x.Type == "email")?.Value ??
                        arr.FirstOrDefault(x => x.Type == "emails")?.Value.Replace("[\"", "").Replace("\"]", "");

            if (string.IsNullOrEmpty(email))
            {
                var preferredUsername = arr.FirstOrDefault(x => x.Type == "preferred_username")?.Value;
                if (preferredUsername?.Contains("@") ?? false) email = preferredUsername;
            }

            if (string.IsNullOrEmpty(email))
            {
                var name = arr.FirstOrDefault(x => x.Type == "name")?.Value;
                if (name?.Contains("@") ?? false) email = name;
            }

            return email;
        }

        public static string GetEmailDomain(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
            return GetEmailDomain(claimsPrincipal.Claims);
        }

        public static string GetEmailDomain(this ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
            return GetEmailDomain(claimsIdentity.Claims);
        }

        public static string GetEmailDomain(this IEnumerable<Claim> claims)
        {
            var email = claims?.GetEmail();
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

        /// <summary>
        /// Extracts a human-readable display name from the claims principal using standard claim type priorities.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal to extract the display name from.</param>
        /// <returns>The display name, or <c>null</c> if no name or email is available.</returns>
        public static string GetDisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
            return claimsPrincipal.Claims.GetDisplayName();
        }

        /// <summary>
        /// Extracts a human-readable display name from the claims identity using standard claim type priorities.
        /// </summary>
        /// <param name="claimsIdentity">The claims identity to extract the display name from.</param>
        /// <returns>The display name, or <c>null</c> if no name or email is available.</returns>
        public static string GetDisplayName(this ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
            return claimsIdentity.Claims.GetDisplayName();
        }

        /// <summary>
        /// Extracts a human-readable display name from a collection of claims.
        /// Resolution priority:
        /// 1. "name" (OIDC standard)
        /// 2. ClaimTypes.Name (WS-Fed / ASP.NET Identity)
        /// 3. "nickname" (OIDC, used by Auth0)
        /// 4. "given_name" + "family_name" (OIDC name parts)
        /// 5. ClaimTypes.GivenName + ClaimTypes.Surname (WS-Fed name parts)
        /// 6. "preferred_username" (only if it does not look like an email address)
        /// 7. Email prefix from <see cref="GetEmail(IEnumerable{Claim})"/>, with separators replaced by spaces and title-cased.
        /// </summary>
        /// <param name="claims">The claims to extract the display name from.</param>
        /// <returns>The display name, or <c>null</c> if no name or email is available.</returns>
        public static string GetDisplayName(this IEnumerable<Claim> claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var arr = claims as Claim[] ?? claims.ToArray();

            // 1. "name" — OIDC standard
            var value = arr.FirstOrDefault(c => c.Type == "name")?.Value;
            if (!string.IsNullOrWhiteSpace(value)) return value;

            // 2. ClaimTypes.Name — WS-Fed / ASP.NET Identity
            value = arr.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (!string.IsNullOrWhiteSpace(value)) return value;

            // 3. "nickname" — OIDC fallback (Auth0 etc.)
            value = arr.FirstOrDefault(c => c.Type == "nickname")?.Value;
            if (!string.IsNullOrWhiteSpace(value)) return value;

            // 4. "given_name" + "family_name" — OIDC name parts
            var givenName = arr.FirstOrDefault(c => c.Type == "given_name")?.Value;
            var familyName = arr.FirstOrDefault(c => c.Type == "family_name")?.Value;
            var combined = CombineNameParts(givenName, familyName);
            if (combined != null) return combined;

            // 5. ClaimTypes.GivenName + ClaimTypes.Surname — WS-Fed name parts
            givenName = arr.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            familyName = arr.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            combined = CombineNameParts(givenName, familyName);
            if (combined != null) return combined;

            // 6. "preferred_username" — only if not email-like
            var preferredUsername = arr.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            if (!string.IsNullOrWhiteSpace(preferredUsername) && !preferredUsername.Contains("@"))
                return preferredUsername;

            // 7. Email prefix fallback — title-cased with separators replaced
            var email = arr.GetEmail();
            if (string.IsNullOrEmpty(email)) return null;

            var prefix = email.Split('@')[0];
            var words = prefix.Split('.', '-', '_');
            var titleCased = string.Join(" ", words.Select(w =>
                string.IsNullOrEmpty(w) ? w : char.ToUpper(w[0], CultureInfo.InvariantCulture) + w.Substring(1).ToLower(CultureInfo.InvariantCulture)));
            return titleCased;
        }

        private static string CombineNameParts(string givenName, string familyName)
        {
            var hasGiven = !string.IsNullOrWhiteSpace(givenName);
            var hasFamily = !string.IsNullOrWhiteSpace(familyName);

            if (hasGiven && hasFamily) return $"{givenName} {familyName}";
            if (hasGiven) return givenName;
            if (hasFamily) return familyName;
            return null;
        }

        /// <summary>
        /// Add role on the user claim if the users email matches one of the provided list of domains.
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <param name="role"></param>
        /// <param name="domains"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddRoleForDomain(this ClaimsPrincipal claimsPrincipal, string role, params string[] domains)
        {
            if (claimsPrincipal == null || claimsPrincipal.Identity == null) throw new ArgumentNullException(nameof(claimsPrincipal), "ClaimsPrincipal or its Identity cannot be null.");

            var claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;

            var emailDomain = claimsIdentity.GetEmailDomain();
            if (string.IsNullOrEmpty(emailDomain)) return;

            var roleClaims = claimsIdentity.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(x => x.Value);

            if (domains.Any(domain => emailDomain.Equals(domain, StringComparison.InvariantCultureIgnoreCase))
                && !roleClaims.Contains(role, StringComparer.InvariantCultureIgnoreCase))
            {
                claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role));
            }
        }
    }
}