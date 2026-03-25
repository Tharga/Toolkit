using System.Collections.Specialized;
using System.ComponentModel;
using FluentAssertions;
using Xunit;

namespace Tharga.Toolkit.Standard.Tests;

public class ObservableConcurrentDictionaryTests
{
    [Fact]
    public void Add_stores_value()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("key", 42);
        dict["key"].Should().Be(42);
    }

    [Fact]
    public void Add_fires_CollectionChanged()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        dict.Add("key", 42);

        captured.Should().NotBeNull();
        captured.Action.Should().Be(NotifyCollectionChangedAction.Add);
    }

    [Fact]
    public void Add_fires_PropertyChanged_for_Count_Keys_Values()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        var propertyNames = new System.Collections.Generic.List<string>();
        dict.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

        dict.Add("key", 42);

        propertyNames.Should().Contain("Count");
        propertyNames.Should().Contain("Keys");
        propertyNames.Should().Contain("Values");
    }

    [Fact]
    public void Remove_removes_value()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("key", 42);

        dict.Remove("key").Should().BeTrue();
        dict.ContainsKey("key").Should().BeFalse();
    }

    [Fact]
    public void Remove_fires_CollectionChanged()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("key", 42);

        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        dict.Remove("key");

        captured.Should().NotBeNull();
        captured.Action.Should().Be(NotifyCollectionChangedAction.Remove);
    }

    [Fact]
    public void Remove_returns_false_for_missing_key()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Remove("missing").Should().BeFalse();
    }

    [Fact]
    public void Clear_removes_all_items()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("a", 1);
        dict.Add("b", 2);

        dict.Clear();

        dict.Count.Should().Be(0);
    }

    [Fact]
    public void Clear_fires_CollectionChanged_Reset()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("a", 1);

        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        dict.Clear();

        captured.Should().NotBeNull();
        captured.Action.Should().Be(NotifyCollectionChangedAction.Reset);
    }

    [Fact]
    public void Indexer_set_fires_CollectionChanged_Replace()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("key", 1);

        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        dict["key"] = 99;

        captured.Should().NotBeNull();
        captured.Action.Should().Be(NotifyCollectionChangedAction.Replace);
    }

    [Fact]
    public void TryGetValue_returns_true_for_existing_key()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("key", 42);

        dict.TryGetValue("key", out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void TryGetValue_returns_false_for_missing_key()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.TryGetValue("missing", out _).Should().BeFalse();
    }

    [Fact]
    public void ContainsKey_returns_correct_value()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("key", 1);

        dict.ContainsKey("key").Should().BeTrue();
        dict.ContainsKey("other").Should().BeFalse();
    }

    [Fact]
    public void Count_reflects_item_count()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Count.Should().Be(0);
        dict.Add("a", 1);
        dict.Count.Should().Be(1);
        dict.Add("b", 2);
        dict.Count.Should().Be(2);
    }

    [Fact]
    public void IsReadOnly_returns_false()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void GetEnumerator_enumerates_all_items()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("a", 1);
        dict.Add("b", 2);

        var items = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, int>>();
        foreach (var item in dict)
        {
            items.Add(item);
        }

        items.Should().HaveCount(2);
    }
}
