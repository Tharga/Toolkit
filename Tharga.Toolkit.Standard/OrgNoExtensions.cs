using System.Text.RegularExpressions;

namespace Tharga.Toolkit
{
    public static class OrgNoExtensions
    {
        public static bool TryParseOrgNo(this string input, out string orgNo)
        {
            return TryParseOrgNo(input, out orgNo, out _);
        }

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