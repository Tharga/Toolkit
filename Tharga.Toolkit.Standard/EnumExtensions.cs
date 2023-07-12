using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit
{
    public static class EnumExtensions
    {
        public static IEnumerable<TTo> MapEnum<TTo, TFrom>(this IEnumerable<TFrom> from) where TTo : struct
        {
            return from.Select(item => item.MapEnum<TTo, TFrom>());
        }

        public static TTo MapEnum<TTo, TFrom>(this TFrom from) where TTo : struct
        {
            if (!typeof(TTo).IsEnum) throw new InvalidOperationException($"The to-type is not an enum, it is of type {typeof(TTo)}.");
            if (!typeof(TFrom).IsEnum) throw new InvalidOperationException($"The from-type is not an enum, it is of type {typeof(TTo)}.");

            try
            {
                return (TTo)Enum.Parse(typeof(TTo), from.ToString());
            }
            catch
            {
                throw new InvalidOperationException($"Cannot convert {from} from enum {typeof(TFrom)} to enum {typeof(TTo)}.");
            }
        }
    }
}