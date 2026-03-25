using System;

namespace Tharga.Toolkit.Timer
{
    /// <summary>
    /// Provides data for the <see cref="ManagedTimer.StateChangedEvent"/> event, raised when the timer state changes.
    /// </summary>
    public class StateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldState">The previous timer state.</param>
        /// <param name="state">The new timer state.</param>
        public StateChangedEventArgs(ManagedTimer.TimerState oldState, ManagedTimer.TimerState state)
        {
            OldState = oldState;
            State = state;
        }

        /// <summary>
        /// Gets the previous timer state.
        /// </summary>
        public ManagedTimer.TimerState OldState { get; }
        /// <summary>
        /// Gets the new timer state.
        /// </summary>
        public ManagedTimer.TimerState State { get; }
    }
}