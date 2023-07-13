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
    /// <param name="predicate">Filter the types that should be loaded.</param>
    /// <param name="assemblies">Provide what assemblies should be used to find types. By default all assemblies in the current app domain with the first part of the assembly name is used.</param>
    void LoadTypes(string cacheKey, Func<TypeInfo, bool> predicate, IEnumerable<Assembly> assemblies = null);

    /// <summary>
    /// Gets types from cache. If a predicate is provided the cache can be populated on the first call.
    /// </summary>
    /// <param name="cacheKey">Unique key for the specific cache.</param>
    /// <param name="predicate">Filter the types that should be loaded.</param>
    TypeInfo[] GetTypes(string cacheKey, Func<TypeInfo, bool> predicate = null);
}