using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cron;
using Microsoft.Extensions.Logging;
using Tharga.Toolkit.TypeService;

namespace Tharga.Toolkit.TimerJob;

internal class TimerJobService : ITimerJobService
{
    private readonly IAssemblyService _assemblyService;
    private readonly ILogger<TimerJobService> _logger;
    private readonly Func<TypeInfo, ITimerJob> _resolver;
    private readonly CronDaemon _cronDaemon = new();

    public TimerJobService(IAssemblyService assemblyService, ILogger<TimerJobService> logger, Func<TypeInfo, ITimerJob> resolver)
    {
        _assemblyService = assemblyService;
        _logger = logger;
        _resolver = resolver;

        assemblyService.LoadTypes(nameof(TimerJobService), x => x.IsOfType(typeof(ITimerJob)) && !x.IsInterface);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var type in _assemblyService.GetTypes(nameof(TimerJobService)))
        {
            var verify = _resolver.Invoke(type) as ITimerJob;
            if (verify == null) throw new InvalidOperationException($"Cannot resolve {type.Name} as implementing ITimerJob.");

            var expressions = verify.GetCronExpressions();
            if (!expressions.Any())
            {
                expressions = expressions.Union(new[] { "0 */1 * * *" }).ToArray(); //Default every hour
            }

            foreach (var expression in expressions)
            {
                _logger.LogDebug("Register timer job {timerJob} to run {expression}.", type.Name, expression);
                _cronDaemon.Add(expression, () =>
                {
                    if (_resolver.Invoke(type) is ITimerJob item)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                _logger.Log(LogLevel.Information, "Executing timer job {timerJob}.", type.Name);
                                await item.ExecuteAsync(cancellationToken);
                            }
                            catch (Exception e)
                            {
                                _logger.Log(LogLevel.Error, e, e.Message);
                            }
                        }, CancellationToken.None);
                    }
                });

                if (verify.RunOnStart())
                {
                    if (_resolver.Invoke(type) is ITimerJob item)
                    {
                        try
                        {
                            _logger.Log(LogLevel.Information, "Executing timer job {timerJob} on start.", type.Name);
                            await item.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception e)
                        {
                            _logger.Log(LogLevel.Error, e, e.Message);
                        }
                    }
                }
            }
        }

        _cronDaemon.Start();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cronDaemon.Stop();
        return Task.CompletedTask;
    }
}