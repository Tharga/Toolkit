# Tharga Toolkit

This project contains the following nuget packages that can be downloaded from nuget.org.
- [Tharga.Toolkit.Standard](https://www.nuget.org/packages/Tharga.Toolkit.Standard)
- [Tharga.Toolkit](https://www.nuget.org/packages/Tharga.Toolkit)
- [Tharga.Test.Toolkit](https://www.nuget.org/packages/Tharga.Test.Toolkit)

# Toolkit Standard

Some features that are yet to be documented.
- Compare
- ManagedTimer
- DateTimeExtensions
- ListExtensions
- HashExtensions
- EnumExtensions
- Enumeration
- Luhn

# Toolkit
Contains everything in [Toolkit Standard](#toolkitstandard) together with the following...

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

Some features that are yet to be documented.
- AssignmentExtension
- DependencyTest