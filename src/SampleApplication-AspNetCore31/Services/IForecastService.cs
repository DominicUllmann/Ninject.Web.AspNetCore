namespace ServiceFacadeWebApi.Contracts
{
	public interface IForecastService
	{

		WeatherForecast AnyForecast();

		WeatherForecast CurrentForecast(StationForecastRequest request);

	}
}
