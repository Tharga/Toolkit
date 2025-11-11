using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit;

public static class EnumerableExtensionsAsync
{
    private static readonly Lazy<Random> _rng = new(() => new Random());

    public static async Task<T> TakeRandomAsync<T>(this IAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
    {
        await using var enumerator = values.GetAsyncEnumerator(cancellationToken);

        if (!await enumerator.MoveNextAsync()) return default;

        var result = enumerator.Current;
        var count = 1;

        while (await enumerator.MoveNextAsync())
        {
            count++;
            if (_rng.Value.Next(count) == 0)
            {
                result = enumerator.Current;
            }
        }

        return result;
    }

    public static async IAsyncEnumerable<T> RandomOrderAsync<T>(this IAsyncEnumerable<T> values, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var list = new List<T>();
        await foreach (var item in values.WithCancellation(cancellationToken))
        {
            list.Add(item);
        }

        //Fisher–Yates shuffle
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = _rng.Value.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        foreach (var item in list)
        {
            yield return item;
        }
    }
}