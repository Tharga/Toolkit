using System;

namespace Tharga.Toolkit.Timer
{
    /// <summary>
    /// Provides data for the <see cref="ManagedTimer.IntervalChangedEvent"/> event, raised when the timer interval changes.
    /// </summary>
    public class IntervalChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldInterval">The previous interval value.</param>
        /// <param name="interval">The new interval value.</param>
        public IntervalChangedEventArgs(TimeSpan oldInterval, TimeSpan interval)
        {
            OldInterval = oldInterval;
            Interval = interval;
        }

        /// <summary>
        /// Gets the previous interval value.
        /// </summary>
        public TimeSpan OldInterval { get; }
        /// <summary>
        /// Gets the new interval value.
        /// </summary>
        public TimeSpan Interval { get; }
    }
}