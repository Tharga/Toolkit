using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tharga.Toolkit.TypeService;

public interface IAssemblyService
{
    /// <summary>
    /// Loads types into the cache.
    /// </summary>
    /// <param name="cacheKey">Unique key for the specific cache.</param>
    /// <param name="filter">Filter the types that should be loaded.</param>
    /// <param name="assemblies">Provide what assemblies should be used to find types. See 'GetAssemblies' in 'AssemblyService' for default behaviour.</param>
    void LoadTypes(string cacheKey, Func<TypeInfo, bool> filter, IEnumerable<Assembly> assemblies = null);

    /// <summary>
    /// Gets types from cache. If a filter is provided the cache can be populated on the first call.
    /// </summary>
    /// <param name="cacheKey">Unique key for the specific cache.</param>
    /// <param name="filter">Filter the types that should be loaded.</param>
    TypeInfo[] GetTypes(string cacheKey, Func<TypeInfo, bool> filter = null);
}