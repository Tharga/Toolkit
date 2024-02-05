using System;

namespace Tharga.Toolkit.Tests;

internal static class DateTestHelper
{
    public static DateTime? GetDateTime(this ETestCase testCase)
    {
        DateTime? now = testCase switch
        {
            ETestCase.Now => DateTime.Now,
            ETestCase.UtcNow => DateTime.UtcNow,
            ETestCase.Min => DateTime.MinValue,
            ETestCase.Max => DateTime.MaxValue,
            ETestCase.Zero => new DateTime(),
            ETestCase.Null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(testCase), testCase, null)
        };
        return now;
    }

    public static TimeSpan? GetTimeSpan(this ETestCase testCase)
    {
        TimeSpan? now = testCase switch
        {
            ETestCase.Min => TimeSpan.MinValue,
            ETestCase.Max => TimeSpan.MaxValue,
            ETestCase.Zero => TimeSpan.Zero,
            ETestCase.Null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(testCase), testCase, null)
        };
        return now;
    }
}