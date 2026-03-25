# Plan: Test Coverage for Tharga.Toolkit.Standard

## Steps

### Step 1: StringExtension tests
- [x] Test `NullIfEmpty`, `IsNullOrEmpty`, `IfEmpty`, `RandomString`, `Random`, `GetRandomString`, `ToBase64`, `FromBase64`, `Truncate` — 23 tests, all passing

### Step 2: PasswordHasher tests
- [x] Test `HashPassword`, `VerifyPassword` (round-trip, wrong password, different salt sizes) — 7 tests, all passing

### Step 3: OrgNoExtensions tests
- [x] Test `TryParseOrgNo` — valid org numbers, invalid format, invalid check digit, empty/null — 7 tests, all passing

### Step 4: ByteSizeExtensions tests
- [x] Test `ToReadableByteSize(int)`, `ToReadableByteSize(long)` — bytes, KB, MB, GB, TB, with/without full unit names, decimal places — 11 tests, all passing

### Step 5: Enumeration tests
- [x] Test `GetAll<T>`, `Equals`, `GetHashCode`, `CompareTo`, `ToString` using a concrete test subclass — 8 tests, all passing

### Step 6: EnumExtensions tests
- [x] Test `MapEnum` — matching enums, mismatched names, collections — 4 tests, all passing

### Step 7: IntegerExtensions tests
- [x] Test `GetNameForNumber` for 1-10 and default/out-of-range — 13 tests (Theory), all passing

### Step 8: ExceptionExtension (Logging) tests
- [x] Test `AddData`, `TryAddData`, `ToDictionary` — adding data, duplicate keys, non-string keys — 8 tests, all passing

### Step 9: ClaimsExtensionsStandard tests
- [x] Test `GetIdentity`, `GetIdentities`, `GetEmail`, `GetEmailDomain`, `AddRoleForDomain` — 21 tests, all passing

### Step 10: ConcurrentTwoLevelDictionary tests
- [x] Test `AddOrUpdate`, `TryGetSubDictonary`, `TryRemove` — add/update/remove, missing keys — 7 tests, all passing

### Step 11: ObservableConcurrentDictionary tests
- [x] Test `Add`, `Remove`, `Clear`, `ContainsKey`, `TryGetValue`, indexer, event raising (CollectionChanged, PropertyChanged) — 15 tests, all passing

### Step 12: SemaphoreExecutor tests
- [x] Test `ExecuteAsync` — sequential execution for same key, concurrent execution for different keys — 3 tests, all passing

### Step 13: ManagedTimer tests
- [x] Test `Start`, `Stop`, `Interval`, state transitions, event firing (Before/After/Skip/StateChanged/IntervalChanged) — 14 tests, all passing

### Step 14: DurationStringOptionsExtensions & TimeSpanStringOptionsExtensions tests
- [x] Test `Get(string culture)`, `Get(Language)` for en/sv cultures — 10 tests, all passing

### Step 15: EnumerableExtensions tests (verify gaps)
- [x] Verified `IsNullOrEmpty`, `EmptyIfNull`, `RandomOrder`, `TakeRandom` not tested anywhere — added 9 tests, all passing

### Step 16: Final verification
- [x] Full test suite: 437 tests (273 Toolkit + 164 Standard), all passing, 0 failures
