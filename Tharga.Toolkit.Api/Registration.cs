using Microsoft.AspNetCore.Mvc;

namespace Tharga.Toolkit.Api;

public static class ThargaToolkitApiRegistration
{
	public static void RegisterThargaApiToolkit(this IServiceCollection services)
	{
	}
}

[ApiController]
[Route("Api/[controller]")]
public class ApplicationController : ControllerBase
{
	public ApplicationController()
	{
	}

	[HttpPost]
	public async Task<IActionResult> Register()
	{
		return Ok();
	}

	//[HttpGet]
	//[Route("test")]
	//public async Task<string> Get()
	//{
	//	return "yee";
	//}

	//[HttpGet(Name = "GetWeatherForecast")]
	//public IEnumerable<WeatherForecast> Get()
	//{
	//	return Enumerable.Range(1, 5).Select(index => new WeatherForecast
	//	{
	//		Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
	//		TemperatureC = Random.Shared.Next(-20, 55),
	//		Summary = Summaries[Random.Shared.Next(Summaries.Length)]
	//	})
	//		.ToArray();
	//}
}