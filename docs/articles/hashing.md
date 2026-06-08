# Hashing

`Tharga.Toolkit` ships two distinct hash surfaces: cryptographic hash digests (`ToHash`, `HashString`, Base32) and password verification (`PasswordHasher` + the `ApiKey` service). The first is for fingerprinting data; the second is for authenticating users.

## Cryptographic hashing

Compute MD5, SHA1, SHA256, SHA384, or SHA512 from strings, byte arrays, URIs, or streams.

```csharp
var bytes = "hello".ToHash(HashType.SHA256);              // Hash (raw bytes)
var hex   = "hello".ToHash(HashFormat.HexLower, HashType.SHA256);  // HashString
var b64   = "hello".ToHash(HashFormat.Base64, HashType.SHA256);

await using var stream = File.OpenRead("file.dat");
var streamHash = await stream.ToHashAsync(HashFormat.Base64, HashType.SHA256);
```

### Format conversion

`HashString` carries both the formatted representation and the original bytes, so you can re-format without recomputing the hash:

```csharp
var hex     = "hello".ToHash(HashFormat.HexLower, HashType.SHA256);
var base32  = hex.ChangeFormat(HashFormat.Base32);
var base64  = hex.ChangeFormat(HashFormat.Base64);
```

Available formats: `Hex`, `HexLower`, `HexWithDashes`, `Base64`, `Base32`.

### Implicit conversions

`Hash` converts implicitly to `byte[]` and `HashString` to `string`, so you can use them anywhere those types are expected:

```csharp
byte[] raw = "hello".ToHash(HashType.SHA256);
string  s  = "hello".ToHash(HashFormat.HexLower, HashType.SHA256);
```

## Base32 encoding (RFC 4648)

```csharp
var encoded = Base32Encoding.Encode(Encoding.UTF8.GetBytes("hello"));
var decoded = Base32Encoding.Decode(encoded);
```

## Password hashing (PBKDF2)

`PasswordHasher` derives a PBKDF2 hash, prepends the salt, and packages everything into a single string. Verification re-derives and compares in constant time.

```csharp
var hash = PasswordHasher.HashPassword("secret");
bool ok  = PasswordHasher.VerifyPassword("secret", hash);   // true
bool bad = PasswordHasher.VerifyPassword("wrong",  hash);   // false
```

This is the Standard package's per-process surface — same algorithm, no DI, no options.

## API key service (Tharga.Toolkit only)

For backend-issued API keys, `ApiKeyService` builds, encrypts, verifies, and extracts the username from a key. It's the same PBKDF2 base, packaged for DI registration and configurable iteration count.

```csharp
// Register with code-based options
builder.Services.RegisterApiKeyService(o =>
{
    o.SaltSize   = 32;
    o.Iterations = 20000;
});

// Or bind from IConfiguration (default section "ApiKey")
builder.Services.RegisterApiKeyService(builder.Configuration);
builder.Services.RegisterApiKeyService(builder.Configuration, "MyApiKeys");
```

Example `appsettings.json`:

```json
{
  "ApiKey": {
    "SaltSize": 32,
    "HashSize": 32,
    "Iterations": 20000
  }
}
```

Use the service via `IApiKeyService`:

```csharp
public class IssueController(IApiKeyService apiKeyService) : ControllerBase
{
    [HttpPost("issue/{username}")]
    public IActionResult Issue(string username)
    {
        var apiKey    = apiKeyService.BuildApiKey(username);
        var encrypted = apiKeyService.Encrypt(apiKey);

        // Store `encrypted` in your database. Return `apiKey` to the caller exactly once.
        return Ok(new { apiKey });
    }
}

public class VerifyMiddleware(IApiKeyService apiKeyService)
{
    public bool Authenticate(string apiKey, string storedEncrypted)
        => apiKeyService.Verify(apiKey, storedEncrypted);

    public string ExtractUser(string apiKey)
        => apiKeyService.GetUsername(apiKey);
}
```

The username is embedded in the key, so `GetUsername(apiKey)` recovers it without a database lookup — useful for routing the verify step to the right stored hash.
