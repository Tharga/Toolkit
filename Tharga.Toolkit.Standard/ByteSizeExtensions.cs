using System;

public static class ByteSizeExtensions
{
    private static readonly string[] ShortUnits = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
    private static readonly string[] FullUnits = { "Bytes", "Kilobytes", "Megabytes", "Gigabytes", "Terabytes", "Petabytes", "Exabytes" };

    public static string ToReadableByteSize(this int byteCount, bool useFullUnit = false, int decimalPlaces = 0)
    {
        return ((long)byteCount).ToReadableByteSize(useFullUnit, decimalPlaces);
    }

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