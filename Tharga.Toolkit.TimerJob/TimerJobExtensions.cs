using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tharga.Toolkit.TypeService;

namespace Tharga.Toolkit.TimerJob;

public static class TimerJobExtensions
{
    public static IServiceCollection AddTimerJobService(this IServiceCollection services, Func<TypeInfo, ITimerJob> resolver, IEnumerable<Assembly> assemblies = null)
    {
        services.AddAssemblyService();

        services.AddTransient<ITimerJobServiceManager, TimerJobServiceManager>(e =>
        {
            var timerJobService = e.GetService<ITimerJobService>();
            var logger = e.GetService<ILogger<TimerJobServiceManager>>();
            var hostApplicationLifetime = e.GetService<IHostApplicationLifetime>();
            return new TimerJobServiceManager(timerJobService, logger, hostApplicationLifetime?.ApplicationStarted ?? CancellationToken.None);
        });
        services.AddSingleton<ITimerJobService, TimerJobService>(e =>
        {
            var assemblyService = e.GetService<IAssemblyService>();
            var logger = e.GetService<ILogger<TimerJobService>>();
            return new TimerJobService(assemblyService, logger, resolver);
        });

        //var types = AssemblyService.GetCurrentDomainDefinedTypes(assemblies).Where(x => x.IsOfType(typeof(ITimerJob)) && !x.IsInterface).ToArray();
        var types = AssemblyService.GetTypes<ITimerJob>(x => !x.IsInterface).ToArray(); //TODO: Remove ToArray here
        foreach (var typeInfo in types)
        {
            services.AddSingleton(typeInfo.AsType());
        }

        return services;
    }

    public static IApplicationBuilder UseTimerJobService(this IApplicationBuilder app)
    {
        var timerJobService = app.ApplicationServices.GetService<ITimerJobServiceManager>();
        timerJobService?.Start();

        return app;
    }

    public static string[] GetCronExpressions(this ITimerJob item)
    {
        var r = item.GetType().GetCustomAttributes(typeof(TimerJobAttribute), true)
            .Select(x => (x as TimerJobAttribute)?.CronExpression)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();
        return r;
    }

    public static bool RunOnStart(this ITimerJob item)
    {
        var r = item.GetType().GetCustomAttributes(typeof(TimerJobAttribute), true)
            .Select(x => (x as TimerJobAttribute)?.RunOnStart)
            .Any(x => x == true);
        return r;
    }
}