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
- [~] Test `GetAll<T>`, `Equals`, `GetHashCode`, `CompareTo`, `ToString` using a concrete test subclass

### Step 6: EnumExtensions tests
- [ ] Test `MapEnum` — matching enums, mismatched names, collections

### Step 7: IntegerExtensions tests
- [ ] Test `GetNameForNumber` for 1-10 and default/out-of-range

### Step 8: ExceptionExtension (Logging) tests
- [ ] Test `AddData`, `TryAddData`, `ToDictionary` — adding data, duplicate keys, null values

### Step 9: ClaimsExtensionsStandard tests
- [ ] Test `GetIdentity`, `GetIdentities`, `GetEmail`, `GetEmailDomain`, `AddRoleForDomain`

### Step 10: ConcurrentTwoLevelDictionary tests
- [ ] Test `AddOrUpdate`, `TryGetSubDictonary`, `TryRemove` — add/update/remove, missing keys

### Step 11: ObservableConcurrentDictionary tests
- [ ] Test `Add`, `Remove`, `Clear`, `ContainsKey`, `TryGetValue`, indexer, event raising (CollectionChanged, PropertyChanged)

### Step 12: SemaphoreExecutor tests
- [ ] Test `ExecuteAsync` — sequential execution for same key, concurrent execution for different keys

### Step 13: ManagedTimer tests
- [ ] Test `Start`, `Stop`, `Interval`, state transitions, event firing (Before/After/Skip/StateChanged/IntervalChanged)

### Step 14: DurationStringOptionsExtensions & TimeSpanStringOptionsExtensions tests
- [ ] Test `Get(string culture)`, `Get(Language)` for en/sv cultures

### Step 15: EnumerableExtensions tests (verify gaps)
- [ ] Check what's already covered in Tharga.Toolkit.Tests, add tests for `IsNullOrEmpty`, `EmptyIfNull`, `RandomOrder` if missing

### Step 16: Final verification
- [ ] Run full test suite, verify all pass, commit
