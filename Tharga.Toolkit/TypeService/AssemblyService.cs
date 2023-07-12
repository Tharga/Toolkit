using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tharga.Toolkit.TypeService;

internal class AssemblyService : IAssemblyService
{
    private static readonly ConcurrentDictionary<string, TypeInfo[]> Cache = new();

    public void LoadTypes(string cacheKey, Func<TypeInfo, bool> predicate, IEnumerable<Assembly> assemblies = null)
    {
        var data = GetCurrentDomainDefinedTypes(assemblies)
            .Where(x => x.FullName != null)
            .Where(predicate)
            .ToArray();

        Cache.AddOrUpdate(cacheKey, data, (_, _) => data);
    }

    public TypeInfo[] GetTypes(string cacheKey, Func<TypeInfo, bool> predicate = null)
    {
        if (Cache.TryGetValue(cacheKey, out var data))
        {
            return data;
        }

        if (predicate != null)
        {
            LoadTypes(cacheKey, predicate);
            if (Cache.TryGetValue(cacheKey, out data))
            {
                return data;
            }
        }

        throw new InvalidOperationException($"Cannot find any loaded types for '{cacheKey}'. Call LoadTypes on startup or provide a predicate.");
    }

    internal static Assembly[] GetAssemblies(IEnumerable<Assembly> assemblies)
    {
        if (assemblies != null) return assemblies.ToArray();

        var current = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var name = current.GetName().Name?.Split('.').First();

        var assms = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => name != null && x.FullName != null && x.FullName.Contains(name))
            .ToArray();

        return new[] { current }.Union(assms).ToArray();
    }

    internal static TypeInfo[] GetCurrentDomainDefinedTypes(IEnumerable<Assembly> assemblies)
    {
        var assms = GetAssemblies(assemblies);
        var types = assms.SelectMany(x => x.DefinedTypes).ToArray();
        return types;
    }

    internal static TypeInfo[] GetCurrentDomainDefinedTypes<T>(IEnumerable<Assembly> assemblies, bool includeInterfaces = false, bool includeAbstract = false)
    {
        var assms = GetAssemblies(assemblies);
        var types = assms.SelectMany(x => x.DefinedTypes)
            .Where(x => x.IsOfType(typeof(T)))
            .Where(x =>
            {
                if (!x.IsInterface && !x.IsAbstract)
                {
                    return true;
                }

                if (includeInterfaces && includeAbstract)
                {
                    return true;
                }

                if (includeInterfaces && x.IsInterface)
                {
                    return true;
                }

                if (includeAbstract && x.IsAbstract && !x.IsInterface)
                {
                    return true;
                }

                return false;
            })
            .ToArray();
        return types;
    }
}