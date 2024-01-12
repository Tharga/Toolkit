using System;

namespace Tharga.Toolkit
{
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

        public static string ToDurationString(this DateTime item, DurationOptions options = null)
        {
            options = options ?? new DurationOptions();

            if (options.StringOptions == null)
            {
                options.StringOptions = DurationStringOptionsExtensions.Get("en");
            }

            var duration = (options.BaseValue ?? DateTime.UtcNow) - item.ToUniversalTime();
            string preString;
            string postString;

            var future = false;
            if (duration == TimeSpan.Zero)
            {
                return options.StringOptions.Now;
            }

            if (duration > TimeSpan.Zero)
            {
                preString = string.Empty;
                postString = $" {options.StringOptions.PostString}";
            }
            else
            {
                duration = duration.Negate();
                preString = $"{options.StringOptions.PreString} ";
                postString = string.Empty;
                future = true;
            }

            if (duration.TotalSeconds < 1 || options.MaxUnit == EUnit.Millisecond)
            {
                if (options.MinUnit < EUnit.Millisecond) return future ? options.StringOptions.Soon : options.StringOptions.Resent;
                return $"{preString}{duration.ToStringDurationString(options.StringOptions.Millisecond)}{postString}";
            }

            if (duration.TotalMinutes < 1 || options.MaxUnit == EUnit.Second)
            {
                if (options.MinUnit < EUnit.Second) return future ? options.StringOptions.Soon : options.StringOptions.Resent;
                return $"{preString}{duration.ToStringDurationString(options.StringOptions.Second)}{postString}";
            }

            if (duration.TotalHours < 1 || options.MaxUnit == EUnit.Minute)
            {
                if (options.MinUnit < EUnit.Minute) return future ? options.StringOptions.Soon : options.StringOptions.Resent;
                return $"{preString}{duration.ToStringDurationString(options.StringOptions.Minute)}{postString}";
            }

            if (duration.TotalDays < 1 || options.MaxUnit == EUnit.Hour)
            {
                if (options.MinUnit < EUnit.Hour) return future ? options.StringOptions.Soon : options.StringOptions.Resent;
                return $"{preString}{duration.ToStringDurationString(options.StringOptions.Hour)}{postString}";
            }

            if (duration.TotalDays < 7 || options.MaxUnit == EUnit.Day)
            {
                if (options.MinUnit < EUnit.Day) return future ? options.StringOptions.Soon : options.StringOptions.Resent;
                return $"{preString}{duration.ToStringDurationString(options.StringOptions.Day)}{postString}";
            }

            if (duration.TotalDays < 30 || options.MaxUnit == EUnit.Week)
            {
                if (options.MinUnit < EUnit.Week) return future ? options.StringOptions.Soon : options.StringOptions.Resent;
                return $"{preString}{duration.ToStringDurationString(options.StringOptions.Week)}{postString}";
            }

            if (duration.TotalDays < 365 || options.MaxUnit == EUnit.Month)
            {
                if (options.MinUnit < EUnit.Month) return future ? options.StringOptions.Soon : options.StringOptions.Resent;
                return $"{preString}{duration.ToStringDurationString(options.StringOptions.Month)}{postString}";
            }

            if (options.MinUnit < EUnit.Year) return future ? options.StringOptions.Soon : options.StringOptions.Resent;
            return $"{preString}{duration.ToStringDurationString(options.StringOptions.Year)}{postString}";
        }

        private static string ToStringDurationString(this TimeSpan value, (EUnit Unit, UnitOption Option) unitOption)
        {
            double val;
            switch (unitOption.Unit)
            {
                case EUnit.Year:
                    val = (int)(value.TotalDays / 365);
                    break;
                case EUnit.Month:
                    val = (int)(value.TotalDays / 30);
                    break;
                case EUnit.Week:
                    val = (int)(value.TotalDays / 7);
                    break;
                case EUnit.Day:
                    val = (int)value.TotalDays;
                    break;
                case EUnit.Hour:
                    val = (long)value.TotalHours;
                    break;
                case EUnit.Minute:
                    val = (long)value.TotalMinutes;
                    break;
                case EUnit.Second:
                    val = (long)value.TotalSeconds;
                    break;
                case EUnit.Millisecond:
                    val = (long)value.TotalMilliseconds;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var plural = !(val > 1) ? unitOption.Option.SignularSign : unitOption.Option.PluralSign;

            return $"{val} {unitOption.Option.Value}{plural}";
        }

        public static string ToTimeSpanString(this TimeSpan? item)
        {
            return item == null ? string.Empty : item.Value.ToTimeSpanString();
        }

        public static string ToTimeSpanString(this TimeSpan item, TimeSpanStringOptions options = null)
        {
            options = options ?? TimeSpanStringOptionsExtensions.Get("en");

            if (item.TotalSeconds <= 1)
            {
                //return $"{item.TotalMilliseconds:0} {options.Milliseconds}";
                return ToStringDurationString(item, options.Millisecond);
            }

            if (item.TotalSeconds < 60)
            {
                //return $"{item.TotalSeconds:0} {options.Seconds}";
                return ToStringDurationString(item, options.Second);
            }

            if (item.TotalMinutes < 60)
            {
                //return $"{item.TotalMinutes:0} {options.Minutes}";
                return ToStringDurationString(item, options.Minute);
            }

            if (item.TotalHours < 24)
            {
                //return $"{item.TotalHours:0} {options.Hours}";
                return ToStringDurationString(item, options.Hour);
            }

            //return $"{item.TotalDays:0} {options.Days}";
            return ToStringDurationString(item, options.Day);
        }
    }
}