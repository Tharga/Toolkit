using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit.Compare
{
    public static class CompareExtensions
    {
        [Flags]
        public enum CompareMode
        {
            Standard = 0x0,
            IgnoreType = 0x1,
            IgnoreSortOrder = 0x2,
        }

        public static IEnumerable<IDiff> Compare(this object s1, object s2, CompareMode compareMode = CompareMode.Standard)
        {
            return DoCompare(null, null, s1, s2, compareMode, new List<object>());
        }

        private static IEnumerable<IDiff> DoCompare(string parentObject1Name, string parentObject2Name, object s1, object s2, CompareMode compareMode, List<object> visited)
        {
            if (ReferenceEquals(s1, s2))
                yield break;

            if (s1 == null)
                yield return new Diff(parentObject1Name, parentObject2Name, "One item has a value and the other is null.", null);
            else if (s2 == null)
                yield return new Diff(parentObject1Name, parentObject2Name, "One item is null and the other has a value.", null);
            else
            {
                var tp1 = s1.GetType();
                var tp2 = s2.GetType();

                var item1Name = parentObject1Name != null ? $"{parentObject1Name}.{s1.GetType().Name}" : s1.GetType().Name;
                var item2Name = parentObject1Name != null ? $"{parentObject2Name}.{s2.GetType().Name}" : s2.GetType().Name;

                if (tp1 != tp2 && (compareMode & CompareMode.IgnoreType) != CompareMode.IgnoreType)
                    yield return new DifferentTypes(item1Name, tp1, tp2, null);
                else
                {
                    if (s1 is string || s2 is string)
                    {
                        if (s1.ToString() != s2.ToString())
                            yield return new Diff(item1Name, item2Name, $"One string has value {s1} and the other string has value {s2}.", null);
                    }
                    else if (s1 is DateTime || s2 is DateTime)
                    {
                        foreach (var diff2 in CompareDateTimes(s1, s2, item1Name, item2Name))
                            yield return diff2;
                    }
                    else if (s1 is int || s2 is int)
                    {
                        foreach (var diff2 in CompareInts(s1, s2, item1Name, item2Name))
                            yield return diff2;
                    }
                    else if (s1 is decimal || s2 is decimal)
                    {
                        foreach (var diff2 in CompareDecimals(s1, s2, item1Name, item2Name))
                            yield return diff2;
                    }
                    else if (tp1.IsValueType)
                    {
                        foreach (var diff in CompareValueTypes(item1Name, item2Name, s1, s2, compareMode, visited))
                            yield return diff;
                    }
                    else
                    {
                        foreach (var diff1 in CompareReferenceTypes(item1Name, item2Name, s1, s2, compareMode, visited))
                            yield return diff1;
                    }
                }
            }
        }

        private static IEnumerable<IDiff> CompareInts(object s1, object s2, string item1Name, string item2Name)
        {
            if (!int.TryParse(s1.ToString(), out var i1))
                yield return new Diff(item1Name, item2Name, $"Cannot parse {item1Name} with value {s1} to an int to compare with {item2Name} with value {s2}.", null);
            else
            {
                if (!int.TryParse(s2.ToString(), out var i2))
                    yield return new Diff(item1Name, item2Name, $"Cannot parse {item2Name} with value {s2} to an int to compare with {item1Name} with value {s1}.", null);
                else if (i1 - i2 != 0)
                    yield return new Diff(item1Name, item2Name, $"Value of int {item1Name} is {s1} and differs from {item2Name} with {s2} by {Math.Abs(i1 - i2)}.", null);
            }
        }

        private static IEnumerable<IDiff> CompareDecimals(object s1, object s2, string item1Name, string item2Name)
        {
            if (!decimal.TryParse(s1.ToString(), out var i1))
                yield return new Diff(item1Name, item2Name, $"Cannot parse {item1Name} with value {s1} to an decimal to compare with {item2Name} with value {s2}.", null);
            else
            {
                if (!decimal.TryParse(s2.ToString(), out var i2))
                    yield return new Diff(item1Name, item2Name, $"Cannot parse {item2Name} with value {s2} to an decimal to compare with {item1Name} with value {s1}.", null);
                else if (i1 - i2 != 0)
                    yield return new Diff(item1Name, item2Name, $"Value of decimal {item1Name} is {s1} and differs from {item2Name} with {s2} by {Math.Abs(i1 - i2)}.", null);
            }
        }

        private static IEnumerable<IDiff> CompareDateTimes(object s1, object s2, string item1Name, string item2Name)
        {
            if (!DateTime.TryParse(s1.ToString(), out var i1))
                yield return new Diff(item1Name, item2Name, $"Cannot parse {item1Name} with value {s1} to an DateTime to compare with {item2Name} with value {s2}.", null);
            else
            {
                if (!DateTime.TryParse(s2.ToString(), out var i2))
                    yield return new Diff(item1Name, item2Name, $"Cannot parse {item2Name} with value {s2} to an DateTime to compare with {item1Name} with value {s1}.", null);
                else if (i1 - i2 != new TimeSpan())
                    yield return new Diff(item1Name, item2Name, $"Value of DateTime {item1Name} is {((DateTime) s1).ToShortDateString() + " " + ((DateTime) s1).ToLongTimeString()} and differs from {item2Name} with {((DateTime) s2).ToShortDateString() + " " + ((DateTime) s2).ToLongTimeString()} by {Math.Abs(((DateTime) s1 - (DateTime) s2).Ticks)} ticks.", null);
            }
        }

        private static IEnumerable<IDiff> CompareValueTypes(string item1Name, string item2Name, object s1, object s2, CompareMode compareMode, List<object> visited)
        {
            var itemHasDiffs = false;

            foreach (var diff2 in CompareMembers(item1Name, item2Name, s1, s2, compareMode, visited))
            {
                itemHasDiffs = true;
                yield return diff2;
            }

            visited.Add(s1);
            visited.Add(s2);

            if (!itemHasDiffs)
            {
                if (s1.ToString() != s2.ToString())
                    yield return new Diff(item1Name, item2Name, string.Format("The value of the item {2} is {0} and the value of the other item ({3}) is {1}.", s1, s2, item1Name, item2Name), null);
            }
        }

        private static IEnumerable<IDiff> CompareReferenceTypes(string item1Name, string item2Name, object s1, object s2, CompareMode compareMode, List<object> visited)
        {
            if (s1 is IEnumerable enumerable && s2 is IEnumerable o)
            {
                var enumr1 = enumerable.GetEnumerator();
                var enumr2 = o.GetEnumerator();
                var used = new List<object>();

                var index = 0;
                while (true)
                {
                    var ptr1 = enumr1.MoveNext();
                    var ptr2 = enumr2.MoveNext();

                    if (ptr1 != ptr2)
                    {
                        yield return new Diff(item1Name, item2Name, $"One value has the value {ptr1} and the other has value {ptr2}.", index);
                    }
                    if (!ptr1 || !ptr2)
                        yield break;

                    var data1 = enumr1.Current;
                    var data2 = enumr2.Current;

                    if (compareMode.HasFlag(CompareMode.IgnoreSortOrder))
                    {
                        //Find a match, anywhere in the enumerator.
                        data2 = GetMatchFromList(item1Name, item2Name, data1, o, compareMode, used);
                    }

                    var diffs = DoCompare(item1Name, item2Name, data1, data2, compareMode, visited);
                    foreach (var diff in diffs)
                    {
                        yield return new Diff(diff.ObjectName, diff.OtherObjectName, diff.Message, index);
                    }
                    index++;
                }
            }
            else
            {
                foreach (var diff2 in CompareMembers(item1Name, item2Name, s1, s2, compareMode, visited))
                    yield return diff2;
            }
        }

        private static object GetMatchFromList(string item1Name, string item2Name, object data1, object s2, CompareMode compareMode, List<object> used)
        {
            var enumr2 = (s2 as IEnumerable)?.GetEnumerator();
            var ptr2 = true;
            while (ptr2)
            {
                if (enumr2 != null)
                {
                    ptr2 = enumr2.MoveNext();
                    if (ptr2)
                    {
                        var data2 = enumr2.Current;
                        var v = new List<object>();
                        if (!DoCompare(item1Name, item2Name, data1, data2, compareMode, v).Any())
                        {
                            if (!used.Any(x => ReferenceEquals(x, data2)))
                            {
                                used.Add(data2);
                                return data2;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static IEnumerable<IDiff> CompareMembers(string item1Name, string item2Name, object s1, object s2, CompareMode compareMode, List<object> visited)
        {
            if (visited.Contains(s1) || visited.Contains(s2))
                yield break;

            visited.Add(s1);
            visited.Add(s2);

            var fields = s1.GetType().GetFields();
            foreach (var field in fields)
            {
                var f1 = field.GetValue(s1);

                var f2 = GetValueFromPropertyOrField(item1Name, item2Name, s2, field.Name, out var diff1);
                if (f2 == null && diff1 != null)
                    yield return diff1;
                else
                {
                    //TODO: Should not the actual property name 'prop.Name' be provided too?
                    var diffs = DoCompare(item1Name, item2Name, f1, f2, compareMode, visited);

                    foreach (var diff in diffs)
                    {
                        yield return diff;
                    }
                }
            }

            var tp = s1.GetType();
            var props = tp.GetProperties();
            foreach (var prop in props)
            {
                var v1 = prop.GetValue(s1, null);
                var v2 = GetValueFromPropertyOrField(item1Name, item2Name, s2, prop.Name, out var diff1);
                if (v2 == null && diff1 != null)
                    yield return diff1;
                else
                {
                    //TODO: Should not the actual property name 'prop.Name' be provided too?
                    var diffs = DoCompare(item1Name, item2Name, v1, v2, compareMode, visited);

                    foreach (var diff in diffs)
                    {
                        yield return diff;
                    }
                }
            }
        }

        private static object GetValueFromPropertyOrField(string item1Name, string item2Name, object s2, string name, out IDiff diff)
        {
            diff = null;

            var prop = s2.GetType().GetProperty(name);
            if (prop != null)
            {
                var val = prop.GetValue(s2, null);
                return val;
            }

            var field = s2.GetType().GetField(name);
            if (field != null)
            {
                var val = field.GetValue(s2);
                return val;
            }

            diff = new Diff(item1Name, item2Name, $"Cannot find the property or field named {name} in object of type {s2.GetType()}.", null);
            return null;
        }
    }
}