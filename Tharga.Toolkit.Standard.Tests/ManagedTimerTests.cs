using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
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

        // Give the async task a moment to start
        await Task.Delay(50);
        timer.State.Should().NotBe(ManagedTimer.TimerState.Stopped);

        timer.Stop();
    }
}
