using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Extension methods and utilities for string manipulation, random string generation, and Base64 encoding.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Returns <c>null</c> if the string is empty; otherwise returns the original string.
        /// </summary>
        public static string NullIfEmpty(this string item)
        {
            if (item == string.Empty)
            {
                return null;
            }

            return item;
        }

        /// <summary>
        /// Returns <c>true</c> if the string is null or empty.
        /// </summary>
        public static bool IsNullOrEmpty(this string item)
        {
            return string.IsNullOrEmpty(item);
        }

        /// <summary>
        /// Returns a fallback value if the string is null or empty.
        /// </summary>
        public static string IfEmpty(this string item, string value)
        {
            if (string.IsNullOrEmpty(item)) return value;
            return item;
        }

        public const string OctCharacters = "01234567";
        public const string HexCharacters = "0123456789ABCDEF";
        public const string BinaryCharacters = "01";
        public const string NumericCharacters = "0123456789";
        public const string LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz";
        public const string UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string LowerCaseAlphaNumericCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
        public const string UpperCaseAlphaNumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public const string AlphaNumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public const string UriSafeCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        public const string ExtendedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%()*+,-.:;<=>?@[]^_`{|}~";
        //public const string Base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        //public const string Base64AlphabetWithPadding = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        /// <summary>
        /// Returns a random string of alphanumeric characters given a specific length.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="availableCharacters"></param>
        /// <returns></returns>
        public static string RandomString(this int length, string availableCharacters = AlphaNumericCharacters)
        {
            return GetRandomString(length, length, availableCharacters);
        }

        /// <summary>
        /// Returns a random string from provided array.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Random(this string characters, int minLength = 8, int maxLength = 16)
        {
            var availableCharacters = new string(characters.Distinct().ToArray());
            return GetRandomString(minLength, maxLength, availableCharacters);
        }

        [Obsolete("Use GetRandomString instead.")]
        public static string RandomString(int minLength = 8, int maxLength = 16, string availableCharacters = AlphaNumericCharacters)
        {
            return GetRandomString(minLength, maxLength, availableCharacters);
        }

        /// <summary>
        /// Generates a cryptographically random string with length between <paramref name="minLength"/> and <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="minLength">Minimum length of the generated string.</param>
        /// <param name="maxLength">Maximum length of the generated string.</param>
        /// <param name="availableCharacters">The character set to draw from.</param>
        /// <exception cref="ArgumentException">Thrown when the character set is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the length range is invalid.</exception>
        public static string GetRandomString(int minLength = 8, int maxLength = 16, string availableCharacters = AlphaNumericCharacters)
        {
            if (string.IsNullOrEmpty(availableCharacters)) throw new ArgumentException("Character set cannot be null or empty.", nameof(availableCharacters));

            if (minLength < 0 || maxLength < minLength) throw new ArgumentOutOfRangeException(nameof(minLength), "Invalid length range.");

            var length = GetSecureInt(minLength, maxLength + 1);
            var chars = new char[length];

            for (var i = 0; i < length; i++)
            {
                var index = GetSecureInt(0, availableCharacters.Length);
                chars[i] = availableCharacters[index];
            }

            return new string(chars);
        }

        private static int GetSecureInt(int minValue, int maxValue)
        {
            if (minValue >= maxValue) throw new ArgumentOutOfRangeException(nameof(minValue));

            var range = (uint)(maxValue - minValue);
            var buffer = new byte[4];

            using (var rng = RandomNumberGenerator.Create())
            {
                while (true)
                {
                    rng.GetBytes(buffer);
                    var value = BitConverter.ToUInt32(buffer, 0);

                    if (value < uint.MaxValue - (uint.MaxValue % range))
                    {
                        return (int)(minValue + (value % range));
                    }
                }
            }
        }

        /// <summary>
        /// Encodes the string to Base64 using UTF-8. Returns <c>null</c> if the input is null.
        /// </summary>
        public static string ToBase64(this string item)
        {
            if (item == null) return null;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(item));
        }

        /// <summary>
        /// Decodes a Base64 string to its UTF-8 representation. Returns <c>null</c> if the input is null.
        /// </summary>
        public static string FromBase64(this string item)
        {
            if (item == null) return null;
            return Encoding.UTF8.GetString(Convert.FromBase64String(item));
        }

        /// <summary>
        /// Truncates the string to the specified maximum length.
        /// </summary>
        public static string Truncate(this string item, int maxLength)
        {
            if (item.Length <= maxLength) return item;
            return item.Substring(0, maxLength);
        }
    }
}