using System;
using System.Security.Cryptography;
using System.Text;

namespace Tharga.Toolkit
{
    public static class HashExtensions
    {
        public static string ToHash(this Uri item, HashFormat format)
        {
            if (item == null) return null;
            return ToHash(item.OriginalString, format);
        }

        public static string ToHash(this string item, HashFormat format)
        {
            if (string.IsNullOrEmpty(item))
            {
                return null;
            }

            var data = Encoding.UTF8.GetBytes(item);
            return data.ToHash(format);
        }

        public static string ToHash(this byte[] item, HashFormat format)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(item);
                return hash.Format(format);
            }
        }

        public static string ToSecureHash(this string item, string salt, HashFormat format)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return null;
            }

            var data = Encoding.UTF8.GetBytes(item);
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            return data.ToSecureHash(saltBytes, format);
        }

        public static string ToSecureHash(this byte[] item, byte[] salt, HashFormat format)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(item, salt, 5000);
            var hash = pbkdf2.GetBytes(32);
            var response = hash.Format(format);
            return response;
        }
    }
}