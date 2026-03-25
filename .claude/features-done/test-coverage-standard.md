# Feature: Test Coverage for Tharga.Toolkit.Standard

## Goal
Bring Tharga.Toolkit.Standard from ~10% to comprehensive test coverage for all modules not already tested in Tharga.Toolkit.Tests.

## Originating Branch
develop

## Scope
Add tests to Tharga.Toolkit.Standard.Tests for 16 untested modules. Tests for Compare, DateTime, Duration, ListExtensions, and Assignment already exist in Tharga.Toolkit.Tests — those are out of scope.

## Acceptance Criteria
- Every public method in the untested modules has at least one test
- Edge cases tested: null inputs, empty collections, boundary values
- All tests pass in Release configuration (`dotnet test -c Release`)
- No existing tests broken

## Done Condition
All 16 modules have tests, all tests pass, committed on feature branch.
