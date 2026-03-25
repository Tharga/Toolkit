namespace Tharga.Toolkit.Password;

/// <summary>
/// Configuration options for the API key service's hashing parameters.
/// </summary>
public record ApiKeyOptions
{
    /// <summary>Salt size in bytes. Default 16 (128-bit).</summary>
    public int SaltSize { get; set; } = 16;

    /// <summary>Hash size in bytes. Default 20 (160-bit).</summary>
    public int HashSize { get; set; } = 20;

    /// <summary>Number of PBKDF2 iterations. Default 10000.</summary>
    public int Iterations { get; set; } = 10000;
}