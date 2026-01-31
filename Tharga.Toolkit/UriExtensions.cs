using System;
using System.Collections.Generic;

namespace Tharga.Toolkit;

public static class UriExtensions
{
    public static Uri RemoveQuery(this Uri uri)
    {
        var builder = new UriBuilder(uri)
        {
            Query = string.Empty
        };

        return builder.Uri;
    }

    public static IEnumerable<string> GetQueryValue(this Uri uri, string name)
    {
        var query = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in query)
        {
            var kvp = part.Split('=', 2);

            if (kvp.Length == 2 && kvp[0] == name)
            {
                yield return Uri.UnescapeDataString(kvp[1]);
            }
        }
    }
}