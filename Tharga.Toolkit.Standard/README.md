# Tharga.Toolkit.Standard

[![NuGet](https://img.shields.io/nuget/v/Tharga.Toolkit.Standard)](https://www.nuget.org/packages/Tharga.Toolkit.Standard)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Toolkit.Standard)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](../LICENSE)

.NET Standard 2.0 toolkit with utilities for strings, dates, collections, comparisons, timers, and more.

## Installation

```bash
dotnet add package Tharga.Toolkit.Standard
```

## Features

### String Extensions

```csharp
// Null/empty helpers
string val = "".NullIfEmpty();         // null
bool empty = "".IsNullOrEmpty();       // true
string safe = ((string)null).IfEmpty("fallback"); // "fallback"

// Random strings (cryptographically secure)
var random = StringExtension.GetRandomString(12, 20);
var pin = 6.RandomString(StringExtension.NumericCharacters); // "482917"

// Base64
var encoded = "hello".ToBase64();      // "aGVsbG8="
var decoded = encoded.FromBase64();    // "hello"

// Truncate
var short = "hello world".Truncate(5); // "hello"
```

### Compare

Deep object comparison that returns a list of differences.

```csharp
var obj1 = new { Name = "Alice", Age = 30 };
var obj2 = new { Name = "Bob", Age = 30 };

var diffs = obj1.Compare(obj2);
foreach (var diff in diffs)
{
    Console.WriteLine(diff.Message);
}

// Ignore sort order in collections
var diffs2 = list1.Compare(list2, CompareExtensions.CompareMode.IgnoreSortOrder);
```

### DateTime Extensions

```csharp
// Local formatting
var dateStr = DateTime.UtcNow.ToLocalDateString();       // "2024-01-15"
var timeStr = DateTime.UtcNow.ToLocalTimeString();       // "14:30:00"

// Duration strings (relative time)
var ago = someDate.ToDurationString();                    // "3 hours ago"
var swedish = someDate.ToDurationString(new DurationOptions
{
    StringOptions = DurationStringOptionsExtensions.Get(Language.Sv)
}); // "3 timmar sedan"

// TimeSpan formatting
var span = TimeSpan.FromMinutes(45).ToTimeSpanString();  // "45 minutes"
```

### ManagedTimer

Async timer with interval correction, skip detection, and rich events.

```csharp
var timer = new ManagedTimer(
    TimeSpan.FromSeconds(5),
    async iteration => { /* your work */ },
    autoStart: true
);

timer.BeforeExecuteEvent += (s, e) => { /* can cancel via e.Cancel = true */ };
timer.AfterExecuteEvent += (s, e) => { /* check e.Exception, e.Elapsed */ };
timer.StateChangedEvent += (s, e) => { /* Started, Executing, Waiting, Stopped */ };

timer.Stop();
```

### Enumerable Extensions

```csharp
var item = items.TakeRandom();                     // random element
var shuffled = items.RandomOrder();                 // random order
var tail = items.TakeAllButFirst();                 // skip first
var init = items.TakeAllButLast();                  // skip last
var chunks = items.TakeChunks(10);                  // split into groups of 10
bool empty = EnumerableExtensions.IsNullOrEmpty(items);
var safe = EnumerableExtensions.EmptyIfNull(items); // never null
```

### Luhn Check Digits

```csharp
var check = "7992739871".CheckDigit();       // "3"
var full = "7992739871".AppendCheckDigit();   // "79927398713"
bool valid = "79927398713".HasValidCheckDigit(); // true

// Also works with int, long, and IList<int>
int digit = 36155.CheckDigit(); // 0
```

### Password Hasher

PBKDF2-based password hashing.

```csharp
var hash = PasswordHasher.HashPassword("myPassword");
bool ok = PasswordHasher.VerifyPassword("myPassword", hash);   // true
bool bad = PasswordHasher.VerifyPassword("wrong", hash);       // false
```

### Collections

```csharp
// Two-level concurrent dictionary
var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
var (before, after) = dict.AddOrUpdate("users", "alice", 42);

// Observable concurrent dictionary (INotifyCollectionChanged)
var observable = new ObservableConcurrentDictionary<string, int>();
observable.CollectionChanged += (s, e) => { /* react to changes */ };
observable.Add("key", 1);
```

### Semaphore Executor

Key-based async semaphore: same key = sequential, different keys = concurrent.

```csharp
var executor = new SemaphoreExecutor<string>();
var result = await executor.ExecuteAsync("user-123", async () =>
{
    // Only one operation per key at a time
    return await ProcessAsync();
});
```

### Other Extensions

```csharp
// Org number parsing (Swedish format)
if ("556123-4567".TryParseOrgNo(out var orgNo))
    Console.WriteLine(orgNo); // "556123-4567"

// Byte size formatting
long bytes = 1536;
Console.WriteLine(bytes.ToReadableByteSize(decimalPlaces: 1)); // "1.5 KB"

// Enum mapping
var target = sourceEnum.MapEnum<TargetEnum, SourceEnum>();

// Exception data helpers
throw new Exception("error")
    .AddData("userId", 123)
    .AddData("action", "save");

// Smart enum
public class Color : Enumeration
{
    public static readonly Color Red = new(1, "Red");
    public static readonly Color Blue = new(2, "Blue");
    private Color(int id, string name) : base(id, name) { }
}
var all = Enumeration.GetAll<Color>();
```

[![GitHub repo](https://img.shields.io/github/repo-size/Tharga/Toolkit?style=flat&logo=github&logoColor=red&label=Repo)](https://github.com/Tharga/Toolkit)
