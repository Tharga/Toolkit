using System;
using System.Text;
using Microsoft.Extensions.Options;

namespace Tharga.Toolkit.Password;

internal class ApiKeyService : IApiKeyService
{
    private readonly ApiKeyOptions _options;

    public ApiKeyService(IOptions<ApiKeyOptions> options)
    {
        _options = options.Value;
    }

    public string BuildApiKey(string username, Func<string> passwordGenerator)
    {
        var password = passwordGenerator?.Invoke() ?? StringExtension.GetRandomString(20, 30, StringExtension.ExtendedCharacters);
        username = Uri.EscapeDataString(username);
        var cred = $"{username}:{password}";
        var apiKey = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
        return apiKey;
    }

    public string Encrypt(string apiKey)
    {
        var hashedPassword = PasswordHasher.HashPassword(apiKey, _options.SaltSize, _options.HashSize, _options.Iterations);
        return hashedPassword;
    }

    public bool Verify(string apiKey, string hashedApiKey)
    {
        return PasswordHasher.VerifyPassword(apiKey, hashedApiKey, _options.SaltSize, _options.HashSize);
    }

    public string GetUsername(string apiKey)
    {
        var raw= apiKey.FromBase64();
        var l = raw.IndexOf(":", StringComparison.Ordinal);
        if (l == -1) throw new InvalidOperationException("Not a valid api key, cannot find ':' separator.");
        var result = raw.Substring(0, l);
        return Uri.UnescapeDataString(result);
    }
}