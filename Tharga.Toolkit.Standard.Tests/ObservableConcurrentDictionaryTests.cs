using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using AwesomeAssertions;
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

    [Fact]
    public void Add_throws_when_the_key_exists()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("a", 1);

        var act = () => dict.Add("a", 2);

        act.Should().Throw<ArgumentException>();
        dict["a"].Should().Be(1);
    }

    [Fact]
    public void Remove_pair_with_a_different_value_does_not_remove()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("a", 1);
        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        var removed = ((ICollection<KeyValuePair<string, int>>)dict).Remove(new KeyValuePair<string, int>("a", 999));

        removed.Should().BeFalse();
        dict["a"].Should().Be(1);
        captured.Should().BeNull();
    }

    [Fact]
    public void Remove_pair_with_the_matching_value_removes_and_fires_Remove()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("a", 1);
        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        var removed = ((ICollection<KeyValuePair<string, int>>)dict).Remove(new KeyValuePair<string, int>("a", 1));

        removed.Should().BeTrue();
        dict.ContainsKey("a").Should().BeFalse();
        captured.Action.Should().Be(NotifyCollectionChangedAction.Remove);
    }

    [Fact]
    public void Indexer_set_for_a_new_key_fires_Add_and_Count()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        NotifyCollectionChangedEventArgs captured = null;
        var propertyNames = new List<string>();
        dict.CollectionChanged += (_, e) => captured = e;
        dict.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

        dict["new"] = 5;

        captured.Action.Should().Be(NotifyCollectionChangedAction.Add);
        captured.NewItems[0].Should().Be(5);
        propertyNames.Should().Contain(["Count", "Keys", "Values"]);
    }

    [Fact]
    public void Indexer_set_for_an_existing_key_fires_Replace_with_the_old_value()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("key", 1);
        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        dict["key"] = 99;

        captured.Action.Should().Be(NotifyCollectionChangedAction.Replace);
        captured.NewItems[0].Should().Be(99);
        captured.OldItems[0].Should().Be(1);
    }

    [Fact]
    public void TryAdd_adds_a_new_key_and_fires_Add()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        var added = dict.TryAdd("a", 1);

        added.Should().BeTrue();
        dict["a"].Should().Be(1);
        captured.Action.Should().Be(NotifyCollectionChangedAction.Add);
    }

    [Fact]
    public void TryAdd_returns_false_for_an_existing_key_and_fires_nothing()
    {
        var dict = new ObservableConcurrentDictionary<string, int>();
        dict.Add("a", 1);
        NotifyCollectionChangedEventArgs captured = null;
        dict.CollectionChanged += (_, e) => captured = e;

        var added = dict.TryAdd("a", 2);

        added.Should().BeFalse();
        dict["a"].Should().Be(1);
        captured.Should().BeNull();
    }
}
