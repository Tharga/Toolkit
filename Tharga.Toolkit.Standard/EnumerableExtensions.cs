using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit
{
    public static class EnumerableExtensions
    {
        private static readonly Lazy<Random> _rng = new Lazy<Random>(() => new Random());

        public static T TakeRandom<T>(this IEnumerable<T> values)
        {
            if (values == null) return default;
            var list = values.ToArray();
            if (!list.Any()) return default;
            var index = _rng.Value.Next(list.Length);
            return list[index];
        }

        public static IEnumerable<T> RandomOrder<T>(this IEnumerable<T> values)
        {
            return values.OrderBy(_ => _rng.Value.Next());
        }

        public static IEnumerable<T> TakeAllButFirst<T>(this IEnumerable<T> values)
        {
            return values?.Skip(1);
        }

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

        public static IEnumerable<IEnumerable<TValue>> TakeChunks<TValue>(this IEnumerable<TValue> values, int chunkSize)
        {
            return values?.Select((v, i) => new { v, groupIndex = i / chunkSize })
                .GroupBy(x => x.groupIndex)
                .Select(g => g.Select(x => x.v));
        }

        public static bool IsNullOrEmpty<T>(IEnumerable<T> values)
        {
            if (values == null) return true;
            if (!values.Any()) return true;
            return false;
        }

        public static IEnumerable<T> EmptyIfNull<T>(IEnumerable<T> items)
        {
            if (items == null) return Enumerable.Empty<T>();
            return items;
        }
    }
}