namespace Tharga.Toolkit;

/// <summary>
/// Represents a hash value formatted as a string (e.g. hex, Base64, Base32).
/// Inherits from <see cref="Hash"/> and retains the raw bytes alongside the formatted string.
/// </summary>
public record HashString : Hash
{
    /// <summary>
    /// Creates a new <see cref="HashString"/> from a formatted string and its format type.
    /// </summary>
    /// <param name="value">The formatted hash string.</param>
    /// <param name="format">The format used to encode the hash.</param>
    public HashString(string value, HashFormat format)
        : base(HashExtensions.UnformatHash(value, format))
    {
        Value = value;
        Format = format;
    }

    /// <summary>Gets the formatted hash string.</summary>
    public string Value { get; }

    /// <summary>Gets the format used to encode this hash string.</summary>
    public HashFormat Format { get; }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>Converts a <see cref="HashString"/> to its string representation.</summary>
    public static implicit operator string(HashString hash) => hash?.Value;
}