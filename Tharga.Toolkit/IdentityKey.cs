namespace Tharga.Toolkit;

public record IdentityKey
{
    private readonly string _value;

    public IdentityKey(string value)
    {
        _value = value;
    }

    public string Value => _value;

    public static implicit operator IdentityKey(string value) => new(value);
    public static implicit operator string(IdentityKey key) => key._value;
}