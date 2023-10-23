using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Toolkit.TypeService;

public static class ServiceCollectionExtensions
{
    private enum ERegistrationType { Transient, Scoped, Singleton }

    public static void AddTransientByType<T>(this IServiceCollection services, Assembly assembly, bool findInterface = true)
    {
        AddByType<T>(services, new[] { assembly }, ERegistrationType.Transient, findInterface);
    }

    public static void AddTransientByType<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies, bool findInterface = true)
    {
        AddByType<T>(services, assemblies.ToArray(), ERegistrationType.Transient, findInterface);
    }

    public static void AddScopedByType<T>(this IServiceCollection services, Assembly assembly, bool findInterface = true)
    {
        AddByType<T>(services, new[] { assembly }, ERegistrationType.Scoped, findInterface);
    }

    public static void AddScopedByType<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies, bool findInterface = true)
    {
        AddByType<T>(services, assemblies.ToArray(), ERegistrationType.Scoped, findInterface);
    }

    public static void AddSingletonByType<T>(this IServiceCollection services, Assembly assembly, bool findInterface = true)
    {
        AddByType<T>(services, new[] { assembly }, ERegistrationType.Singleton, findInterface);
    }

    public static void AddSingletonByType<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies, bool findInterface = true)
    {
        AddByType<T>(services, assemblies.ToArray(), ERegistrationType.Singleton, findInterface);
    }

    private static void AddByType<T>(this IServiceCollection services, Assembly[] assemblies, ERegistrationType registrationType, bool findInterface)
    {
        var types = AssemblyService.GetTypes<T>(x => !x.IsGenericType && !x.IsInterface, assemblies).ToArray();
        foreach (var type in types)
        {
            var implementationType = type.AsType();

            Type serviceType;
            if (findInterface)
            {
                var serviceTypes = type.ImplementedInterfaces.Where(x => x.IsInterface && !x.IsGenericType && x != typeof(T)).ToArray();
                if (serviceTypes.Length > 1) throw new InvalidOperationException($"There are {serviceTypes.Length} interfaces for repository type '{type.Name}' ({string.Join(", ", serviceTypes.Select(x => x.Name))}).");
                serviceType = serviceTypes.Length == 0 ? implementationType : serviceTypes.Single();
            }
            else
            {
                serviceType = type;
            }

            switch (registrationType)
            {
                case ERegistrationType.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
                case ERegistrationType.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;
                case ERegistrationType.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registrationType), registrationType, null);
            }
        }
    }
}