using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit.Logging
{
    public static class ExceptionExtension
    {
        public static T AddData<T>(this T item, object key, object value)
            where T : Exception
        {
            if (item.Data.Contains(key)) item.Data.Remove(key);
            item.Data.Add(key, value);
            return item;
        }

        public static bool TryAddData<T>(this T item, object key, object value)
            where T : Exception
        {
            if (item.Data.Contains(key)) return false;
            item.Data.Add(key, value);
            return true;
        }

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