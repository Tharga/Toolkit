using System;

namespace Tharga.Toolkit
{
    public enum EMaxUnit
    {
        Year,
        Month,
        Week,
        Day,
        Hour,
        Minute,
        Second,
        Millisecond
    }

    public static class DateTimeExtensions
    {
        public static string ToLocalDateTimeString(this DateTime? item, string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (item == null) return string.Empty;
            return item.Value.ToLocalTime().ToString(format);
        }

        public static string ToLocalDateTimeString(this DateTime item, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return item.ToLocalTime().ToString(format);
        }

        public static string ToLocalDurationString(this DateTime? item)
        {
            if (item == null) return string.Empty;
            return item.Value.ToLocalTime().ToDurationString();
        }

        public static string ToLocalDurationString(this DateTime item)
        {
            return item.ToLocalTime().ToDurationString();
        }

        public static string ToDateTimeString(this DateTime? item)
        {
            return item == null ? string.Empty : item.Value.ToDateTimeString();
        }

        public static string ToDateTimeString(this DateTime item)
        {
            return $"{item.ToShortDateString()} {item.ToLongTimeString()}";
        }

        public static string ToTimeString(this TimeSpan timeSpan)
        {
            return $"{timeSpan.Hours}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        public static string ToDurationString(this DateTime? item)
        {
            return item == null ? string.Empty : item.Value.ToDurationString();
        }

        public static string ToDurationString(this DateTime item, EMaxUnit maxUnit = EMaxUnit.Day)
        {
            var duration = DateTime.UtcNow - item.ToUniversalTime();
            string preString;
            string postString;

            if (duration > TimeSpan.Zero)
            {
                preString = string.Empty;
                postString = " ago";
            }
            else
            {
                duration = duration.Negate();
                preString = "In ";
                postString = string.Empty;
            }

            if (duration.TotalSeconds < 1 || maxUnit == EMaxUnit.Millisecond)
            {
                var r = GetPlural(duration.TotalMilliseconds);
                return $"{preString}{r.Value} millisecond{r.Plural}{postString}";
            }

            if (duration.TotalMinutes < 1 || maxUnit == EMaxUnit.Second)
            {
                var r = GetPlural(duration.TotalSeconds);
                return $"{preString}{r.Value} second{r.Plural}{postString}";
            }

            if (duration.TotalHours < 1 || maxUnit == EMaxUnit.Minute)
            {
                var r = GetPlural(duration.TotalMinutes);
                return $"{preString}{r.Value} minute{r.Plural}{postString}";
            }

            if (duration.TotalDays < 1 || maxUnit == EMaxUnit.Hour)
            {
                var r = GetPlural(duration.TotalHours);
                return $"{preString}{r.Value} hour{r.Plural}{postString}";
            }

            if (duration.TotalDays < 7 || maxUnit == EMaxUnit.Day)
            {
                var r = GetPlural(duration.TotalDays);
                return $"{preString}{r.Value} day{r.Plural}{postString}";
            }

            if (duration.TotalDays < 30 || maxUnit == EMaxUnit.Week)
            {
                var r = GetPlural(duration.TotalDays / 7);
                return $"{preString}{r.Value} week{r.Plural}{postString}";
            }

            if (duration.TotalDays < 365 || maxUnit == EMaxUnit.Month)
            {
                var r = GetPlural(duration.TotalDays / 30);
                return $"{preString}{r.Value} month{r.Plural}{postString}";
            }

            {
                var r = GetPlural(duration.TotalDays / 365);
                return $"{preString}{r.Value} year{r.Plural}{postString}";
            }
        }

        private static (int Value, string Plural) GetPlural(double value)
        {
            if ((int) value == 1)
                return (1, string.Empty);
            return ((int)value, "s");
        }

        public static string ToTimeSpanString(this TimeSpan? item)
        {
            return item == null ? string.Empty : item.Value.ToTimeSpanString();
        }

        public static string ToTimeSpanString(this TimeSpan item)
        {
            if (item.TotalSeconds <= 1)
            {
                return $"{item.TotalMilliseconds:0} ms";
            }

            if (item.TotalSeconds < 60)
            {
                return $"{item.TotalSeconds:0} seconds";
            }

            if (item.TotalMinutes < 60)
            {
                return $"{item.TotalMinutes:0} minutes";
            }

            if (item.TotalHours < 24)
            {
                return $"{item.TotalHours:0} hours";
            }

            return $"{item.TotalDays:0} days";
        }
    }
}