# Utilities

Everything that doesn't fit under [Claims](claims-and-identity.md) or [Hashing](hashing.md) — the day-to-day helpers most consumers reach for.

## Compare

Deep object comparison that returns a list of differences, with optional sort-order tolerance for collections.

```csharp
var diffs = obj1.Compare(obj2);
foreach (var diff in diffs) Console.WriteLine(diff.Message);

var ignoreOrder = list1.Compare(list2, CompareExtensions.CompareMode.IgnoreSortOrder);
```

## ManagedTimer

Async timer with interval correction (so drift doesn't accumulate), skip detection (knows when an iteration ran late), and rich events for state changes.

```csharp
var timer = new ManagedTimer(
    TimeSpan.FromSeconds(5),
    async iteration => { /* your work */ },
    autoStart: true);

timer.BeforeExecuteEvent += (s, e) => { if (ShouldSkip()) e.Cancel = true; };
timer.AfterExecuteEvent  += (s, e) => { if (e.Exception != null) Log(e.Exception); };
timer.StateChangedEvent  += (s, e) => Console.WriteLine(e.State);

timer.Stop();
```

States: `Stopped`, `Started`, `Waiting`, `Executing`.

## DateTime and duration

```csharp
DateTime.UtcNow.ToLocalDateString();    // "2026-06-08"
DateTime.UtcNow.ToLocalTimeString();    // "14:30:00"

someDate.ToDurationString();            // "3 hours ago"
someDate.ToDurationString(new DurationOptions
{
    StringOptions = DurationStringOptionsExtensions.Get(Language.Sv)
});                                      // "3 timmar sedan"

TimeSpan.FromMinutes(45).ToTimeSpanString();   // "45 minutes"
```

## Enumerable

```csharp
items.TakeRandom();              // one random element
items.RandomOrder();             // shuffled enumeration
items.TakeAllButFirst();         // tail
items.TakeAllButLast();          // init
items.TakeChunks(10);            // groups of 10

EnumerableExtensions.IsNullOrEmpty(items);   // null-safe
EnumerableExtensions.EmptyIfNull(items);     // never null
```

Async equivalents in `Tharga.Toolkit`:

```csharp
var picked = await asyncEnumerable.TakeRandomAsync();
await foreach (var x in asyncEnumerable.RandomOrderAsync()) { /* ... */ }
```

## Luhn

Compute and validate Luhn check digits — works on `string`, `int`, `long`, and `IList<int>`:

```csharp
"7992739871".CheckDigit();           // "3"
"7992739871".AppendCheckDigit();     // "79927398713"
"79927398713".HasValidCheckDigit();  // true

36155.CheckDigit();                  // 0
```

## OrgNo (Swedish organization number)

```csharp
if ("556123-4567".TryParseOrgNo(out var orgNo))
    Console.WriteLine(orgNo);   // "556123-4567" (normalized)
```

## Byte size

```csharp
((long)1536).ToReadableByteSize();                 // "1.5 KB"
((long)1536).ToReadableByteSize(decimalPlaces: 1); // "1.5 KB"
```

## Collections

```csharp
// Two-level concurrent dictionary
var dict = new ConcurrentTwoLevelDictionary<string, string, int>();
var (before, after) = dict.AddOrUpdate("users", "alice", 42);

// Observable concurrent dictionary (raises INotifyCollectionChanged)
var observable = new ObservableConcurrentDictionary<string, int>();
observable.CollectionChanged += (s, e) => { /* react */ };
observable.Add("key", 1);
```

## SemaphoreExecutor — key-based concurrency

Same key → sequential. Different keys → concurrent. Useful when you need per-user or per-tenant serialization without a global lock.

```csharp
var executor = new SemaphoreExecutor<string>();
var result = await executor.ExecuteAsync("user-123", async () =>
{
    // only one operation per "user-123" runs at a time
    return await ProcessAsync();
});
```

## Smart Enum

```csharp
public class Color : Enumeration
{
    public static readonly Color Red  = new(1, "Red");
    public static readonly Color Blue = new(2, "Blue");
    private Color(int id, string name) : base(id, name) { }
}

var all = Enumeration.GetAll<Color>();
```

## Exception data

Fluent helpers for attaching context to thrown exceptions:

```csharp
throw new InvalidOperationException("save failed")
    .AddData("userId", 123)
    .AddData("action", "save");

// Later, when handling:
var data = ex.ToDictionary();
```

## URI (Tharga.Toolkit only)

```csharp
var uri = new Uri("https://example.com/path?page=1&sort=name");
var clean  = uri.RemoveQuery();           // https://example.com/path
var values = uri.GetQueryValue("sort");   // ["name"]
```
