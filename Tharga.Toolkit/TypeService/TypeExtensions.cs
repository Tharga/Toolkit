using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Toolkit.TypeService;

public static class TypeExtensions
{
    public static IServiceCollection AddAssemblyService(this IServiceCollection services)
    {
        services.AddSingleton<IAssemblyService, AssemblyService>();
        return services;
    }

    public static string ToAssemblyQualifiedNameWithoutVersion(this Type type)
    {
        var r = $"{type.FullName}, {type.Assembly.GetName().Name}";
        return r;
    }

    public static Type GetType(string typeName)
    {
        var type = GetTypeWithVersion(typeName, "1.0.0.0") ?? GetTypeWithVersion(typeName);
        if (type == null) throw new TypeMissingException(typeName);
        return type;
    }

    private static Type GetTypeWithVersion(string typeName, string version = null)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return null;
        }

        try
        {
            var tvn = ReplaceAssemblyVersion(typeName, version);

            var t = Type.GetType(tvn);
            return t;
        }
        catch (FileLoadException)
        {
            return null;
        }
    }

    public static string ReplaceAssemblyVersion(string typeName, string version)
    {
        var tvn = typeName;
        if (!string.IsNullOrEmpty(version))
        {
            var startIndex = 0;
            while (true)
            {
                var p1 = tvn.IndexOf("Version=", startIndex, StringComparison.Ordinal);
                if (p1 == -1) break;
                var p2 = tvn.IndexOf(",", p1, StringComparison.Ordinal);
                startIndex = p2;
                tvn = tvn.Substring(0, p1 + 8) + version + tvn.Substring(p2);
            }
        }

        return tvn;
    }

    public static Type GetGenericType(this Type item, Type baseType, int index)
    {
        if (item.IsGenericType) return item.GenericTypeArguments[index];

        foreach (var inf in item.GetInterfaces())
        {
            if (inf.IsGenericType)
            {
                return inf.GenericTypeArguments[index];
            }

            return inf.GetGenericType(baseType, index);
        }

        throw new InvalidOperationException($"Cannot find generic parameter of type {baseType.Name} from object of type {item.Name}.");
    }

    public static bool IsOfType(this Type item, Type type, bool includeInterfaceSearch = true)
    {
        if (item == null) return false;
        if (item == type) return true;
        if (item.ToString().Split('[')[0] == type.ToString().Split('[')[0]) return true;

        if (type.IsAssignableFrom(item))
        {
            return true;
        }

        if (item.BaseType != null && item.BaseType != typeof(object))
        {
            return item.BaseType.IsOfType(type);
        }

        if (includeInterfaceSearch)
        {
            foreach (var inf in item.GetInterfaces())
            {
                var r = inf.IsOfType(type);
                if (r) return true;
            }
        }

        return false;
    }

    public static bool HasGenericParameter(this TypeInfo type, Type aggregatorType)
    {
        var p = type;
        while (!p.IsGenericType)
        {
            var parent = p.ImplementedInterfaces.FirstOrDefault();
            if (parent == null)
            {
                parent = p.BaseType;
            }

            if (parent == null) return false;

            p = parent.GetTypeInfo();
        }

        if (!p.GenericTypeArguments.Any()) return false;
        return p.GenericTypeArguments[0] == aggregatorType;
    }
}