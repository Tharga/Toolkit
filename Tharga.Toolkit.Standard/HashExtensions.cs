using System;
using System.Security.Cryptography;
using System.Text;

namespace Tharga.Toolkit
{
    public static class HashExtensions
    {
        public enum Style
        {
            Regular,
            Base64
        }

        [Obsolete("Use ToHash with style.")]
        public static string ToHash(this Uri item)
        {
            return ToHash(item, Style.Regular);
        }

        public static string ToHash(this Uri item, Style style)
        {
            if (item == null) return null;
            return ToHash(item.OriginalString, style);
        }

        [Obsolete("Use ToHash with style.")]
        public static string ToHash(this string item)
        {
            return ToHash(item, Style.Regular);
        }

        public static string ToHash(this string item, Style style)
        {
            if (string.IsNullOrEmpty(item))
            {
                return null;
            }

            var data = Encoding.UTF8.GetBytes(item);
            return data.ToHash(style);
        }

        [Obsolete("Use ToHash with style.")]
        public static string ToHash(this byte[] item)
        {
            return ToHash(item, Style.Regular);
        }

        public static string ToHash(this byte[] item, Style style)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(item);
                return Format(style, hash);
            }
        }

        [Obsolete("Use ToSecureHash with style.")]
        public static string ToSecureHash(this string item, string salt)
        {
            return ToSecureHash(item, salt, Style.Regular);
        }

        public static string ToSecureHash(this string item, string salt, Style style)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return null;
            }

            var data = Encoding.UTF8.GetBytes(item);
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            return data.ToSecureHash(saltBytes, style);
        }

        [Obsolete("Use ToSecureHash with style.")]
        public static string ToSecureHash(this byte[] item, byte[] salt)
        {
            return ToSecureHash(item, salt, Style.Regular);
        }

        public static string ToSecureHash(this byte[] item, byte[] salt, Style style)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(item, salt, 5000);
            var hash = pbkdf2.GetBytes(32);
            var response = Format(style, hash);
            return response;
        }

        private static string Format(Style style, byte[] hash)
        {
            switch (style)
            {
                case Style.Regular:
                    return BitConverter.ToString(hash).Replace("-", "");
                case Style.Base64:
                    return Convert.ToBase64String(hash);
                default:
                    throw new ArgumentOutOfRangeException($"Unknown style '{style}'.");
            }
        }
    }
}