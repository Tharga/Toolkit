using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Tharga.Toolkit.Password
{
    /// <summary>
    /// Hashes and verifies passwords with PBKDF2.
    /// </summary>
    /// <remarks>
    /// New hashes use PBKDF2-HMAC-SHA512 and are written as <c>$HASH|V2$SHA512${iterations}${salt}${hash}</c>, with salt and hash in Base64,
    /// so everything needed to verify them is stored in the string.
    /// Hashes in the original <c>$HASH|V1${iterations}${salt and hash}</c> format (PBKDF2-HMAC-SHA1) still verify.
    /// Use <see cref="NeedsRehash"/> to find hashes that were created with older or weaker parameters.
    /// </remarks>
    public static class PasswordHasher
    {
        /// <summary>The number of PBKDF2 iterations used when none is given.</summary>
        public const int DefaultIterations = 210000;

        private const int DefaultSaltSize = 16;
        private const int DefaultHashSize = 32;
        private const int LegacyHashSize = 20;
        private const string CurrentVersion = "HASH|V2";
        private const string CurrentAlgorithm = "SHA512";
        private const string LegacyAlgorithm = "SHA1";
        private const string FormatMessage = "Unexpected hash format. Should be formatted as '$HASH|V2$SHA512${iterations}${salt}${hash}' or '$HASH|V1${iterations}${hash}'.";

        /// <summary>
        /// Create a password hash.
        /// </summary>
        /// <param name="password">The password that is to be hashed.</param>
        /// <param name="saltSize">Size of the salt in bytes. Default 16 is 128 bit.</param>
        /// <param name="hashSize">Size of the hash in bytes. Default 32 is 256 bit.</param>
        /// <param name="iterations">Number of PBKDF2 iterations. Default is <see cref="DefaultIterations"/>.</param>
        /// <returns>The hash, in the format <c>$HASH|V2$SHA512${iterations}${salt}${hash}</c>.</returns>
        public static string HashPassword(string password, int saltSize = DefaultSaltSize, int hashSize = DefaultHashSize, int iterations = DefaultIterations)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (saltSize < 1) throw new ArgumentOutOfRangeException(nameof(saltSize), saltSize, "The salt size must be at least one byte.");
            if (hashSize < 1) throw new ArgumentOutOfRangeException(nameof(hashSize), hashSize, "The hash size must be at least one byte.");
            if (iterations < 1) throw new ArgumentOutOfRangeException(nameof(iterations), iterations, "The number of iterations must be at least one.");

            var salt = new byte[saltSize];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            var hash = Pbkdf2Sha512(Encoding.UTF8.GetBytes(password), salt, iterations, hashSize);

            return $"${CurrentVersion}${CurrentAlgorithm}${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verify hashed password with the original password.
        /// </summary>
        /// <param name="password">The password that is to be verified.</param>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="saltSize">Size of the salt in bytes, for a hash in the original <c>V1</c> format only. Default 16 is 128 bit.</param>
        /// <param name="hashSize">Size of the hash in bytes, for a hash in the original <c>V1</c> format only. Default 20 is 160 bit.</param>
        /// <returns><c>true</c> if the password matches the hash.</returns>
        /// <exception cref="FormatException">The hashed password is not in a known format.</exception>
        public static bool VerifyPassword(string password, string hashedPassword, int saltSize = DefaultSaltSize, int hashSize = LegacyHashSize)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            var stored = Parse(hashedPassword, saltSize, hashSize);
            var actual = stored.Algorithm == LegacyAlgorithm
                ? Pbkdf2Sha1(password, stored.Salt, stored.Iterations, stored.Hash.Length)
                : Pbkdf2Sha512(Encoding.UTF8.GetBytes(password), stored.Salt, stored.Iterations, stored.Hash.Length);

            return FixedTimeEquals(actual, stored.Hash);
        }

        /// <summary>
        /// Tells whether a hash was created with older or weaker parameters than the current ones, so that it should be replaced.
        /// Create the new hash with <see cref="HashPassword"/> the next time the password is verified.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="iterations">The number of iterations a hash should have at least. Default is <see cref="DefaultIterations"/>.</param>
        /// <returns><c>true</c> if the hash is in the original <c>V1</c> format, or has fewer iterations than <paramref name="iterations"/>.</returns>
        /// <exception cref="FormatException">The hashed password is not in a known format.</exception>
        public static bool NeedsRehash(string hashedPassword, int iterations = DefaultIterations)
        {
            var stored = Parse(hashedPassword, DefaultSaltSize, LegacyHashSize);
            return stored.Algorithm != CurrentAlgorithm || stored.Iterations < iterations;
        }

        private static StoredHash Parse(string hashedPassword, int legacySaltSize, int legacyHashSize)
        {
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));

            var parts = hashedPassword.Split('$');
            if (parts.Length == 4) return ParseLegacy(parts, legacySaltSize, legacyHashSize);
            if (parts.Length != 6 || parts[0] != string.Empty || parts[1] != CurrentVersion || parts[2] != CurrentAlgorithm) throw new FormatException(FormatMessage);

            return new StoredHash(CurrentAlgorithm, ParseIterations(parts[3]), Convert.FromBase64String(parts[4]), Convert.FromBase64String(parts[5]));
        }

        private static StoredHash ParseLegacy(string[] parts, int saltSize, int hashSize)
        {
            var saltAndHash = Convert.FromBase64String(parts[3]);
            if (saltSize < 0 || hashSize < 1 || saltAndHash.Length < saltSize + hashSize) throw new FormatException(FormatMessage);

            var salt = new byte[saltSize];
            var hash = new byte[hashSize];
            Array.Copy(saltAndHash, 0, salt, 0, saltSize);
            Array.Copy(saltAndHash, saltSize, hash, 0, hashSize);

            return new StoredHash(LegacyAlgorithm, ParseIterations(parts[2]), salt, hash);
        }

        private static int ParseIterations(string value)
        {
            var iterations = int.Parse(value);
            if (iterations < 1) throw new FormatException(FormatMessage);
            return iterations;
        }

        private static byte[] Pbkdf2Sha1(string password, byte[] salt, int iterations, int length)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(length);
            }
        }

        // Written out because netstandard2.0 has no PBKDF2 with a SHA-2 hash. Matches Rfc2898DeriveBytes.Pbkdf2 with HashAlgorithmName.SHA512.
        private static byte[] Pbkdf2Sha512(byte[] password, byte[] salt, int iterations, int length)
        {
            using (var hmac = new HMACSHA512(password))
            {
                var blockSize = hmac.HashSize / 8;
                var result = new byte[length];
                var input = new byte[salt.Length + 4];
                Buffer.BlockCopy(salt, 0, input, 0, salt.Length);

                for (var block = 1; (block - 1) * blockSize < length; block++)
                {
                    input[salt.Length] = (byte)(block >> 24);
                    input[salt.Length + 1] = (byte)(block >> 16);
                    input[salt.Length + 2] = (byte)(block >> 8);
                    input[salt.Length + 3] = (byte)block;

                    var u = hmac.ComputeHash(input);
                    var t = (byte[])u.Clone();
                    for (var i = 1; i < iterations; i++)
                    {
                        u = hmac.ComputeHash(u);
                        for (var j = 0; j < t.Length; j++) t[j] ^= u[j];
                    }

                    var offset = (block - 1) * blockSize;
                    Buffer.BlockCopy(t, 0, result, offset, Math.Min(blockSize, length - offset));
                }

                return result;
            }
        }

        // The same comparison as CryptographicOperations.FixedTimeEquals, which netstandard2.0 does not have: every byte is compared.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left.Length != right.Length) return false;

            var difference = 0;
            for (var i = 0; i < left.Length; i++)
            {
                difference |= left[i] ^ right[i];
            }

            return difference == 0;
        }

        private sealed class StoredHash
        {
            public StoredHash(string algorithm, int iterations, byte[] salt, byte[] hash)
            {
                Algorithm = algorithm;
                Iterations = iterations;
                Salt = salt;
                Hash = hash;
            }

            public string Algorithm { get; }
            public int Iterations { get; }
            public byte[] Salt { get; }
            public byte[] Hash { get; }
        }
    }
}
