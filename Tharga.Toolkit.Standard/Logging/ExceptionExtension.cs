using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit.Logging
{
    /// <summary>
    /// Provides extension methods for enriching and inspecting exception data.
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// Adds or replaces a key-value pair in the exception's Data dictionary.
        /// </summary>
        public static T AddData<T>(this T item, object key, object value)
            where T : Exception
        {
            if (item.Data.Contains(key)) item.Data.Remove(key);
            item.Data.Add(key, value);
            return item;
        }

        /// <summary>
        /// Attempts to add a key-value pair to the exception's Data dictionary. Returns false if the key already exists.
        /// </summary>
        public static bool TryAddData<T>(this T item, object key, object value)
            where T : Exception
        {
            if (item.Data.Contains(key)) return false;
            item.Data.Add(key, value);
            return true;
        }

        /// <summary>
        /// Converts the exception's Data dictionary into a strongly-typed Dictionary with string keys, filtering out non-string keys.
        /// </summary>
        public static Dictionary<string, object> ToDictionary<T>(this T item)
            where T : Exception
        {
            return item.Data
                .Cast<DictionaryEntry>()
                .Where(x => x.Key is string)
                .ToDictionary(
                    x => (string)x.Key,
                    x => x.Value
                );
        }
    }
}