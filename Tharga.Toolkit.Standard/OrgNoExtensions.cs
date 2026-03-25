using System.Text.RegularExpressions;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Provides extension methods for parsing and validating Swedish organization numbers.
    /// </summary>
    public static class OrgNoExtensions
    {
        /// <summary>
        /// Attempts to parse and validate a Swedish organization number from the input string.
        /// </summary>
        public static bool TryParseOrgNo(this string input, out string orgNo)
        {
            return TryParseOrgNo(input, out orgNo, out _);
        }

        /// <summary>
        /// Attempts to parse and validate a Swedish organization number from the input string,
        /// providing an error type if parsing fails.
        /// </summary>
        public static bool TryParseOrgNo(this string input, out string orgNo, out ErrorType? errorType)
        {
            orgNo = default;
            errorType = default;

            if (string.IsNullOrEmpty(input))
            {
                errorType = ErrorType.NoValue;
                return false;
            }

            var item = Regex.Replace(input, @"\D", "");
            if (item.Length != 10)
            {
                errorType = ErrorType.InvalidFormat;
                return false;
            }

            if (item.HasValidCheckDigit())
            {
                orgNo = $"{item.Substring(0, 6)}-{item.Substring(6)}";
                return true;
            }

            errorType = ErrorType.InvalidCheckDigit;
            return false;
        }
    }
}