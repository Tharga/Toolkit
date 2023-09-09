using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Tharga.Toolkit.TimerJob;

internal class TimerJobServiceManager : ITimerJobServiceManager
{
    private readonly ITimerJobService _timerJobService;
    private readonly ILogger<TimerJobServiceManager> _logger;
    private readonly CancellationToken _cancellationToken;

    public TimerJobServiceManager(ITimerJobService timerJobService, ILogger<TimerJobServiceManager> logger, CancellationToken cancellationToken)
    {
        _timerJobService = timerJobService;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            try
            {
                await _timerJobService.StartAsync(_cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }, _cancellationToken);
    }
}