using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tharga.Toolkit;

public static class HashExtensions
{
    public static Hash ToHash(this byte[] item, HashType type = HashType.MD5)
    {
        switch (type)
        {
            case HashType.MD5:
            {
                using var hash = MD5.Create();
                return hash.ComputeHash(item);
            }

            case HashType.SHA1:
            {
                using var hash = SHA1.Create();
                return hash.ComputeHash(item);
            }

            case HashType.SHA256:
            {
                using var hash = SHA256.Create();
                return hash.ComputeHash(item);
            }

            case HashType.SHA384:
            {
                using var hash = SHA384.Create();
                return hash.ComputeHash(item);
            }

            case HashType.SHA512:
            {
                using var hash = SHA512.Create();
                return hash.ComputeHash(item);
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static Hash ToHash(this string item, HashType type = HashType.MD5, Encoding encoding = null)
    {
        if (string.IsNullOrEmpty(item)) return null;

        encoding ??= Encoding.UTF8;

        var data = encoding.GetBytes(item);
        return data.ToHash(type);
    }

    public static Hash ToHash(this Uri item, HashType type = HashType.MD5, Encoding encoding = null)
    {
        if (item == null) return null;
        return item.OriginalString.ToHash(type, encoding);
    }

    public static HashString ToHash(this byte[] item, HashFormat format, HashType type = HashType.MD5)
    {
        var hash = item.ToHash(type);
        return hash.FormatHash(format);
    }

    public static HashString ToHash(this string item, HashFormat format, HashType type = HashType.MD5, Encoding encoding = null)
    {
        if (string.IsNullOrEmpty(item)) return null;

        var hash = item.ToHash(type, encoding);
        return hash.FormatHash(format);
    }

    public static HashString ToHash(this Uri item, HashFormat format, HashType type = HashType.MD5, Encoding encoding = null)
    {
        if (item == null) return null;

        var hash = item.ToHash(type, encoding);
        return hash.FormatHash(format);
    }

    public static Task<Hash> ToHashAsync(this Stream stream, HashType type = HashType.MD5)
    {
        return ComputeHashCoreAsync(stream, null, type);
    }

    public static async Task<HashString> ToHashAsync(this Stream stream, HashFormat format, HashType type = HashType.MD5)
    {
        var result = await stream.ToHashAsync(type);
        return result.FormatHash(format);
    }

    public static Task<Hash> ToHashAndOutputAsync(this Stream input, Stream output, HashType type = HashType.MD5)
    {
        return ComputeHashCoreAsync(input, output, type);
    }

    public static async Task<HashString> ToHashAndOutputAsync(this Stream input, Stream output, HashFormat format, HashType type = HashType.MD5)
    {
        var result = await ToHashAndOutputAsync(input, output, type);
        return result.FormatHash(format);
    }

    private static async Task<Hash> ComputeHashCoreAsync(Stream input, Stream output, HashType type)
    {
        using HashAlgorithm hashAlgorithm = type switch
        {
            HashType.MD5 => MD5.Create(),
            HashType.SHA1 => SHA1.Create(),
            HashType.SHA256 => SHA256.Create(),
            HashType.SHA384 => SHA384.Create(),
            HashType.SHA512 => SHA512.Create(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var pool = ArrayPool<byte>.Shared;
        var buffer = pool.Rent(65536);

        try
        {
            await using var crypto = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write);

            int read;
            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                crypto.Write(buffer, 0, read);

                if (output != null)
                {
                    await output.WriteAsync(buffer, 0, read);
                }
            }

            await crypto.FlushFinalBlockAsync();

            return hashAlgorithm.Hash!;
        }
        finally
        {
            pool.Return(buffer);
        }
    }

    public static HashString FormatHash(this Hash hash, HashFormat format)
    {
        switch (format)
        {
            case HashFormat.Hex:
                return new HashString(BitConverter.ToString(hash).Replace("-", ""), format);

            case HashFormat.HexLower:
                return new HashString(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(), format);

            case HashFormat.HexWithDashes:
                return new HashString(BitConverter.ToString(hash), format);

            case HashFormat.Base64:
                return new HashString(Convert.ToBase64String(hash), format);

            case HashFormat.Base64UrlSafe:
                {
                    var b64 = Convert.ToBase64String(hash);
                    return new HashString(b64.Replace("+", "-").Replace("/", "_").TrimEnd('='), format);
                }

            case HashFormat.Base32:
                return new HashString(Base32Encoding.Encode(hash), format);

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    public static HashString ChangeFormat(this HashString hash, HashFormat format)
    {
        var raw = UnformatHash(hash);
        return FormatHash(raw, format);
    }

    public static Hash UnformatHash(this HashString hash)
    {
        if (hash == null) return null;

        return UnformatHash(hash.Value, hash.Format);
    }

    internal static Hash UnformatHash(string value, HashFormat format)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        switch (format)
        {
            case HashFormat.Hex:
            case HashFormat.HexLower:
            case HashFormat.HexWithDashes:
            {
                // Remove optional whitespace or dashes
                var clean = value.Replace("-", "").Trim();
                var bytes = new byte[clean.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(clean.Substring(i * 2, 2), 16);
                }
                return bytes;
            }

            case HashFormat.Base64:
                return Convert.FromBase64String(value);

            case HashFormat.Base64UrlSafe:
            {
                var base64 = value
                    .Replace("-", "+")
                    .Replace("_", "/");

                // Fix padding (=) for valid base64 length
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                return Convert.FromBase64String(base64);
            }

            case HashFormat.Base32:
                return Base32Encoding.Decode(value);

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}