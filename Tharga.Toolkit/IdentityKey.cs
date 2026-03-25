namespace Tharga.Toolkit;

/// <summary>
/// A portable, serialized identity key derived from claims. Contains a Base64-encoded JSON
/// dictionary of identity claim types and their values.
/// </summary>
public record IdentityKey
{
    private readonly string _value;

    public IdentityKey(string value)
    {
        _value = value;
    }

    public string Value => _value;

    //public static implicit operator IdentityKey(string value) => new(value);
    //public static implicit operator string(IdentityKey key) => key._value;
}