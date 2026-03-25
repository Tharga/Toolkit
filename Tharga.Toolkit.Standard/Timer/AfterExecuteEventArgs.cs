using System;

namespace Tharga.Toolkit.Timer
{
    /// <summary>
    /// Provides data for the <see cref="ManagedTimer.AfterExecuteEvent"/> event, raised after a timer callback has executed.
    /// </summary>
    public class AfterExecuteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterExecuteEventArgs"/> class.
        /// </summary>
        /// <param name="elapsed">The time elapsed during the callback execution.</param>
        /// <param name="exception">The exception thrown during execution, or null if no exception occurred.</param>
        /// <param name="iteration">The iteration number of the execution.</param>
        public AfterExecuteEventArgs(TimeSpan elapsed, Exception exception, long iteration)
        {
            Elapsed = elapsed;
            Exception = exception;
            Iteration = iteration;
        }

        /// <summary>
        /// Gets the time elapsed during the callback execution.
        /// </summary>
        public TimeSpan Elapsed { get; }
        /// <summary>
        /// Gets the exception thrown during execution, or null if no exception occurred.
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// Gets the iteration number of the execution.
        /// </summary>
        public long Iteration { get; }
    }
}