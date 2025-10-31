using System;

namespace Tharga.Toolkit.Password;

public interface IApiKeyService
{
    /// <summary>
    /// An api key is build by a username and a random password, '{username}:{password}' that is base64 encoded.
    /// Before storing it should be Encrypted. Then use the Verify method to check if provided api key is valid.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="passwordGenerator"></param>
    /// <returns></returns>
    string BuildApiKey(string username, Func<string> passwordGenerator = null);

    /// <summary>
    /// Encrypts the api key, so that it can be stored.
    /// </summary>
    /// <param name="apiKey">The api key that is to be encrypted.</param>
    /// <returns></returns>
    string Encrypt(string apiKey);

    /// <summary>
    /// Verifies provided api key with the hashed version.
    /// </summary>
    /// <param name="apiKey">Original api key that is to be verified.</param>
    /// <param name="hashedApiKey">The hashed version of the api key.</param>
    /// <returns></returns>
    bool Verify(string apiKey, string hashedApiKey);

    /// <summary>
    /// Extracts the username part of the api key.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    string GetUsername(string apiKey);
}