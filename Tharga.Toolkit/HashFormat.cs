namespace Tharga.Toolkit;

/// <summary>
/// Specifies the output text format used when converting a computed hash to a string.
/// </summary>
public enum HashFormat
{
    /// <summary>
    /// Uppercase hexadecimal format without separators.
    /// Example: <c>5D41402ABC4B2A76B9719D911017C592</c>
    /// </summary>
    Hex,

    /// <summary>
    /// Lowercase hexadecimal format without separators.
    /// Example: <c>5d41402abc4b2a76b9719d911017c592</c>
    /// </summary>
    HexLower,

    /// <summary>
    /// Standard Base64 encoding.
    /// Example: <c>XUFAKrxLKna5cZ2REBfFkg==</c>
    /// </summary>
    Base64,

    /// <summary>
    /// URL-safe Base64 encoding, using '-' and '_' instead of '+' and '/',
    /// and omitting padding characters.
    /// Example: <c>XUFAKrxLKna5cZ2REBfFkg</c>
    /// </summary>
    Base64UrlSafe,

    /// <summary>
    /// Hexadecimal format separated by dashes between bytes.
    /// Example: <c>5D-41-40-2A-BC-4B-2A-76-B9-71-9D-91-10-17-C5-92</c>
    /// </summary>
    HexWithDashes,

    /// <summary>
    /// RFC 4648 Base32 encoding using the alphabet A-Z and 2-7.
    /// Padding characters may be included depending on implementation.
    /// Example: <c>KRSXG5A=</c>
    /// </summary>
    Base32
}