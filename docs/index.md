---
_layout: landing
---

# Tharga.Toolkit

A collection of .NET utility libraries — string manipulation, hashing, claims extraction, API keys, deep object comparison, managed timers, collections, and more. Two packages share a common surface: a modern `Tharga.Toolkit` (net8.0+) and a portable `Tharga.Toolkit.Standard` (netstandard2.0).

## Packages

| Package | Target | What it adds |
|---|---|---|
| [Tharga.Toolkit.Standard](https://www.nuget.org/packages/Tharga.Toolkit.Standard) | netstandard2.0 | Compare, ManagedTimer, DateTime extensions, enumerable helpers, Luhn, PasswordHasher, Enumeration, concurrent collections, SemaphoreExecutor, OrgNo, ByteSize, basic Claims extensions. |
| [Tharga.Toolkit](https://www.nuget.org/packages/Tharga.Toolkit) | net8.0, net9.0, net10.0 | Everything in Standard, plus Hash (MD5/SHA*), Base32, IdentityKey + claims verification, URI helpers, async enumerable extensions, ApiKey service with DI registration. |

Pick **Standard** if you target .NET Framework or need maximum portability. Pick the main package for everything else — it includes Standard's surface.

## Quick start

```
dotnet add package Tharga.Toolkit
```

```csharp
// Hash a string
var hash = "hello".ToHash(HashType.SHA256);
var hex = "hello".ToHash(HashFormat.HexLower, HashType.SHA256);

// Get display name from claims
var name = User.GetDisplayName();   // "Daniel Bohlin"

// Register and use the API key service
builder.Services.RegisterApiKeyService(o => o.Iterations = 20000);
var apiKey = apiKeyService.BuildApiKey("alice");
var encrypted = apiKeyService.Encrypt(apiKey);
bool valid = apiKeyService.Verify(apiKey, encrypted);

// Deep object comparison
var diffs = obj1.Compare(obj2);

// Managed async timer
var timer = new ManagedTimer(TimeSpan.FromSeconds(5), async _ => await DoWork(), autoStart: true);
```

## Where next

- **[Articles](articles/index.md)** — getting started, claims & identity, hashing, utilities
- **[API reference](xref:Tharga.Toolkit)** — every public type, method, and option, generated from XML doc comments
- **[GitHub](https://github.com/Tharga/Toolkit)** — source, issues, releases
