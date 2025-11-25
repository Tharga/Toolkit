using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit
{
    public static class DurationStringOptionsExtensions
    {
        /// <summary>
        ///     Get by culture.
        ///     The culture 'en-US' will return English, this is default.
        ///     sv-SE will return Swedish.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DurationStringOptions Get(string culture)
        {
            var parts = culture.Split('-');

            if (!Enum.TryParse<Language>(parts.First(), true, out var language))
                throw new InvalidOperationException($"Unknown culture '{culture}'.");

            return Get(language);
        }

        /// <summary>
        ///     Get by language.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DurationStringOptions Get(Language language)
        {
            switch (language)
            {
                case Language.En:
                    return new DurationStringOptions
                    {
                        PreString = "In",
                        PostString = "ago",
                        UnitOptions = new Dictionary<EUnit, UnitOption>
                        {
                            { EUnit.Millisecond, new UnitOption { Value = "millisecond", PluralSign = "s" } },
                            { EUnit.Second, new UnitOption { Value = "second", PluralSign = "s" } },
                            { EUnit.Minute, new UnitOption { Value = "minute", PluralSign = "s" } },
                            { EUnit.Hour, new UnitOption { Value = "hour", PluralSign = "s" } },
                            { EUnit.Day, new UnitOption { Value = "day", PluralSign = "s" } },
                            { EUnit.Week, new UnitOption { Value = "week", PluralSign = "s" } },
                            { EUnit.Month, new UnitOption { Value = "month", PluralSign = "s" } },
                            { EUnit.Year, new UnitOption { Value = "year", PluralSign = "s" } }
                        },
                        Now = new Dictionary<EUnit, string>
                        {
                            { EUnit.Millisecond, "Now" },
                            { EUnit.Second, "Now" },
                            { EUnit.Minute, "This minute" },
                            { EUnit.Hour, "This hour" },
                            { EUnit.Day, "Today" },
                            { EUnit.Week, "This week" },
                            { EUnit.Month, "This month" },
                            { EUnit.Year, "This year" },
                        },
                        Resent = new Dictionary<EUnit, string>
                        {
                            { EUnit.Millisecond, "Just now" },
                            { EUnit.Second, "Just now" },
                            { EUnit.Minute, "A minute ago" },
                            { EUnit.Hour, "This hour" },
                            { EUnit.Day, "Today" },
                            { EUnit.Week, "This week" },
                            { EUnit.Month, "This month" },
                            { EUnit.Year, "This year" },
                        },
                        Soon = new Dictionary<EUnit, string>
                        {
                            { EUnit.Millisecond, "Soon" },
                            { EUnit.Second, "Soon" },
                            { EUnit.Minute, "Soon" },
                            { EUnit.Hour, "Later today" },
                            { EUnit.Day, "Tomorrow" },
                            { EUnit.Week, "This week" },
                            { EUnit.Month, "This month" },
                            { EUnit.Year, "This year" },
                        },
                    };
                case Language.Sv:
                    return new DurationStringOptions
                    {
                        PreString = "Om",
                        PostString = "sedan",
                        UnitOptions = new Dictionary<EUnit, UnitOption>
                        {
                            { EUnit.Millisecond, new UnitOption { Value = "millisekund", PluralSign = "er" } },
                            { EUnit.Second, new UnitOption { Value = "sekund", PluralSign = "er" } },
                            { EUnit.Minute, new UnitOption { Value = "minut", PluralSign = "er" } },
                            { EUnit.Hour, new UnitOption { Value = "timma", PluralSign = "r" } },
                            { EUnit.Day, new UnitOption { Value = "dag", PluralSign = "ar" } },
                            { EUnit.Week, new UnitOption { Value = "veck", SignularSign = "a", PluralSign = "or" } },
                            { EUnit.Month, new UnitOption { Value = "månad", PluralSign = "er" } },
                            { EUnit.Year, new UnitOption { Value = "år" } }
                        },
                        Now = new Dictionary<EUnit, string>
                        {
                            { EUnit.Millisecond, "Nu" },
                            { EUnit.Second, "Nu" },
                            { EUnit.Minute, "Nu" },
                            { EUnit.Hour, "Nu" },
                            { EUnit.Day, "Idag" },
                            { EUnit.Week, "Denna vecka" },
                            { EUnit.Month, "Denna månad" },
                            { EUnit.Year, "I år" },
                        },
                        Resent = new Dictionary<EUnit, string>
                        {
                            { EUnit.Millisecond, "Nyss" },
                            { EUnit.Second, "Nyss" },
                            { EUnit.Minute, "Nyss" },
                            { EUnit.Hour, "Nyss" },
                            { EUnit.Day, "Idag" },
                            { EUnit.Week, "Denna vecka" },
                            { EUnit.Month, "Denna månad" },
                            { EUnit.Year, "Under året" },
                        },
                        Soon = new Dictionary<EUnit, string>
                        {
                            { EUnit.Millisecond, "Snart" },
                            { EUnit.Second, "Snart" },
                            { EUnit.Minute, "Snart" },
                            { EUnit.Hour, "Senare idag" },
                            { EUnit.Day, "Idag" },
                            { EUnit.Week, "I veckan" },
                            { EUnit.Month, "Denna månad" },
                            { EUnit.Year, "I år" },
                        },
                    };
                default:
                    throw new ArgumentOutOfRangeException($"Unknown language '{language}'.");
            }
        }
    }
}