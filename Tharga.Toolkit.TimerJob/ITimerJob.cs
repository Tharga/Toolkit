using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.TimerJob;

public interface ITimerJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}