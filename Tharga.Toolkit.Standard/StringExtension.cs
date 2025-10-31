using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tharga.Toolkit
{
    public static class StringExtension
    {
        public static string NullIfEmpty(this string item)
        {
            if (item == string.Empty)
            {
                return null;
            }

            return item;
        }

        public static bool IsNullOrEmpty(this string item)
        {
            return string.IsNullOrEmpty(item);
        }

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

        public static string ToBase64(this string item)
        {
            var bytes = Encoding.UTF8.GetBytes(item);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string item)
        {
            var bytes = Convert.FromBase64String(item);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}