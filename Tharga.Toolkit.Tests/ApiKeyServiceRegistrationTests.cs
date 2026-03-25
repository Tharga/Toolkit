using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Toolkit.Password;
using Xunit;

namespace Tharga.Toolkit.Tests;

public class ApiKeyServiceRegistrationTests
{
    [Fact]
    public void RegisterApiKeyService_registers_IApiKeyService()
    {
        var services = new ServiceCollection();
        services.RegisterApiKeyService();

        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IApiKeyService>();
        service.Should().NotBeNull();
    }

    [Fact]
    public void RegisterApiKeyService_applies_custom_options()
    {
        var services = new ServiceCollection();
        services.RegisterApiKeyService(o =>
        {
            o.SaltSize = 32;
            o.HashSize = 32;
            o.Iterations = 5000;
        });

        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IApiKeyService>();
        service.Should().NotBeNull();
    }
}
