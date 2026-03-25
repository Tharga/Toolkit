using System;
using System.Threading;

namespace Tharga.Toolkit.Timer
{
    /// <summary>
    /// Provides a high-resolution UTC timestamp that guarantees strictly increasing tick values across concurrent calls.
    /// </summary>
    public static class HiResDateTime
    {
        private static long _lastTimeStamp = DateTime.UtcNow.Ticks;

        /// <summary>
        /// Gets the current UTC time in ticks, guaranteed to be strictly greater than any previously returned value.
        /// </summary>
        public static long UtcNowTicks
        {
            get
            {
                long orig, newval;
                do
                {
                    orig = _lastTimeStamp;
                    var now = DateTime.UtcNow.Ticks;
                    newval = Math.Max(now, orig + 1);
                } while (Interlocked.CompareExchange(ref _lastTimeStamp, newval, orig) != orig);

                return newval;
            }
        }
    }
}