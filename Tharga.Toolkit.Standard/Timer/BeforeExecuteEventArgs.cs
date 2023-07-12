using System;

namespace Tharga.Toolkit.Timer
{
    public class BeforeExecuteEventArgs : EventArgs
    {
        public BeforeExecuteEventArgs(long iteration)
        {
            Iteration = iteration;
        }

        public long Iteration { get; }
        public bool Cancel { get; set; } = false;
    }
}