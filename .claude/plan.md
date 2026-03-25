# Plan: Documentation Improvements

## Phase 1: XML documentation comments

### Step 1: Tharga.Toolkit XML docs
- [x] HashExtensions, Hash, HashString — all public methods documented
- [x] HashFormat, HashType — already had docs
- [x] ClaimsExtensions, IdentityKey — all public methods documented
- [x] UriExtensions — all public methods documented
- [x] Base32Encoding — Encode, Decode documented
- [x] EnumerableExtensionsAsync — TakeRandomAsync, RandomOrderAsync documented
- [x] IApiKeyService — already had docs
- [x] ApiKeyOptions, ApiKeyServiceRegistration — documented

### Step 2: Tharga.Toolkit.Standard XML docs (extensions)
- [~] StringExtension
- [ ] DateTimeExtensions
- [ ] EnumerableExtensions
- [ ] EnumExtensions
- [ ] IntegerExtensions
- [ ] ByteSizeExtensions
- [ ] OrgNoExtensions
- [ ] ExceptionExtension (Logging)
- [ ] ClaimsExtensionsStandard

### Step 3: Tharga.Toolkit.Standard XML docs (classes)
- [ ] CompareExtensions, Diff, DifferentTypes, IDiff
- [ ] ManagedTimer, event args (Before/After/Skip/StateChanged/IntervalChanged), HiResDateTime
- [ ] Enumeration
- [ ] Luhn
- [ ] PasswordHasher
- [ ] ConcurrentTwoLevelDictionary
- [ ] ObservableConcurrentDictionary
- [ ] SemaphoreExecutor
- [ ] DurationOptions, DurationStringOptions, DurationStringOptionsExtensions
- [ ] TimeSpanStringOptions, TimeSpanStringOptionsExtensions, UnitOption
- [ ] EUnit, Language, ErrorType

## Phase 2: README improvements

### Step 4: Root README
- [ ] Project overview, installation, package choice guidance, links to sub-READMEs

### Step 5: Tharga.Toolkit/README.md
- [ ] Feature descriptions with code examples

### Step 6: Tharga.Toolkit.Standard/README.md
- [ ] Feature descriptions with code examples

## Phase 3: NuGet metadata

### Step 7: Update .csproj files
- [ ] PackageTags, PackageLicenseExpression, RepositoryUrl, RepositoryType

### Step 8: Final verification
- [ ] Build succeeds with no XML doc warnings, all tests pass
