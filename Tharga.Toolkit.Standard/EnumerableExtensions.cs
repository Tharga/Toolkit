using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>: random selection, ordering, chunking, and null handling.
    /// </summary>
    public static class EnumerableExtensions
    {
        private static readonly Lazy<Random> _rng = new Lazy<Random>(() => new Random());

        /// <summary>Selects a random element from the sequence. Returns <c>default</c> if null or empty.</summary>
        public static T TakeRandom<T>(this IEnumerable<T> values)
        {
            if (values == null) return default;
            var list = values.ToArray();
            if (!list.Any()) return default;
            var index = _rng.Value.Next(list.Length);
            return list[index];
        }

        /// <summary>Returns the elements of the sequence in random order.</summary>
        public static IEnumerable<T> RandomOrder<T>(this IEnumerable<T> values)
        {
            return values.OrderBy(_ => _rng.Value.Next());
        }

        /// <summary>Returns all elements except the first.</summary>
        public static IEnumerable<T> TakeAllButFirst<T>(this IEnumerable<T> values)
        {
            return values?.Skip(1);
        }

        /// <summary>Returns all elements except the last.</summary>
        public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> values)
        {
            if (values == null) return null;
            return TakeAllButLastInternal(values);
        }

        private static IEnumerable<T> TakeAllButLastInternal<T>(IEnumerable<T> values)
        {
            using (var it = values.GetEnumerator())
            {
                bool hasRemainingItems;
                var isFirst = true;
                var item = default(T);
                do
                {
                    hasRemainingItems = it.MoveNext();
                    if (hasRemainingItems)
                    {
                        if (!isFirst) yield return item;
                        item = it.Current;
                        isFirst = false;
                    }
                } while (hasRemainingItems);
            }
        }

        /// <summary>Splits the sequence into chunks of the specified size.</summary>
        public static IEnumerable<IEnumerable<TValue>> TakeChunks<TValue>(this IEnumerable<TValue> values, int chunkSize)
        {
            return values?.Select((v, i) => new { v, groupIndex = i / chunkSize })
                .GroupBy(x => x.groupIndex)
                .Select(g => g.Select(x => x.v));
        }

        /// <summary>Returns <c>true</c> if the sequence is null or contains no elements.</summary>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> values)
        {
            if (values == null) return true;
            if (!values.Any()) return true;
            return false;
        }

        /// <summary>Returns an empty sequence if the input is null; otherwise returns the input.</summary>
        public static IEnumerable<T> EmptyIfNull<T>(IEnumerable<T> items)
        {
            if (items == null) return Enumerable.Empty<T>();
            return items;
        }
    }
}