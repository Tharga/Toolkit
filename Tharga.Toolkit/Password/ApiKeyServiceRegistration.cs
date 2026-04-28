using System;
using Microsoft.Extensions.Configuration;
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
        var o = new ApiKeyOptions();
        options?.Invoke(o);

        services.AddSingleton(Options.Create(o));
        services.AddTransient<IApiKeyService, ApiKeyService>();
    }

    /// <summary>
    /// Registers <see cref="IApiKeyService"/> and binds <see cref="ApiKeyOptions"/> from a configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Configuration containing the section to bind from.</param>
    /// <param name="sectionName">The configuration section name. Defaults to <c>"ApiKey"</c>.</param>
    public static void RegisterApiKeyService(this IServiceCollection services, IConfiguration configuration, string sectionName = "ApiKey")
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.Configure<ApiKeyOptions>(configuration.GetSection(sectionName));
        services.AddTransient<IApiKeyService, ApiKeyService>();
    }
}
