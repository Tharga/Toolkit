using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tharga.Toolkit.TypeService;

public interface IAssemblyService
{
    void LoadTypes(string cacheKey, Func<TypeInfo, bool> predicate, IEnumerable<Assembly> assemblies = null);
    TypeInfo[] GetTypes(string cacheKey, Func<TypeInfo, bool> predicate = null);
}