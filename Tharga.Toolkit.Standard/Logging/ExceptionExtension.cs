﻿using System;

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
    }
}