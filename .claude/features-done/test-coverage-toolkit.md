# Feature: Test Coverage for Tharga.Toolkit

## Goal
Fill the remaining test gaps in the main Tharga.Toolkit project. Hash and ApiKeyService are well covered — this targets the 6 untested files.

## Originating Branch
develop

## Scope
Add tests to Tharga.Toolkit.Tests for 6 untested files: UriExtensions, Base32Encoding, ClaimsExtensions, EnumerableExtensionsAsync, IdentityKey, ApiKeyServiceRegistration.

## Acceptance Criteria
- Every public method in the 6 files has at least one test
- Edge cases tested: null URIs, empty byte arrays, missing claims
- All tests pass in Release configuration
- No existing tests broken

## Done Condition
All 6 files have tests, all tests pass, committed on feature branch.
