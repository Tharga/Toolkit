namespace Tharga.Toolkit;

/// <summary>
/// Represents a raw hash value as a byte array. Supports implicit conversion to and from <c>byte[]</c>.
/// </summary>
public record Hash
{
    private Hash(byte[] bytes)
    {
        Value = bytes;
    }

    /// <summary>
    /// Gets the raw hash bytes.
    /// </summary>
    public byte[] Value { get; }

    /// <summary>Converts a <see cref="Hash"/> to a byte array.</summary>
    public static implicit operator byte[](Hash hash) => hash?.Value;

    /// <summary>Converts a byte array to a <see cref="Hash"/>.</summary>
    public static implicit operator Hash(byte[] hash) => new Hash(hash);
}