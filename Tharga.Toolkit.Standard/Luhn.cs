using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Provides extension methods for computing and validating Luhn check digits.
    /// </summary>
    public static class Luhn
    {
        private static readonly int[] Results = { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

        /// <summary>
        /// Computes the Luhn check digit for a list of digits.
        /// </summary>
        /// <param name="digits">The list of digits to compute the check digit for.</param>
        /// <returns>The computed Luhn check digit.</returns>
        public static int CheckDigit(this IList<int> digits)
        {
            var i = 0;
            var lengthMod = digits.Count % 2;
            return digits.Sum(d => i++ % 2 == lengthMod ? d : Results[d]) * 9 % 10;
        }

        /// <summary>
        /// Appends the Luhn check digit to the end of a list of digits.
        /// </summary>
        /// <param name="digits">The list of digits to append the check digit to.</param>
        /// <returns>The original list with the check digit appended.</returns>
        public static IList<int> AppendCheckDigit(this IList<int> digits)
        {
            var result = digits;
            result.Add(digits.CheckDigit());
            return result;
        }

        /// <summary>
        /// Validates whether the last digit in the list is a valid Luhn check digit.
        /// </summary>
        /// <param name="digits">The list of digits including the check digit as the last element.</param>
        /// <returns>True if the check digit is valid; otherwise, false.</returns>
        public static bool HasValidCheckDigit(this IList<int> digits)
        {
            return digits.Last() == CheckDigit(digits.Take(digits.Count - 1).ToList());
        }

        private static IList<int> ToDigitList(this string digits)
        {
            return digits.Select(d => d - 48).ToList();
        }

        /// <summary>
        /// Computes the Luhn check digit for a string of digit characters.
        /// </summary>
        /// <param name="digits">The string of digit characters to compute the check digit for.</param>
        /// <returns>The computed Luhn check digit as a string.</returns>
        public static string CheckDigit(this string digits)
        {
            return digits.ToDigitList().CheckDigit().ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Appends the Luhn check digit to the end of a string of digit characters.
        /// </summary>
        /// <param name="digits">The string of digit characters.</param>
        /// <returns>The original string with the check digit appended.</returns>
        public static string AppendCheckDigit(this string digits)
        {
            return digits + digits.CheckDigit();
        }

        /// <summary>
        /// Validates whether the last character in the string is a valid Luhn check digit.
        /// </summary>
        /// <param name="digits">The string of digit characters including the check digit as the last character.</param>
        /// <returns>True if the check digit is valid; otherwise, false.</returns>
        public static bool HasValidCheckDigit(this string digits)
        {
            return digits.ToDigitList().HasValidCheckDigit();
        }

        private static IList<int> ToDigitList(this int digits)
        {
            return digits.ToString(CultureInfo.InvariantCulture).Select(d => d - 48).ToList();
        }

        /// <summary>
        /// Computes the Luhn check digit for an integer value.
        /// </summary>
        /// <param name="digits">The integer value to compute the check digit for.</param>
        /// <returns>The computed Luhn check digit.</returns>
        public static int CheckDigit(this int digits)
        {
            return digits.ToDigitList().CheckDigit();
        }

        /// <summary>
        /// Appends the Luhn check digit to an integer by multiplying by 10 and adding the check digit.
        /// </summary>
        /// <param name="digits">The integer value.</param>
        /// <returns>The integer with the check digit appended as the last digit.</returns>
        public static int AppendCheckDigit(this int digits)
        {
            return digits * 10 + digits.CheckDigit();
        }

        /// <summary>
        /// Validates whether the last digit of the integer is a valid Luhn check digit.
        /// </summary>
        /// <param name="digits">The integer value including the check digit as the last digit.</param>
        /// <returns>True if the check digit is valid; otherwise, false.</returns>
        public static bool HasValidCheckDigit(this int digits)
        {
            return digits.ToDigitList().HasValidCheckDigit();
        }

        private static IList<int> ToDigitList(this long digits)
        {
            return digits.ToString(CultureInfo.InvariantCulture).Select(d => d - 48).ToList();
        }

        /// <summary>
        /// Computes the Luhn check digit for a long integer value.
        /// </summary>
        /// <param name="digits">The long integer value to compute the check digit for.</param>
        /// <returns>The computed Luhn check digit.</returns>
        public static int CheckDigit(this long digits)
        {
            return digits.ToDigitList().CheckDigit();
        }

        /// <summary>
        /// Appends the Luhn check digit to a long integer by multiplying by 10 and adding the check digit.
        /// </summary>
        /// <param name="digits">The long integer value.</param>
        /// <returns>The long integer with the check digit appended as the last digit.</returns>
        public static long AppendCheckDigit(this long digits)
        {
            return digits * 10 + digits.CheckDigit();
        }

        /// <summary>
        /// Validates whether the last digit of the long integer is a valid Luhn check digit.
        /// </summary>
        /// <param name="digits">The long integer value including the check digit as the last digit.</param>
        /// <returns>True if the check digit is valid; otherwise, false.</returns>
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