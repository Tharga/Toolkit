using System;

namespace Tharga.Toolkit.Timer
{
    public class SkipEventArgs : EventArgs
    {
        public SkipEventArgs(int skippedIterations)
        {
            SkippedIterations = skippedIterations;
        }

        public int SkippedIterations { get; }
    }
}