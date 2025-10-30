using System;
using System.Security.Cryptography;

namespace Tharga.Toolkit.Password
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Create a password hash.
        /// </summary>
        /// <param name="password">The password that is to be hashed.</param>
        /// <param name="saltSize">Size of the salt. Default 16 is 128 bit.</param>
        /// <param name="hashSize">Size of the hash. Default 20 is 160 bit.</param>
        /// <param name="iterations">Number of iterations. Default is 10000</param>
        /// <returns></returns>
        public static string HashPassword(string password, int saltSize = 16, int hashSize = 20, int iterations = 10000)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(hashSize);

            // Combine salt and hash
            var hashBytes = new byte[saltSize + hashSize];
            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return $"$HASH|V1${iterations}${base64Hash}";
        }

        /// <summary>
        /// Verify hashed password with the original password.
        /// </summary>
        /// <param name="password">The password that is to be verified.</param>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="saltSize">Size of the salt. Default 16 is 128 bit.</param>
        /// <param name="hashSize">Size of the hash. Default 20 is 160 bit.</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static bool VerifyPassword(string password, string hashedPassword, int saltSize = 16, int hashSize = 20)
        {
            // Extract iteration and Base64 string
            var parts = hashedPassword.Split('$');
            if (parts.Length != 4)
            {
                throw new FormatException("Unexpected hash format. Should be formatted as '$HASH|V1${iterations}${hash}'");
            }

            var iterations = int.Parse(parts[2]);
            var hashBytes = Convert.FromBase64String(parts[3]);

            // Get salt
            var salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(hashSize);

            // Get result
            for (var i = 0; i < hashSize; i++)
            {
                if (hashBytes[i + saltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
