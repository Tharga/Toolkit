using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Tharga.Toolkit;

public static class ClaimsExtensions
{
    public static IdentityKey GetKey(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null) throw new ArgumentNullException(nameof(claimsPrincipal));
        return claimsPrincipal.Claims.GetKey();
    }

    public static IdentityKey GetKey(this ClaimsIdentity claimsIdentity)
    {
        if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
        return claimsIdentity.Claims.GetKey();
    }

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

    public static bool VerifyKey(this IdentityKey identityKey, (string Identity, string Type) key)
    {
        return VerifyKey(identityKey, key.Identity, key.Type);
    }

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

    public static IEnumerable<(string Identity, string Type)> GetIdentities(this IdentityKey identityKey)
    {
        var json = identityKey.Value.FromBase64();
        var vals = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        foreach (var val in vals)
        {
            yield return (val.Value, val.Key);
        }
    }

    public static string GetIdentity(this IdentityKey identityKey, string type)
    {
        var json = identityKey.Value.FromBase64();
        var vals = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        return vals.FirstOrDefault(x => x.Key == type).Value;
    }
}