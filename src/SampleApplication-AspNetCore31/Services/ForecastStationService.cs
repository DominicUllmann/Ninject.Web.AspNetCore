using System;
using ServiceFacadeWebApi.Contracts;

namespace ServiceFacadeWebApi.Services
{
	public class ForecastStationService : IForecastStationService
	{

		private static Random _random = new Random();
		private readonly string _station;

		public ForecastStationService(string station)
		{
			_station = station;
		}

		public string Station => _station;

		public WeatherForecast CurrentForecast()
		{
			return new WeatherForecast() {Date = DateTime.Now, Summary = _station, TemperatureC = _random.Next(0, 25)};
		}
	}
}
