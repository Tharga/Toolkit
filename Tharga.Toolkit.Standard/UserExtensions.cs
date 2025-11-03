using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Tharga.Toolkit
{
    public static class ClaimsExtensions
    {
        private static readonly string[] _keyClaimTypes =
        {
            ClaimTypes.NameIdentifier, // WS-Fed / ASP.NET Identity
            "sub",                     // OpenID Connect
            "oid",                     // Azure AD
            "nameid",                  // SAML / ADFS
            "uid"                      // Custom / LDAP
        };

        public static string GetKey(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
            return claimsPrincipal.Claims.GetKey();
        }

        public static string GetKey(this ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
            return claimsIdentity.Claims.GetKey();
        }

        public static string GetKey(this IEnumerable<Claim> claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var arr = claims as Claim[] ?? claims.ToArray();

            foreach (var type in _keyClaimTypes)
            {
                var value = arr.FirstOrDefault(c => string.Equals(c.Type, type, StringComparison.OrdinalIgnoreCase))?.Value;
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }

            return null;
        }

        [Obsolete("Use GetKey instead.")]
        public static string GetSub(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.GetKey();
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