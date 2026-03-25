using System;
using System.Collections.Generic;
using FluentAssertions;
using Tharga.Toolkit.Logging;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class ExceptionExtensionTests
{
    [Fact]
    public void AddData_adds_key_value_pair()
    {
        var ex = new InvalidOperationException();
        ex.AddData("key", "value");
        ex.Data["key"].Should().Be("value");
    }

    [Fact]
    public void AddData_replaces_existing_key()
    {
        var ex = new InvalidOperationException();
        ex.AddData("key", "first");
        ex.AddData("key", "second");
        ex.Data["key"].Should().Be("second");
    }

    [Fact]
    public void AddData_returns_same_exception_for_fluent_chaining()
    {
        var ex = new InvalidOperationException();
        var result = ex.AddData("a", 1).AddData("b", 2);
        result.Should().BeSameAs(ex);
        ex.Data["a"].Should().Be(1);
        ex.Data["b"].Should().Be(2);
    }

    [Fact]
    public void TryAddData_returns_true_for_new_key()
    {
        var ex = new InvalidOperationException();
        ex.TryAddData("key", "value").Should().BeTrue();
        ex.Data["key"].Should().Be("value");
    }

    [Fact]
    public void TryAddData_returns_false_for_existing_key()
    {
        var ex = new InvalidOperationException();
        ex.Data.Add("key", "first");
        ex.TryAddData("key", "second").Should().BeFalse();
        ex.Data["key"].Should().Be("first");
    }

    [Fact]
    public void ToDictionary_converts_exception_data()
    {
        var ex = new InvalidOperationException();
        ex.Data.Add("name", "test");
        ex.Data.Add("count", 42);

        var dict = ex.ToDictionary();
        dict.Should().BeOfType<Dictionary<string, object>>();
        dict["name"].Should().Be("test");
        dict["count"].Should().Be(42);
    }

    [Fact]
    public void ToDictionary_ignores_non_string_keys()
    {
        var ex = new InvalidOperationException();
        ex.Data.Add("name", "test");
        ex.Data.Add(42, "numeric key");

        var dict = ex.ToDictionary();
        dict.Should().HaveCount(1);
        dict.Should().ContainKey("name");
    }

    [Fact]
    public void ToDictionary_returns_empty_dict_for_no_data()
    {
        var ex = new InvalidOperationException();
        ex.ToDictionary().Should().BeEmpty();
    }
}
