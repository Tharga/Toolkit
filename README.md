# Tharga Toolkit

Find the lastest version of this package at [nuget.org](https://www.nuget.org/packages/Tharga.Toolkit).

## Assembly Service

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