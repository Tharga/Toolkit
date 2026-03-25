# Tharga.Toolkit

[![NuGet](https://img.shields.io/nuget/v/Tharga.Toolkit)](https://www.nuget.org/packages/Tharga.Toolkit)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Toolkit)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](../LICENSE)

Modern .NET toolkit (net8.0+) with hashing, claims, API keys, and more. Includes all features from [Tharga.Toolkit.Standard](../Tharga.Toolkit.Standard/README.md).

## Installation

```bash
dotnet add package Tharga.Toolkit
```

## Features

### Hash

Compute hashes from strings, byte arrays, URIs, and streams. Supports MD5, SHA1, SHA256, SHA384, and SHA512.

```csharp
// String hash
var hash = "hello".ToHash(HashType.SHA256);

// Formatted output
var hex = "hello".ToHash(HashFormat.HexLower, HashType.SHA256);

// Stream hash
await using var stream = File.OpenRead("file.dat");
var streamHash = await stream.ToHashAsync(HashFormat.Base64, HashType.SHA256);

// Change format
var base32 = hex.ChangeFormat(HashFormat.Base32);
```

### Base32 Encoding

RFC 4648 Base32 encoding and decoding.

```csharp
var encoded = Base32Encoding.Encode(Encoding.UTF8.GetBytes("hello"));
var decoded = Base32Encoding.Decode(encoded);
```

### Claims Extensions

Extract and verify identity keys from claims principals.

```csharp
// Get a portable identity key
var key = claimsPrincipal.GetKey();

// Verify against a known identity
bool match = key.VerifyKey("user-123", "sub");

// Extract all identities
var identities = key.GetIdentities();
```

### URI Extensions

```csharp
var uri = new Uri("https://example.com/path?page=1&sort=name");

// Remove query string
var clean = uri.RemoveQuery(); // https://example.com/path

// Get specific query values
var values = uri.GetQueryValue("sort"); // ["name"]
```

### Async Enumerable Extensions

```csharp
// Pick a random element from an async stream
var item = await asyncEnumerable.TakeRandomAsync();

// Shuffle an async stream
await foreach (var x in asyncEnumerable.RandomOrderAsync())
{
    // ...
}
```

### API Key Service

Build, encrypt, and verify API keys with dependency injection support.

```csharp
// Register in DI
services.RegisterApiKeyService(options =>
{
    options.SaltSize = 32;
    options.Iterations = 20000;
});

// Use via IApiKeyService
var apiKey = apiKeyService.BuildApiKey("username");
var encrypted = apiKeyService.Encrypt(apiKey);
bool valid = apiKeyService.Verify(apiKey, encrypted);
var username = apiKeyService.GetUsername(apiKey);
```

[![GitHub repo](https://img.shields.io/github/repo-size/Tharga/Toolkit?style=flat&logo=github&logoColor=red&label=Repo)](https://github.com/Tharga/Toolkit)
