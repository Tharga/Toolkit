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

        private bool _running;
        private TimeSpan _interval;
        private CancellationTokenSource _cancellationTokenSource;
        private long _iteration;
        private long _startTime;

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
        /// Occurs after the timer callback has executed.
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
            lock (_syncRoot)
            {
                if (_running) return;
                _running = true;
            }

            Task.Run(async () =>
            {
                _cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    SetState(TimerState.Started);

                    if (_startTime > 0)
                    {
                        var vv = new TimeSpan(((HiResDateTime.UtcNowTicks - _startTime) / (_iteration + 1)));
                        var iv = Interval - vv;
                        if (iv.Ticks > 0)
                        {
                            //Console.WriteLine($"Too fast, waiting {iv.TotalMilliseconds:0}ms.");
                            await Task.Delay(iv, _cancellationTokenSource.Token);
                        }
                        //else
                        //{
                        //    Console.WriteLine($"Too slow.");
                        //}
                    }

                    _iteration = 0;
                    _startTime = HiResDateTime.UtcNowTicks;

                    while (true)
                    {
                        var lapTime = HiResDateTime.UtcNowTicks;

                        if (!_cancellationTokenSource.Token.IsCancellationRequested)
                        {
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

                        SetState(TimerState.Waiting);

                        //var now = new DateTime(HiResDateTime.UtcNowTicks);
                        //var corr = new TimeSpan(HiResDateTime.UtcNowTicks - _startTime).TotalMilliseconds;
                        //Console.WriteLine($"{_iteration} {now:HH:mm:ss fff}: wait:{iv.TotalMilliseconds:0}ms corr:{corr:0}ms");

                        await Task.Delay(iv, _cancellationTokenSource.Token);

                        _iteration++;

                        if (!_autoReset) return;
                    }
                }
                catch (TaskCanceledException)
                {
                    //TODO: When this has been triggered, the timer will restart too, fast and try to recall the same iteration once again, without waiting for the delay.
                }
                catch (Exception e)
                {
                    //TODO: Fire event
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    _cancellationTokenSource.Dispose();
                    _running = false;
                    SetState(TimerState.Stopped);
                }
            });
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
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            TimerEngine();
        }

        /// <summary>
        /// Stops the timer by requesting cancellation.
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}