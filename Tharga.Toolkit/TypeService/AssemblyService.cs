using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tharga.Toolkit.TypeService;

public class AssemblyService : IAssemblyService
{
    private static readonly ConcurrentDictionary<string, TypeInfo[]> _cache = new();

    internal AssemblyService()
    {
    }

    public void LoadTypes(string cacheKey, Func<TypeInfo, bool> filter, IEnumerable<Assembly> assemblies = null)
    {
        var data = GetTypes(filter, assemblies).ToArray();
        _cache.AddOrUpdate(cacheKey, data, (_, _) => data);
    }

    public TypeInfo[] GetTypes(string cacheKey, Func<TypeInfo, bool> filter = null)
    {
        if (_cache.TryGetValue(cacheKey, out var data))
        {
            return data;
        }

        if (filter != null)
        {
            LoadTypes(cacheKey, filter);
            if (_cache.TryGetValue(cacheKey, out data))
            {
                return data;
            }
        }

        throw new InvalidOperationException($"Cannot find any loaded types for '{cacheKey}'. Call {nameof(LoadTypes)} on startup or provide a {nameof(filter)}.");
    }

    /// <summary>
    /// Get all assemblies in the current app domain based on the first part of the namespace from the EntryAssembly or ExecutingAssembly.
    /// Example. 'Namespace [FirstPart].[SecondPart] will return all assemblies that starts with [FirstPart].'
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetAssemblies()
    {
        return GetAssemblies(null);
    }

    /// <summary>
    /// Use specific class to find the current assembly type.
    /// </summary>
    /// <typeparam name="TAssemblyType">Type for the assembly to be used as entry point to look for types.</typeparam>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetAssemblies<TAssemblyType>()
    {
        var current = Assembly.GetAssembly(typeof(TAssemblyType));
        return GetAssemblies(null, current);
    }

    /// <summary>
    /// Get all types based on the provided type and parameters.
    /// </summary>
    /// <typeparam name="TBaseType">Type, basetype of interface.</typeparam>
    /// <param name="filter">Filter for what types to return.</param>
    /// <param name="assemblies">Assemblies where to find the types. See 'GetAssemblies' for default behaviour.</param>
    /// <returns></returns>
    public static IEnumerable<TypeInfo> GetTypes<TBaseType>(Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null)
    {
        var assms = GetAssemblies(assemblies);
        var types = assms.SelectMany(x => x.DefinedTypes)
            .Where(x => x.IsOfType<TBaseType>())
            .Where(x => filter?.Invoke(x) ?? true);
        return types;
    }

    /// <summary>
    /// Get all types for provided parameters.
    /// </summary>
    /// <param name="filter">Filter for what types to return.</param>
    /// <param name="assemblies">Assemblies where to find the types. See 'GetAssemblies' for default behaviour.</param>
    /// <returns></returns>
    public static IEnumerable<TypeInfo> GetTypes(Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null)
    {
        var assms = GetAssemblies(assemblies);
        var types = assms.SelectMany(x => x.DefinedTypes)
            .Where(x => filter?.Invoke(x) ?? true);
        return types;
    }

    private static IEnumerable<Assembly> GetAssemblies(IEnumerable<Assembly> assemblies, Assembly current = null)
    {
        if (assemblies != null) return assemblies.ToArray();

        current ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var name = current.GetName().Name?.Split('.').First();

        var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => name != null && x.FullName != null && x.FullName.Contains(name))
            .ToArray();

        return new[] { current }.Union(appDomainAssemblies).ToArray();
    }
}