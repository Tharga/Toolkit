# Feature: Test Coverage for Tharga.Toolkit

## Goal
Fill the remaining test gaps in the main Tharga.Toolkit project. Hash and ApiKeyService are well covered — this targets the 6 untested files.

## Scope

### Untested files (ordered by priority)

**High priority:**
1. **UriExtensions** — `RemoveQuery(Uri)`, `GetQueryValue(Uri, string)`
2. **Base32Encoding** — `Encode(byte[])`, `Decode(string)` (round-trip, edge cases)
3. **ClaimsExtensions** — `GetKey` (3 overloads), `VerifyKey` (2 overloads), `GetIdentities`, `GetIdentity`

**Medium priority:**
4. **EnumerableExtensionsAsync** — `TakeRandomAsync`, `RandomOrderAsync`
5. **IdentityKey** — constructor, Value property, record equality

**Lower priority:**
6. **ApiKeyServiceRegistration** — `RegisterApiKeyService` (DI registration test)

## Acceptance Criteria
- Every public method in the files above has at least one test
- Edge cases tested: null URIs, empty streams, missing claims
- All tests pass in Release configuration
- No existing tests broken

## Done Condition
All files listed above have tests, all tests pass.
