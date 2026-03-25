using System;

namespace Tharga.Toolkit.Timer
{
    /// <summary>
    /// Provides data for the <see cref="ManagedTimer.BeforeExecuteEvent"/> event, raised before a timer callback is executed.
    /// </summary>
    public class BeforeExecuteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeExecuteEventArgs"/> class.
        /// </summary>
        /// <param name="iteration">The iteration number of the upcoming execution.</param>
        public BeforeExecuteEventArgs(long iteration)
        {
            Iteration = iteration;
        }

        /// <summary>
        /// Gets the iteration number of the upcoming execution.
        /// </summary>
        public long Iteration { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the upcoming execution should be cancelled.
        /// </summary>
        public bool Cancel { get; set; } = false;
    }
}