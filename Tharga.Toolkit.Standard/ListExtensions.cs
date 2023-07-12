using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit
{
    public static class ListExtensions
    {
        private static readonly Random Rng = new Random();

        public static T TakeRandom<T>(this IEnumerable<T> values)
        {
            var list = values.ToList();
            if (!list.Any()) return default;
            var index = GetRandomInt(0, list.Count);
            return list[index];
        }

        private static int GetRandomInt(int min = 0, int max = 10000000)
        {
            return Rng.Next(max - min) + min;
        }

        public static IEnumerable<T> TakeAllButFirst<T>(this IEnumerable<T> values)
        {
            return values?.Skip(1);
        }

        public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> values)
        {
            if (values == null) return null;
            return TakeAllButLastEx(values);
        }

        private static IEnumerable<T> TakeAllButLastEx<T>(IEnumerable<T> values)
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
    }
}