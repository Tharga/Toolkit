using System;
using System.Collections.Generic;

namespace Tharga.Toolkit
{
    public static class TimeSpanStringOptionsExtensions
    {
        /// <summary>
        /// The culture 'en-US' will return English, this is default.
        /// sv-SE will return Swedish.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static TimeSpanStringOptions Get(string culture)
        {
            var parts = culture.Split('-');

            switch (parts[0].ToLower())
            {
                case "en":
                    return new TimeSpanStringOptions
                    {
                        UnitOptions = new Dictionary<EUnit, UnitOption>
                        {
                            { EUnit.Millisecond, new UnitOption { Value = "ms" } },
                            { EUnit.Second, new UnitOption { Value = "second", PluralSign = "s" } },
                            { EUnit.Minute, new UnitOption { Value = "minute", PluralSign = "s" } },
                            { EUnit.Hour, new UnitOption { Value = "hour", PluralSign = "s" } },
                            { EUnit.Day, new UnitOption { Value = "day", PluralSign = "s" } },
                            { EUnit.Week, new UnitOption { Value = "week", PluralSign = "s" } },
                            { EUnit.Month, new UnitOption { Value = "month", PluralSign = "s" } },
                            { EUnit.Year, new UnitOption { Value = "year", PluralSign = "s" } }
                        },
                    };
                case "sv":
                    return new TimeSpanStringOptions
                    {
                        UnitOptions = new Dictionary<EUnit, UnitOption>
                        {
                            { EUnit.Millisecond, new UnitOption { Value = "ms" } },
                            { EUnit.Second, new UnitOption { Value = "sekund", PluralSign = "er" } },
                            { EUnit.Minute, new UnitOption { Value = "minut", PluralSign = "er" } },
                            { EUnit.Hour, new UnitOption { Value = "timma", PluralSign = "r" } },
                            { EUnit.Day, new UnitOption { Value = "dag", PluralSign = "ar" } },
                            { EUnit.Week, new UnitOption { Value = "veck", SignularSign = "a", PluralSign = "or" } },
                            { EUnit.Month, new UnitOption { Value = "månad", PluralSign = "er" } },
                            { EUnit.Year, new UnitOption { Value = "år" } }
                        },
                    };
                default:
                    throw new ArgumentOutOfRangeException($"Unknown culture '{culture}'.");
            }

        }
    }
}