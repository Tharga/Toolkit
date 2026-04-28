using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

    [Fact]
    public void RegisterApiKeyService_with_configuration_binds_options_from_default_section()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ApiKey:SaltSize"] = "24",
                ["ApiKey:HashSize"] = "28",
                ["ApiKey:Iterations"] = "20000"
            })
            .Build();

        var services = new ServiceCollection();
        services.RegisterApiKeyService(configuration);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ApiKeyOptions>>().Value;

        options.SaltSize.Should().Be(24);
        options.HashSize.Should().Be(28);
        options.Iterations.Should().Be(20000);
    }

    [Fact]
    public void RegisterApiKeyService_with_configuration_binds_options_from_custom_section()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["MyApi:SaltSize"] = "8",
                ["MyApi:HashSize"] = "12",
                ["MyApi:Iterations"] = "3000"
            })
            .Build();

        var services = new ServiceCollection();
        services.RegisterApiKeyService(configuration, "MyApi");

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ApiKeyOptions>>().Value;

        options.SaltSize.Should().Be(8);
        options.HashSize.Should().Be(12);
        options.Iterations.Should().Be(3000);
    }

    [Fact]
    public void RegisterApiKeyService_with_configuration_registers_IApiKeyService()
    {
        var configuration = new ConfigurationBuilder().Build();

        var services = new ServiceCollection();
        services.RegisterApiKeyService(configuration);

        var provider = services.BuildServiceProvider();
        provider.GetService<IApiKeyService>().Should().NotBeNull();
    }

    [Fact]
    public void RegisterApiKeyService_with_configuration_uses_defaults_when_section_missing()
    {
        var configuration = new ConfigurationBuilder().Build();

        var services = new ServiceCollection();
        services.RegisterApiKeyService(configuration);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ApiKeyOptions>>().Value;

        options.SaltSize.Should().Be(16);
        options.HashSize.Should().Be(20);
        options.Iterations.Should().Be(10000);
    }

    [Fact]
    public void RegisterApiKeyService_with_null_configuration_throws()
    {
        var services = new ServiceCollection();
        var act = () => services.RegisterApiKeyService((IConfiguration)null);
        act.Should().Throw<ArgumentNullException>();
    }
}
