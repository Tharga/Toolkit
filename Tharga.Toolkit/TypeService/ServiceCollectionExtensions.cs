using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Toolkit.TypeService;

public static class ServiceCollectionExtensions
{
    internal enum ERegistrationType { Transient, Scoped, Singleton }

    public static void AddTransientByType<T>(this IServiceCollection services, Assembly assembly)
    {
        AddByType<T>(services, new[] { assembly }, ERegistrationType.Transient);
    }

    public static void AddTransientByType<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        AddByType<T>(services, assemblies.ToArray(), ERegistrationType.Transient);
    }

    public static void AddScopedByType<T>(this IServiceCollection services, Assembly assembly)
    {
        AddByType<T>(services, new[] { assembly }, ERegistrationType.Scoped);
    }

    public static void AddScopedByType<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        AddByType<T>(services, assemblies.ToArray(), ERegistrationType.Scoped);
    }

    public static void AddSingletonByType<T>(this IServiceCollection services, Assembly assembly)
    {
        AddByType<T>(services, new[] { assembly }, ERegistrationType.Singleton);
    }

    public static void AddSingletonByType<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        AddByType<T>(services, assemblies.ToArray(), ERegistrationType.Singleton);
    }

    internal static void AddByType<T>(this IServiceCollection services, Assembly[] assemblies, ERegistrationType registrationType, Action<Type, Type> beforeRegistration = null, Action<Type, Type> afterRegistration = null)
    {
        var types = AssemblyService.GetTypes<T>(x => !x.IsGenericType && !x.IsInterface, assemblies).ToArray();
        foreach (var type in types)
        {
            var serviceTypes = type.ImplementedInterfaces.Where(x => x.IsInterface && !x.IsGenericType && x != typeof(T)).ToArray();
            if (serviceTypes.Length > 1) throw new InvalidOperationException($"There are {serviceTypes.Length} interfaces for repository type '{type.Name}' ({string.Join(", ", serviceTypes.Select(x => x.Name))}).");
            var implementationType = type.AsType();
            var serviceType = serviceTypes.Length == 0 ? implementationType : serviceTypes.Single();

            beforeRegistration?.Invoke(serviceType, implementationType);

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

            afterRegistration?.Invoke(serviceType, implementationType);
        }
    }
}