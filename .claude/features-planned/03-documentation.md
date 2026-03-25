# Feature: Documentation Improvements

## Goal
Improve documentation so users can understand and use the toolkit without reading source code. Currently only 18% of source files have XML doc comments and READMEs are just feature lists.

## Scope

### Phase 1: XML documentation comments
Add `/// <summary>` to all public classes and methods in both projects. Priority order:
1. **Tharga.Toolkit** — HashExtensions, ClaimsExtensions, UriExtensions, Base32Encoding, EnumerableExtensionsAsync, ApiKeyService/IApiKeyService
2. **Tharga.Toolkit.Standard** — StringExtension, DateTimeExtensions, EnumerableExtensions, CompareExtensions, ManagedTimer, Luhn, PasswordHasher, Enumeration, ConcurrentTwoLevelDictionary, ObservableConcurrentDictionary, SemaphoreExecutor, OrgNoExtensions, ByteSizeExtensions, ExceptionExtension, EnumExtensions, IntegerExtensions

### Phase 2: README improvements
Update the three README.md files:
1. **Root README** — project overview, installation (NuGet), which package to choose (.NET vs Standard), link to sub-project READMEs
2. **Tharga.Toolkit/README.md** — feature descriptions with code examples for Hash, ApiKey, Claims, Uri, Base32
3. **Tharga.Toolkit.Standard/README.md** — feature descriptions with code examples for String, DateTime, Compare, Timer, Collections, Luhn, OrgNo, Password

### Phase 3: NuGet metadata
- Add `PackageTags` to both .csproj files
- Add `PackageLicenseExpression` (MIT)
- Add `RepositoryUrl` and `RepositoryType`

## Acceptance Criteria
- All public classes and methods have XML doc comments
- READMEs include installation instructions, feature overview, and code examples
- NuGet metadata is complete
- Project builds without warnings related to missing XML docs

## Done Condition
All three phases complete, build succeeds in Release.
