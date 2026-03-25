using System;
using System.Collections.Generic;

namespace Tharga.Toolkit;

/// <summary>
/// Extension methods for working with <see cref="Uri"/> instances.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Returns a new URI with the query string removed.
    /// </summary>
    public static Uri RemoveQuery(this Uri uri)
    {
        var builder = new UriBuilder(uri)
        {
            Query = string.Empty
        };

        return builder.Uri;
    }

    /// <summary>
    /// Gets all values for a given query parameter name from the URI.
    /// Values are URL-decoded.
    /// </summary>
    /// <param name="uri">The URI to extract query values from.</param>
    /// <param name="name">The query parameter name to match.</param>
    /// <returns>An enumerable of matching query parameter values.</returns>
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