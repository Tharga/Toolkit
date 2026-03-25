using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tharga.Toolkit.Password;

/// <summary>
/// Provides dependency injection registration for the API key service.
/// </summary>
public static class ApiKeyServiceRegistration
{
    /// <summary>
    /// Registers <see cref="IApiKeyService"/> in the service collection with optional configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">Optional action to configure <see cref="ApiKeyOptions"/>.</param>
    public static void RegisterApiKeyService(this IServiceCollection services, Action<ApiKeyOptions> options = null)
    {
        //TODO: Make it possible to read from configuration.
        var o = new ApiKeyOptions();
        options?.Invoke(o);

        services.AddSingleton(Options.Create(o));
        services.AddTransient<IApiKeyService, ApiKeyService>();
    }
}