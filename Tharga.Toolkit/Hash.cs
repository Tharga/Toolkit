namespace Tharga.Toolkit;

public record Hash
{
    private Hash(byte[] bytes)
    {
        Value = bytes;
    }

    public byte[] Value { get; }

    public static implicit operator byte[](Hash hash) => hash?.Value;
    public static implicit operator Hash(byte[] hash) => new Hash(hash);
}