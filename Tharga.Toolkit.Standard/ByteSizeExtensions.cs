using System;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Provides extension methods for converting byte counts into human-readable size strings.
    /// </summary>
    public static class ByteSizeExtensions
    {
        private static readonly string[] ShortUnits = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        private static readonly string[] FullUnits = { "Bytes", "Kilobytes", "Megabytes", "Gigabytes", "Terabytes", "Petabytes", "Exabytes" };

        /// <summary>
        /// Converts an integer byte count into a human-readable size string (e.g. "1 KB" or "1 Kilobytes").
        /// </summary>
        public static string ToReadableByteSize(this int byteCount, bool useFullUnit = false, int decimalPlaces = 0)
        {
            return ((long)byteCount).ToReadableByteSize(useFullUnit, decimalPlaces);
        }

        /// <summary>
        /// Converts a long byte count into a human-readable size string with automatic unit scaling (e.g. "1 KB" or "1 Kilobytes").
        /// </summary>
        public static string ToReadableByteSize(this long byteCount, bool useFullUnit = false, int decimalPlaces = 0)
        {
            if (byteCount < 0) throw new ArgumentOutOfRangeException(nameof(byteCount), "Byte count cannot be negative.");

            if (byteCount == 0) return $"0 {(useFullUnit ? FullUnits[0] : ShortUnits[0])}";

            var unitIndex = (int)Math.Floor(Math.Log(byteCount, 1024));
            unitIndex = Math.Min(unitIndex, ShortUnits.Length - 1);

            var adjustedSize = byteCount / Math.Pow(1024, unitIndex);
            var format = $"F{decimalPlaces}";

            var unit = useFullUnit ? FullUnits[unitIndex] : ShortUnits[unitIndex];

            return $"{adjustedSize.ToString(format)} {unit}";
        }
    }
}