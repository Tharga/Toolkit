using System;
using System.Collections.Generic;

namespace Tharga.Toolkit
{
    public static class EnumerableExtensions
    {
        private static readonly Random _rng = new();

        public static async IAsyncEnumerable<T> RandomOrderAsync<T>(this IAsyncEnumerable<T> values)
        {
            var list = new List<T>();
            await foreach (var item in values)
            {
                list.Add(item);
            }

            //Fisher–Yates shuffle
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = _rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            foreach (var item in list)
            {
                yield return item;
            }
        }
    }
}
