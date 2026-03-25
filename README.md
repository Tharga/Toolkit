# Tharga Toolkit

A collection of .NET utility libraries providing extensions and helpers for common operations. Available as two NuGet packages: a modern **.NET** version (net8.0+) and a **.NET Standard 2.0** version for broader compatibility.

[![GitHub repo Issues](https://img.shields.io/github/issues/Tharga/Toolkit?style=flat&logo=github&logoColor=red&label=Issues)](https://github.com/Tharga/Toolkit/issues?q=is%3Aopen)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Installation

```bash
# .NET version (net8.0, net9.0, net10.0) — includes all Standard features plus extras
dotnet add package Tharga.Toolkit

# Standard version (netstandard2.0) — for broader compatibility
dotnet add package Tharga.Toolkit.Standard
```

### Which package should I use?

- **Tharga.Toolkit** — if your project targets .NET 8 or later. Includes everything from Standard plus additional features (Hash, Claims, ApiKey, async enumerables, URI extensions).
- **Tharga.Toolkit.Standard** — if you need .NET Standard 2.0 compatibility (e.g. shared libraries targeting both .NET Framework and .NET).

## .NET

[![NuGet](https://img.shields.io/nuget/v/Tharga.Toolkit)](https://www.nuget.org/packages/Tharga.Toolkit)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Toolkit)

Includes all Standard features, plus:

| Feature | Description |
|---------|-------------|
| HashExtensions | Compute MD5, SHA1, SHA256, SHA384, SHA512 hashes from bytes, strings, URIs, and streams |
| Base32Encoding | RFC 4648 Base32 encode/decode |
| ClaimsExtensions | Extract and verify identity keys from ClaimsPrincipal |
| UriExtensions | Remove query strings, extract query parameter values |
| EnumerableExtensionsAsync | Random selection and shuffling for IAsyncEnumerable |
| ApiKeyService | Build, encrypt, verify, and parse API keys with DI support |

See [Tharga.Toolkit/README.md](Tharga.Toolkit/README.md) for usage examples.

## Standard

[![NuGet](https://img.shields.io/nuget/v/Tharga.Toolkit.Standard)](https://www.nuget.org/packages/Tharga.Toolkit.Standard)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Toolkit.Standard)

| Feature | Description |
|---------|-------------|
| Compare | Deep object comparison with diff output |
| ManagedTimer | Async timer with interval correction, skip detection, and state events |
| DateTimeExtensions | Localized date/time/duration formatting |
| EnumerableExtensions | Random selection, chunking, null-safe operations |
| StringExtension | Null checks, random string generation, Base64, truncation |
| Luhn | Check digit computation and validation |
| PasswordHasher | PBKDF2 password hashing and verification |
| Enumeration | Smart enum base class |
| Collections | ConcurrentTwoLevelDictionary, ObservableConcurrentDictionary |
| SemaphoreExecutor | Key-based async semaphore for concurrent execution control |
| OrgNoExtensions | Swedish organization number parsing and validation |
| ByteSizeExtensions | Human-readable byte size formatting |
| ClaimsExtensions | Identity and email extraction from claims |

See [Tharga.Toolkit.Standard/README.md](Tharga.Toolkit.Standard/README.md) for usage examples.

## Tests

481 tests across both projects (317 Toolkit + 164 Standard), covering all public APIs.

## License

[MIT](LICENSE)
