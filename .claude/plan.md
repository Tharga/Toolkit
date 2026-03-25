# Plan: Test Coverage for Tharga.Toolkit

## Steps

### Step 1: UriExtensions tests
- [x] Test `RemoveQuery` — URI with query, without query
- [x] Test `GetQueryValue` — single value, multiple values, missing name, encoded values, no query — 7 tests

### Step 2: Base32Encoding tests
- [x] Test `Encode` — known RFC 4648 vectors, empty/null
- [x] Test `Decode` — known vectors, empty/null, invalid chars, case insensitivity
- [x] Round-trip encoding — 11 tests

### Step 3: ClaimsExtensions tests
- [x] Test `GetKey` — from ClaimsPrincipal, ClaimsIdentity, IEnumerable<Claim>, null, no matching claims
- [x] Test `VerifyKey` — matching identity, wrong identity, with/without type, tuple overload, base64 match
- [x] Test `GetIdentities` and `GetIdentity` from IdentityKey — 15 tests

### Step 4: EnumerableExtensionsAsync tests
- [x] Test `TakeRandomAsync` — element from sequence, empty returns default, single element
- [x] Test `RandomOrderAsync` — returns all elements, empty returns empty — 5 tests

### Step 5: IdentityKey tests
- [x] Test constructor, Value property, record equality, GetHashCode — 4 tests

### Step 6: ApiKeyServiceRegistration tests
- [x] Test `RegisterApiKeyService` — registers IApiKeyService, applies custom options — 2 tests

### Step 7: Final verification
- [x] Full test suite: 481 tests (317 Toolkit + 164 Standard), all passing, 0 failures
