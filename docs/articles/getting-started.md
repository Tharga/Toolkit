# Getting started

## Pick a package

Two packages share the same root namespace (`Tharga.Toolkit`):

- **`Tharga.Toolkit.Standard`** targets `netstandard2.0`. Use it from .NET Framework or any shared library that must remain portable. Includes string/datetime helpers, Compare, ManagedTimer, Luhn, PasswordHasher, OrgNo, ByteSize, basic Claims extensions, concurrent collections, SemaphoreExecutor, and `Enumeration`.
- **`Tharga.Toolkit`** targets `net8.0` / `net9.0` / `net10.0`. References Standard internally, so installing it gives you everything from Standard plus: hashing (MD5/SHA*), Base32 encoding, `IdentityKey` and claim verification, URI helpers, async enumerable extensions, and the `ApiKey` service with DI registration.

Install whichever fits. If you target .NET 8+, install the main package and you're done — there's no need to also reference Standard.

```
dotnet add package Tharga.Toolkit
```

```
dotnet add package Tharga.Toolkit.Standard
```

## Basic patterns

Most APIs are extension methods. A few are services that register through DI (`ApiKeyService`) or stateful types (`ManagedTimer`, concurrent dictionaries).

### Strings, dates, collections

```csharp
"".NullIfEmpty();                              // null
((string)null).IfEmpty("fallback");            // "fallback"
"hello world".Truncate(5);                     // "hello"
6.RandomString(StringExtension.NumericCharacters);  // "482917"

DateTime.UtcNow.ToLocalDateString();           // "2026-06-08"
someDate.ToDurationString();                   // "3 hours ago"

items.TakeRandom();
items.TakeChunks(10);
items.RandomOrder();
```

### Compare

```csharp
var diffs = obj1.Compare(obj2);
foreach (var diff in diffs) Console.WriteLine(diff.Message);

var ignoreOrder = list1.Compare(list2, CompareExtensions.CompareMode.IgnoreSortOrder);
```

### ManagedTimer

```csharp
var timer = new ManagedTimer(
    TimeSpan.FromSeconds(5),
    async iteration => { /* your work */ },
    autoStart: true);

timer.BeforeExecuteEvent += (s, e) => { /* e.Cancel = true to skip */ };
timer.AfterExecuteEvent  += (s, e) => { /* inspect e.Exception, e.Elapsed */ };
timer.StateChangedEvent  += (s, e) => { /* Started, Executing, Waiting, Stopped */ };

timer.Stop();
```

### API keys

```csharp
// Register
builder.Services.RegisterApiKeyService(o => o.Iterations = 20000);

// Or bind from IConfiguration (default section "ApiKey")
builder.Services.RegisterApiKeyService(builder.Configuration);

// Use
var apiKey   = apiKeyService.BuildApiKey("alice");
var encrypted = apiKeyService.Encrypt(apiKey);
bool valid    = apiKeyService.Verify(apiKey, encrypted);
var username  = apiKeyService.GetUsername(apiKey);
```

## Where next

- **[Claims and identity](claims-and-identity.md)** — `GetEmail`, `GetDisplayName`, `IdentityKey`, role-by-domain
- **[Hashing](hashing.md)** — `ToHash`, `HashString`, Base32, password hashing
- **[Utilities](utilities.md)** — everything else
