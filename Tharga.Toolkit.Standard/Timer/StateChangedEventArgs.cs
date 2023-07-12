using System;

namespace Tharga.Toolkit.Timer
{
    public class StateChangedEventArgs : EventArgs
    {
        public StateChangedEventArgs(ManagedTimer.TimerState oldState, ManagedTimer.TimerState state)
        {
            OldState = oldState;
            State = state;
        }

        public ManagedTimer.TimerState OldState { get; }
        public ManagedTimer.TimerState State { get; }
    }
}