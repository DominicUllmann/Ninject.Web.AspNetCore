using ServiceFacadeWebApi.Contracts;

namespace ServiceFacadeWebApi.Services
{
	public interface IForecastStationService
	{
		string Station { get; }


		WeatherForecast CurrentForecast();
	}

}
