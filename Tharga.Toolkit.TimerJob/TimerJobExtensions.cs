using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tharga.Toolkit.TypeService;

namespace Tharga.Toolkit.TimerJob;

public static class TimerJobExtensions
{
    public static IServiceCollection AddTimerJobService(this IServiceCollection services, Func<TypeInfo, ITimerJob> resolver = null, IEnumerable<Assembly> assemblies = null)
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
            return new TimerJobService(assemblyService, logger, (t) =>
            {
                var instance = resolver?.Invoke(t) ?? e.GetService(t.AsType());
                return instance as ITimerJob;
            });
        });

        //var types = AssemblyService.GetCurrentDomainDefinedTypes(assemblies).Where(x => x.IsOfType(typeof(ITimerJob)) && !x.IsInterface).ToArray();
        var types = AssemblyService.GetTypes<ITimerJob>(x => !x.IsInterface).ToArray(); //TODO: Remove ToArray here
        foreach (var typeInfo in types)
        {
            services.AddSingleton(typeInfo.AsType());
        }

        return services;
    }

    public static IHost UseTimerJobService(this IHost app)
    {
        var timerJobService = app.Services.GetService<ITimerJobServiceManager>();
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