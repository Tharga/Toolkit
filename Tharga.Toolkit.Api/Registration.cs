using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Toolkit.Api;

public interface IApplicationRepository<TKey>
{
	Task<IApplicationRegistration<TKey>> AddRequestAsync(IApplicationRegistration<TKey> applicationRegistration);
	IAsyncEnumerable<IApplicationRegistration<TKey>> GetAll();
}

internal class ApplicationMemoryRepository<TKey> : IApplicationRepository<TKey>
{
	private readonly ConcurrentDictionary<TKey, IApplicationRegistration<TKey>> _regs = new();

	public async Task<IApplicationRegistration<TKey>> AddRequestAsync(IApplicationRegistration<TKey> applicationRegistration)
	{
		var result = _regs.AddOrUpdate(applicationRegistration.Id, _ => applicationRegistration, (_, _) => applicationRegistration);
		return result;
	}

	public async IAsyncEnumerable<IApplicationRegistration<TKey>> GetAll()
	{
		foreach (var reg in _regs)
		{
			yield return reg.Value;
		}
	}
}

public static class ThargaToolkitApiRegistration
{
	public static void RegisterThargaApiToolkit<TKey>(this IServiceCollection services, IApplicationRepository<TKey> repository = default)
	{
		services.AddSingleton(_ => repository ?? new ApplicationMemoryRepository<TKey>());
	}
}

public interface IApplicationRequest
{
}

public record ApplicationRequest : IApplicationRequest
{
}

public interface IApplicationRegistration<TKey>
{
	TKey Id { get; }
}

public record ApplicationRegistrationX<TKey> : IApplicationRegistration<TKey>
{
	public required TKey Id { get; init; }
	public required IApplicationRequest Request { get; init; }
}

[ApiController]
[Route("Api/[controller]")]
public class ApplicationController : ControllerBase
{
	private readonly IApplicationRepository<Guid> _applicationRepository;

	public ApplicationController(IApplicationRepository<Guid> applicationRepository)
	{
		_applicationRepository = applicationRepository;
	}

	[HttpPost]
	public async Task<IActionResult> Register(IApplicationRequest request)
	{
		await _applicationRepository.AddRequestAsync(new ApplicationRegistrationX<Guid>
		{
			Id = Guid.NewGuid(),
			Request = request
		});

		return Ok();
	}

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var result = await _applicationRepository.GetAll().ToArrayAsync();

		return Ok();
	}
}