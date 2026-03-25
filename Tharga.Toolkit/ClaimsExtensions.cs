using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Tharga.Toolkit;

/// <summary>
/// Extension methods for extracting and verifying identity keys from claims.
/// Works with <see cref="IdentityKey"/> to provide a portable, serialized identity representation.
/// </summary>
public static class ClaimsExtensions
{
    /// <summary>
    /// Extracts an <see cref="IdentityKey"/> from the claims principal's identity claims.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal to extract the key from.</param>
    /// <returns>An <see cref="IdentityKey"/>, or <c>null</c> if no recognized identity claims are found.</returns>
    public static IdentityKey GetKey(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
        return claimsPrincipal.Claims.GetKey();
    }

    /// <summary>
    /// Extracts an <see cref="IdentityKey"/> from the claims identity.
    /// </summary>
    public static IdentityKey GetKey(this ClaimsIdentity claimsIdentity)
    {
        if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
        return claimsIdentity.Claims.GetKey();
    }

    /// <summary>
    /// Extracts an <see cref="IdentityKey"/> from a collection of claims by matching known identity claim types.
    /// </summary>
    public static IdentityKey GetKey(this IEnumerable<Claim> claims)
    {
        if (claims == null) throw new ArgumentNullException(nameof(claims));

        var keys = new Dictionary<string, string>();
        var arr = claims as Claim[] ?? claims.ToArray();

        foreach (var keyClaimType in ClaimsExtensionsStandard.KeyClaimTypes)
        {
            var value = arr.FirstOrDefault(c => string.Equals(c.Type, keyClaimType, StringComparison.OrdinalIgnoreCase))?.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                keys.Add(keyClaimType, value);
            }
        }

        if (!keys.Any()) return null;

        var json = System.Text.Json.JsonSerializer.Serialize(keys);
        return new IdentityKey(json.ToBase64());
    }

    /// <summary>
    /// Verifies whether the identity key contains the specified identity.
    /// </summary>
    /// <param name="identityKey">The identity key to verify against.</param>
    /// <param name="key">A tuple of identity value and claim type to match.</param>
    /// <returns><c>true</c> if the identity key contains the specified identity.</returns>
    public static bool VerifyKey(this IdentityKey identityKey, (string Identity, string Type) key)
    {
        return VerifyKey(identityKey, key.Identity, key.Type);
    }

    /// <summary>
    /// Verifies whether the identity key contains the specified identity, optionally filtered by claim type.
    /// </summary>
    public static bool VerifyKey(this IdentityKey identityKey, string identity, string type = null)
    {
        if (identityKey.Value == identity && type == null) return true; //Match on the base64 type.

        var json = identityKey.Value.FromBase64();
        var vals = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        foreach (var val in vals)
        {
            if (type != null && type != val.Key) continue;

            if (val.Value.Equals(identity))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Extracts all identity/type pairs stored in the identity key.
    /// </summary>
    public static IEnumerable<(string Identity, string Type)> GetIdentities(this IdentityKey identityKey)
    {
        var json = identityKey.Value.FromBase64();
        var vals = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        foreach (var val in vals)
        {
            yield return (val.Value, val.Key);
        }
    }

    /// <summary>
    /// Gets the identity value for a specific claim type from the identity key.
    /// </summary>
    /// <param name="identityKey">The identity key to search.</param>
    /// <param name="type">The claim type to look up.</param>
    /// <returns>The identity value, or <c>null</c> if the type is not found.</returns>
    public static string GetIdentity(this IdentityKey identityKey, string type)
    {
        var json = identityKey.Value.FromBase64();
        var vals = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        return vals.FirstOrDefault(x => x.Key == type).Value;
    }
}