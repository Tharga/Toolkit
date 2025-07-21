﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tharga.Toolkit.TypeService;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds registration to the IOC by using a type filter.
    /// Abstract types and Interfaces are automatically filtered out for implementation.
    /// A good way is to use the filter in combination with 'IsOfType', then all types based on that type will be found.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="registrationType">Type of registration as Transient, Scoped or Singleton.</param>
    /// <param name="filter">The actual type filter. Ex. x => x.IsOfType(typeof(MyService))</param>
    /// <param name="assemblies">An optional list of assemblies of where to look for types. If not provided the baseAssembly, or default will be used.</param>
    /// <param name="baseAssembly">Provide a base assembly of where to start looking for types. By default, EntryAssembly or ExecutingAssembly will be used.</param>
    /// <param name="findInterface">If true, interface types will be automatically found based on a concrete type. If false, the concrete type is registered without an interface.</param>
    public static void Add(this IServiceCollection services, ERegistrationType registrationType, Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        AddByType(services, registrationType, filter, assemblies, baseAssembly, findInterface);
    }

    /// <summary>
    /// Adds registration to the IOC by using a type and optional filter.
    /// </summary>
    /// <typeparam name="TServiceBase"></typeparam>
    /// <param name="services"></param>
    /// <param name="registrationType">Type of registration as Transient, Scoped or Singleton.</param>
    /// <param name="filter">The actual type filter. Ex. x => x.IsOfType(typeof(MyService))</param>
    /// <param name="assemblies">An optional list of assemblies of where to look for types. If not provided the baseAssembly, or default will be used.</param>
    /// <param name="baseAssembly">Provide a base assembly of where to start looking for types. By default, EntryAssembly or ExecutingAssembly will be used.</param>
    /// <param name="findInterface">If true, interface types will be automatically found based on a concrete type. If false, the concrete type is registered without an interface.</param>
    public static void Add<TServiceBase>(this IServiceCollection services, ERegistrationType registrationType, Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        var composedFilter = new Func<TypeInfo, bool>(x => x.IsOfType<TServiceBase>() && (filter?.Invoke(x) ?? true));
        AddByType(services, registrationType, composedFilter, assemblies, baseAssembly, findInterface);
    }

    public static void AddTransient(this IServiceCollection services, Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        AddByType(services, ERegistrationType.Transient, filter, assemblies, baseAssembly, findInterface);
    }

    public static void AddTransient<TServiceBase>(this IServiceCollection services, Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        var composedFilter = new Func<TypeInfo, bool>(x => x.IsOfType<TServiceBase>() && (filter?.Invoke(x) ?? true));
        AddByType(services, ERegistrationType.Transient, composedFilter, assemblies, baseAssembly, findInterface);
    }

    public static void AddScoped(this IServiceCollection services, Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        AddByType(services, ERegistrationType.Scoped, filter, assemblies, baseAssembly, findInterface);
    }

    public static void AddScoped<TServiceBase>(this IServiceCollection services, Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        var composedFilter = new Func<TypeInfo, bool>(x => x.IsOfType<TServiceBase>() && (filter?.Invoke(x) ?? true));
        AddByType(services, ERegistrationType.Scoped, composedFilter, assemblies, baseAssembly, findInterface);
    }

    public static void AddSingleton(this IServiceCollection services, Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        AddByType(services, ERegistrationType.Singleton, filter, assemblies, baseAssembly, findInterface);
    }

    public static void AddSingleton<TServiceBase>(this IServiceCollection services, Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        var composedFilter = new Func<TypeInfo, bool>(x => x.IsOfType<TServiceBase>() && (filter?.Invoke(x) ?? true));
        AddByType(services, ERegistrationType.Singleton, composedFilter, assemblies, baseAssembly, findInterface);
    }

    public static IEnumerable<(Type ServiceType, Type ImplementationType)> GetServiceTypePairs<TServiceBase>(Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        var composedFilter = new Func<TypeInfo, bool>(x => x.IsOfType<TServiceBase>() && (filter?.Invoke(x) ?? true));
        return GetServiceTypePairs(composedFilter, assemblies, baseAssembly, findInterface);
    }

    public static IEnumerable<(Type ServiceType, Type ImplementationType)> GetServiceTypePairs(Func<TypeInfo, bool> filter, IEnumerable<Assembly> assemblies = null, Assembly baseAssembly = null, bool findInterface = true)
    {
        var assembliesArray = assemblies?.ToArray();

        var allTypes = AssemblyService.GetTypes(filter, assembliesArray, baseAssembly).ToArray();

        var types = new List<(Type ServiceType, Type ImplementationType)>();
        var typeInfos = allTypes.OrderByDescending(x => x.IsInterface).ToArray();
        foreach (var type in typeInfos)
        {
            if (type.IsInterface && findInterface)
            {
                var composedFilter = new Func<TypeInfo, bool>(x => !x.IsAbstract && !x.IsInterface && filter(x));
                var implementationTypes = AssemblyService.GetTypes(composedFilter, assembliesArray, baseAssembly).ToArray();
                foreach (var implementationType in implementationTypes.Where(x => x.IsInterfaceDirectlyImplemented(type)))
                {
                    types.AddRange(GetTypeWithInterface(implementationType));
                }
            }
            else if (!type.IsAbstract)
            {
                if (findInterface)
                {
                    types.AddRange(GetTypeWithInterface(type));
                }
                else
                {
                    types.Add((type.AsType(), type.AsType()));
                }
            }
        }

        return types.Distinct();
    }

    private static IEnumerable<(Type ServiceType, Type ImplementationType)> GetTypeWithInterface(TypeInfo implementationType)
    {
        var interfaces = implementationType.AsType().GetDirectlyImplementedInterfaces();
        if (interfaces.Length == 0)
        {
            yield return (implementationType.AsType(), implementationType.AsType());
        }
        else
        {
            foreach (var serviceType in interfaces)
            {
                yield return (serviceType, implementationType.AsType());
            }
        }
    }

    public static Type[] GetDirectlyImplementedInterfaces(this Type type) =>
        type.GetInterfaces()
            .Where(i => !type.GetInterfaces().SelectMany(x => x.GetInterfaces()).Contains(i))
            //.Where(i => !i.IsGenericType)
            .ToArray();

    public static bool IsInterfaceDirectlyImplemented(this Type type, Type typeInterface)
    {
        if (!typeInterface.IsInterface) throw new ArgumentException($"{typeInterface.FullName} is not an interface");

        var all = type.GetInterfaces();
        var inherited = new HashSet<Type>(all.SelectMany(i => i.GetInterfaces()));

        return all.Contains(typeInterface) && !inherited.Contains(typeInterface);
    }

    private static void AddByType(this IServiceCollection services, ERegistrationType registrationType, Func<TypeInfo, bool> filter, IEnumerable<Assembly> assemblies, Assembly baseAssembly, bool findInterface)
    {
        var types = GetServiceTypePairs(filter, assemblies, baseAssembly, findInterface);

        foreach (var type in types)
        {
            switch (registrationType)
            {
                case ERegistrationType.Transient:
                    services.AddTransient(type.ServiceType, type.ImplementationType);
                    break;
                case ERegistrationType.Scoped:
                    services.AddScoped(type.ServiceType, type.ImplementationType);
                    break;
                case ERegistrationType.Singleton:
                    services.AddSingleton(type.ServiceType, type.ImplementationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registrationType), $"Unknown {nameof(registrationType)} {registrationType}.", null);
            }
        }
    }
}