using System;

namespace Tharga.Toolkit.Timer
{
    public class AfterExecuteEventArgs : EventArgs
    {
        public AfterExecuteEventArgs(TimeSpan elapsed, Exception exception, long iteration)
        {
            Elapsed = elapsed;
            Exception = exception;
            Iteration = iteration;
        }

        public TimeSpan Elapsed { get; }
        public Exception Exception { get; }
        public long Iteration { get; }
    }
}