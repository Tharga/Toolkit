using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Timer
{
    /// <summary>
    /// A managed timer that supports async callbacks, automatic interval correction, iteration tracking, and skip detection.
    /// </summary>
    public class ManagedTimer
    {
        /// <summary>
        /// Represents the possible states of a <see cref="ManagedTimer"/>.
        /// </summary>
        public enum TimerState
        {
            /// <summary>The timer is stopped.</summary>
            Stopped,
            /// <summary>The timer has started.</summary>
            Started,
            /// <summary>The timer callback is currently executing.</summary>
            Executing,
            /// <summary>The timer callback has finished executing.</summary>
            ExecuteComplete,
            /// <summary>The timer is waiting for the next interval.</summary>
            Waiting
        }
        /// <summary>
        /// Specifies the timing mode for the managed timer.
        /// </summary>
        public enum Mode
        {
            /// <summary>Attempts to fire at exact intervals from the start time, compensating for execution duration.</summary>
            OnTime,
            /// <summary>Waits the full interval between the end of one execution and the start of the next.</summary>
            Between
        }

        private readonly object _syncRoot = new object();
        private readonly Func<long, Task> _elapsedAsync;
        private readonly Mode _mode;
        private readonly bool _autoReset;

        // _running, _stopRequested, _restartRequested and _cancellationTokenSource
        // are read and written under _syncRoot only.
        private bool _running;
        private bool _stopRequested;
        private bool _restartRequested;
        private TimeSpan _interval;
        private CancellationTokenSource _cancellationTokenSource;
        private long _iteration;
        private long _startTime;
        private long _nextDueTime;

        /// <summary>
        /// Gets the current state of the timer.
        /// </summary>
        public TimerState State { get; private set; } = TimerState.Stopped;

        /// <summary>
        /// Occurs when the timer interval is changed.
        /// </summary>
        public event EventHandler<IntervalChangedEventArgs> IntervalChangedEvent;
        /// <summary>
        /// Occurs before the timer callback is executed.
        /// </summary>
        public event EventHandler<BeforeExecuteEventArgs> BeforeExecuteEvent;
        /// <summary>
        /// Occurs after the timer callback has executed, with any exception the callback threw.
        /// Also occurs when an exception thrown by an event handler stops the timer; <see cref="AfterExecuteEventArgs.Exception"/> then holds that exception.
        /// </summary>
        public event EventHandler<AfterExecuteEventArgs> AfterExecuteEvent;
        /// <summary>
        /// Occurs when the timer state changes.
        /// </summary>
        public event EventHandler<StateChangedEventArgs> StateChangedEvent;
        /// <summary>
        /// Occurs when one or more iterations are skipped because execution took longer than the interval.
        /// </summary>
        public event EventHandler<SkipEventArgs> SkipEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedTimer"/> class.
        /// </summary>
        /// <param name="timeSpan">The interval between timer executions.</param>
        /// <param name="elapsedAsync">The async callback to invoke on each timer tick, receiving the current iteration number.</param>
        /// <param name="mode">The timing mode to use.</param>
        /// <param name="autoStart">Whether to start the timer immediately upon construction.</param>
        /// <param name="autoReset">Whether the timer should automatically repeat after each execution.</param>
        public ManagedTimer(TimeSpan timeSpan, Func<long, Task> elapsedAsync, Mode mode = Mode.OnTime, bool autoStart = false, bool autoReset = true)
        {
            if (timeSpan.Ticks < 0) throw new ArgumentException("Interval needs to be larger than zero.");
            if (elapsedAsync == null) throw new ArgumentNullException(nameof(elapsedAsync));

            _interval = timeSpan;
            _elapsedAsync = elapsedAsync;
            _mode = mode;
            _autoReset = autoReset;
            if (autoStart) Start();
        }

        /// <summary>
        /// Gets or sets the interval between timer executions. Raises <see cref="IntervalChangedEvent"/> when changed.
        /// </summary>
        public TimeSpan Interval
        {
            get => _interval;
            set
            {
                if (_interval == value) return;
                var oldInterval = _interval;
                _interval = value;
                IntervalChangedEvent?.Invoke(this, new IntervalChangedEventArgs(oldInterval, value));
            }
        }

        private void TimerEngine()
        {
            CancellationTokenSource cancellationTokenSource;
            lock (_syncRoot)
            {
                if (_running)
                {
                    // A run that has been stopped but has not ended yet starts again
                    // as soon as it ends, so a Start() that follows a Stop() is not lost.
                    if (_stopRequested) _restartRequested = true;
                    return;
                }

                cancellationTokenSource = BeginRun();
            }

            StartRun(cancellationTokenSource.Token);
        }

        // Called with _syncRoot held. The source is created here, before the run's
        // task starts, so a Stop() straight after Start() has something to cancel.
        private CancellationTokenSource BeginRun()
        {
            _running = true;
            _stopRequested = false;
            _restartRequested = false;
            _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource;
        }

        private void StartRun(CancellationToken cancellationToken)
        {
            // RunAsync handles every exception itself, so nothing needs to observe this task.
            Task.Run(() => RunAsync(cancellationToken));
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var lapTime = HiResDateTime.UtcNowTicks;
            try
            {
                SetState(TimerState.Started);

                // After a restart, wait out what is left of the wait that Stop() cut
                // short, so the next execution is never earlier than it was due.
                var remaining = new TimeSpan(_nextDueTime - HiResDateTime.UtcNowTicks);
                if (remaining.Ticks > 0)
                {
                    await Task.Delay(remaining, cancellationToken);
                }

                _iteration = 0;
                _startTime = HiResDateTime.UtcNowTicks;

                while (!cancellationToken.IsCancellationRequested)
                {
                    lapTime = HiResDateTime.UtcNowTicks;

                    var args = new BeforeExecuteEventArgs(_iteration);
                    BeforeExecuteEvent?.Invoke(this, args);

                    if (!args.Cancel)
                    {
                        Exception exception = null;
                        try
                        {
                            SetState(TimerState.Executing);
                            await _elapsedAsync(_iteration);
                            SetState(TimerState.ExecuteComplete);
                        }
                        catch (Exception e)
                        {
                            exception = e;
                        }

                        AfterExecuteEvent?.Invoke(this, new AfterExecuteEventArgs(GetElapsed(lapTime), exception, _iteration));
                    }

                    TimeSpan iv;
                    if (_mode == Mode.OnTime)
                    {
                        var vv = new TimeSpan((HiResDateTime.UtcNowTicks - _startTime) - (_iteration * Interval.Ticks));
                        iv = Interval - vv;
                    }
                    else
                    {
                        iv = Interval;
                    }

                    //NOTE: Calculate how many iterations to skip
                    var skip = 0;
                    while (iv.Ticks <= 0)
                    {
                        _iteration += 1;
                        var vv = new TimeSpan((HiResDateTime.UtcNowTicks - _startTime) - (_iteration * Interval.Ticks));
                        iv = Interval - vv;
                        skip++;
                    }

                    if (skip > 0)
                    {
                        SkipEvent?.Invoke(this, new SkipEventArgs(skip));
                    }

                    // Recorded before the wait starts, so a restart after a Stop() can honour it.
                    _nextDueTime = HiResDateTime.UtcNowTicks + iv.Ticks;
                    SetState(TimerState.Waiting);

                    //var now = new DateTime(HiResDateTime.UtcNowTicks);
                    //var corr = new TimeSpan(HiResDateTime.UtcNowTicks - _startTime).TotalMilliseconds;
                    //Console.WriteLine($"{_iteration} {now:HH:mm:ss fff}: wait:{iv.TotalMilliseconds:0}ms corr:{corr:0}ms");

                    await Task.Delay(iv, cancellationToken);

                    _iteration++;

                    if (!_autoReset) return;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Stop() cancelled a wait. _nextDueTime still holds when the next execution was due.
            }
            catch (Exception e)
            {
                // An exception from an event handler (the callback's own exceptions are caught
                // above) ends the run. It is reported instead of being lost in this task.
                OnRunFailed(e, GetElapsed(lapTime));
            }
            finally
            {
                EndRun();
            }
        }

        private void EndRun()
        {
            CancellationTokenSource next = null;

            // Written under the same lock that TimerEngine's test-and-set guard uses,
            // so a Start() on another thread either sees that the run has ended, or
            // asks for a restart that is picked up here. The lock is only ever held
            // for these short sections, never while user code runs, so this cannot deadlock.
            lock (_syncRoot)
            {
                if (_restartRequested)
                {
                    next = BeginRun();
                }
                else
                {
                    _running = false;
                    _stopRequested = false;
                    _cancellationTokenSource = null;
                }
            }

            try
            {
                SetState(TimerState.Stopped);
            }
            catch (Exception e)
            {
                OnRunFailed(e, TimeSpan.Zero);
            }

            if (next != null) StartRun(next.Token);
        }

        // Reports an error that ended a run through AfterExecuteEvent, the event that
        // already carries the callback's errors.
        private void OnRunFailed(Exception exception, TimeSpan elapsed)
        {
            try
            {
                var handler = AfterExecuteEvent;
                if (handler != null)
                {
                    handler.Invoke(this, new AfterExecuteEventArgs(elapsed, exception, _iteration));
                    return;
                }
            }
            catch (Exception e)
            {
                exception = new AggregateException(exception, e);
            }

            // No handler took the error, so it is written out rather than lost.
            Console.WriteLine(exception);
        }

        private void SetState(TimerState state)
        {
            var oldState = State;
            State = state;
            StateChangedEvent?.Invoke(this, new StateChangedEventArgs(oldState, state));
        }

        private TimeSpan GetElapsed(long startTime)
        {
            return new TimeSpan(HiResDateTime.UtcNowTicks - startTime);
        }

        /// <summary>
        /// Starts the timer. Calling it while the timer is running has no effect.
        /// </summary>
        /// <remarks>
        /// The first start executes the callback straight away. A start after <see cref="Stop"/> first waits
        /// for whatever was left of the interval when the timer was stopped, so the callback never runs
        /// earlier than it was due. A start that follows a stop while the stopped run is still ending
        /// (for example while the callback is still executing) starts the timer again once that run has ended.
        /// </remarks>
        public void Start()
        {
            TimerEngine();
        }

        /// <summary>
        /// Stops the timer by requesting cancellation. A callback that is executing runs to completion.
        /// </summary>
        /// <remarks>
        /// Safe to call at any time: before <see cref="Start"/>, right after it, more than once, and after the timer has ended.
        /// </remarks>
        public void Stop()
        {
            CancellationTokenSource cancellationTokenSource;
            lock (_syncRoot)
            {
                _restartRequested = false;
                if (!_running) return;
                _stopRequested = true;
                cancellationTokenSource = _cancellationTokenSource;
            }

            // Cancelled outside the lock, because cancelling can run the rest of the run,
            // event handlers included, on this thread. The source is never disposed, so
            // cancelling it after its run has ended is harmless; it owns no timer or wait handle.
            cancellationTokenSource.Cancel();
        }
    }
}