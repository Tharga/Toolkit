using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tharga.Toolkit
{
    public static class Luhn
    {
        private static readonly int[] Results = { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

        public static int CheckDigit(this IList<int> digits)
        {
            var i = 0;
            var lengthMod = digits.Count % 2;
            return digits.Sum(d => i++ % 2 == lengthMod ? d : Results[d]) * 9 % 10;
        }

        public static IList<int> AppendCheckDigit(this IList<int> digits)
        {
            var result = digits;
            result.Add(digits.CheckDigit());
            return result;
        }

        public static bool HasValidCheckDigit(this IList<int> digits)
        {
            return digits.Last() == CheckDigit(digits.Take(digits.Count - 1).ToList());
        }

        private static IList<int> ToDigitList(this string digits)
        {
            return digits.Select(d => d - 48).ToList();
        }

        public static string CheckDigit(this string digits)
        {
            return digits.ToDigitList().CheckDigit().ToString(CultureInfo.InvariantCulture);
        }

        public static string AppendCheckDigit(this string digits)
        {
            return digits + digits.CheckDigit();
        }

        public static bool HasValidCheckDigit(this string digits)
        {
            return digits.ToDigitList().HasValidCheckDigit();
        }

        private static IList<int> ToDigitList(this int digits)
        {
            return digits.ToString(CultureInfo.InvariantCulture).Select(d => d - 48).ToList();
        }

        public static int CheckDigit(this int digits)
        {
            return digits.ToDigitList().CheckDigit();
        }

        public static int AppendCheckDigit(this int digits)
        {
            return digits * 10 + digits.CheckDigit();
        }

        public static bool HasValidCheckDigit(this int digits)
        {
            return digits.ToDigitList().HasValidCheckDigit();
        }

        private static IList<int> ToDigitList(this long digits)
        {
            return digits.ToString(CultureInfo.InvariantCulture).Select(d => d - 48).ToList();
        }

        public static int CheckDigit(this long digits)
        {
            return digits.ToDigitList().CheckDigit();
        }

        public static long AppendCheckDigit(this long digits)
        {
            return digits * 10 + digits.CheckDigit();
        }

        public static bool HasValidCheckDigit(this long digits)
        {
            return digits.ToDigitList().HasValidCheckDigit();
        }

        //public static bool IsValid(string digits)
        //{
        //    return digits.All(char.IsDigit) && digits.Reverse()
        //        .Select(c => c - 48)
        //        .Select((thisNum, i) => i % 2 == 0
        //            ? thisNum
        //            : ((thisNum *= 2) > 9 ? thisNum - 9 : thisNum)
        //        ).Sum() % 10 == 0;
        //}
    }
}