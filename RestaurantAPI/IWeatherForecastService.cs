using System.Collections.Generic;

namespace RestaurantAPI
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
    }
}