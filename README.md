# Tharga Toolkit
Contains a *.NET* version and a *Standard* version.

[![GitHub repo Issues](https://img.shields.io/github/issues/Tharga/Toolkit?style=flat&logo=github&logoColor=red&label=Issues)](https://github.com/Tharga/Toolkit/issues?q=is%3Aopen)

## .NET
[![NuGet](https://img.shields.io/nuget/v/Tharga.Toolkit)](https://www.nuget.org/packages/Tharga.Toolkit)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Toolkit)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

### Features
- All features from the standard package
- Assembly and TypeService Service

## Standard
[![NuGet](https://img.shields.io/nuget/v/Tharga.Toolkit.Standard)](https://www.nuget.org/packages/Tharga.Toolkit.Standard)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Toolkit.Standard)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

### Features
- Compare
- ManagedTimer
- DateTimeExtensions
- ListExtensions
- HashExtensions
- EnumExtensions
- Enumeration
- Luhn

## Assembly and TypeService Service

Register the service
```
public void ConfigureServices(IServiceCollection services)
{
	services.AddAssemblyService();
}
```

When `GetTypes` is called the first time, data is stored in cache. All other calls will use that cache.
```
assemblyService.GetTypes("CacheKey", x => x.IsOfType(typeof([SomeType]), false) && !x.IsAbstract);
```

Preload cache by calling `LoadTypes`.
```
assemblyService.LoadTypes("CacheKey", x => x.IsOfType(typeof([SomeType]), false) && !x.IsAbstract);
```

# Test Toolkit
[![NuGet](https://img.shields.io/nuget/v/Tharga.Test.Toolkit)](https://www.nuget.org/packages/Tharga.Test.Toolkit)
![Nuget](https://img.shields.io/nuget/dt/Tharga.Test.Toolkit)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

### Features
- AssignmentExtension
- DependencyTest