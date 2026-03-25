# Feature: Test Coverage for Tharga.Toolkit.Standard

## Goal
Bring Tharga.Toolkit.Standard from ~10% to comprehensive test coverage. Many Standard features are already tested in Tharga.Toolkit.Tests (Compare, DateTime, Duration, ListExtensions, Assignment) — this feature covers the remaining untested modules.

## Scope

### Untested modules (ordered by priority)

**High priority (core utilities):**
1. **StringExtension** — `NullIfEmpty`, `IsNullOrEmpty`, `IfEmpty`, `RandomString`, `Random`, `GetRandomString`, `ToBase64`, `FromBase64`, `Truncate`
2. **PasswordHasher** — `HashPassword`, `VerifyPassword`
3. **OrgNoExtensions** — `TryParseOrgNo` (2 overloads, valid/invalid/check digit scenarios)
4. **EnumerableExtensions** — `TakeRandom`, `RandomOrder`, `TakeAllButFirst`, `TakeAllButLast`, `TakeChunks`, `IsNullOrEmpty`, `EmptyIfNull` (note: some of these are tested via Tharga.Toolkit.Tests — verify before duplicating)
5. **ByteSizeExtensions** — `ToReadableByteSize(int)`, `ToReadableByteSize(long)`

**Medium priority (framework classes):**
6. **Enumeration** — `GetAll<T>`, `Equals`, `GetHashCode`, `CompareTo`, `ToString`
7. **EnumExtensions** — `MapEnum` (2 overloads)
8. **IntegerExtensions** — `GetNameForNumber`
9. **ExceptionExtension** — `AddData`, `TryAddData`, `ToDictionary`
10. **ClaimsExtensionsStandard** — `GetIdentity` (3 overloads), `GetIdentities`, `GetEmail` (3), `GetEmailDomain` (3), `AddRoleForDomain`

**Lower priority (complex/async):**
11. **ConcurrentTwoLevelDictionary** — `AddOrUpdate`, `TryGetSubDictonary`, `TryRemove`
12. **ObservableConcurrentDictionary** — `Add`, `Remove`, `Clear`, `ContainsKey`, `TryGetValue`, indexer, event raising
13. **SemaphoreExecutor** — `ExecuteAsync` with concurrent and sequential key execution
14. **ManagedTimer** — `Start`, `Stop`, `Interval`, state changes, events (Before/After/Skip/StateChanged/IntervalChanged)
15. **DurationStringOptionsExtensions** — `Get(string)`, `Get(Language)`
16. **TimeSpanStringOptionsExtensions** — `Get(string)`

## Acceptance Criteria
- Every public method in the modules above has at least one test
- Edge cases tested: null inputs, empty collections, boundary values
- All tests pass in Release configuration
- No existing tests broken

## Done Condition
All modules listed above have tests, all tests pass.
