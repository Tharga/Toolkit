using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tharga.Toolkit;

public static class Base32Encoding
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
    private static readonly Dictionary<char, byte> AlphabetMap = Alphabet
        .Select((c, i) => new { Char = c, Index = (byte)i })
        .ToDictionary(c => c.Char, c => c.Index);

    public static string Encode(byte[] data)
    {
        if (data == null || data.Length == 0) return string.Empty;

        var result = new StringBuilder((data.Length * 8 + 4) / 5);

        int buffer = data[0];
        var next = 1;
        var bitsLeft = 8;

        while (bitsLeft > 0 || next < data.Length)
        {
            if (bitsLeft < 5)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[next++] & 0xff;
                    bitsLeft += 8;
                }
                else
                {
                    var pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            var index = 0x1f & (buffer >> (bitsLeft - 5));
            bitsLeft -= 5;
            result.Append(Alphabet[index]);
        }

        while (result.Length % 8 != 0)
        {
            result.Append('=');
        }

        return result.ToString();
    }

    public static byte[] Decode(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<byte>();

        input = input.TrimEnd('=').ToUpperInvariant();

        var output = new List<byte>(input.Length * 5 / 8);

        var buffer = 0;
        var bitsLeft = 0;

        foreach (var c in input)
        {
            if (!AlphabetMap.TryGetValue(c, out var val))
            {
                throw new FormatException($"Invalid Base32 character '{c}'.");
            }

            buffer = (buffer << 5) | val;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                output.Add((byte)((buffer >> (bitsLeft - 8)) & 0xff));
                bitsLeft -= 8;
            }
        }

        return output.ToArray();
    }
}