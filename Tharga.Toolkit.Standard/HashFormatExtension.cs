using System;

namespace Tharga.Toolkit
{
    public static class HashFormatExtension
    {
        public static string Format(this byte[] hash, HashFormat format)
        {
            switch (format)
            {
                case HashFormat.Hex:
                    return BitConverter.ToString(hash).Replace("-", "");
                case HashFormat.Base64:
                    return Convert.ToBase64String(hash);
                default:
                    throw new ArgumentOutOfRangeException($"Unknown style '{format}'.");
            }
        }
    }
}