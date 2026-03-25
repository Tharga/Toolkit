using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Provides extension methods for mapping between enum types by name.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Maps a sequence of values from one enum type to another by matching enum member names.
        /// </summary>
        public static IEnumerable<TTo> MapEnum<TTo, TFrom>(this IEnumerable<TFrom> from) where TTo : struct
        {
            return from.Select(item => item.MapEnum<TTo, TFrom>());
        }

        /// <summary>
        /// Maps a single value from one enum type to another by matching the enum member name.
        /// </summary>
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