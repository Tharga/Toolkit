using System;

namespace Tharga.Toolkit.Timer
{
    /// <summary>
    /// Provides data for the <see cref="ManagedTimer.SkipEvent"/> event, raised when iterations are skipped due to execution exceeding the interval.
    /// </summary>
    public class SkipEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkipEventArgs"/> class.
        /// </summary>
        /// <param name="skippedIterations">The number of iterations that were skipped.</param>
        public SkipEventArgs(int skippedIterations)
        {
            SkippedIterations = skippedIterations;
        }

        /// <summary>
        /// Gets the number of iterations that were skipped.
        /// </summary>
        public int SkippedIterations { get; }
    }
}