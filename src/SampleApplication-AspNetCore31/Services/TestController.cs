using System;
using Microsoft.AspNetCore.Mvc;
using ServiceFacadeWebApi.Contracts;

namespace ServiceFacadeWebApi.Services
{
	public class TestController : ITestService
	{

		private string _dependency;

		public TestController(string dependency)
		{
			_dependency = dependency;
		}

		[HttpPost]
		public string Test()
		{
			return _dependency;
		}

		//[HttpPost]
		public WeatherForecast Test2()
		{
			return new WeatherForecast() { Date = DateTime.Today, Summary = "Test", TemperatureC = 12 };
		}

		[HttpGet]
		public string Get1()
		{
			return "HelloG";
		}

		[HttpGet]
		public string Get2()
		{
			return "HelloG2";
		}

	}
}
