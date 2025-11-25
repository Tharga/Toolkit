namespace Tharga.Toolkit;

public record HashString : Hash
{
    public HashString(string value, HashFormat format)
        : base(HashExtensions.UnformatHash(value, format))
    {
        Value = value;
        Format = format;
    }

    public string Value { get; }
    public HashFormat Format { get; }

    public override string ToString() => Value;

    public static implicit operator string(HashString hash) => hash?.Value;
}