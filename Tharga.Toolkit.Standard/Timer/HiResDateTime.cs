using System;
using System.Threading;

namespace Tharga.Toolkit.Timer
{
    public static class HiResDateTime
    {
        private static long _lastTimeStamp = DateTime.UtcNow.Ticks;

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