# Claims and identity

Helpers for extracting identity, email, and display name from `ClaimsPrincipal` / `ClaimsIdentity` / raw claim collections — plus the `IdentityKey` portable identity representation in the main package.

## Identity extraction

`GetIdentity()` returns the first matching identity claim, checking known claim types in order:

```csharp
public static readonly string[] KeyClaimTypes =
{
    ClaimTypes.NameIdentifier, // WS-Fed / ASP.NET Identity
    "sub",                     // OpenID Connect
    "oid",                     // Azure AD
    "nameid",                  // SAML / ADFS
    "uid"                      // Custom / LDAP
};

var (identity, type) = principal.GetIdentity();   // first non-empty match
var all = principal.Claims.GetIdentities();       // every match
```

## Email

`GetEmail()` checks `email`, then `emails` (Azure B2C array form), then `preferred_username` if it contains `@`, then `name` if it contains `@`. `GetEmailDomain()` extracts just the host portion.

```csharp
var email  = principal.GetEmail();          // "alice@example.com"
var domain = principal.GetEmailDomain();    // "example.com"
```

## Display name

`GetDisplayName()` resolves a human-readable name with a fallback chain that covers OIDC, WS-Fed, Azure AD, and Auth0:

1. `"name"` — OIDC standard
2. `ClaimTypes.Name` — WS-Fed / ASP.NET Identity
3. `"nickname"` — OIDC fallback (Auth0)
4. `"given_name"` + `"family_name"` — OIDC name parts (combined when both exist)
5. `ClaimTypes.GivenName` + `ClaimTypes.Surname` — WS-Fed name parts
6. `"preferred_username"` — only if it doesn't look like an email
7. Email prefix from `GetEmail()`, title-cased with `.`, `-`, `_` replaced by spaces

```csharp
var displayName = principal.GetDisplayName();
// "daniel.bohlin@example.com" → "Daniel Bohlin"
```

## Role by email domain

`AddRoleForDomain()` adds a role claim when the user's email domain matches one of the provided allowlist values. Useful for granting an internal role to anyone with a company email — without maintaining a per-user mapping.

```csharp
principal.AddRoleForDomain("Developer", "thargelion.se", "contoso.com");
```

## IdentityKey (Tharga.Toolkit only)

`IdentityKey` is a base64-encoded JSON map of claim type → value, used to express identity in a portable, serializable form. Build it from claims, verify it later, and look up specific identity types.

```csharp
var key = principal.GetKey();                    // IdentityKey or null
key.VerifyKey("user-123", "sub");                // true if the "sub" claim is "user-123"
key.VerifyKey(("user-123", "sub"));              // tuple overload
key.VerifyKey(key.Value);                        // match base64 verbatim
var sub = key.GetIdentity("sub");                // "user-123"
foreach (var (value, type) in key.GetIdentities())
{
    // every (identity, type) pair
}
```

Storing the `Value` string in a database row lets you match it back to a principal regardless of which claim type the IdP happens to emit — `nameid`, `sub`, `oid`, `uid`, or `NameIdentifier` all map to the same `IdentityKey`.
