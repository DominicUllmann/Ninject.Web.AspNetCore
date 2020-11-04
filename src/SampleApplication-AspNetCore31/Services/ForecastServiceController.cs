using System;
using System.Collections.Generic;
using System.Linq;
using ServiceFacadeWebApi.Contracts;

namespace ServiceFacadeWebApi.Services

{
	public class ForecastServiceController : IForecastService
	{
		private readonly IList<IForecastStationService> _stations;

		public ForecastServiceController(IList<IForecastStationService> stations)
		{
			_stations = stations;
		}

		public WeatherForecast AnyForecast()
		{
			return _stations.FirstOrDefault()?.CurrentForecast();
		}

		public WeatherForecast CurrentForecast(StationForecastRequest request)
		{
			var station = _stations.FirstOrDefault(x => x.Station == request.Station);
			return station != null ? station.CurrentForecast() : throw new ApplicationException("Station unknown");
		}
	}
}