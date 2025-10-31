using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tharga.Toolkit.Password;

public static class ApiKeyServiceRegistration
{
    public static void RegisterApiKeyService(this IServiceCollection services, Action<ApiKeyOptions> options = null)
    {
        //TODO: Make it possible to read from configuration.
        var o = new ApiKeyOptions();
        options?.Invoke(o);

        services.AddSingleton(Options.Create(o));
        services.AddTransient<IApiKeyService, ApiKeyService>();
    }
}