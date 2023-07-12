using System;

namespace Tharga.Toolkit.Timer
{
    public class IntervalChangedEventArgs : EventArgs
    {
        public IntervalChangedEventArgs(TimeSpan oldInterval, TimeSpan interval)
        {
            OldInterval = oldInterval;
            Interval = interval;
        }

        public TimeSpan OldInterval { get; }
        public TimeSpan Interval { get; }
    }
}