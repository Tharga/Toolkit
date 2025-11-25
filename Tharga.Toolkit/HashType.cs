namespace Tharga.Toolkit;

/// <summary>
/// Specifies the type of hash algorithm to use.
/// </summary>
public enum HashType
{
    /// <summary>
    /// MD5 hash (128-bit).
    /// Fast but cryptographically broken.
    /// Suitable for checksums and non-security scenarios.
    /// </summary>
    MD5,

    /// <summary>
    /// SHA-1 hash (160-bit).
    /// Deprecated and insecure for cryptographic use.
    /// Kept only for legacy system compatibility.
    /// </summary>
    SHA1,

    /// <summary>
    /// SHA-256 hash (256-bit).
    /// Part of the SHA-2 family.
    /// Secure and widely used for modern applications.
    /// </summary>
    SHA256,

    /// <summary>
    /// SHA-384 hash (384-bit).
    /// Part of the SHA-2 family.
    /// More secure than SHA-256 but less commonly used.
    /// </summary>
    SHA384,

    /// <summary>
    /// SHA-512 hash (512-bit).
    /// Part of the SHA-2 family.
    /// Strong and secure for long-term integrity protection.
    /// </summary>
    SHA512
}