using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Tharga.Toolkit.Timer;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class ManagedTimerTests
{
    [Fact]
    public void Constructor_throws_for_negative_interval()
    {
        var act = () => new ManagedTimer(TimeSpan.FromMilliseconds(-1), _ => Task.CompletedTask);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_throws_for_null_callback()
    {
#pragma warning disable CS8625
        var act = () => new ManagedTimer(TimeSpan.FromSeconds(1), null);
#pragma warning restore CS8625
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Initial_state_is_stopped()
    {
        var timer = new ManagedTimer(TimeSpan.FromSeconds(1), _ => Task.CompletedTask);
        timer.State.Should().Be(ManagedTimer.TimerState.Stopped);
    }

    [Fact]
    public void Interval_property_returns_configured_value()
    {
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(500), _ => Task.CompletedTask);
        timer.Interval.Should().Be(TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void Interval_change_fires_IntervalChangedEvent()
    {
        var timer = new ManagedTimer(TimeSpan.FromSeconds(1), _ => Task.CompletedTask);
        IntervalChangedEventArgs captured = null;
        timer.IntervalChangedEvent += (_, e) => captured = e;

        timer.Interval = TimeSpan.FromSeconds(2);

        captured.Should().NotBeNull();
        captured.OldInterval.Should().Be(TimeSpan.FromSeconds(1));
        captured.Interval.Should().Be(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Interval_same_value_does_not_fire_event()
    {
        var timer = new ManagedTimer(TimeSpan.FromSeconds(1), _ => Task.CompletedTask);
        var fired = false;
        timer.IntervalChangedEvent += (_, _) => fired = true;

        timer.Interval = TimeSpan.FromSeconds(1);

        fired.Should().BeFalse();
    }

    [Fact]
    public async Task Start_executes_callback()
    {
        var tcs = new TaskCompletionSource<long>();
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), iteration =>
        {
            tcs.TrySetResult(iteration);
            return Task.CompletedTask;
        });

        timer.Start();

        var result = await Task.WhenAny(tcs.Task, Task.Delay(2000));
        result.Should().Be(tcs.Task, "callback should have been invoked");
        (await tcs.Task).Should().Be(0);

        timer.Stop();
    }

    [Fact]
    public async Task Start_fires_StateChanged_events()
    {
        var states = new ConcurrentQueue<ManagedTimer.TimerState>();
        var tcs = new TaskCompletionSource<bool>();

        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ =>
        {
            tcs.TrySetResult(true);
            return Task.CompletedTask;
        });

        timer.StateChangedEvent += (_, e) => states.Enqueue(e.State);

        timer.Start();
        await Task.WhenAny(tcs.Task, Task.Delay(2000));
        timer.Stop();

        // Allow time for stop state
        await Task.Delay(100);

        var stateList = states.ToArray();
        stateList.Should().Contain(ManagedTimer.TimerState.Started);
        stateList.Should().Contain(ManagedTimer.TimerState.Executing);
        stateList.Should().Contain(ManagedTimer.TimerState.ExecuteComplete);
    }

    [Fact]
    public async Task Start_fires_BeforeExecute_and_AfterExecute()
    {
        var beforeFired = false;
        AfterExecuteEventArgs afterArgs = null;
        var tcs = new TaskCompletionSource<bool>();

        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ =>
        {
            tcs.TrySetResult(true);
            return Task.CompletedTask;
        });

        timer.BeforeExecuteEvent += (_, _) => beforeFired = true;
        timer.AfterExecuteEvent += (_, e) => afterArgs = e;

        timer.Start();
        await Task.WhenAny(tcs.Task, Task.Delay(2000));
        timer.Stop();
        await Task.Delay(100);

        beforeFired.Should().BeTrue();
        afterArgs.Should().NotBeNull();
        afterArgs.Exception.Should().BeNull();
        afterArgs.Iteration.Should().Be(0);
    }

    [Fact]
    public async Task AfterExecute_captures_exception()
    {
        AfterExecuteEventArgs afterArgs = null;
        var tcs = new TaskCompletionSource<bool>();

        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ =>
        {
            tcs.TrySetResult(true);
            throw new InvalidOperationException("test error");
        });

        timer.AfterExecuteEvent += (_, e) => afterArgs = e;

        timer.Start();
        await Task.WhenAny(tcs.Task, Task.Delay(2000));
        timer.Stop();
        await Task.Delay(100);

        afterArgs.Should().NotBeNull();
        afterArgs.Exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task BeforeExecute_cancel_prevents_execution()
    {
        var executed = false;
        var tcs = new TaskCompletionSource<bool>();

        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        timer.BeforeExecuteEvent += (_, e) =>
        {
            e.Cancel = true;
            tcs.TrySetResult(true);
        };

        timer.Start();
        await Task.WhenAny(tcs.Task, Task.Delay(2000));
        timer.Stop();
        await Task.Delay(100);

        executed.Should().BeFalse();
    }

    [Fact]
    public async Task AutoReset_false_stops_after_one_execution()
    {
        var count = 0;
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ =>
        {
            count++;
            return Task.CompletedTask;
        }, autoReset: false);

        timer.Start();
        await Task.Delay(300);

        count.Should().Be(1);
    }

    [Fact]
    public async Task Stop_ends_timer()
    {
        var count = 0;
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ =>
        {
            count++;
            return Task.CompletedTask;
        });

        timer.Start();
        await Task.Delay(200);
        timer.Stop();
        var countAtStop = count;
        await Task.Delay(200);

        count.Should().Be(countAtStop);
    }

    [Fact]
    public async Task AutoStart_starts_timer_immediately()
    {
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(500), _ => Task.CompletedTask, autoStart: true);

        // Poll for state transition — bare delays are flaky on busy CI runners.
        var deadline = DateTime.UtcNow.AddSeconds(2);
        while (timer.State == ManagedTimer.TimerState.Stopped && DateTime.UtcNow < deadline)
        {
            await Task.Delay(20);
        }
        timer.State.Should().NotBe(ManagedTimer.TimerState.Stopped);

        timer.Stop();
    }

    // The tests below wait on signals raised by the timer, never on fixed sleeps.
    // The timeout only bounds a failing run, so it is generous.
    private static readonly TimeSpan SignalTimeout = TimeSpan.FromSeconds(5);

    // Task.Delay can complete a little before its due time when measured with a
    // Stopwatch (the system timer is coarse), so lower bounds allow for that.
    private static readonly TimeSpan TimerResolution = TimeSpan.FromMilliseconds(50);

    private static TaskCompletionSource NewSignal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    [Fact]
    public void Stop_before_Start_does_not_throw()
    {
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ => Task.CompletedTask);

        var act = () => timer.Stop();

        act.Should().NotThrow();
        timer.State.Should().Be(ManagedTimer.TimerState.Stopped);
    }

    [Fact]
    public async Task Stop_right_after_Start_does_not_throw_and_stops_the_timer()
    {
        var stopped = NewSignal();
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ => Task.CompletedTask);
        timer.StateChangedEvent += (_, e) =>
        {
            if (e.State == ManagedTimer.TimerState.Stopped) stopped.TrySetResult();
        };

        timer.Start();
        var act = () => timer.Stop();

        act.Should().NotThrow();
        await stopped.Task.WaitAsync(SignalTimeout);
    }

    [Fact]
    public async Task Stop_after_the_timer_has_ended_does_not_throw()
    {
        var executed = NewSignal();
        var stopped = NewSignal();
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ =>
        {
            executed.TrySetResult();
            return Task.CompletedTask;
        });
        timer.StateChangedEvent += (_, e) =>
        {
            if (e.State == ManagedTimer.TimerState.Stopped) stopped.TrySetResult();
        };

        timer.Start();
        await executed.Task.WaitAsync(SignalTimeout);
        timer.Stop();
        await stopped.Task.WaitAsync(SignalTimeout);

        var act = () =>
        {
            timer.Stop();
            timer.Stop();
        };

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(ManagedTimer.Mode.OnTime)]
    [InlineData(ManagedTimer.Mode.Between)]
    public async Task Restart_after_a_stop_during_the_wait_waits_for_the_interval(ManagedTimer.Mode mode)
    {
        // Several executions first: the old restart path averaged the time per
        // iteration so far, which only matched the interval on the first one.
        const int executionsBeforeStop = 3;
        var interval = TimeSpan.FromMilliseconds(250);
        var clock = Stopwatch.StartNew();
        var executions = new ConcurrentQueue<TimeSpan>();
        var waitingAfterLastExecution = NewSignal();
        var stopped = NewSignal();
        var executedAfterRestart = NewSignal();

        var timer = new ManagedTimer(interval, _ =>
        {
            executions.Enqueue(clock.Elapsed);
            if (executions.Count == executionsBeforeStop + 1) executedAfterRestart.TrySetResult();
            return Task.CompletedTask;
        }, mode);
        timer.StateChangedEvent += (_, e) =>
        {
            if (e.State == ManagedTimer.TimerState.Waiting && executions.Count == executionsBeforeStop) waitingAfterLastExecution.TrySetResult();
            if (e.State == ManagedTimer.TimerState.Stopped) stopped.TrySetResult();
        };

        timer.Start();
        await waitingAfterLastExecution.Task.WaitAsync(SignalTimeout);
        timer.Stop();
        await stopped.Task.WaitAsync(SignalTimeout);
        timer.Start();
        await executedAfterRestart.Task.WaitAsync(SignalTimeout);
        timer.Stop();

        // Each execution is at least one interval after the one before it, so the
        // first one after the restart is no earlier than this.
        var times = executions.ToArray();
        var earliest = times[0] + interval * executionsBeforeStop - TimerResolution;
        times[executionsBeforeStop].Should().BeGreaterThanOrEqualTo(earliest);
    }

    [Fact]
    public async Task Start_right_after_a_Stop_during_execution_starts_the_timer_again()
    {
        var count = 0;
        var executing = NewSignal();
        var release = NewSignal();
        var executedAgain = NewSignal();

        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), async _ =>
        {
            if (Interlocked.Increment(ref count) == 1)
            {
                executing.TrySetResult();
                await release.Task;
            }
            else
            {
                executedAgain.TrySetResult();
            }
        });

        timer.Start();
        await executing.Task.WaitAsync(SignalTimeout);
        timer.Stop();
        timer.Start();
        release.TrySetResult();

        await executedAgain.Task.WaitAsync(SignalTimeout);
        timer.Stop();
    }

    [Fact]
    public async Task Exception_from_an_event_handler_is_reported_through_AfterExecuteEvent_and_stops_the_timer()
    {
        var reported = new TaskCompletionSource<AfterExecuteEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        var stopped = NewSignal();
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ => Task.CompletedTask);
        timer.BeforeExecuteEvent += (_, _) => throw new InvalidOperationException("handler failed");
        timer.AfterExecuteEvent += (_, e) => reported.TrySetResult(e);
        timer.StateChangedEvent += (_, e) =>
        {
            if (e.State == ManagedTimer.TimerState.Stopped) stopped.TrySetResult();
        };

        timer.Start();

        var args = await reported.Task.WaitAsync(SignalTimeout);
        args.Exception.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("handler failed");
        await stopped.Task.WaitAsync(SignalTimeout);
    }

    [Fact]
    public async Task Exception_from_a_Stopped_state_handler_is_reported_through_AfterExecuteEvent()
    {
        var reported = new TaskCompletionSource<AfterExecuteEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        var executed = NewSignal();
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ =>
        {
            executed.TrySetResult();
            return Task.CompletedTask;
        });
        timer.StateChangedEvent += (_, e) =>
        {
            if (e.State == ManagedTimer.TimerState.Stopped) throw new InvalidOperationException("handler failed");
        };
        timer.AfterExecuteEvent += (_, e) =>
        {
            if (e.Exception != null) reported.TrySetResult(e);
        };

        timer.Start();
        await executed.Task.WaitAsync(SignalTimeout);
        timer.Stop();

        var args = await reported.Task.WaitAsync(SignalTimeout);
        args.Exception.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be("handler failed");
    }

    [Fact]
    public async Task AfterExecuteEvent_handler_that_throws_stops_the_timer()
    {
        var stopped = NewSignal();
        var timer = new ManagedTimer(TimeSpan.FromMilliseconds(50), _ => Task.CompletedTask);
        timer.AfterExecuteEvent += (_, _) => throw new InvalidOperationException("handler failed");
        timer.StateChangedEvent += (_, e) =>
        {
            if (e.State == ManagedTimer.TimerState.Stopped) stopped.TrySetResult();
        };

        timer.Start();

        await stopped.Task.WaitAsync(SignalTimeout);
        timer.State.Should().Be(ManagedTimer.TimerState.Stopped);
    }
}
